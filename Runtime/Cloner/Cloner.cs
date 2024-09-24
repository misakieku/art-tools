using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;

namespace Misaki.ArtTool
{
    [ExecuteInEditMode]
    public class Cloner : MonoBehaviour
    {
        public List<InputObjectData> inputObjects = new();

        public DistributionMode distributionMode = DistributionMode.Grid;
        public bool isRandomDistribution = false;
        public uint seed = 123456;
        public bool autoGenerate;
        public bool isRenderInstancing;

        public MeshFilter inputMeshFilter;
        public ObjectDistributionSetting objectDistributionSetting;

        public SplineContainer inputSplineContainer;
        public SplineDistributionSetting splineDistributionSetting;

        public LinearDistributionSetting linearDistributionSetting;

        public GridDistributionSetting gridDistributionSetting;

        public RadialDistributionSetting radialDistributionSetting;

        public List<EffectorData> effectors = new();

        public DebugMode debugMode;
        public float gizmosSize = 0.25f;

        private static readonly ArrayPool<PointData> _pointPool = ArrayPool<PointData>.Shared;

        public int pointSize;
        private PointData[] _points;
        private List<GameObject> _instantiateObjects = new();
        private readonly Dictionary<int, List<Matrix4x4>> _objectGroup = new();

        private bool _isPointsDirty = false;

        [MenuItem("GameObject/Cloner/Cloner Object")]
        public static void CreateCloner()
        {
            var clonerObject = new GameObject("Cloner");
            GameObjectUtility.EnsureUniqueNameForSibling(clonerObject);

            clonerObject.AddComponent<Cloner>();
        }

        private void OnEnable()
        {
            foreach (var item in effectors)
            {
                if (item.effector == null)
                {
                    return;
                }

                item.effector.propertyChanged += OnPropertyChanged;
            }

            if (transform.childCount > 0 && _instantiateObjects.Count == 0)
            {
                for (var i = 0; i < transform.childCount; i++)
                {
                    _instantiateObjects[i] = transform.GetChild(i).gameObject;
                }
            }
        }

        private void OnDisable()
        {
            foreach (var item in effectors)
            {
                if (item.effector == null)
                {
                    return;
                }

                item.effector.propertyChanged -= OnPropertyChanged;
            }
        }

        //private void OnDestroy()
        //{
        //    Clear();
        //}

        public void OnPropertyChanged(object sender, EventArgs e)
        {
            if (sender is not EffectorBase && sender is not FieldBase)
            {
                return;
            }

            _isPointsDirty = true;
        }

        private void Update()
        {
            if (_isPointsDirty && autoGenerate)
            {
                GeneratePoints();
                InstantiateGameObjectOnPoints();
            }

            if (_objectGroup == null || _objectGroup.Count <= 0 || !isRenderInstancing)
            {
                return;
            }

            foreach (var item in _objectGroup)
            {
                if (inputObjects.Count <= item.Key || inputObjects[item.Key] == null)
                {
                    continue;
                }

                var objectData = inputObjects[item.Key];
                var mat = objectData.Material;

                var renderParams = new RenderParams(mat)
                {
                    shadowCastingMode = ShadowCastingMode.On
                };
                Graphics.RenderMeshInstanced(renderParams, objectData.Mesh, 0, item.Value, pointSize);
            }
        }

        public void GeneratePoints()
        {
            Clear();

            switch (distributionMode)
            {
                case DistributionMode.Object:

                    if (inputMeshFilter == null)
                    {
                        throw new NullReferenceException("You need to set the target mesh filter before using the object distribution mode");
                    }

                    objectDistributionSetting.meshData = new(inputMeshFilter, Allocator.TempJob);
                    pointSize = objectDistributionSetting.DistributionCount;

                    break;
                case DistributionMode.Spline:

                    if (inputSplineContainer == null)
                    {
                        throw new NullReferenceException("You need to set the target spline container before using the spline distribution mode");
                    }

                    splineDistributionSetting.splineWorldMatrix = inputSplineContainer.transform.localToWorldMatrix;
                    splineDistributionSetting.nativeSpline = new(inputSplineContainer.Spline, Allocator.TempJob);
                    splineDistributionSetting.splineLength = splineDistributionSetting.nativeSpline.CalculateLength(splineDistributionSetting.splineWorldMatrix);

                    pointSize = splineDistributionSetting.DistributionCount;
                    break;

                case DistributionMode.Linear:
                    pointSize = (int)linearDistributionSetting.count;
                    break;

                case DistributionMode.Grid:
                    pointSize = gridDistributionSetting.DistributionCount;
                    break;

                case DistributionMode.Radial:
                    pointSize = (int)radialDistributionSetting.count;
                    break;
                case DistributionMode.Honeycomb:
                    break;
                default:
                    break;
            }

            // Allocate a empty native collection to avoid job error
            EnsureNativeCollectionValid();

            if (pointSize == 0)
            {
                return;
            }

            _points = _pointPool.Rent(pointSize);

            foreach (var effectorData in effectors)
            {
                effectorData.effector.Initialize();
            }

            var worldMatrix = transform.localToWorldMatrix;

            // Since NativeSpline is not available in managed thread, we have to use jobs
            var pointsArray = new NativeArray<PointData>(_points.Length, Allocator.TempJob);
            var pointsGenerationJob = new PointsGenerationJob()
            {
                worldMatrix = worldMatrix,
                pointSize = pointSize,

                distributionMode = distributionMode,

                objectDistributionSetting = objectDistributionSetting,
                splineDistributionSetting = splineDistributionSetting,
                linearDistributionSetting = linearDistributionSetting,
                gridDistributionSetting = gridDistributionSetting,
                radialDistributionSetting = radialDistributionSetting,

                points = pointsArray
            };

            var handle = pointsGenerationJob.Schedule(pointSize, 64);
            handle.Complete();

            pointsArray.CopyTo(_points);

            pointsArray.Dispose();
            splineDistributionSetting.nativeSpline.Dispose();
            objectDistributionSetting.meshData.Dispose();

            // Switch to managed thread for effectors because of interface
            Parallel.For(0, pointSize, i =>
            {
                for (var e = effectors.Count - 1; e >= 0; e--)
                {
                    if (!effectors[e].enable || effectors[e].effector == null)
                    {
                        continue;
                    }

                    _points[i] = effectors[e].effector.Operate(i, worldMatrix, _points);
                }

                if (math.all(_points[i].matrix.GetScale() == float3.zero))
                {
                    _points[i].isValid = false;
                }
            });

            _isPointsDirty = false;
        }

        private void EnsureNativeCollectionValid()
        {
            if (distributionMode != DistributionMode.Spline)
            {
                splineDistributionSetting.nativeSpline = new(new List<BezierKnot>(), false, transform.localToWorldMatrix, Allocator.TempJob);
            }

            if (distributionMode != DistributionMode.Object)
            {
                objectDistributionSetting.meshData = new MeshData(Allocator.TempJob);
            }
        }

        public void InstantiateGameObjectOnPoints()
        {
            if (_points == null && pointSize <= 0)
            {
                return;
            }

            Clear(false, true);

            if (isRandomDistribution)
            {
                // Use Fisher-Yates Shuffle algorithm to swap value
                var random = new Random(seed);
                for (var i = pointSize - 1; i > 0; i--)
                {
                    var k = random.NextInt(0, i + 1);
                    (_points[i], _points[k]) = (_points[k], _points[i]);
                }
            }

            foreach (var item in _objectGroup)
            {
                item.Value.Clear();
            }

            var currentIndex = 0;
            var objectIndex = 0;
            while (currentIndex < pointSize)
            {
                if (!_points[currentIndex].isValid)
                {
                    currentIndex++;
                    continue;
                }

                objectIndex %= inputObjects.Count;

                if (inputObjects[objectIndex].gameObject == null)
                {
                    currentIndex++;
                    continue;
                }

                if (!_objectGroup.ContainsKey(objectIndex))
                {
                    _objectGroup[objectIndex] = new();
                }

                for (var f = 0; f < inputObjects[objectIndex].frequency; f++)
                {
                    _objectGroup[objectIndex].Add(_points[currentIndex].matrix);
                    currentIndex++;

                    if (currentIndex >= pointSize)
                    {
                        break;
                    }
                }

                objectIndex++;
            }

            for (var i = 0; i < _objectGroup.Count; i++)
            {
                var group = _objectGroup.ElementAt(i);
                if (group.Value.Count <= 0)
                {
                    _objectGroup.Remove(group.Key);
                }
            }

            currentIndex = 0;
            if (!isRenderInstancing)
            {
                _instantiateObjects.Capacity = pointSize;

                var transformAccess = new TransformAccessArray(pointSize);
                var pointsList = new NativeList<float4x4>(pointSize, Allocator.TempJob);

                foreach (var item in _objectGroup)
                {
                    var instantiateCount = item.Value.Count;

                    var instanceIDs = new NativeArray<int>(instantiateCount, Allocator.TempJob);
                    var transformIDs = new NativeArray<int>(instantiateCount, Allocator.TempJob);

                    GameObject.InstantiateGameObjects(inputObjects[item.Key].gameObject.GetInstanceID(), instantiateCount, instanceIDs, transformIDs);

                    for (var i = 0; i < instantiateCount; i++)
                    {
                        transformAccess.Add(transformIDs[i]);
                        pointsList.Add(item.Value[i]);

                        var instantiateObject = (GameObject)Resources.InstanceIDToObject(instanceIDs[i]);
                        instantiateObject.transform.parent = this.transform;
                        _instantiateObjects.Add(instantiateObject);

                        currentIndex++;
                    }

                    instanceIDs.Dispose();
                    transformIDs.Dispose();
                }

                var transformAccessJob = new TransformAccessJob()
                {
                    points = pointsList
                };

                var handle = transformAccessJob.Schedule(transformAccess);
                handle.Complete();

                transformAccess.Dispose();
                pointsList.Dispose();
            }
        }

        public void Clear(bool isClearPoints = true, bool isClearObjects = true)
        {
            if (isClearPoints && _points != null)
            {
                _pointPool.Return(_points, true);
                pointSize = 0;
            }

            if (isClearObjects)
            {
                if (_instantiateObjects == null || _instantiateObjects.Count <= 0)
                {
                    return;
                }

                for (var i = 0; i < _instantiateObjects.Count; i++)
                {
#if UNITY_EDITOR
                    DestroyImmediate(_instantiateObjects[i]);
#else
                    Destroy(_instantiateObjects[i]);
#endif
                }

                _instantiateObjects.Clear();
                _objectGroup.Clear();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (pointSize <= 0 || _points == null || _points.Length < pointSize)
            {
                return;
            }

            for (var i = 0; i < pointSize; i++)
            {
                var point = _points[i];

                switch (debugMode)
                {
                    case DebugMode.None:
                        break;
                    case DebugMode.Transform:
                        if (!point.isValid)
                        {
                            continue;
                        }

                        var scale = point.matrix.GetScale();
                        Gizmos.matrix = point.matrix;

                        // Draw the x-axis in red
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(Vector3.zero, Vector3.right * scale * gizmosSize);

                        // Draw the y-axis in green
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(Vector3.zero, Vector3.up * scale * gizmosSize);

                        // Draw the z-axis in blue
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(Vector3.zero, Vector3.forward * scale * gizmosSize);
                        break;

                    case DebugMode.Index:
                        if (!point.isValid)
                        {
                            continue;
                        }

                        Handles.matrix = point.matrix;
                        Handles.Label(Vector3.zero, i.ToString());
                        break;

                    case DebugMode.Validity:

                        if (point.isValid)
                        {
                            Gizmos.color = Color.green;
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                        }

                        Gizmos.DrawSphere(point.matrix.GetPosition(), gizmosSize / 2.0f);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
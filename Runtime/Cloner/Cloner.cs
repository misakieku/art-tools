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

        public List<EffectorData> effectors;

        public DebugMode debugMode;

        private static readonly ArrayPool<PointData> _pointPool = ArrayPool<PointData>.Shared;

        private int _pointSize;
        private PointData[] _points;
        private List<GameObject> _instantiateObjects = new();
        private readonly Dictionary<int, List<Matrix4x4>> _objectGroup = new();

        private bool _isPointsDirty = false;

        private const float GIZMOS_LINE_BASE_LENGTH = 0.5f;

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
                    _instantiateObjects.Add(transform.GetChild(i).gameObject);
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
                Graphics.RenderMeshInstanced(renderParams, objectData.Mesh, 0, item.Value, _pointSize);
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
                        throw new NullReferenceException();
                    }

                    objectDistributionSetting.meshData = new(inputMeshFilter, Allocator.TempJob);

                    break;
                case DistributionMode.Spline:

                    if (inputSplineContainer == null)
                    {
                        throw new NullReferenceException();
                    }

                    splineDistributionSetting.splineWorldMatrix = inputSplineContainer.transform.localToWorldMatrix;
                    splineDistributionSetting.nativeSpline = new(inputSplineContainer.Spline, Allocator.TempJob);
                    splineDistributionSetting.splineLength = splineDistributionSetting.nativeSpline.CalculateLength(splineDistributionSetting.splineWorldMatrix);

                    _pointSize = splineDistributionSetting.DistributionCount;
                    break;

                case DistributionMode.Linear:
                    _pointSize = (int)linearDistributionSetting.count;
                    break;

                case DistributionMode.Grid:
                    _pointSize = gridDistributionSetting.DistributionCount;
                    break;

                case DistributionMode.Radial:
                    break;
                case DistributionMode.Honeycomb:
                    break;
                default:
                    break;
            }

            // Allocate a empty native spline to avoid job error
            if (distributionMode != DistributionMode.Spline)
            {
                splineDistributionSetting.nativeSpline = new(new List<BezierKnot>(), false, transform.localToWorldMatrix, Allocator.TempJob);
            }

            if (_pointSize == 0)
            {
                return;
            }

            if (_points == null || _pointSize > _points.Length)
            {
                _points = _pointPool.Rent(_pointSize);
            }

            foreach (var effectorData in effectors)
            {
                effectorData.effector.Initialize();
            }

            var worldMatrix = transform.localToWorldMatrix;
            //Parallel.For(0, _pointSize, i =>
            ////for (var i = 0; i < _pointSize; i++)
            //{
            //    var pointMatrix = float4x4.identity;
            //    var isValid = true;
            //    switch (distributionMode)
            //    {
            //        case DistributionMode.Object:
            //            break;
            //        case DistributionMode.Spline:
            //            Distribution.SplineDistribution(i, _pointSize, splineDistributionSetting, out pointMatrix, out isValid);
            //            break;
            //        case DistributionMode.Linear:
            //            Distribution.LinearDistribution(i, linearDistributionSetting, out pointMatrix, out isValid);
            //            break;
            //        case DistributionMode.Grid:
            //            Distribution.GridDistribution(i, gridDistributionSetting, out pointMatrix, out isValid);
            //            break;
            //        case DistributionMode.Radial:
            //            break;
            //        case DistributionMode.Honeycomb:
            //            break;
            //        default:
            //            break;
            //    }

            //    pointMatrix = math.mul(worldMatrix, pointMatrix);

            //    _points[i].matrix = pointMatrix;
            //    _points[i].isValid = isValid;
            //    //}
            //});

            // Since NativeSpline is not available in managed thread, we have to use jobs
            var pointsArray = new NativeArray<PointData>(_points.Length, Allocator.TempJob);
            var pointsGenerationJob = new PointsGenerationJob()
            {
                worldMatrix = worldMatrix,
                pointSize = _pointSize,

                distributionMode = distributionMode,

                splineDistributionSetting = splineDistributionSetting,
                linearDistributionSetting = linearDistributionSetting,
                gridDistributionSetting = gridDistributionSetting,

                points = pointsArray
            };

            var handle = pointsGenerationJob.Schedule(_pointSize, 64);
            handle.Complete();

            pointsArray.CopyTo(_points);

            pointsArray.Dispose();
            splineDistributionSetting.nativeSpline.Dispose();
            objectDistributionSetting.meshData.Dispose();

            // Switch to managed thread for effectors because of interface
            Parallel.For(0, _pointSize, i =>
            {
                for (var e = 0; e < effectors.Count; e++)
                {
                    if (!effectors[e].enable)
                    {
                        continue;
                    }

                    if (effectors[e].effector == null)
                    {
                        continue;
                    }

                    effectors[e].effector.Operate(i, worldMatrix, _points);
                }

                if (math.all(_points[i].matrix.GetScale() == float3.zero))
                {
                    _points[i].isValid = false;
                }
            });

            _isPointsDirty = false;
        }

        public void InstantiateGameObjectOnPoints()
        {
            if (_points == null && _pointSize <= 0)
            {
                return;
            }

            if (_instantiateObjects.Count > 0)
            {
                for (var i = 0; i < _instantiateObjects.Count; i++)
                {
                    if (_instantiateObjects[i] == null)
                    {
                        continue;
                    }

#if UNITY_EDITOR
                    DestroyImmediate(_instantiateObjects[i]);
#else
                    Destroy(child.gameObject);
#endif
                }
            }

            if (isRandomDistribution)
            {
                // Use Fisher-Yates Shuffle algorithm to swap value
                var random = new Random(seed);
                for (var i = _pointSize - 1; i > 0; i--)
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
            while (currentIndex < _pointSize)
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

                    if (currentIndex >= _pointSize)
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

            if (!isRenderInstancing)
            {
                foreach (var item in _objectGroup)
                {
                    Span<Vector3> positionsSpan = stackalloc Vector3[item.Value.Count];
                    Span<Quaternion> rotationsSpan = stackalloc Quaternion[item.Value.Count];
                    var scaleArray = new Vector3[item.Value.Count];

                    MatrixHelper.DecomposeMatrixListAsSpan(item.Value,
                        positionsSpan, rotationsSpan, scaleArray);

                    var iop = InstantiateAsync(
                        inputObjects[item.Key].gameObject, item.Value.Count,
                        transform, positionsSpan, rotationsSpan);

                    iop.completed += (op) =>
                    {
                        var instantiatedObjects = iop.GetAwaiter().GetResult();
                        for (var i = 0; i < instantiatedObjects.Length; i++)
                        {
                            var instantiatedObject = instantiatedObjects[i];
                            instantiatedObject.transform.localScale = scaleArray[i];
                            _instantiateObjects.Add(instantiatedObject);
                        }
                    };
                }
            }
        }

        public void Clear(bool isClearPoints = true, bool isClearObjects = true)
        {
            if (isClearPoints && _points != null)
            {
                _pointPool.Return(_points, true);
            }

            if (isClearObjects)
            {
                _objectGroup.Clear();

                if (_instantiateObjects.Count <= 0)
                {
                    return;
                }

                for (var i = 0; i < _instantiateObjects.Count; i++)
                {
                    if (_instantiateObjects[i] == null)
                    {
                        continue;
                    }

#if UNITY_EDITOR
                    DestroyImmediate(_instantiateObjects[i]);
#else
                    Destroy(child.gameObject);
#endif
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_pointSize <= 0 || _points.Length < _pointSize)
            {
                return;
            }

            for (var i = 0; i < _pointSize; i++)
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
                        Gizmos.DrawLine(Vector3.zero, Vector3.right * scale * GIZMOS_LINE_BASE_LENGTH);

                        // Draw the y-axis in green
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(Vector3.zero, Vector3.up * scale * GIZMOS_LINE_BASE_LENGTH);

                        // Draw the z-axis in blue
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(Vector3.zero, Vector3.forward * scale * GIZMOS_LINE_BASE_LENGTH);
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

                        Gizmos.DrawSphere(point.matrix.c3.xyz, GIZMOS_LINE_BASE_LENGTH / 2.0f);
                        break;

                    default:
                        break;
                }

            }
        }
    }
}
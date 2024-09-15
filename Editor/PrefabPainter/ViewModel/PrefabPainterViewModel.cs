using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Misaki.ArtToolEditor
{
    public class PrefabPainterViewModel
    {
        private static PrefabPainterViewModel _instance;
        public static PrefabPainterViewModel Instance
        {
            get
            {
                _instance ??= new PrefabPainterViewModel();
                return _instance;
            }
        }

        private const string NamePrefix = "_(PainterInstance)";

        public PaintType CurrentPaintType = PaintType.Disable;

        public List<SourcePrefab> SourcePrefabs = new();
        public bool RandomOrder = true;

        public SourcePrefab CurrentSelection = new();

        public BrustSetting CurrentBrushSetting = new();

        private List<GameObject> paintedObjects;

        // Use a grid to store painted objects for faster overlap checking
        private const int GridSize = 5;
        private Dictionary<Vector3, List<GameObject>> _grid = new Dictionary<Vector3, List<GameObject>>();

        private Vector3 _lastBrushPosition = Vector3.zero;
        private double _lastBrushApplication = 0;
        private int _paintIndex;

        public void OnNavigatedTo()
        {
            paintedObjects = new List<GameObject>();

            var count = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (var i = 0; i < count; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    paintedObjects.AddRange(Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(go => go.name.Contains(NamePrefix)));
                }
            }

            // Add to grid
            foreach (var go in paintedObjects)
            {
                var gridPosition = new Vector3(
                    Mathf.Floor(go.transform.position.x / GridSize) * GridSize,
                    Mathf.Floor(go.transform.position.y / GridSize) * GridSize,
                    Mathf.Floor(go.transform.position.z / GridSize) * GridSize
                );

                if (!_grid.ContainsKey(gridPosition))
                {
                    _grid[gridPosition] = new List<GameObject>();
                }

                _grid[gridPosition].Add(go);
            }
        }

        public void DrawBrush()
        {
            if (CurrentPaintType == PaintType.Disable)
            {
                return;
            }

            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, CurrentBrushSetting.PaintLayer))
            {
                Handles.color = Color.blue;
                Handles.DrawWireDisc(hit.point, hit.normal, CurrentBrushSetting.BrushSize);

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0 && !Event.current.alt)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                    switch (CurrentPaintType)
                    {
                        case PaintType.Paint:
                            PaintPrefabs(hit.point, hit.normal);
                            break;
                        case PaintType.Erase:
                            ErasePrefabs(hit.point);
                            break;
                        default:
                            break;
                    }
                    Event.current.Use();
                }
            }
        }

        private void PaintPrefabs(Vector3 position, Vector3 normal)
        {

            if (CurrentBrushSetting.Density <= 0 || Vector3.Distance(position, _lastBrushPosition) < CurrentBrushSetting.SplatSpacing)
            {
                return;
            }

            var activePrefabs = SourcePrefabs.FindAll(prefab => prefab.Enabled);
            if (!activePrefabs.Any())
            {
                return;
            }

            // Prevent painting too frequently
            if ((EditorApplication.timeSinceStartup - _lastBrushApplication) < Mathf.Max(0.016f, (1f - CurrentBrushSetting.Density)))
            {
                return;
            }

            _lastBrushApplication = EditorApplication.timeSinceStartup;

            var useableArea = CurrentBrushSetting.BrushSize * CurrentBrushSetting.BrushSize * Mathf.PI * CurrentBrushSetting.Density;

            if (CurrentBrushSetting.AvoidOverlap)
            {
                var gridPos = new Vector3(
                    Mathf.Floor(position.x / GridSize) * GridSize,
                    Mathf.Floor(position.y / GridSize) * GridSize,
                    Mathf.Floor(position.z / GridSize) * GridSize
                );

                if (_grid.TryGetValue(gridPos, out var objectList))
                {
                    var count = objectList.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var go = objectList[i];

                        if (go == null)
                        {
                            objectList.RemoveAt(i);
                            continue;
                        }

                        if (Vector3.Distance(go.transform.position, position) < CurrentBrushSetting.BrushSize)
                        {
                            if (BoundUtilities.GetSphereBounds(go, out var spBound))
                            {
                                useableArea -= spBound.radius * spBound.radius * Mathf.PI;
                            }
                        }
                    }
                }
            }

            while (useableArea > 0)
            {
                // Select a prefab
                var totalFrequency = activePrefabs.Sum(prefab => prefab.Frequency);

                var index = 0;
                if (RandomOrder)
                {
                    var randomValue = Random.Range(0, totalFrequency);
                    var cumulativeFrequency = 0.0f;

                    for (var i = 0; i < activePrefabs.Count; i++)
                    {
                        cumulativeFrequency += activePrefabs[i].Frequency;
                        if (randomValue < cumulativeFrequency)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    _paintIndex = (_paintIndex + 1) % activePrefabs.Count;
                    index = _paintIndex;
                }

                var selectedPrefab = activePrefabs[index];

                // Filter by slope
                if (Vector3.Angle(normal, Vector3.up) > selectedPrefab.SlopeFilter)
                {
                    return;
                }

                // Randomize position, rotation, and scale
                var randomOffset2D = Random.insideUnitSphere * CurrentBrushSetting.BrushSize;
                var randomOffset = new Vector3(randomOffset2D.x, 0, randomOffset2D.y);
                randomOffset = Vector3.ProjectOnPlane(randomOffset, normal);

                Vector3 right, forward;
                if (Vector3.Dot(normal, Vector3.up) > 0.999f)
                {
                    right = Vector3.Cross(Vector3.forward, normal);
                    forward = Vector3.Cross(normal, right);
                }
                else
                {
                    right = Vector3.Cross(Vector3.up, normal);
                    forward = Vector3.Cross(normal, right);
                }

                var rotation = Quaternion.LookRotation(forward, normal);

                var scale = Vector3.one;

                if ((selectedPrefab.Randomness & RandomnessType.Position) != 0)
                {
                    randomOffset += new Vector3(
                        Random.Range(selectedPrefab.PositionMin.x, selectedPrefab.PositionMax.x),
                        Random.Range(selectedPrefab.PositionMin.y, selectedPrefab.PositionMax.y),
                        Random.Range(selectedPrefab.PositionMin.z, selectedPrefab.PositionMax.z)
                    );
                }

                if ((selectedPrefab.Randomness & RandomnessType.Rotation) != 0)
                {
                    rotation *= Quaternion.Euler(
                        Random.Range(selectedPrefab.RotationMin.x, selectedPrefab.RotationMax.x),
                        Random.Range(selectedPrefab.RotationMin.y, selectedPrefab.RotationMax.y),
                        Random.Range(selectedPrefab.RotationMin.z, selectedPrefab.RotationMax.z)
                    );
                }

                if ((selectedPrefab.Randomness & RandomnessType.Scale) != 0)
                {
                    scale = new Vector3(
                        Random.Range(selectedPrefab.ScaleMin.x, selectedPrefab.ScaleMax.x),
                        Random.Range(selectedPrefab.ScaleMin.y, selectedPrefab.ScaleMax.y),
                        Random.Range(selectedPrefab.ScaleMin.z, selectedPrefab.ScaleMax.z)
                    );
                }

                var finalPosition = position + randomOffset;
                var gridPosition = new Vector3(
                    Mathf.Floor(finalPosition.x / GridSize) * GridSize,
                    Mathf.Floor(finalPosition.y / GridSize) * GridSize,
                    Mathf.Floor(finalPosition.z / GridSize) * GridSize
                );

                // Avoid overlap
                if (CurrentBrushSetting.AvoidOverlap)
                {
                    var shouldSkip = false;

                    if (_grid.TryGetValue(gridPosition, out var objectList))
                    {
                        var count = objectList.Count;
                        for (var i = 0; i < count; i++)
                        {
                            var go = objectList[i];

                            if (go == null)
                            {
                                paintedObjects.RemoveAt(i);
                                continue;
                            }

                            if (Vector3.Distance(go.transform.position, finalPosition) < selectedPrefab.ObjectSpacing * 2)
                            {
                                shouldSkip = true;
                                break;
                            }
                        }
                    }

                    if (shouldSkip)
                    {
                        return;
                    }
                }

                if (!_grid.ContainsKey(gridPosition))
                {
                    _grid[gridPosition] = new List<GameObject>();
                }

                // Instantiate and place the prefab
                var gameObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab.Prefab);

                gameObject.name = selectedPrefab.Prefab.name + NamePrefix;

                gameObject.transform.position = finalPosition;
                gameObject.transform.rotation = rotation;
                gameObject.transform.localScale = scale;

                paintedObjects.Add(gameObject);
                _grid[gridPosition].Add(gameObject);

                useableArea -= selectedPrefab.ObjectSpacing * selectedPrefab.ObjectSpacing * Mathf.PI;

                _lastBrushPosition = position;
            }
        }

        private void ErasePrefabs(Vector3 position)
        {
            var count = paintedObjects.Count;

            for (var i = count - 1; i >= 0; i--)
            {
                var go = paintedObjects[i];

                if (go == null)
                {
                    paintedObjects.RemoveAt(i);
                    continue;
                }

                if (Vector3.Distance(go.transform.position, position) < CurrentBrushSetting.BrushSize)
                {
                    paintedObjects.RemoveAt(i);

                    var gridPosition = new Vector3(
                        Mathf.Floor(go.transform.position.x / GridSize) * GridSize,
                        Mathf.Floor(go.transform.position.y / GridSize) * GridSize,
                        Mathf.Floor(go.transform.position.z / GridSize) * GridSize
                    );

                    if (_grid.TryGetValue(gridPosition, out var objectList))
                    {
                        objectList.Remove(go);
                    }

                    Object.DestroyImmediate(go);
                }
            }
        }
    }
}
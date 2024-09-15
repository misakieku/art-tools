using Misaki.ArtToolEditor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PrefabPainterWindow : EditorWindow
{
    public PrefabPainterWindow()
    {
        ViewModel = PrefabPainterViewModel.Instance;
    }

    public PrefabPainterViewModel ViewModel;

    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    [SerializeField]
    private VisualTreeAsset listViewItemTemplate = default;

    [MenuItem("Tools/Prefab Painter")]
    public static void ShowExample()
    {
        var wnd = GetWindow<PrefabPainterWindow>();
        var icon = EditorGUIUtility.IconContent("Prefab Icon");
        wnd.titleContent = new GUIContent("Prefab Painter", icon.image);
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;

        VisualElement visualAsset = visualTreeAsset.Instantiate();

        var visualAssetRoot = visualAsset.Q<VisualElement>("Root");
        visualAssetRoot.dataSource = this;

        SetupModeButton(visualAsset);
        SetupListView(visualAsset);

        root.Add(visualAsset);
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        ViewModel.OnNavigatedTo();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        ViewModel.DrawBrush();
    }

    private void SetupModeButton(VisualElement visualAsset)
    {
        var disableModeButton = visualAsset.Q<Button>("Disable-Mode-Button");
        var paintModeButton = visualAsset.Q<Button>("Paint-Mode-Button");
        var eraseModeButton = visualAsset.Q<Button>("Erase-Mode-Button");

        disableModeButton.clicked += () =>
        {
            disableModeButton.AddToClassList("Selected");
            paintModeButton.RemoveFromClassList("Selected");
            eraseModeButton.RemoveFromClassList("Selected");

            ViewModel.CurrentPaintType = PaintType.Disable;
        };

        paintModeButton.clicked += () =>
        {
            paintModeButton.AddToClassList("Selected");
            disableModeButton.RemoveFromClassList("Selected");
            eraseModeButton.RemoveFromClassList("Selected");

            ViewModel.CurrentPaintType = PaintType.Paint;
        };

        eraseModeButton.clicked += () =>
        {
            eraseModeButton.AddToClassList("Selected");
            disableModeButton.RemoveFromClassList("Selected");
            paintModeButton.RemoveFromClassList("Selected");

            ViewModel.CurrentPaintType = PaintType.Erase;
        };
    }

    private void SetupListView(VisualElement visualAsset)
    {
        var listView = visualAsset.Q<ListView>("SourcePrefabs-ListView");
        var PrefabSettingSection = visualAsset.Q<VisualElement>("Prefab-Setting");
        PrefabSettingSection.style.display = DisplayStyle.None;

        listView.makeItem = () => listViewItemTemplate.CloneTree();

        listView.selectionChanged += objects =>
        {
            if (objects.Count() <= 0)
            {
                return;
            }

            if (objects.First() is not SourcePrefab sourcePrefab)
            {
                return;
            }

            ViewModel.CurrentSelection = sourcePrefab;
            PrefabSettingSection.style.display = DisplayStyle.Flex;
        };

        listView.bindItem = (element, i) =>
        {
            var objectField = element.Q<ObjectField>();
            objectField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue == null)
                {
                    return;
                }

                var icon = AssetPreview.GetAssetPreview(evt.newValue);
                while (icon == null)
                {
                    icon = AssetPreview.GetAssetPreview(evt.newValue);
                }
                ViewModel.SourcePrefabs[i].Icon = icon;
            });
        };

        listView.itemsAdded += (items) =>
        {
            for (var i = 0; i < items.Count(); i++)
            {
                var index = listView.itemsSource.Count - i - 1;
                ViewModel.SourcePrefabs[index] = new SourcePrefab();
            }

            listView.Rebuild();
        };

        listView.itemsRemoved += (items) =>
        {
            if (ViewModel.SourcePrefabs.Count - items.Count() <= 0)
            {
                PrefabSettingSection.style.display = DisplayStyle.None;
                return;
            }

            listView.Rebuild();
        };

        listView.itemIndexChanged += (from, to) =>
        {
            listView.Rebuild();
        };
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Misaki.ArtToolEditor
{
    public enum ObjectSearchTarget { MeshRender, Light };

    public class RenderingLayerSwitcher : EditorWindow
    {
        private RenderingLayerMask _renderingLayerMask;
        private ObjectSearchTarget _searchTarget;
        private bool _includeChildren;

        private List<Component> _selectionList = new();

        [MenuItem("Tools/Rendering Layer Switcher")]
        public static void ShowEditor()
        {
            EditorWindow window = GetWindow<RenderingLayerSwitcher>();
            window.titleContent = new GUIContent("Rendering Layer Switcher");
        }

        public void OnGUI()
        {
            _renderingLayerMask = EditorGUILayout.RenderingLayerMaskField("Rendering Layer Mask:", _renderingLayerMask);
            _searchTarget = (ObjectSearchTarget)EditorGUILayout.EnumPopup("Search Target:", _searchTarget);
            EditorGUILayout.LabelField("Selections", Selection.gameObjects.Length.ToString());
            _includeChildren = EditorGUILayout.Toggle("Include Children:", _includeChildren);

            if (GUILayout.Button("Change"))
            {
                _selectionList.Clear();

                switch (_searchTarget)
                {
                    case ObjectSearchTarget.MeshRender:
                        _selectionList.AddRange(Selection.GetFiltered<MeshRenderer>(SelectionMode.Editable | (_includeChildren ? SelectionMode.Deep : SelectionMode.TopLevel)));
                        break;

                    case ObjectSearchTarget.Light:
                        _selectionList.AddRange(Selection.GetFiltered<Light>(SelectionMode.Editable | (_includeChildren ? SelectionMode.Deep : SelectionMode.TopLevel)));
                        break;
                }

                Undo.RecordObjects(_selectionList.ToArray(), "Change Rendering Layer Mask");
                SetRenderingLayerMask(_selectionList);
            }
        }

        private void SetRenderingLayerMask<T>(IEnumerable<T> selection) where T : Component
        {
            if (selection == null)
                return;

            foreach (var obj in selection)
            {
                var componentsArray = _includeChildren ? obj.GetComponentsInChildren<T>() : obj.GetComponents<T>();

                foreach (var component in componentsArray)
                {
                    switch (component)
                    {
                        case MeshRenderer renderer:
                            renderer.renderingLayerMask = _renderingLayerMask;
                            break;

                        case Light light:
                            light.renderingLayerMask = _renderingLayerMask;
                            break;
                    }
                }
            }

        }
    }
}

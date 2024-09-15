using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Misaki.ArtToolEditor
{
    public enum ReceiveGiMode { LightProbes, Lightmaps };
    public enum ObjectSearchMode { Scene, Selection };

    public class ReceiveGiModeSwitcher : EditorWindow
    {
        private ReceiveGiMode _receiveGiMode;
        private ObjectSearchMode _searchMode;
        private int _selectedSceneIndex = 0;
        private bool _includeChildren;
        private bool _setAllToStatic;

        private string[] _sceneArray;
        private readonly List<MeshRenderer> _selectionList = new();

        [MenuItem("Tools/Receive GI Mode Switcher")]
        public static void ShowEditor()
        {
            EditorWindow window = GetWindow<ReceiveGiModeSwitcher>();
            window.titleContent = new GUIContent("Receive GI Mode Switcher");
        }

        private void OnEnable()
        {
            _sceneArray = new string[UnityEngine.SceneManagement.SceneManager.sceneCount];
            for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                _sceneArray[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name;
            }
        }

        public void OnGUI()
        {
            _receiveGiMode = (ReceiveGiMode)EditorGUILayout.EnumPopup("Lighting Mode:", _receiveGiMode);
            _searchMode = (ObjectSearchMode)EditorGUILayout.EnumPopup("Search Mode:", _searchMode);

            switch (_searchMode)
            {
                case ObjectSearchMode.Scene:
                    _selectedSceneIndex = EditorGUILayout.Popup("Scene:", _selectedSceneIndex, _sceneArray);
                    break;
                case ObjectSearchMode.Selection:
                    EditorGUILayout.LabelField("Selection:", Selection.objects.Length.ToString());
                    break;
            }

            _includeChildren = EditorGUILayout.Toggle("Include Children:", _includeChildren);
            _setAllToStatic = EditorGUILayout.Toggle("Change all to static mesh:", _setAllToStatic);

            if (GUILayout.Button("Change"))
            {
                _selectionList.Clear();

                switch (_searchMode)
                {
                    case ObjectSearchMode.Scene:
                        var go = UnityEngine.SceneManagement.SceneManager.GetSceneAt(_selectedSceneIndex).GetRootGameObjects();

                        for (var i = 0; i < go.Length; i++)
                        {
                            _selectionList.AddRange(_includeChildren ? go[i].GetComponentsInChildren<MeshRenderer>() : go[i].GetComponents<MeshRenderer>());
                        }
                        break;

                    case ObjectSearchMode.Selection:
                        _selectionList.AddRange(Selection.GetFiltered<MeshRenderer>(SelectionMode.Editable | (_includeChildren ? SelectionMode.Deep : SelectionMode.TopLevel)));
                        break;
                }

                Undo.RecordObjects(_selectionList.ToArray(), "Receive GI Mode Switcher");

                SetReceiveGiMode();
            }
        }

        private void SetReceiveGiMode()
        {
            if (_selectionList.Count <= 0)
                return;

            for (var m = 0; m < _selectionList.Count; m++)
            {
                if (_setAllToStatic == true)
                    GameObjectUtility.SetStaticEditorFlags(_selectionList[m].gameObject, StaticEditorFlags.ContributeGI);

                if (_receiveGiMode == ReceiveGiMode.LightProbes)
                    _selectionList[m].receiveGI = ReceiveGI.LightProbes;
                else if (_receiveGiMode == ReceiveGiMode.Lightmaps)
                    _selectionList[m].receiveGI = ReceiveGI.Lightmaps;
            }
        }
    }
}

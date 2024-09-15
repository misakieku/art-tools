using Misaki.ArtTool;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [CustomEditor(typeof(Cloner))]
    public class ClonerEditor : Editor
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset = default;

        private Cloner dataSource;

        private void OnEnable()
        {
            dataSource = target as Cloner;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var visualAsset = visualTreeAsset.Instantiate();
            visualAsset.dataSource = dataSource;

            var effectorListView = visualAsset.Q<ListView>("effectors-listview");
            //effectorListView.itemsAdded += EffectorListView_itemsAdded;
            effectorListView.itemsRemoved += EffectorListView_itemsRemoved;
            effectorListView.Q<ObjectField>().RegisterValueChangedCallback(e =>
            {
                if (e.newValue is EffectorBase effector)
                {
                    effector.propertyChanged += dataSource.OnPropertyChanged;
                }
                else if (e.previousValue is EffectorBase oldEffector)
                {
                    oldEffector.propertyChanged -= dataSource.OnPropertyChanged;
                }
            });

            visualAsset.Q<Button>("generate-button").clicked += () =>
            {
                dataSource.GeneratePoints();
            };

            visualAsset.Q<Button>("instantiate-button").clicked += () =>
            {
                dataSource.InstantiateGameObjectOnPoints();
            };

            visualAsset.Q<Button>("clear-button").clicked += () =>
            {
                dataSource.Clear();
            };

            var renderModeDropdown = visualAsset.Q<DropdownField>("render-mode-dropdown");
            renderModeDropdown.RegisterValueChangedCallback(e =>
            {
                if (renderModeDropdown.index == Convert.ToInt32(dataSource.isRenderInstancing))
                {
                    return;
                }

                dataSource.Clear(false, true);
            });

            root.Add(visualAsset);

            root.TrackSerializedObjectValue(serializedObject, OnValueChanged);

            return root;
        }

        private void OnValueChanged(SerializedObject serializedObject)
        {
            var cloner = serializedObject.targetObject as Cloner;

            if (cloner.autoGenerate)
            {
                cloner.GeneratePoints();
                cloner.InstantiateGameObjectOnPoints();
            }
        }

        //private void EffectorListView_itemsAdded(IEnumerable<int> items)
        //{
        //    foreach (var index in items)
        //    {
        //        dataSource.effectors[index].effector.propertyChanged += dataSource.OnPropertyChanged;
        //    }
        //}

        private void EffectorListView_itemsRemoved(IEnumerable<int> items)
        {
            foreach (var index in items)
            {
                var effector = dataSource.effectors[index].effector;
                if (effector == null)
                {
                    return;
                }

                effector.propertyChanged -= dataSource.OnPropertyChanged;
            }
        }
    }
}

using Misaki.ArtTool;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [CustomEditor(typeof(LinearField))]
    public class LinearFieldEditor : Editor
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset = default;

        private LinearField dataSource;

        private void OnEnable()
        {
            dataSource = target as LinearField;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var visualAsset = visualTreeAsset.Instantiate();
            visualAsset.dataSource = serializedObject.targetObject;

            root.Add(visualAsset);
            root.TrackSerializedObjectValue(serializedObject, OnValueChanged);

            return root;
        }

        private void OnValueChanged(SerializedObject serializedObject)
        {
            var linearField = serializedObject.targetObject as LinearField;
            linearField.propertyChanged.Invoke(linearField, null);
        }
    }
}
using Misaki.ArtTool;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [CustomEditor(typeof(PlainEffector))]
    public class PlainEffectorEditor : Editor
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset = default;

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
            var plainEffector = serializedObject.targetObject as PlainEffector;
            plainEffector.propertyChanged.Invoke(plainEffector, null);
        }
    }
}
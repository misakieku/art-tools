using Misaki.ArtTool;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [CustomEditor(typeof(RandomEffector))]
    public class RandomEffectorEditor : Editor
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
            var randomEffector = serializedObject.targetObject as RandomEffector;
            randomEffector.propertyChanged.Invoke(randomEffector, null);
        }
    }
}
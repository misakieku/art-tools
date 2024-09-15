using Misaki.ArtTool;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misaki.ArtToolEditor
{
    [CustomEditor(typeof(SphereField))]
    public class SphereFieldEditor : Editor
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset = default;

        private SphereField dataSource;

        private void OnEnable()
        {
            dataSource = target as SphereField;
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
            var linearField = serializedObject.targetObject as SphereField;
            linearField.propertyChanged.Invoke(linearField, null);
        }

        private void OnSceneGUI()
        {
            Handles.matrix = dataSource.transform.localToWorldMatrix;
            Handles.color = Color.cyan;

            dataSource.radius = Handles.RadiusHandle(Quaternion.identity, Vector3.zero, dataSource.radius, true);
        }
    }
}

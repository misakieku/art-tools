using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Misaki.ArtToolEditor
{
    [CreateAssetMenu(fileName = "LightPresetDatabase", menuName = "Lighting Tools/LightPresetDatabase")]
    public class LightPresetDatabase : ScriptableObject
    {
        public List<LightPreset> lightPreset = new();
    }

    [CustomEditor(typeof(LightPresetDatabase))]
    public class LightPresetDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Refresh Menu Item"))
            {
                LightPresetMenu.Initialize();
            }
        }
    }
}
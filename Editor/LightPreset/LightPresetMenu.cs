using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Misaki.ArtToolEditor
{
    public static class LightPresetMenu
    {
        private const string DatabasePath = "Packages/com.misaki.art-tools/Editor/LightPreset/LightPresetDatabase.asset";
        private static LightPresetDatabase database;

        private readonly static Type menuType = typeof(Menu);

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            RemoveMenuItems();

            database = AssetDatabase.LoadAssetAtPath<LightPresetDatabase>(DatabasePath);
            if (database == null)
            {
                Debug.LogError("LightPresetDatabase not found at path: " + DatabasePath);
                return;
            }

            var addMenuItemMethod = menuType.GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var lightPreset in database.lightPreset)
            {
                AddMenuItem(addMenuItemMethod, lightPreset);
            }
        }

        private static void RemoveMenuItems()
        {
            var removeMenuItemMethod = menuType.GetMethod("RemoveMenuItem", BindingFlags.NonPublic | BindingFlags.Static);
            if (removeMenuItemMethod == null)
            {
                return;
            }

            removeMenuItemMethod.Invoke(null, new object[] { "GameObject/Light/LightPreset" });
        }

        private static void AddMenuItem(MethodInfo addMenuItemMethod, LightPreset lightPreset)
        {
            if (addMenuItemMethod == null)
            {
                return;
            }

            var name = "GameObject/Light/LightPreset/" + lightPreset.Name;
            var shortcut = "";
            var @checked = false;
            var priority = 100;
            var execute = new Action(() => CreateLight(lightPreset));
            var validate = new Func<bool>(() => true);

            addMenuItemMethod.Invoke(null, new object[] { name, shortcut, @checked, priority, execute, validate });
        }

        private static void CreateLight(LightPreset lightPreset)
        {
            Light light;

            if (Selection.activeGameObject == null)
                light = UnityEngine.Object.Instantiate(lightPreset.Light, Vector3.zero, Quaternion.identity);
            else
                light = UnityEngine.Object.Instantiate(lightPreset.Light, Selection.activeGameObject.transform.position, Quaternion.identity, Selection.activeGameObject.transform);

            var parent = light.transform.parent;
            string[] nameArray;
            UnityEngine.Object[] childrenArray;

            if (parent == null)
            {
                childrenArray = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                nameArray = new string[childrenArray.Length];
            }
            else
            {
                nameArray = new string[parent.childCount];
                childrenArray = new UnityEngine.Object[parent.childCount];
                for (var i = 0; i < parent.childCount; i++)
                {
                    childrenArray[i] = parent.GetChild(i).gameObject;
                }
            }

            for (var i = 0; i < nameArray.Length; i++)
            {
                nameArray[i] = childrenArray[i].name;
            }

            light.name = ObjectNames.GetUniqueName(nameArray, lightPreset.Name);
            Selection.activeGameObject = light.gameObject;

            Undo.RegisterCreatedObjectUndo(light, "Create " + lightPreset.Name);
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace FLGCoreEditor.Utilities
{
    public class FindMissingScriptsRecursivelyAndRemove : EditorWindow
    {
        private static int _goCount;
        private static int _componentsCount;
        private static int _missingCount;

        private static bool _bHaveRun;

        [MenuItem("FLGCore/Editor/Utility/FindMissingScriptsRecursivelyAndRemove")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursivelyAndRemove));
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }

            if (GUILayout.Button("Remove Missing Mono Scripts from Selected GameObjects")) // Updated Button Text
            {
                RemoveMissingMonoScripts();
            }

            if (!_bHaveRun) return;

            EditorGUILayout.TextField($"{_goCount} GameObjects Selected");
            if (_goCount > 0) EditorGUILayout.TextField($"{_componentsCount} Components");
            if (_goCount > 0) EditorGUILayout.TextField($"{_missingCount} Deleted");
        }

        private static void FindInSelected()
        {
            var go = Selection.gameObjects;
            _goCount = 0;
            _componentsCount = 0;
            _missingCount = 0;
            foreach (var g in go)
            {
                FindInGo(g);
            }

            _bHaveRun = true;
            Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");

            AssetDatabase.SaveAssets();
        }

        private static void FindInGo(GameObject g)
        {
            _goCount++;
            var components = g.GetComponents<Component>();

            var r = 0;

            for (var i = 0; i < components.Length; i++)
            {
                _componentsCount++;
                if (components[i] != null) continue;
                _missingCount++;
                var s = g.name;
                var t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }

                Debug.Log($"{s} has a missing script at {i}", g);

                var serializedObject = new SerializedObject(g);

                var prop = serializedObject.FindProperty("m_Component");

                prop.DeleteArrayElementAtIndex(i - r);
                r++;

                serializedObject.ApplyModifiedProperties();
            }

            foreach (Transform childT in g.transform)
            {
                FindInGo(childT.gameObject);
            }
        }

        private static void RemoveMissingMonoScripts()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No GameObjects selected for removal of missing Mono scripts.");
                return;
            }

            _goCount = 0;
            _componentsCount = 0;
            _missingCount = 0;

            foreach (GameObject obj in selectedObjects)
            {
                _goCount++;
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty prop = serializedObject.FindProperty("m_Component");

                int removedCount = 0;

                for (int i = prop.arraySize - 1; i >= 0; i--)
                {
                    SerializedProperty componentProp = prop.GetArrayElementAtIndex(i);
                    Object component = componentProp.objectReferenceValue;

                    if (component == null) // If the Mono script is missing
                    {
                        prop.DeleteArrayElementAtIndex(i);
                        removedCount++;
                    }
                }

                serializedObject.ApplyModifiedProperties();
                _missingCount += removedCount;
                Debug.Log($"Removed {removedCount} missing Mono scripts from {obj.name}");
            }

            Debug.Log($"Removed a total of {_missingCount} missing Mono scripts from {_goCount} GameObjects.");
        }
    }
}

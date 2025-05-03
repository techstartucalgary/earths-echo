using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShaderTracker : EditorWindow
{
    string shaderName = "Hidden/Universal/HDRDebugView";

    [MenuItem("Tools/Find Shader References")]
    public static void ShowWindow()
    {
        GetWindow<ShaderTracker>("Shader Reference Finder");
    }

    void OnGUI()
    {
        shaderName = EditorGUILayout.TextField("Shader Name", shaderName);

        if (GUILayout.Button("Find Assets Using Shader"))
        {
            string[] allPaths = AssetDatabase.GetAllAssetPaths();
            List<string> results = new List<string>();

            foreach (string path in allPaths)
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset is Material mat && mat.shader != null && mat.shader.name == shaderName)
                {
                    results.Add(path);
                }
            }

            if (results.Count == 0)
                Debug.Log("✅ No references found.");
            else
                foreach (var r in results)
                    Debug.Log($"❗ Shader reference found: {r}");
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.IO;

public class FindShadersInProject : EditorWindow
{
    private string shaderName = "WFX/Transparent Diffuse";

    [MenuItem("Tools/Find Materials Using Shader")]
    public static void ShowWindow()
    {
        GetWindow<FindShadersInProject>("Find Shader Usage");
    }

    private void OnGUI()
    {
        shaderName = EditorGUILayout.TextField("Shader Name", shaderName);

        if (GUILayout.Button("Find Materials"))
        {
            FindMaterialsUsingShader(shaderName);
        }
    }

    private static void FindMaterialsUsingShader(string targetShaderName)
    {
        Shader targetShader = Shader.Find(targetShaderName);
        if (targetShader == null)
        {
            Debug.LogError("Shader not found: " + targetShaderName);
            return;
        }

        string[] allMaterialGUIDs = AssetDatabase.FindAssets("t:Material");

        foreach (string guid in allMaterialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader == targetShader)
            {
                Debug.Log($"Material using {targetShaderName}: {path}", mat);
            }
        }
    }
}

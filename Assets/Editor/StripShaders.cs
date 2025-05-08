using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

class StripShaders : IPreprocessShaders
{
    public int callbackOrder => -1000; // Ensure this runs early

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        // Log all shaders being processed with additional details
        Debug.Log($"[ShaderStripper] 🔍 Processing shader: '{shader.name}' (Stage: {snippet.shaderType}, Pass: {snippet.passName})");

        string[] blockedShaders = new[]
        {
            "Hidden/Universal/HDRDebugView",
            "Hidden/VoxelizeShader"
        };

        foreach (string blocked in blockedShaders)
        {
            // Perform case-insensitive comparison
            if (shader.name.IndexOf(blocked, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log($"[ShaderStripper] ❌ Stripping shader: {shader.name}");
                data.Clear();
                return;
            }
        }

        // Log shaders that are not stripped
        Debug.Log($"[ShaderStripper] ✅ Keeping shader: {shader.name}");
    }
}

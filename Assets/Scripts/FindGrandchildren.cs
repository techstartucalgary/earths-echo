using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGrandchildren : MonoBehaviour
{
    public Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindDeepChild(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    string GetFullPathToParent(Transform t, string parentName)
    {
        if (t.parent == null || t.name == parentName)
        {
            return ""; // This is the root.
        }
        // Recursively build the path.
        return GetFullPathToParent(t.parent, parentName) + "/" + t.name;
    }
}

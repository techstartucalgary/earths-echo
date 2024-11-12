using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions {
    public static Vector2 With(this Vector2 vector, float? x = null, float? y = null) {
        return new Vector2(x ?? vector.x, y ?? vector.y);
    }

    public static Vector2 Add(this Vector2 vector, float? x = null, float? y = null) {
        return new Vector2(vector.x + (x ?? 0), vector.y + (y ?? 0));
    }
}

public static class Vector3Extensions {
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
        return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
    }
}

public static class GameObjectExtensions {
    public static T GetOrAdd<T> (this GameObject gameObject) where T : Component {
        T component = gameObject.GetComponent<T>();
        if (!component) component = gameObject.AddComponent<T>();

        return component;
    }

    public static T OrNull<T> (this T obj) where T : Object => obj ? obj : null;

    public static void DestroyChildren(this GameObject gameObject) {
        gameObject.transform.DestroyChildren();
    }

    public static void EnableChildren(this GameObject gameObject) {
        gameObject.transform.EnableChildren();
    }

    public static void DisableChildren(this GameObject gameObject) {
        gameObject.transform.DisableChildren();
    }
}

public static class TransformExtensions {
    public static IEnumerable<Transform> Children(this Transform parent) {
        foreach (Transform child in parent) {
            yield return child;
        }
    }

    public static void DestroyChildren(this Transform parent) {
        parent.PerformActionOnChildren(Children => Object.Destroy(Children.gameObject));
    }

    public static void EnableChildren(this Transform parent) {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(true));
    }

    public static void DisableChildren(this Transform parent) {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(false));
    }

    static void PerformActionOnChildren(this Transform parent, System.Action<Transform> action) {
        for (var i = parent.childCount - 1; i >= 0; i--) {
            action(parent.GetChild(i));
        }
    }
}
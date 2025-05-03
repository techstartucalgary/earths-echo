using UnityEngine;

[DefaultExecutionOrder(-32000)]             // run before almost everything
public class ListenerClampEarly : MonoBehaviour
{
    const float SAFE_COORD = 0f;            // where we teleport NaNs/Infs

    void Update()
    {
        var lis = GetComponent<AudioListener>();
        if (!lis) return;

        var p = lis.transform.position;
        if (float.IsFinite(p.x) && float.IsFinite(p.y) && float.IsFinite(p.z)) return;

        Debug.LogWarning($"[ListenerClampEarly] Non‑finite position {p} → reset.");
        lis.transform.position = new Vector3(
            float.IsFinite(p.x) ? p.x : SAFE_COORD,
            float.IsFinite(p.y) ? p.y : SAFE_COORD,
            float.IsFinite(p.z) ? p.z : SAFE_COORD
        );
    }
}

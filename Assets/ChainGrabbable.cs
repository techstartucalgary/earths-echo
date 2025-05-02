using UnityEngine;

public class ChainGrabbable : MonoBehaviour
{
    [HideInInspector] public FixedJoint2D grabJoint;

    private void Awake()
    {
        if (GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("ChainGrabbable requires a Rigidbody2D.");
        }
    }
}

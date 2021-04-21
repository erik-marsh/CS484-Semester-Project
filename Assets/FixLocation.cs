// Prevents the movement of an object, keeping it in a fixed location.
// Used to keep the VR camera rig in a single location

using UnityEngine;

public class FixLocation : MonoBehaviour
{
    public Vector3 fixedPosition = Vector3.zero;

    void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
        transform.position = Vector3.zero;
    }
}

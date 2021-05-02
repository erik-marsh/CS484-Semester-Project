using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour
{
    public GameObject cameraVR;

    private void Update()
    {
        transform.position = cameraVR.transform.position;
    }
}

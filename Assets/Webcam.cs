using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Webcam : MonoBehaviour
{
    void Start()
    {
        var devices = WebCamTexture.devices;
        foreach (WebCamDevice d in devices)
		{
            Debug.Log(d.name);
		}

        string webcamName = devices[0].name;

        Renderer rend = this.GetComponentInChildren<Renderer>();

        WebCamTexture tex = new WebCamTexture(webcamName);
        rend.material.mainTexture = tex;
        tex.Play();
    }
}

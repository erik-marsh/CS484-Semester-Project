using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Webcam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var devices = WebCamTexture.devices;
        foreach (WebCamDevice d in devices)
		{
            Debug.Log(d.name);
		}

        Renderer rend = this.GetComponentInChildren<Renderer>();

        WebCamTexture tex = new WebCamTexture("e2eSoft iVCam");
        rend.material.mainTexture = tex;
        tex.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;

public class PointerHandler : MonoBehaviour
{
    public static PointerHandler inst;

    public GameObject barrierSphere;
    public Material transparentSphereMaterial;
    public Material opaqueSphereMaterial;

    public GameObject visibilityCube;
    public Vector3 visibilityCubeDormantPosition;

    public SteamVR_LaserPointer laserPointerLeft;
    public SteamVR_LaserPointer laserPointerRight;

    public SteamVR_Action_Boolean positionVisibilityCube;
    public SteamVR_Action_Boolean clearVisibilityCube;
    public SteamVR_Action_Boolean togglePointerLasersLeft;
    public SteamVR_Action_Boolean togglePointerLasersRight;
    public SteamVR_Input_Sources controller;

    public List<GameObject> viewportSquareVerts;
    private Renderer barrierSphereRenderer;

    private bool isFadingInSphere = false;
    private bool isFadingOutSphere = false;
    private float sphereFadeInTimer = 1.0f;
    private float sphereFadeOutTimer = 1.0f;
    public float sphereFadeLength = 1.0f;

    void Awake()
    {
        inst = this;

        laserPointerLeft.PointerIn += PointerInside;
        laserPointerLeft.PointerOut += PointerOutside;
        laserPointerLeft.PointerClick += PointerClick;

        laserPointerRight.PointerIn += PointerInside;
        laserPointerRight.PointerOut += PointerOutside;
        laserPointerRight.PointerClick += PointerClick;

        positionVisibilityCube.AddOnStateDownListener(LeftGripButtonPress, controller);
        clearVisibilityCube.AddOnStateDownListener(TouchpadUpDownCenterPress, controller);
        togglePointerLasersLeft.AddOnStateDownListener(TouchpadLeftRightPress, controller);
        togglePointerLasersRight.AddOnStateDownListener(TouchpadLeftRightPress, controller);

        viewportSquareVerts = new List<GameObject>();
        barrierSphereRenderer = barrierSphere.GetComponent<Renderer>();

        //barrierSphereRenderer.material = transparentSphereMaterial;
    }

    private void Update()
    {
        // lerping wont work

        //if (isFadingInSphere && sphereFadeInTimer < sphereFadeLength)
        //{
        //    Debug.Log("fading in");
        //    sphereFadeInTimer += Time.deltaTime;
        //    barrierSphereRenderer.material.Lerp(transparentSphereMaterial, opaqueSphereMaterial, sphereFadeInTimer / sphereFadeLength);
        //}

        //if (isFadingOutSphere && sphereFadeOutTimer < sphereFadeLength)
        //{
        //    Debug.Log("fading out");
        //    sphereFadeOutTimer += Time.deltaTime;
        //    barrierSphereRenderer.material.Lerp(opaqueSphereMaterial, transparentSphereMaterial, sphereFadeOutTimer / sphereFadeLength);
        //}
    }

    void AddViewportVert(Vector3 pos)
    {
        var newPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newPoint.transform.position = pos;

        viewportSquareVerts.Add(newPoint);

        // count assumed to be 5
        Debug.Log(viewportSquareVerts.Count);
        if (viewportSquareVerts.Count > 4)
        {
            Destroy(viewportSquareVerts[0]);
            viewportSquareVerts[0] = null;

            for (int i = 0; i < 4; i++)
            {
                viewportSquareVerts[i] = viewportSquareVerts[i + 1];
            }

            viewportSquareVerts[4] = null;
            viewportSquareVerts.RemoveAt(4);
        }
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.name);
        if (e.target.name == "BarrierSphere")
        {
            Debug.Log("Cube was clicked");
            Debug.Log(e.hitPoint);
            AddViewportVert(e.hitPoint);
        }
        else if (e.target.name == "InstructionButton")
        {
            Debug.Log("InstructionButton was clicked");
            UIMgr.inst.ChangeUIState(UIState.INSTRUCTIONS);
        }
        else if (e.target.name == "StartButton")
        {
            Debug.Log("StartButton was clicked");
            UIMgr.inst.ChangeUIState(UIState.USER);
        }
        else if (e.target.name == "MainMenuExitButton")
        {
            Debug.Log("MainMenuExitButton was clicked");
            Utils.ExitUnity();
        }
        else if (e.target.name == "InstructionsExitButton")
        {
            Debug.Log("InstructionsExitButton was clicked");
            UIMgr.inst.ChangeUIState(UIState.MAIN_MENU);
        }
        else if (e.target.name == "UserExitButton")
        {
            Debug.Log("UserExitButton was clicked");
            UIMgr.inst.ChangeUIState(UIState.MAIN_MENU);
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "UserUI" || e.target.name == "UserExitButton")
        {
            Debug.Log("Fading in UserUI");
            UIMgr.inst.StartFadingInUserUI();
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.name == "UserUI" || e.target.name == "UserExitButton")
        {
            Debug.Log("Fading out UserUI");
            UIMgr.inst.StartFadingOutUserUI();
        }
    }

    public void LeftGripButtonPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (viewportSquareVerts.Count < 4) return;

        // calculate the centroid of these four points
        int len = viewportSquareVerts.Count;
        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            centroid += viewportSquareVerts[len - 1 - i].transform.position;
        }
        centroid /= 4;

        float radius, polar, elevation;
        CartesianToSpherical(centroid, out radius, out polar, out elevation);
        polar *= Mathf.Rad2Deg;
        elevation *= Mathf.Rad2Deg;

        //Debug.Log("radius: " + radius);
        //Debug.Log("polar: " + polar);
        //Debug.Log("elevation: " + elevation);

        //possible rotation
        // x = elevation
        // y = -90 - polar if polar < 0
        //     90 - polar if polar  > 0
        Vector3 eulers = new Vector3(elevation, -90 - polar, 0);
        visibilityCube.transform.localPosition = centroid;
        visibilityCube.transform.localEulerAngles = eulers;

        barrierSphereRenderer.material = opaqueSphereMaterial;
        //isFadingInSphere = true;
        //isFadingOutSphere = false;
        //sphereFadeInTimer = 0.0f;
    }

    public void TouchpadUpDownCenterPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // remove all spheres and re-enable transparancy
        ClearVerts();

        barrierSphereRenderer.material = transparentSphereMaterial;
        //isFadingInSphere = false;
        //isFadingOutSphere = true;
        //sphereFadeOutTimer = 0.0f;
        visibilityCube.transform.position = visibilityCubeDormantPosition;
    }

    public void ClearVerts()
    {
        for (int i = 0; i < viewportSquareVerts.Count; i++)
        {
            GameObject.Destroy(viewportSquareVerts[i]);
            viewportSquareVerts[i] = null;
        }
        viewportSquareVerts.Clear();
    }

    public void TouchpadLeftRightPress(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

    }

    // from https://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/
    public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
    {
        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                        + (cartCoords.y * cartCoords.y)
                        + (cartCoords.z * cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            outPolar += Mathf.PI;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
    }

    //public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    //{
    //    float a = radius * Mathf.Cos(elevation);
    //    outCart.x = a * Mathf.Cos(polar);
    //    outCart.y = radius * Mathf.Sin(elevation);
    //    outCart.z = a * Mathf.Sin(polar);
    //}
}
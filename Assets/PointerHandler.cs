/* SceneHandler.cs*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;

public class PointerHandler : MonoBehaviour
{
    public GameObject barrierSphere;
    public Material transparentSphereMaterial;
    public Material opaqueSphereMaterial;

    public GameObject visibilityCube;
    public Vector3 visibilityCubeDormantPosition;

    public SteamVR_LaserPointer laserPointer;

    public SteamVR_Action_Boolean positionVisibilityCube;
    public SteamVR_Input_Sources leftHand;

    public SteamVR_Action_Boolean clearVisibilityCube;
    public SteamVR_Input_Sources rightHand;

    private List<GameObject> viewportSquareVerts;
    private Renderer barrierSphereRenderer;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;

        positionVisibilityCube.AddOnStateDownListener(LeftGripButtonDown, leftHand);
        clearVisibilityCube.AddOnStateDownListener(RightGripButtonDown, rightHand);

        viewportSquareVerts = new List<GameObject>();
        barrierSphereRenderer = barrierSphere.GetComponent<Renderer>();

        barrierSphereRenderer.material = transparentSphereMaterial;
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
            GameObject.Destroy(viewportSquareVerts[0]);
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
            Utils.ExitUnity();
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        //if (e.target.name == "BarrierSphere")
        //{
        //    Debug.Log("Cube was entered");
        //}
        //else if (e.target.name == "Button")
        //{
        //    Debug.Log("Button was entered");
        //}
        if (e.target.name == "UserUI")
		{
            Debug.Log("Fading in UserUI");
		}
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        //if (e.target.name == "BarrierSphere")
        //{
        //    Debug.Log("Cube was exited");
        //}
        //else if (e.target.name == "Button")
        //{
        //    Debug.Log("Button was exited");
        //}
    }

    public void LeftGripButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
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

        Color color = new Color(46, 46, 46, 255);
        barrierSphereRenderer.material = opaqueSphereMaterial;
    }

    public void RightGripButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // remove all spheres and re-enable transparancy
        for (int i = 0; i < viewportSquareVerts.Count; i++)
        {
            GameObject.Destroy(viewportSquareVerts[i]);
            viewportSquareVerts[i] = null;
        }
        viewportSquareVerts.Clear();

        barrierSphereRenderer.material = transparentSphereMaterial;
        visibilityCube.transform.position = visibilityCubeDormantPosition;
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
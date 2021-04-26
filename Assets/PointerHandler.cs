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
    public SteamVR_LaserPointer laserPointer;
    public GameObject spawnSphere;

    public SteamVR_Action_Boolean positionVisibilityCube;
    public SteamVR_Input_Sources handType;

    //private List<Vector3> viewportSquareVerts;
    //private List<GameObject>
    private List<GameObject> viewportSquareVerts;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;

        positionVisibilityCube.AddOnStateDownListener(GripButtonDown, handType);

        //viewportSquareVerts = new List<Vector3>();
        viewportSquareVerts = new List<GameObject>();
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
            //viewportSquareVerts.Add(e.hitPoint);
            AddViewportVert(e.hitPoint);
            //Instantiate(spawnSphere, e.hitPoint, Quaternion.identity);
        }
        else if (e.target.name == "Button")
        {
            Debug.Log("Button was clicked");
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

    public void GripButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (viewportSquareVerts.Count < 4) return;

        int len = viewportSquareVerts.Count;
        Vector3[] viewport = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            viewport[i] = viewportSquareVerts[len - 1 - i].transform.position;
        }

        // calculate the centroid of these four points
        Vector3 centroid = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            centroid += viewport[i];
        }
        centroid /= 4;

        var viewportCube = GameObject.Find("VisibilityCube");

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
        viewportCube.transform.localPosition = centroid;
        viewportCube.transform.localEulerAngles = eulers;


        //Debug.Log("pretend a cube is created");
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ActionScript : MonoBehaviour
{
    public SteamVR_Action_Boolean SphereOnOff;
    public SteamVR_Input_Sources handType;
    public GameObject sphere;

    private void Start()
    {
        SphereOnOff.AddOnStateDownListener(TriggerDown, handType);
        SphereOnOff.AddOnStateUpListener(TriggerUp, handType);
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        sphere.GetComponent<MeshRenderer>().enabled = false;
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        sphere.GetComponent<MeshRenderer>().enabled = true;
    }
}

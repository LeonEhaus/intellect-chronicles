using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionText : MonoBehaviour
{
    [SerializeField]
    private string[] text;
    [SerializeField]
    private GameObject haloObject;

    public string[] getText()
    {
        return text;
    }

    private void Start()
    {
        deactivateHalo();
    }

    public void enableHalo()
    {
        (haloObject.GetComponent("Halo") as Behaviour).enabled = true;
    }

    public void deactivateHalo()
    {
        (haloObject.GetComponent("Halo") as Behaviour).enabled = false;
    }
}   

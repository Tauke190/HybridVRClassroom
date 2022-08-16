using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using CurvedUI;


public class UIRaycontroller : MonoBehaviour
{
    public XRController Lcontroller;
    public XRController Rcontroller;
    public InputHelpers.Button UIPress;

    



    private void Update()
    {
        if (InputHelpers.IsPressed(Lcontroller.inputDevice, UIPress, out bool lpressed, 0.1f))
        {
            Debug.Log("L pressed");
     
        }
        if (InputHelpers.IsPressed(Rcontroller.inputDevice, UIPress, out bool rpressed, 0.1f))
        {
            Debug.Log("R pressed");
        }

    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public enum LOCOMOTION_MODE { HEAD, JOYSTICK }
[RequireComponent(typeof(CharacterController))]
public class CustomPlayerLocomotion : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float rotationIncrement = 30;
    public float gravity = 30;
    public float maxSpeed = 10;

    public Transform head;

    public bool useGravity;

    public LOCOMOTION_MODE mode = LOCOMOTION_MODE.HEAD;

    private float speed = 0.0f;

    private CharacterController characterController = null;

    private InputDevice leftController;
    private InputDevice rightController;

    private bool joysticktouchdown;

    private int previousState; //-1= left 1=right 0=none

    public TMP_Dropdown handSelect;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void SetDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
        }

        devices.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
        }
    }

    private void Update()
    {
        if (!leftController.isValid || !rightController.isValid) 
        {
            SetDevice();
            return;
        }

        HandleHeight();
        CalculateMovement();

        if (BoardSelection.Instance.boardDropdown.value != 2 && !VRPointer.Instance.moveRay.activeInHierarchy)
        {
            Vector2 joystickvalue = GetPrimaryJoystickValue();

            if (Mathf.Abs(joystickvalue.x) > 0.5f)
            {
                if (joystickvalue.x > 0 && previousState != 1)
                {
                    SnapRotation(true);
                }

                if (joystickvalue.x < 0 && previousState != -1)
                {
                    SnapRotation(false);
                }
            }
        }
    }

    public Vector2 GetPrimaryJoystickValue()
    {
        InputDevice targetController = handSelect.value == 0 ? rightController : leftController;

        if (!targetController.isValid)
        {
            SetDevice();
            return Vector2.zero;
        }

        targetController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickvalue);

        return joystickvalue;
    }

    private void HandleHeight()
    {
        float headheight = Mathf.Clamp(head.localPosition.y, 1, 2);
        characterController.height = headheight;

        Vector3 newCentre = Vector3.zero;
        newCentre.y = characterController.height / 2;
        newCentre.y += characterController.skinWidth;

        newCentre.x = head.localPosition.x;
        newCentre.z = head.localPosition.z;

        characterController.center = newCentre;
    }

    private void CalculateMovement()
    {
        InputDevice targetController = handSelect.value == 0 ? leftController : rightController;

        targetController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool joysticktouch);
        targetController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickvalue);

        Quaternion orientation = CalculateOrientation(joystickvalue.x, joystickvalue.y);
        Vector3 movement = Vector3.zero;

        if (!joysticktouch && joysticktouchdown)
        {
            joysticktouchdown = false;
            speed = 0;
        }

        if (joystickvalue.magnitude < 0.1f)
        {
            speed = 0;
        }

        if (joysticktouch)
        {
            speed += CalculateIncrementValue(joystickvalue.x, joystickvalue.y);
            speed = Mathf.Clamp(speed, -maxSpeed * 0.5f, maxSpeed);
        }

        if (joysticktouch && !joysticktouchdown)
        {
            joysticktouchdown = true;
            return;
        }

        if (useGravity)
        {
            movement += orientation * (speed * Vector3.forward);
            movement.y = -gravity * Time.deltaTime;
            characterController.Move(movement * Time.deltaTime);
        }
        else
        {
            movement += orientation * (speed * Vector3.forward) * Time.deltaTime;
            characterController.Move(movement);
        }
    }

    private Quaternion CalculateOrientation(float x, float y)
    {
        float rotation = Mathf.Atan2(x, y);
        rotation *= Mathf.Rad2Deg;

        if (mode == LOCOMOTION_MODE.HEAD)
        {
            Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y, 0);
            return Quaternion.Euler(orientationEuler);
        }
        else if (mode == LOCOMOTION_MODE.JOYSTICK)
        {
            Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
            return Quaternion.Euler(orientationEuler);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    private float CalculateIncrementValue(float x, float y)
    {
        if (mode == LOCOMOTION_MODE.HEAD)
        {
            return y * sensitivity;
        }
        else if (mode == LOCOMOTION_MODE.JOYSTICK)
        {

            return new Vector2(x, y).magnitude * sensitivity;
        }
        else
        {
            return 0;
        }
    }

    private void SnapRotation(bool right)
    {
        previousState = right ? 1 : -1;
        float snapValue = !right ? -Mathf.Abs(rotationIncrement) : Mathf.Abs(rotationIncrement);
        transform.RotateAround(head.position, Vector3.up, snapValue);

        Invoke("ClearPreviousState", 0.75f);
    }

    private void ClearPreviousState()
    {
        previousState = 0;
    }
}

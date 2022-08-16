using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using TMPro;

public class VRInputModule : BaseInputModule
{
    public Camera cam;

    private PointerEventData data = null;
    private GameObject pointedobj;

    private InputDevice targetDevice;

    private bool down;

    protected override void Awake()
    {
        base.Awake();
        data = new PointerEventData(eventSystem);
    }

    public void SetDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(VRPointer.Instance.Characteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
    }

    public override void Process()
    {
        //reset and set camera
        data.Reset();
        data.position = new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);

        //get the object intersecting raycast
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        pointedobj = data.pointerCurrentRaycast.gameObject;

        //clear raycast cache
        m_RaycastResultCache.Clear();

        //handle hover state
        HandlePointerExitAndEnter(data, pointedobj);

        if (!targetDevice.isValid)
        {
            SetDevice();
            return;
        }

        targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerbutton);

        if (triggerbutton && !down)
        {
            ProcessPress(data);
            down = true;
         
        }
        if (!triggerbutton && down)
        {
            ProcessRelase(data);
            down = false;
        }


    }

    public PointerEventData GetData()
    {
        return data;
    }

    private void ProcessPress(PointerEventData data)
    {

        data.pointerPressRaycast = data.pointerCurrentRaycast;

        GameObject pointerpressobj = ExecuteEvents.ExecuteHierarchy(pointedobj, data, ExecuteEvents.pointerDownHandler);

        if(pointerpressobj == null)
        {
            pointerpressobj = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointedobj);
        }

        data.pressPosition = data.position;
        data.pointerPress = pointerpressobj;
        data.rawPointerPress = pointedobj;

    }
    private void ProcessRelase(PointerEventData data)
    {
        ExecuteEvents.Execute(data.pointerPress,data, ExecuteEvents.pointerUpHandler);

        GameObject PointerUpdhandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointedobj);

        if(data.pointerPress = PointerUpdhandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        eventSystem.SetSelectedGameObject(null);

        data.pressPosition = Vector3.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;

    }



}

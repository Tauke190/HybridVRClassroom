using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;



public class HandPresence : MonoBehaviour
{

    private InputDevice targetdevice;
    [SerializeField]
    InputDeviceCharacteristics controllercharacteristics;
    [SerializeField]
    private List<GameObject> Controllerprefabs;
    [SerializeField]
    private GameObject handprefab;
    private GameObject spawnedcontroller;
    private GameObject spawnedHand;
    [SerializeField]
    private bool showhands;
    private Animator handanimator;

    private void Start()
    {
        IntitializeController();
    }

    private void IntitializeController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllercharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetdevice = devices[0];
            GameObject prefab = Controllerprefabs.Find(controller => controller.name == targetdevice.name);
            if (prefab)
            {
                spawnedcontroller = Instantiate(prefab, transform);
            }
            else
            {
                Debug.Log("Did not find controller");
                spawnedcontroller = Instantiate(Controllerprefabs[0], transform);

            }

            spawnedHand = Instantiate(handprefab, this.transform);
            handanimator = spawnedHand.GetComponent<Animator>();
        }
    }



    private void UpdateHandAnimation()
    {
        if(targetdevice.TryGetFeatureValue(CommonUsages.trigger,out float triggervalue))
        {
            handanimator.SetFloat("Trigger", triggervalue);
        }
        else
        {
            handanimator.SetFloat("Trigger", 0);
        }
        if (targetdevice.TryGetFeatureValue(CommonUsages.grip, out float gripvalue))
        {
            handanimator.SetFloat("Grip", gripvalue);
        }
        else
        {
            handanimator.SetFloat("Grip", 0);
        }
    }
    private void Update()
    {

        if(!targetdevice.isValid)
        {
            IntitializeController();
        }

        else
        {
            targetdevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primarybuttonvalue);
            if (primarybuttonvalue)
            {
                Debug.Log("Primary Button is being pressed");
            }
            targetdevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 Touchpadvalue);
            if (Touchpadvalue.x != 0 && Touchpadvalue.y != 0)
            {
                Debug.Log("Touchpad is being pressed");
            }
            targetdevice.TryGetFeatureValue(CommonUsages.trigger, out float TriggerVakue);
            if (TriggerVakue > 0.1f)
            {
                Debug.Log("Trigger is being pressed");
            }

            if (showhands)
            {
                spawnedHand.SetActive(true);
                spawnedcontroller.SetActive(false);
                UpdateHandAnimation();

            }
            else
            {
                spawnedHand.SetActive(false);
                spawnedcontroller.SetActive(true);
            }
        }
      

    }

}

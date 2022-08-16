using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ToggleTeleport : MonoBehaviour
{
    public static ToggleTeleport Instance;

    public GameObject diamondPrefab;

    public event Action<bool> OnTeleportToggle;

    public GameObject teleportRay;

    private XRController controller;

    private bool previousState;

    private bool freeze;

    private void Awake()
    {
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        teleportRay.SetActive(false);
        controller = teleportRay.GetComponent<XRController>();
    }

    private void Update()
    {
        InputHelpers.IsPressed(controller.inputDevice, controller.selectUsage, out bool pressed, 0.2f);

        if (pressed != previousState  && teleportRay.activeInHierarchy && !freeze)
        {
            StartCoroutine(WaitAndToggle(false, 0.2f));
        }

        previousState = pressed;
    }

    private IEnumerator WaitAndToggle(bool value, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        TeleportToggle(value);
    }

    public void TeleportToggle(bool isOn)
    {
        if (teleportRay.activeInHierarchy == isOn)
        {
            return;
        }

        teleportRay.SetActive(isOn);
        OnTeleportToggle(isOn);

        freeze = true;

        Invoke("UnFreeze", 0.5f);
    }

    private void UnFreeze()
    {
        freeze = false;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using EDVR.HybirdClassoom;
using TMPro;



[RequireComponent(typeof(LineRenderer))]
public class VRPointer : MonoBehaviour
{
    public static VRPointer Instance { get; private set; }

    public float defaultLength = 5;
    public float pickUpSpeed = 5f;

    public InputDeviceCharacteristics Characteristics { get; private set; }

    private InputDevice targetDevice;

    private LineRenderer lineRenderer;
    private Vector3 endposition = new Vector3();
    private GameObject targetObject;
    private bool down;

    private NetworkInteractObject bone;

    public bool RightJoyStick { get; private set; }
    public bool LeftJoyStick { get; private set; }

    private VRInputModule inputmodule;

    public GameObject vrUi;
    public GameObject panel1;
    public GameObject[] otherPanels;

    private bool uiSpawnButtonDown;

    public TMP_Dropdown controllerDropdown;

    public Transform rightTransform;
    public Transform leftTransform;

    public enum MoveState { None, Select, Move };
    [Tooltip("Choose the mode before building")]
    public MoveState moveState { get; private set; }
    //public bool Move { get; private set; }

    public TextMeshPro vrInfoText;

    public GameObject moveRay;

    public float rotateSensitivity = 100f;

    private bool moveComplete = true;

    public Camera inputCamera;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void SetController(int index)
    {
        Characteristics = controllerDropdown.value == 0 ? InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller :
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;

        transform.SetParent(controllerDropdown.value == 0 ? rightTransform : leftTransform);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        SetDevice();
    }

    private void Start()
    {
        inputmodule = FindObjectOfType<VRInputModule>();
        ToggleOffVRUI();

        SetController(controllerDropdown.value);

        controllerDropdown.onValueChanged.AddListener(SetController);

        UiEvents.Instance.OnMoveButtonPressed += OnMoveButtonPressed;
        UiEvents.Instance.OnMoveComplete += MoveComplete;

        vrInfoText.gameObject.SetActive(false);

        moveRay.SetActive(false);

        moveState = MoveState.None;
    }

    private void OnDestroy()
    {
        UiEvents.Instance.OnMoveButtonPressed -= OnMoveButtonPressed;
        UiEvents.Instance.OnMoveComplete -= MoveComplete;
    }

    private void OnMoveButtonPressed()
    {
        moveState = MoveState.Select;

        //ClearTargetObjectReference();

        bone = null;

        vrInfoText.text = "Click an object to move";
        vrInfoText.gameObject.SetActive(true);
        Transform head = PlatformManager.instance.teacherRigParts[0];
        vrInfoText.transform.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        Vector3 pos = head.position + vrInfoText.transform.forward * 2f;
        vrInfoText.transform.position = new Vector3(pos.x, Mathf.Max(1f, head.position.y), pos.z);
    }

    private void MoveComplete()
    {
        moveComplete = true;
    }

    public void ToggleOffVRUI()
    {
        vrUi.SetActive(false);
        panel1.SetActive(true);

        foreach (var item in otherPanels)
        {
            item.SetActive(false);
        }
    }

    private void SetDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(Characteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            inputmodule.SetDevice();
        }
    }

    public void DropBone()
    {
        if (bone)
        {
            bone.Drop();
            bone = null;
        }

        if (targetObject)
        {
            targetObject = null;
        }
    }

    private void Update()
    {
        if (ToggleTeleport.Instance.teleportRay.activeInHierarchy || moveRay.activeInHierarchy)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            return;
        }

        if (!targetDevice.isValid)
        {
            SetDevice();
            return;
        }

        targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerbutton);
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickvalue);
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton);

        if (primaryButton && !uiSpawnButtonDown && moveState == MoveState.None)
        {
            if (vrUi.activeInHierarchy)
            {
                ToggleOffVRUI();
            }
            else
            {
                vrUi.SetActive(true);
                Transform head = PlatformManager.instance.teacherRigParts[0];
                vrUi.transform.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
                Vector3 pos = head.position + vrUi.transform.forward * 2f;
                vrUi.transform.position = new Vector3(pos.x, Mathf.Max(1.5f, head.position.y), pos.z);
            }

            uiSpawnButtonDown = true;
        }

        if (!primaryButton && uiSpawnButtonDown)
        {
            uiSpawnButtonDown = false;
        }

        PointerEventData data = inputmodule.GetData();

        float targetlength = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;

        if (Mathf.Abs(joystickvalue.x) > Mathf.Abs(joystickvalue.y))
        {
            if (joystickvalue.x > 0.5f)
            {
                RightJoyStick = true;
                LeftJoyStick = false;
            }
            else if (joystickvalue.x < -0.5f)
            {
                RightJoyStick = false;
                LeftJoyStick = true;
            }
        }
        else
        {
            RightJoyStick = false;
            LeftJoyStick = false;
        }

        RaycastHit hit = CreateRaycast(targetlength);
        endposition = transform.position + (transform.forward * targetlength);

        if (!PlatformManager.instance.PlayerOnStage)
        {
            if (hit.collider != null && hit.transform.GetComponent<IInteractable>() != null)
            {
                endposition = hit.point;
                if (targetObject != hit.transform.gameObject && !down && !bone && moveState != MoveState.Move)
                {
                    //ClearTargetObjectReference();

                    if (targetObject)
                    {
                        targetObject.GetComponent<IInteractable>().OnPointerExit();
                        targetObject = null;
                    }

                    targetObject = hit.transform.gameObject;
                    targetObject.GetComponent<IInteractable>().OnPointerEnter();
                }
            }
            else
            {
                //ClearTargetObjectReference();

                if (targetObject)
                {
                    targetObject.GetComponent<IInteractable>().OnPointerExit();
                    targetObject = null;
                }
            }
        }

        if (!targetObject && data.pointerCurrentRaycast.distance == 0)
        {
            endposition = transform.position;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endposition);

        if (targetObject && triggerbutton && !down)
        {
            down = true;

            if (moveState == MoveState.Select)
            {
                moveState = MoveState.Move;
                vrInfoText.gameObject.SetActive(false);
                //ClearTargetObjectReference();
                targetObject.GetComponent<IInteractable>().OnPointerDown();
                targetObject = null;
            }
            else if (moveState == MoveState.None && targetObject.GetComponent<NetworkInteractObject>())
            {
                bone = targetObject.GetComponent<NetworkInteractObject>();
                targetObject.GetComponent<IInteractable>().OnPointerDown();
            }
        }

        if (!triggerbutton && down)
        {
            down = false;

            if (moveState == MoveState.Move)
            {
                moveRay.SetActive(true);
            }
        }

        if (bone && !triggerbutton)
        {
            bone.Drop();
            bone = null;
        }

        if (!triggerbutton && moveComplete)
        {
            if (moveState == MoveState.Move)
            {
                moveState = MoveState.None;
                down = false;
            }
            moveComplete = false;
        }
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, length);
        return hit;
    }

    private bool EnableRay(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.SphereCast(ray, 0.5f, out hit, length);

        return hit.collider != null && hit.transform.GetComponent<IInteractable>() != null;
    }
}

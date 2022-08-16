using UnityEngine;
using EPOOutline;
using Photon.Pun;
using EDVR.HybirdClassoom;
using System.Collections.Generic;
using UnityEngine.XR;

[RequireComponent(typeof(Outlinable))]
public class NetworkInteractObject : MonoBehaviour, IInteractable
{
    private Outlinable outline;
    private Transform hand;
    private bool move;

    public Rigidbody Rb { get; private set; }

    private BonePuzzleManager bonemanager;

    public GameObject Target { get; private set; }
    public enum Type { Puzzle, View };
    public Type type;

    private InputDevice leftController;
    private InputDevice rightController;

    private float previousControllerDistance;

    private bool targetPositionReceived;

    private Vector3 targetPos;
    private Vector3 targetRot;

    private bool iniitialized;

    [HideInInspector]
    public bool isTarget;

    private bool mouseInteract;

    public bool Placed { get; private set; }

    private void SetDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
        }

        devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
        }
    }

    private void Start()
    {
        bonemanager = FindObjectOfType<BonePuzzleManager>();

        if (isTarget)
        {
            Invoke("DisableKinematics", 1f);
            return;
        }

        outline = GetComponent<Outlinable>();
        outline.enabled = false;

        Rb = GetComponent<Rigidbody>();
        Rb.isKinematic = true;

        if (type == Type.Puzzle)
        {
            Target = Instantiate(gameObject, transform.parent);
            Target.GetComponent<Rigidbody>().isKinematic = true;
            Target.GetComponent<NetworkInteractObject>().isTarget = true;
            Target.GetComponent<Outlinable>().enabled = false;
            Target.GetComponent<PhotonTransformView>().enabled = false;

            Target.transform.position = targetPositionReceived ? targetPos : transform.position;
            Target.transform.eulerAngles = targetPositionReceived ? targetRot : transform.eulerAngles;

            foreach (Renderer item in Target.GetComponents<Renderer>())
            {
                item.material = bonemanager.holderMaterial;
                item.enabled = false;
            }

            foreach (Renderer item in Target.GetComponentsInChildren<Renderer>())
            {
                item.material = bonemanager.holderMaterial;
                item.enabled = false;
            }
        }

        if (FindObjectOfType<VRPointer>())
        {
            hand = FindObjectOfType<VRPointer>().transform;
        }

        GetComponent<Outlinable>().enabled = false;

        Invoke("DisableKinematics", 1f);
    }

    private void DisableKinematics()
    {
        iniitialized = true;

        if (isTarget)
        {
            return;
        }

        Rb.isKinematic = PlatformManager.instance.mode != PlatformManager.Mode.Teacher;

        ToggleTargetRenderer(true);
    }

    public void OnPointerDown()
    {
        if (!iniitialized)
        {
            return;
        }

        if (mouseInteract)
        {
            PickUp();
            if (outline)
            {
                outline.enabled = false;
            }
            return;
        }

        if (VRPointer.Instance.moveState == VRPointer.MoveState.Move)
        {
            bonemanager.StartMoving();
        }
        else if (VRPointer.Instance.moveState == VRPointer.MoveState.None && !isTarget)
        {
            PickUp();
        }

        if (!isTarget)
        {
            outline.enabled = false;
        }
    }

    public void OnPointerEnter()
    {
        if (!iniitialized)
        {
            return;
        }

        if (mouseInteract && outline)
        {
            outline.enabled = true;
            return;
        }

        if (VRPointer.Instance.moveState == VRPointer.MoveState.Select)
        {
            bonemanager.ToggleAllBoneOutlines(true);
        }

        if (outline && VRPointer.Instance.moveState == VRPointer.MoveState.None && !isTarget)
        {
            outline.enabled = true;
        }
    }

    public void OnPointerExit()
    {
        if (!iniitialized)
        {
            return;
        }

        if (mouseInteract && outline)
        {
            outline.enabled = false;
            return;
        }

        if (VRPointer.Instance.moveState == VRPointer.MoveState.Select)
        {
            bonemanager.ToggleAllBoneOutlines(false);
        }

        if (outline)
        {
            outline.enabled = false;
        }
    }

    public void OnPointerUp()
    {
        if (!iniitialized)
        {
            return;
        }

        if (move)
        {
            move = false;
            Drop();
        }
    }

    private void PickUp()
    {
        move = true;
        Rb.isKinematic = true;
        if (Target)
        {
            Target.GetComponent<Outlinable>().enabled = true;
        }
        ToggleTargetRenderer(true);

        if (FindObjectOfType<BoneRef>())
        {
            FindObjectOfType<BoneRef>().SyncPickUp(this);
        }
    }

    public void Drop()
    {
        move = false;
        if (Target)
        {
            Target.GetComponent<Outlinable>().enabled = false;

            bool placed = false;

            if (Vector3.Distance(Target.transform.position, transform.position) <= bonemanager.minDistance && Quaternion.Angle(transform.rotation, Target.transform.rotation) <= 45f)
            {
                transform.position = Target.transform.position;
                transform.rotation = Target.transform.rotation;
                ToggleTargetRenderer(false);
                placed = true;
            }
            else
            {
                rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
                rightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVelocity);

                Rb.isKinematic = false;
                Rb.velocity = velocity;
                Rb.angularVelocity = angularVelocity;
            }

            FindObjectOfType<BoneRef>().SyncDrop(this, placed);
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Target.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }

    private void Update()
    {
        if (isTarget)
        {
            return;
        }

        float currentControllerDistance = Vector3.Distance(PlatformManager.instance.teacherRigParts[1].position, PlatformManager.instance.teacherRigParts[2].position);

        if (move)
        {
            Vector3 handpos = GetMouseAsWorldPoint();
            Quaternion handRot = Target.transform.rotation;

            if (Vector3.Distance(transform.position, mouseInteract ? handpos : hand.position) < 0.1f)
            {
                transform.position = mouseInteract ? handpos : hand.position;
                transform.rotation = mouseInteract ? handRot : hand.rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, mouseInteract ? handpos : hand.position, Time.deltaTime * 10f);

                if (type != Type.View)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, mouseInteract ? handRot : hand.transform.rotation, Time.deltaTime * 10f);
                }
            }
        }
        else {
            if (type == Type.View)
            {
                Rb.isKinematic = true;

                if (!leftController.isValid || !rightController.isValid)
                {
                    SetDevice();
                }
                else
                {
                    leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGrip);
                    rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGrip);
                    rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickvalue);

                    if (leftGrip && rightGrip)
                    {
                        float delta = currentControllerDistance - previousControllerDistance;

                        Vector3 targetScale = transform.localScale + (transform.localScale * delta * 2);
                        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
                    }
                    else
                    {
                        transform.Rotate(Vector3.up, joystickvalue.x * Time.deltaTime * 20f);
                    }
                }
            }
        }

        previousControllerDistance = currentControllerDistance;
    }

    private void ToggleTargetRenderer(bool value)
    {
        if (!Target)
        {
            return;
        }

        foreach (Renderer item in Target.GetComponents<Renderer>())
        {
            item.enabled = value;
        }

        foreach (Renderer item in Target.GetComponentsInChildren<Renderer>())
        {
            item.enabled = value;
        }
    }

    public void RpcPickUp()
    {
        Target.GetComponent<Outlinable>().enabled = true;
        ToggleTargetRenderer(true);
    }

    public void RpcDrop(bool _placed)
    {
        Target.GetComponent<Outlinable>().enabled = false;
        ToggleTargetRenderer(!Placed);

        Placed = _placed;
    }

    public void SetTargetPosition(float x, float y, float z)
    {
        if (Target)
        {
            Target.transform.position = new Vector3(x, y, z);
        }

        targetPos = new Vector3(x, y, z);

        targetPositionReceived = true;
    }

    public void SetTargetRotation(float x, float y, float z)
    {
        if (Target)
        {
            Target.transform.rotation = Quaternion.Euler(x, y, z);
        }

        targetRot = new Vector3(x, y, z);

        targetPositionReceived = true;
    }

    private void OnMouseEnter()
    {
        if (PlatformManager.instance.OnStage && !isTarget)
        {
            mouseInteract = true;
            OnPointerEnter();
        }
    }

    private void OnMouseExit()
    {
        if (PlatformManager.instance.OnStage && !isTarget)
        {
            OnPointerExit();
        }
    }

    private void OnMouseDown()
    {
        if (PlatformManager.instance.OnStage && !isTarget)
        {
            OnPointerDown();
        }
    }

    private void OnMouseUp()
    {
        if (PlatformManager.instance.OnStage && !isTarget)
        {
            OnPointerDown();
        }
    }
}

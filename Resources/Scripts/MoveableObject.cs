using EPOOutline;
using UnityEngine;
using Photon.Pun;
using EDVR.HybirdClassoom;

public class MoveableObject : MonoBehaviour, IInteractable
{
    public bool sync;

    private bool moving;
    private float xRot = 0f;
    private Transform followReticle;
    private Transform parentTransform;
    private PhotonView pv;
    private CustomPlayerLocomotion player;
    private Outlinable outline;

    private bool initialized;

    private void Start()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        followReticle = FindObjectOfType<BonePuzzleManager>().reticle;

        UiEvents.Instance.OnMoveComplete += MoveComplete;

        if (sync)
        {
            pv = GetComponent<PhotonView>();
            if (pv == null)
            {
                Debug.LogError("Photon View Required if Sync enabled.");
            }
        }

        parentTransform = transform.parent;

        player = FindObjectOfType<CustomPlayerLocomotion>();

        outline = GetComponent<Outlinable>();
        outline.enabled = false;

        initialized = true;
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    private void OnDestroy()
    {
        UiEvents.Instance.OnMoveComplete -= MoveComplete;
    }

    private void MoveComplete()
    {
        if (!moving)
        {
            return;
        }

        if (moving && sync && pv)
        {
            pv.RPC("RpcMove", RpcTarget.Others, parentTransform.position.x, parentTransform.position.z, transform.localEulerAngles.x, parentTransform.eulerAngles.y);
        }
        moving = false;

        if (GetComponent<Whiteboard>())
        {
            GetComponent<Whiteboard>().RecordPosition();
        }
    }

    [PunRPC]
    private void RpcSyncPos(float x, float z, float rotX, float rotY)
    {
        parentTransform.position = new Vector3(x, parentTransform.position.y, z);
        parentTransform.eulerAngles = new Vector3(parentTransform.eulerAngles.x, rotY, parentTransform.eulerAngles.z);
        transform.localEulerAngles = new Vector3(rotX, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void OnPointerDown()
    {
        outline.enabled = false;

        if (VRPointer.Instance.moveState == VRPointer.MoveState.Move)
        {
            followReticle.transform.position = new Vector3(parentTransform.position.x, followReticle.position.y, parentTransform.position.z);

            xRot = transform.localEulerAngles.x;

            moving = true;
            FindObjectOfType<BonePuzzleManager>().CreateMoveArea();
        }
    }

    public void OnPointerEnter()
    {
        if (VRPointer.Instance.moveState == VRPointer.MoveState.Select)
        {
            outline.enabled = true;
        }
    }

    public void OnPointerExit()
    {
        outline.enabled = false;
    }

    public void OnPointerUp()
    {
        //moving = false;
    }

    private void Update()
    {
        if (moving)
        {
            Vector3 dest = new Vector3(followReticle.transform.position.x, transform.position.y, followReticle.transform.position.z);

            if (parentTransform)
            {
                parentTransform.position = Vector3.Lerp(parentTransform.position, dest, Time.deltaTime * 10f);
                transform.localPosition = Vector3.zero;
            }

            if (player)
            {
                Vector2 joystickvalue = player.GetPrimaryJoystickValue() * VRPointer.Instance.rotateSensitivity * Time.deltaTime;

                if (parentTransform)
                {
                    xRot -= joystickvalue.y;
                    xRot = Mathf.Clamp(xRot, -75f, 75f);

                    transform.localRotation = Quaternion.Euler(xRot, 0, 0);
                    parentTransform.Rotate(Vector3.up * joystickvalue.x);
                }
            }
        }
    }
}

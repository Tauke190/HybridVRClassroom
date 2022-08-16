using UnityEngine;
using DG.Tweening;

public class CameraMover : MonoBehaviour
{
    public Transform destinationTransform;

    private Vector3 initialPos;
    private Quaternion initialRot;

    private Vector3 destinationPos;
    private Quaternion destinationRot;

    private bool move;

    private bool canvasmode;

    public float sensitivity = 100f;

    private float xRot = 0f;

    public Transform playerBody;

    private Quaternion initialPlayerRotation;

    private bool reset;

    public GameObject chatUI;

    public void InitializePosition()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;

        initialPlayerRotation = playerBody.rotation;
    }

    public void ToggleCanvasMode(bool canvasMode)
    {
        destinationPos = canvasMode ? destinationTransform.position : initialPos;
        destinationRot = canvasMode ? destinationTransform.rotation : initialRot;

        move = true;

        canvasmode = canvasMode;
    }

    public void ResetOnSeatChanged()
    {
        playerBody.rotation = initialPlayerRotation;
        xRot = 0;
    }

    private void Update()
    {
        if (reset)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(xRot, 0, 0), Time.deltaTime * 10f);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, initialPlayerRotation, Time.deltaTime * 10f);

            if(Quaternion.Angle(transform.localRotation, Quaternion.Euler(xRot, 0, 0)) < 0.1f &&
                Quaternion.Angle(playerBody.rotation, initialPlayerRotation) < 0.1f)
            {
                playerBody.rotation = initialPlayerRotation;
                reset = false;
            }
        }

        if (!move)
        {
            if (!canvasmode && !reset && !chatUI.activeInHierarchy)
            {
                float x = Input.GetAxis("Horizontal") * sensitivity * Time.deltaTime;
                float y = Input.GetAxis("Vertical") * sensitivity * Time.deltaTime;

                xRot -= y;
                xRot = Mathf.Clamp(xRot, -75f, 75f);

                transform.localRotation = Quaternion.Euler(xRot, 0, 0);
                playerBody.Rotate(Vector3.up * x);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    xRot = 0f;
                    reset = true;
                }
            }
            return;
        }

        if (Vector3.Distance(transform.position, destinationPos) < 0.1f)
        {
            transform.position = destinationPos;
            transform.rotation = destinationRot;

            FindObjectOfType<SimpleSlides>().ToggleCanvas(destinationPos == destinationTransform.position);

            move = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, destinationRot, Time.deltaTime * 10f);
        }
    }
}

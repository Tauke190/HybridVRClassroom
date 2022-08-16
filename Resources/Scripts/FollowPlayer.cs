using EDVR.HybirdClassoom;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float sensitivity = 10f;

    private Transform head;
    private bool startFollowing;
    private Vector3 destPos;
    private Quaternion destRot;

    private void Start()
    {
        head = PlatformManager.instance.teacherRigParts[0];
    }

    private void OnEnable()
    {
        if (!PlatformManager.instance)
        {
            return;
        }

        head = PlatformManager.instance.teacherRigParts[0];

        Calculate();

        transform.position = destPos;
        transform.rotation = destRot;
    }

    private void Calculate()
    {
        destRot = Quaternion.Euler(new Vector3(0, head.eulerAngles.y, 0));
        destPos = head.position + head.forward * 2f;
        destPos = new Vector3(destPos.x, transform.position.y, destPos.z);
    }

    private void Update()
    {
        Calculate();

        if (Vector3.Distance(transform.position, destPos) > 2f)
        {
            startFollowing = true;
        }

        if (startFollowing)
        {
            transform.position = Vector3.Lerp(transform.position, destPos, Time.deltaTime * sensitivity);
            transform.rotation = Quaternion.Slerp(transform.rotation, destRot, Time.deltaTime * sensitivity);

            if (Vector3.Distance(transform.position, destPos) < 0.1f)
            {
                startFollowing = false;
            }
        }
    }
}

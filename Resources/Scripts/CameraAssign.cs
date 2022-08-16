using UnityEngine;

public class CameraAssign : MonoBehaviour
{
    private void Start()
    {
        if (VRPointer.Instance)
        {
            GetComponent<Canvas>().worldCamera = VRPointer.Instance.inputCamera;
        }
    }
}

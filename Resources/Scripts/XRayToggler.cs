using UnityEngine;

public class XRayToggler : MonoBehaviour
{
    public GameObject xRay;

    private void Start()
    {
        ToggleXRay(false);
    }

    public void ToggleXRay(bool value)
    {
        xRay.SetActive(value);
    }
}

using UnityEngine;

public class VRToggleButton : MonoBehaviour, IInteractable
{
    public GameObject assignedBody;

    private GameObject tick;

    public void OnPointerDown()
    {
        FindObjectOfType<SeeThroughSync>().Sync(this);
    }

    public void OnPointerEnter()
    {
        
    }

    public void OnPointerExit()
    {
        
    }

    public void OnPointerUp()
    {
        
    }

    private void Start()
    {
        tick = transform.GetChild(0).gameObject;
        tick.SetActive(true);
    }

    public void Toggle()
    {
        tick.SetActive(!tick.activeInHierarchy);
        assignedBody.SetActive(tick.activeInHierarchy);
    }
}

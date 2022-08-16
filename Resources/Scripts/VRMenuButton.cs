using UnityEngine;
using EPOOutline;
using UnityEngine.Events;

public class VRMenuButton : MonoBehaviour, IInteractable
{
    private Outlinable outline;

    public UnityEvent OnPointerClickEvent;

    private void Start()
    {
        if (GetComponent<Outlinable>())
        {
            outline = GetComponent<Outlinable>();
            outline.enabled = false;
        }
    }

    public void OnPointerEnter()
    {
        Glow();
    }

    public void OnPointerDown()
    {
        OnPointerClickEvent.Invoke();
    }

    public void OnPointerUp()
    {

    }

    public void OnPointerExit()
    {
        StopGlow();
    }

    private void OnDisable()
    {
        StopGlow();
    }

    private void Glow()
    {
        if (outline)
        {
            outline.enabled = true;
        }
    }

    private void StopGlow()
    {
        if (outline)
        {
            outline.enabled = false;
        }
    }
}

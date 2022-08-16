using EPOOutline;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChairRef : MonoBehaviour
{
    public string id;
    public Vector3 teleportOffset = new Vector3(0, -1, 1);
    public float diamondOffset = 1.271f;

    private GameObject diamondObject;
    private TeleportationAnchor anchor;
    private ToggleTeleport teleportToggler;

    private void Start()
    {
        teleportToggler = FindObjectOfType<ToggleTeleport>();

        if (!teleportToggler)
        {
            return;
        }

        GameObject _anchor = new GameObject();
        _anchor.name = transform.name + "Anchor";
        _anchor.transform.SetParent(transform);
        _anchor.transform.localPosition = teleportOffset;
        _anchor.transform.localEulerAngles = new Vector3(0, 180, 0);

        anchor = GetComponent<TeleportationAnchor>();
        anchor.teleportAnchorTransform = _anchor.transform;

        diamondObject = Instantiate(teleportToggler.diamondPrefab, transform);
        diamondObject.transform.localEulerAngles = Vector3.zero;
        diamondObject.transform.localPosition = new Vector3(0, diamondOffset, 0);
        diamondObject.transform.rotation = teleportToggler.diamondPrefab.transform.rotation;

        diamondObject.SetActive(false);
        diamondObject.GetComponent<Outlinable>().enabled = false;

        anchor.hoverEntered.AddListener(ToggleOnDiamondOutline);
        anchor.hoverExited.AddListener(ToggleOffDiamondOutline);

        teleportToggler.OnTeleportToggle += ToggleTeleport;
    }

    private void OnDestroy()
    {
        if (teleportToggler)
        {
            teleportToggler.OnTeleportToggle -= ToggleTeleport;
        }
    }

    private void ToggleTeleport(bool isOn)
    {
        diamondObject.SetActive(isOn);
    }

    private void ToggleOnDiamondOutline(HoverEnterEventArgs call)
    {
        diamondObject.GetComponent<Outlinable>().enabled = true;
    }

    private void ToggleOffDiamondOutline(HoverExitEventArgs call)
    {
        diamondObject.GetComponent<Outlinable>().enabled = false;
    }
}

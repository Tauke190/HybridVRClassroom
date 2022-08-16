using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class keys : MonoBehaviour
{
    public string normal;
    public string shift;

    private TextMeshProUGUI keyText;
    private keyboardvr keyboard;

    private void Start()
    {
        keyText = GetComponentInChildren<TextMeshProUGUI>();
        keyText.text = normal;

        keyboard = FindObjectOfType<keyboardvr>();
        keyboard.OnShift += Shift;

        GetComponent<Button>().onClick.AddListener(Pressed);
    }

    private void OnValidate()
    {
        gameObject.name = normal;
        GetComponentInChildren<TextMeshProUGUI>().text = normal;
    }

    private void Pressed()
    {
        keyboard.Append(keyText.text);
    }

    private void Shift(bool _shift)
    {
        keyText.text = _shift ? shift : normal;
    }

    private void OnDestroy()
    {
        keyboard.OnShift -= Shift;
    }
}

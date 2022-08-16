using UnityEngine;
using TMPro;
using System;

public class keyboardvr : MonoBehaviour
{
    public TMP_InputField inputField;

    public event Action<bool> OnShift;

    private bool isShift;

    public void Append(string a)
    {
        inputField.text += a;
    }

    public void ShiftPressed()
    {
        isShift = !isShift;
        OnShift?.Invoke(isShift);
    }

    public void BackSpace()
    {
        inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
    }

    public void Space()
    {
        inputField.text += " ";
    }
}

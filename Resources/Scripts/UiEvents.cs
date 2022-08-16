using System;
using UnityEngine;
using TMPro;
using EDVR.HybirdClassoom;
public class UiEvents : MonoBehaviour
{
    public static UiEvents Instance { get; private set; }

    public event Action OnZoom;
    public event Action OnRaiseHand;
    public event Action<bool> OnMuteToggle;
    public event Action<string> OnSendChat;
    public event Action OnMoveButtonPressed;
    public event Action OnMoveComplete;
    public event Action OnMoveCancel;
    public event Action<bool> OnRecord;

    public GameObject raiseHandButton;

    public TMP_InputField chatField;
    public TMP_InputField chatFieldVr;

    private bool mute;

    private bool recording;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        raiseHandButton.SetActive(PlatformManager.instance.mode == PlatformManager.Mode.StudentPC);
    }

    public void Zoom()
    {
        OnZoom?.Invoke();
    }

    public void RaiseHand()
    {
        OnRaiseHand?.Invoke();
        raiseHandButton.SetActive(false);

        Invoke("ToggleRaiseHandButton", 2f);
    }

    private void ToggleRaiseHandButton()
    {
        raiseHandButton.SetActive(true);
    }

    public void ToggleMute()
    {
        mute = !mute;
        OnMuteToggle?.Invoke(mute);
    }

    public void SendChat()
    {
        if (PlatformManager.instance.mode == PlatformManager.Mode.Teacher)
        {
            if (chatFieldVr.text != "")
            {
                OnSendChat?.Invoke(chatFieldVr.text);
                chatFieldVr.text = "";
            }
        }
        else
        {
            if (chatField.text != "")
            {
                OnSendChat?.Invoke(chatField.text);
                chatField.text = "";
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    public void MoveButtonPressed()
    {
        OnMoveButtonPressed?.Invoke();
    }

    public void MoveComplete()
    {
        OnMoveComplete?.Invoke();
    }

    public void CancelMove()
    {
        OnMoveCancel?.Invoke();
    }

    public void ToggleRecord()
    {
        recording = !recording;
        OnRecord?.Invoke(recording);
    }
}

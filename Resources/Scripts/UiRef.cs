using UnityEngine;
using UnityEngine.UI;
using Byn.Unity.Examples;
using TMPro;

public class UiRef : MonoBehaviour
{
    public Sprite muteSprite;
    public Sprite soundSprite;
    public Image[] soundImage;
    public TextMeshProUGUI recordButtonText;

    private void Start()
    {
        UiEvents.Instance.OnMuteToggle += ToggleMute;
        UiEvents.Instance.OnRecord += Record;
    }

    private void OnDestroy()
    {
        UiEvents.Instance.OnMuteToggle -= ToggleMute;
        UiEvents.Instance.OnRecord -= Record;
    }

    public void ToggleMute(bool isMute)
    {
        Sprite _sprite = isMute ? muteSprite : soundSprite;

        foreach (var item in soundImage)
        {
            item.sprite = _sprite;
        }

        ConferenceApp app = FindObjectOfType<ConferenceApp>();

        if (app)
        {
            app.ToggleMute(isMute);
        }
    }

    private void Record(bool isRecording)
    {
        recordButtonText.text = isRecording ? "Stop Recording" : "Start Recording";
    }
}

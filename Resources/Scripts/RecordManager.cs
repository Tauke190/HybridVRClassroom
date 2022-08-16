using UnityEngine;
using System.Collections.Generic;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance { get; private set; }

    public float RecordTimer { get; private set; }

    private bool recording;

    public float frameRate = 50;

    public string RecordDateTime { get; private set; }

    public Dictionary<Behaviour, string> UidList { get; private set; } = new Dictionary<Behaviour, string>();

    public float DeltaTime()
    {
        return 1 / frameRate;
    }

    private void Awake()
    {
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UiEvents.Instance.OnRecord += Record;
    }

    private void OnDestroy()
    {
        UiEvents.Instance.OnRecord -= Record;
    }

    private void Record(bool isRecording)
    {
        recording = isRecording;

        if (recording)
        {
            RecordDateTime = System.DateTime.Now.ToFileTime().ToString();
            RecordTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (recording)
        {
            RecordTimer += Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UiEvents.Instance.ToggleRecord();
        }
    }
}

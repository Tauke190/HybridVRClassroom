using FrostweepGames.Plugins.Native;
using UnityEngine;

public class AskForMic : MonoBehaviour
{
    void Start()
    {
        CustomMicrophone.RequestMicrophonePermission();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byn.Unity.Examples;

public class ConferenceAtStart : MonoBehaviour
{

    public ConferenceApp conferenceApp;




    private void Start()
    {
        conferenceApp.JoinConferenceCall(true, true, "Machikne");
        StartCoroutine(JoinCall());
    }

    private IEnumerator JoinCall()
    {
        yield return new WaitForSeconds(0f);    
    }
}

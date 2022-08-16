using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemToogle : MonoBehaviour
{
    public bool[] Systems;
    public GameObject[] Toogles;
    public LayerCutoff LayerCutoff;


    void OnEnable()
    {
        foreach (GameObject Toogle in Toogles)
        {
            Toogle.transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }

    public void UpdateToggle()
    {
        for (int i = 0; i < Toogles.Length; i++)
        {
            if (!Toogles[i].transform.GetChild(0).gameObject.activeInHierarchy)
            {
                Systems[i] = false;
            }
            else
            {
                Systems[i] = true;
            }
        }
        if(Systems[0])
        {
            LayerCutoff.SetMuscles(true);
        }
        if (Systems[1])
        {
            LayerCutoff.SetSkeleton(true);
        }
        if (Systems[2])
        {
          //  LayerCutoff.SetLymppatic(true);
        }
        if (Systems[3])
        {
            LayerCutoff.SetRespiratory(true);
        }
        if (Systems[4])
        {
            LayerCutoff.SetCirulatory(true);
        }
        if (Systems[5])
        {
            LayerCutoff.SetNervous(true);
        }
        if (Systems[6])
        {
            LayerCutoff.SetDigestive(true);
        }
        if (!Systems[0])
        {
            LayerCutoff.SetMuscles(false);
        }
        if (!Systems[1])
        {
            LayerCutoff.SetSkeleton(false);
        }
        if (!Systems[2])
        {
          //  LayerCutoff.SetLymppatic(false);
        }
        if (!Systems[3])
        {
            LayerCutoff.SetRespiratory(false);
        }
        if (!Systems[4])
        {
            LayerCutoff.SetCirulatory(false);
        }
        if (!Systems[5])
        {
            LayerCutoff.SetNervous(false);
        }
        if (!Systems[6])
        {
            LayerCutoff.SetDigestive(false);
        }
    }
}

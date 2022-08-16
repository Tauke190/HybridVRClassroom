using UnityEngine;
using UnityEngine.UI;
using EDVR.HybirdClassoom;

public class SeatSelectButton : MonoBehaviour
{
    public string id;

    private void Start()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        PlatformManager.instance.ChangeSeat(id);
    }   
}

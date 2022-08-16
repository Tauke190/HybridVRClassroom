using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SystemInaCircle : MonoBehaviourPun, IPunObservable
{
    public Slider slider;

    private Animator animator;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(slider.value);
        }
        else
        {
            slider.value = (float)stream.ReceiveNext();
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("Blend", slider.value);
    }
}

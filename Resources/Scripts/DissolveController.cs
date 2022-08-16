using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DissolveController : MonoBehaviourPun, IPunObservable
{
    [Header("Sliders")]
    public Slider SkinSlider;
    public Slider MusclesSlider;
    public Slider SkeletonSlider;
    public Slider CirculatorySlider;
    public Slider NervousSlider;
    public Slider RespiratorySlider;
    public Slider DigestiveSlider;

    [Header("Transfomrs")]
    public Transform SkinTransform;
    public Transform MusclesTransform;
    public Transform SkeletonTransform;
    public Transform CirculatoryTransform;
    public Transform NervousTransform;
    public Transform RespiratoryTransform;
    public Transform DigestiveTransform;

    private Renderer SkinRenderer;
    private Renderer[] MusclesRenderers;
    private Renderer[] SkeletonRenderers;
    private Renderer[] RespiratoryRenderer;
    private Renderer DigestiveRenderer;

    private void Start()
    {
        SkinRenderer = SkinTransform.gameObject.GetComponentInChildren<Renderer>();
        MusclesRenderers = MusclesTransform.gameObject.GetComponentsInChildren<Renderer>();
        SkeletonRenderers = SkeletonTransform.gameObject.GetComponentsInChildren<Renderer>();
        RespiratoryRenderer = RespiratoryTransform.gameObject.GetComponentsInChildren<Renderer>();
        DigestiveRenderer = DigestiveTransform.gameObject.GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        foreach (Material material in SkinRenderer.materials)
        {
            material.SetFloat("_SliceAmount", SkinSlider.value);
        }

        foreach (Renderer musclesrender in MusclesRenderers)
        {
            musclesrender.material.SetFloat("_SliceAmount", MusclesSlider.value);
        }

        foreach (Renderer skeletonrenderer in SkeletonRenderers)
        {
            skeletonrenderer.material.SetFloat("_SliceAmount", SkeletonSlider.value);
        }

        foreach (Renderer Respiratoryrender in RespiratoryRenderer)
        {
            Respiratoryrender.material.SetFloat("_SliceAmount", RespiratorySlider.value);
        }

        DigestiveRenderer.material.SetFloat("_SliceAmount", DigestiveSlider.value);

        bool valuecircultory = CirculatorySlider.value > 0.5f ? false : true;
        CirculatoryTransform.gameObject.SetActive(valuecircultory);

        bool valuenervous = NervousSlider.value > 0.5f ? false : true;
        NervousTransform.gameObject.SetActive(valuenervous);
    }

    public void Reset()
    {
        RpcReset();
        photonView.RPC("Reset", RpcTarget.All);
    }

    private void RpcReset()
    {
        foreach (Material material in SkinRenderer.materials)
        {
            material.SetFloat("_SliceAmount", 0);
        }
        foreach (Renderer musclesrender in MusclesRenderers)
        {
            musclesrender.material.SetFloat("_SliceAmount", 0);
        }
        foreach (Renderer skeletonrenderer in SkeletonRenderers)
        {
            skeletonrenderer.material.SetFloat("_SliceAmount", 0);
        }
        foreach (Renderer Respiratoryrender in RespiratoryRenderer)
        {
            Respiratoryrender.material.SetFloat("_SliceAmount", 0);
        }
        CirculatoryTransform.gameObject.SetActive(true);
        NervousTransform.gameObject.SetActive(true);
        DigestiveRenderer.material.SetFloat("_SliceAmount", 0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SkinSlider.value);
            stream.SendNext(MusclesSlider.value);
            stream.SendNext(SkeletonSlider.value);
            stream.SendNext(RespiratorySlider.value);
            stream.SendNext(DigestiveSlider.value);
            stream.SendNext(CirculatorySlider.value);
            stream.SendNext(NervousSlider.value);
        }
        else
        {
            SkinSlider.value = (float)stream.ReceiveNext();
            MusclesSlider.value = (float)stream.ReceiveNext();
            SkeletonSlider.value = (float)stream.ReceiveNext();
            RespiratorySlider.value = (float)stream.ReceiveNext();
            DigestiveSlider.value = (float)stream.ReceiveNext();
            CirculatorySlider.value = (float)stream.ReceiveNext();
            NervousSlider.value = (float)stream.ReceiveNext();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LayerCutoff : MonoBehaviour
{

    public GameObject Skinlayer;
    [SerializeField]
    private Transform Muscularsystem;
    [SerializeField]
    private Transform SkeletonSystem;
    [SerializeField]
    private Transform CirculatorySystem;
    [SerializeField]
    private Transform NervousSystem;
    [SerializeField]
    private Transform RespiratorySystem;
    [SerializeField]
    private Transform ExcretorySystem;
    [SerializeField]
    private Transform DigestiveSystem;

    private Renderer[] MusclesRenderers;
    private Renderer[] SkeletonRenderers;
    private Renderer[] CirulatoryRenderer;
    private Renderer[] NervousRenderer;
   
    private Renderer[] RespiratoryRenderer;
    private Renderer DigestiveRenderer;
    private Renderer SkinRenderer;



    void Start()
    {
        MusclesRenderers = Muscularsystem.gameObject.GetComponentsInChildren<Renderer>();
        SkeletonRenderers = SkeletonSystem.gameObject.GetComponentsInChildren<Renderer>();
        RespiratoryRenderer = RespiratorySystem.gameObject.GetComponentsInChildren<Renderer>();
        CirulatoryRenderer = CirculatorySystem.gameObject.GetComponentsInChildren<Renderer>();
        NervousRenderer = NervousSystem.gameObject.GetComponentsInChildren<Renderer>();
        DigestiveRenderer = DigestiveSystem.gameObject.GetComponentInChildren<Renderer>();

        SkinRenderer = Skinlayer.GetComponentInChildren<Renderer>();

        foreach (Renderer musclesrenderer in MusclesRenderers)
        {
            musclesrenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

        foreach (Renderer skeletons in SkeletonRenderers)
        {
            skeletons.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

        foreach (Renderer respiratory in RespiratoryRenderer)
        {
            respiratory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
        foreach (Renderer Nervours in NervousRenderer)
        {
            Nervours.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
        foreach (Renderer Circulatory in CirulatoryRenderer)
        {
            Circulatory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

      
        DigestiveRenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);

        SkinRenderer.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
    }
    public void SetMuscles(bool value)
    {
        if (value)
        {
            foreach (Renderer musclesrenderer in MusclesRenderers)
            {
                musclesrenderer.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
        else
        {
            foreach (Renderer musclesrenderer in MusclesRenderers)
            {
                musclesrenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
        }
    }

    public void SetSkeleton(bool value)
    {
        if(value)
        {

            foreach (Renderer skeletons in SkeletonRenderers)
            {
                skeletons.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
        else
        {

            foreach (Renderer skeletons in SkeletonRenderers)
            {
                skeletons.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
        }
    }
    public void SetRespiratory(bool value)
    {
        if (value)
        {
            foreach (Renderer respiratory in RespiratoryRenderer)
            {
                respiratory.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
        else
        {
            foreach (Renderer respiratory in RespiratoryRenderer)
            {
                respiratory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
        }
    
    }
    public void SetCirulatory(bool value)
    {
        if (value)
        {
            foreach (Renderer Circulatory in CirulatoryRenderer)
            {
                Circulatory.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
        else
        {
         foreach (Renderer Circulatory in CirulatoryRenderer)
        {
            Circulatory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
        }

    }
  
    public void SetDigestive(bool value)
    {
        if (value)
        {
            DigestiveRenderer.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
        else
        {
            DigestiveRenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

    }
    public void SetNervous(bool value)
    {
        if (value)
        {

            foreach (Renderer Nervours in NervousRenderer)
            {
                Nervours.material.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }

        }
        else
        {

            foreach (Renderer Nervours in NervousRenderer)
            {
                Nervours.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }

        }

    }

    public void Reset()
    {
        foreach (Renderer Nervours in NervousRenderer)
        {
            Nervours.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
        DigestiveRenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        foreach (Renderer Circulatory in CirulatoryRenderer)
        {
            Circulatory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
        foreach (Renderer respiratory in RespiratoryRenderer)
        {
            respiratory.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

        foreach (Renderer skeletons in SkeletonRenderers)
        {
            skeletons.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

        foreach (Renderer musclesrenderer in MusclesRenderers)
        {
            musclesrenderer.material.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }

    }
}

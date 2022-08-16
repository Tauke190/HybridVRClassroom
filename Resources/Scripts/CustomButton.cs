using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Color normalColor;
    public Color highlightColor;

    private Image buttonImage;

    public UnityEvent OnClick;

    private bool goneout;

    private void Start()
    {
        buttonImage = GetComponent<Image>();

        buttonImage.color = normalColor;
    }

    private void OnValidate()
    {
        GetComponent<Image>().color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.DOColor(highlightColor + new Color(0, 0, 0, -0.2f * highlightColor.a), 0.15f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.DOColor(highlightColor, 0.15f);

        goneout = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.DOColor(normalColor, 0.15f);
        goneout = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (goneout)
        {
            return;
        }

        buttonImage.DOColor(highlightColor, 0.15f);
    }
}

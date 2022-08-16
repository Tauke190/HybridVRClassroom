using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRSliderButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float sensitity = 1;

    public enum TYPE { UP, DOWN }
    public Slider slider;
    public TYPE type;

    private bool down;

    private void Update()
    {
        if (down)
        {
            if (type == TYPE.UP)
            {
                slider.value += Time.deltaTime * sensitity;
            }
            else
            {
                slider.value -= Time.deltaTime * sensitity;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("down");
        down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        down = false;
    }
}

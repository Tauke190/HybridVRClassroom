using UnityEngine;

public class rotatetotate : MonoBehaviour {

    private RectTransform rectComponent;

    public float rotateSpeed = 200f;

    // Use this for initialization
    void Start () {
        rectComponent = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {

        float currentSpeed = rotateSpeed * Time.deltaTime;
        rectComponent.Rotate(0f, 0f, currentSpeed);
    }
}
using UnityEngine;

public class IgnoreCollisionWithPlayer : MonoBehaviour
{
    private void Start()
    {
        if (FindObjectOfType<CustomPlayerLocomotion>())
        {
            Collider playerCollider = FindObjectOfType<CustomPlayerLocomotion>().GetComponent<Collider>();

            foreach (var item in GetComponents<Collider>())
            {
                Debug.Log(item.transform.name);
                Physics.IgnoreCollision(item, playerCollider);
            }

            foreach (var item in GetComponentsInChildren<Collider>())
            {
                Debug.Log(item.transform.name);
                Physics.IgnoreCollision(item, playerCollider);
            }
        }
    }
}

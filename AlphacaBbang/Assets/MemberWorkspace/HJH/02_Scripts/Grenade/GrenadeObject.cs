using System.Collections;
using UnityEngine;

public class GrenadeObject : MonoBehaviour
{
    private GrenadeBehavior behavior;
    private bool isTriggered = false;

    private void Awake()
    {
        behavior = GetComponent<GrenadeBehavior>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        if (collision.collider.CompareTag("Floor"))
        {
            isTriggered = true;

            StartCoroutine(behavior.Boom(gameObject));
        }
    }
}
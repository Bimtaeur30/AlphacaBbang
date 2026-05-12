using System.Collections;
using UnityEngine;

public class GrenadeObject : MonoBehaviour
{
    [SerializeField] private float boomTime;
    private GrenadeBehavior behavior;
    private bool isTriggered = false;

    [SerializeField] private LayerMask triggerLayer;

    private void Awake()
    {
        behavior = GetComponent<GrenadeBehavior>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        if (((1 << collision.gameObject.layer) & triggerLayer) != 0)
        {
            isTriggered = true;
            StartCoroutine(behavior.Boom(gameObject, boomTime));
        }
    }
}
using UnityEngine;

public class ImageScaler : MonoBehaviour
{
    [SerializeField] private float radius;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
}

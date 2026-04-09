using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Transform cam;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 dir = transform.position - cam.position;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}

using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class MouseFollowSphere : MonoBehaviour
    {
        [SerializeField] private float distance = 10f; 

        void Update()
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            mouseScreenPos.z = distance;
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            transform.position = mouseWorldPos;
        }
    }
}
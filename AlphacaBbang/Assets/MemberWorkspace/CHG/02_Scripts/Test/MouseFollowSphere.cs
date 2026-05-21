using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class MouseFollowSphere : MonoBehaviour
    {
        [SerializeField] private float distance = 10f;
        void LateUpdate()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 screenPos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, distance);
            transform.position = Camera.main.ScreenToWorldPoint(screenPos);
        }
    }
}
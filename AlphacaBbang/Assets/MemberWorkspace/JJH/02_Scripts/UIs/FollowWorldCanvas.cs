using UnityEngine;

namespace JJH._02_Scripts.UIs
{
    public class FollowWorldCanvas : MonoBehaviour
    {
        [SerializeField] private Transform targetTrans;
        [SerializeField] private Vector3 offset;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (targetTrans == null)
                return;

            Vector3 targetPos = targetTrans.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, 0.05f);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class PlayerVisibility : MonoBehaviour
    {
        private void Update()
        {
            RotateToMouse(); //test
        }

        #region TestCodes

        private void RotateToMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 lookPoint = hit.point;
                lookPoint.y = transform.position.y;

                Vector3 dir = (lookPoint - transform.position).normalized;

                if (dir.sqrMagnitude < 0.001f)
                    return;

                Quaternion targetRotation = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime);
            }
        }

        #endregion

    }
}
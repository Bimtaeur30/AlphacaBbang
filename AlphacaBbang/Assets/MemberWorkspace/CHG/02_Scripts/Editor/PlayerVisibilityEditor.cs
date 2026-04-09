using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MemberWorkspace.CHG._02_Scripts.Editor
{
    [CustomEditor(typeof(PlayerVisibility))]
    public class PlayerVisibilityEditor : UnityEditor.Editor
    {
        /*private void OnSceneGUI()
        {
            PlayerVisibility visibility = (PlayerVisibility)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(visibility.transform.position, Vector2.up,
                Vector3.forward, 360, visibility.viewRadius);
            Vector3 viewAngleA = visibility.DirFromAngle(-visibility.viewAngle / 2, false);
            Vector3 viewAngleB = visibility.DirFromAngle(visibility.viewAngle / 2, false);
            
            Handles.DrawLine(visibility.transform.position, visibility.transform.position + viewAngleA * visibility.viewRadius);
            Handles.DrawLine(visibility.transform.position, visibility.transform.position + viewAngleB * visibility.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visible in visibility.visibleTargets)
            {
                Handles.DrawLine(visibility.transform.position, visibility.transform.position);
            }
        }*/
    }
}

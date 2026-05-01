using System;
using UnityEngine;
using TMPro;

namespace MemberWorkspace.CHG._02_Scripts.TalkSystem
{
    public class TalkBoxMesh : MonoBehaviour
    {
        [SerializeField] private float paddingX = 0.2f;
        [SerializeField] private float paddingY = 0.2f;
        
        [SerializeField] private TextMeshPro text;
        [SerializeField] private Transform backGround;

        private void LateUpdate()
        {
            if (text == null || backGround == null) return;
            UpdateBackGround();
        }

        private void UpdateBackGround()
        {
            text.ForceMeshUpdate();
            Bounds bounds = text.textBounds;
            
            float width = bounds.size.x + paddingX * 2f;
            float height = bounds.size.y + paddingY * 2f;
            backGround.localScale = new Vector3(width, height, 1f);
            
            Vector3 center = bounds.center;
            
            backGround.localPosition = new Vector3(center.x, center.y, -0.01f);
        }
    }
}

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
        [SerializeField] private SpriteRenderer backGround;
        

        public void SetBackGroundSize()
        {
            if (text == null || backGround == null) return;
            text.ForceMeshUpdate();
            Bounds bounds = text.textBounds;
            Debug.Log(bounds.size);
            
            float width = bounds.size.x + paddingX * 2f;
            float height = bounds.size.y + paddingY * 2f;
            if (width < 0 || height < 0)
            {
                width = 0;
                height = 0;
            }
            Vector3 center = bounds.center;
            
            backGround.size = new Vector2(width, height);
            backGround.transform.localPosition = new Vector3(center.x, center.y, -0.01f);
        }
    }
}

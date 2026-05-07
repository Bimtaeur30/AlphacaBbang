using System;
using UnityEngine;
using TMPro;
using Febucci.UI;

namespace MemberWorkspace.CHG._02_Scripts.TalkSystem
{
    public class TalkBoxMesh : MonoBehaviour
    {
        [SerializeField] private float paddingX = 0.2f;
        [SerializeField] private float paddingY = 0.2f;
        
        [SerializeField] private TextMeshPro text;
        [SerializeField] private SpriteRenderer backGround;
        

        private int shownCharCount = 0;

        public void ResetCount()
        {
            shownCharCount = 0;
        }

        public void SetBackGroundSize(char _)
        {
            shownCharCount++;
            if (text == null || backGround == null) return;
            text.ForceMeshUpdate();
            Bounds bounds = text.textBounds;

            int totalCount = text.textInfo.characterCount;
            if (totalCount == 0) return;

            float charWidthAvg = bounds.size.x / totalCount;
            float currentWidth = charWidthAvg * shownCharCount + paddingX * 2f;
            float fullWidth = bounds.size.x + paddingX * 2f;
            float height = bounds.size.y + paddingY * 2f;

            float leftEdge = bounds.center.x - fullWidth * 0.5f;
            float centerX = leftEdge + currentWidth * 0.5f;

            backGround.size = new Vector2(currentWidth, height);
            backGround.transform.localPosition = new Vector3(centerX, bounds.center.y, -0.01f);

            
        }
    }
}
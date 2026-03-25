using System;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class ObstacleFade : MonoBehaviour
    {
        private Renderer _render;

        [ContextMenu("ChangeColor")]
        private void ChangeColor()
        {
            _render = GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _render.GetPropertyBlock(block);
            
            Color color = _render.sharedMaterial.GetColor("_BaseColor");
            color.a = 0.5f;
            block.SetColor("_BaseColor", color);
            
            _render.SetPropertyBlock(block);

        }
    }
}

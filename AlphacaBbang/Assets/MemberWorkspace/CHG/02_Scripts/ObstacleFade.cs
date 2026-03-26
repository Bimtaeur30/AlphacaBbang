using System;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class ObstacleFade : MonoBehaviour
    {
        [SerializeField] private Transform[] target;
        [SerializeField] private LayerMask layerMask;
        
        private Renderer _render;
        
        
        
        [ContextMenu("ChangeColor")]
        private void ChangeFade(Transform t, float alpha)
        {
            _render = t.transform.GetComponent<MeshRenderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            _render.GetPropertyBlock(block);
            
            Color color = _render.sharedMaterial.GetColor("_BaseColor");
            color.a = alpha;
            block.SetColor("_BaseColor", color);
            
            _render.SetPropertyBlock(block);

        }

        #region TestCode

        private void OnTriggerEnter(Collider collider)
        {
            foreach (Transform t in target)
            {
                ChangeFade(t, 0.3f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (Transform t in target)
            {
                ChangeFade(t, 1f);
            }
        }

        #endregion
    }
}

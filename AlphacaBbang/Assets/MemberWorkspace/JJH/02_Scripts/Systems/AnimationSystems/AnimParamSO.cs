using UnityEngine;

namespace JJH._02_Scripts_Systems.AnimationSystems
{
    [CreateAssetMenu(fileName = "Anim Parameter", menuName = "Bbang/SO/Animator/Anim Param", order = 0)]
    public class AnimParamSO : ScriptableObject
    {
        [field: SerializeField] public string ParamName { get; private set; }
        [field: SerializeField] public int ParamHash { get; private set; }

        private void OnValidate()
        {
            ParamHash = Animator.StringToHash(ParamName);
        }
    }
}

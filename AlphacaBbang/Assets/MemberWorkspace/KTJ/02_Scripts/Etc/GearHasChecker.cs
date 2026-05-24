using Reflex.Attributes;
using UnityEngine;

public class GearHasChecker : MonoBehaviour
{
    [Inject] private SceneChangeManager _sceneChangeManager;
    public void Check()
    {
        if (true) // 인벤토리에 기어 3개 다 존재하면
        {
            _sceneChangeManager.SceneLoad(SceneType.END_SCENE);
        }
    }
}

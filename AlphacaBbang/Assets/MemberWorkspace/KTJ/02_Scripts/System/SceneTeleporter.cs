using Reflex.Attributes;
using UnityEngine;

public class SceneTeleporter : MonoBehaviour
{
    [Inject] private SceneChangeManager sceneChangeManager;
    [SerializeField] private SceneType targetScene;

    public void SceneChange()
    {
        sceneChangeManager.SceneLoad(targetScene);
    }
}

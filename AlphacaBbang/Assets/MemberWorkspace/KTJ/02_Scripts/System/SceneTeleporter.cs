using Reflex.Attributes;
using UnityEngine;

public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private SceneChangeManager sceneChangeManager;
    [SerializeField] private SceneType targetScene;

    public void SceneChange()
    {
        sceneChangeManager.SceneLoad(targetScene);
    }
}

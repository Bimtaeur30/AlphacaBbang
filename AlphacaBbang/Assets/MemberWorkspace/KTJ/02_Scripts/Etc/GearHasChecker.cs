using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.CHG._02_Scripts;
using Reflex.Attributes;
using UnityEngine;

public class GearHasChecker : MonoBehaviour
{
    [Inject] private SceneChangeManager _sceneChangeManager;
    [SerializeField] private InventoryContainer InventoryContainer;
    [SerializeField] private EventChannelSO systemChannel;
    public void Check()
    {
        if (InventoryContainer.CanEscape()) // 인벤토리에 기어 3개 다 존재하면
        {
            _sceneChangeManager.SceneLoad(SceneType.END_SCENE);
        }
        else
        {
            systemChannel.RaiseEvent(SystemEvents.SystemNotificationEvent.Init("부품 부족", "빨, 초, 파 부품이 인벤토리에 있어야해요."));
        }
    }
}

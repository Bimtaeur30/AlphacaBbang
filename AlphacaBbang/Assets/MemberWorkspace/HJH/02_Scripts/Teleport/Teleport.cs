using UnityEngine;
using UnityEngine.InputSystem;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float teleportDistance;
    public int TeleportIndex;

    private bool isTeleport = false;

    public void TeleportButton()
    {
        Debug.Log($"눌림 2 {isTeleport}");
        if (!isTeleport) return;
        TeleportController.Instance.TeleportTo(TeleportIndex);
        Debug.Log("눌림 3");
    }

    private void Update()
    {
        isTeleport = Vector3.Distance(transform.position, player.position) <= teleportDistance;
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TeleportButton();
            Debug.Log("눌림 1");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isTeleport ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, teleportDistance);
    }
}

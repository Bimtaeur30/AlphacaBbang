using UnityEngine;

public class TeleportInteractable : MonoBehaviour, IInteractable
{
    public string ObjectText => objectText;
    public string ActionText => actionText;
    public float InteractRange => interactRange;

    [Header("Text")]
    public string objectText = "Æ÷Å»";
    public string actionText = "ÀÌµ¿";

    [Header("Settings")]
    public float interactRange = 2f;
    public int teleportIndex = 0;

    public void Interact()
    {
        Debug.Log("Çàµ¿Àº µÊ.");
        TeleportController.Instance.TeleportTo(teleportIndex);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawSphere(transform.position, interactRange);
        Gizmos.color = new Color(1f, 1f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
using UnityEngine;

public class SpawnPortals : MonoBehaviour
{
    public GameObject portalPrefab;
    public Transform spawnPoint;

    public void SpawnPortal()
    {
        if (portalPrefab == null) return;
        Instantiate(portalPrefab, spawnPoint.position, Quaternion.identity);
    }
}

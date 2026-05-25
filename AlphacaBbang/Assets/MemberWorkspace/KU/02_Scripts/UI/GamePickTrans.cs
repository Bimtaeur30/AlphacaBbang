using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GamePickTrans : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        StartCoroutine(WaitTime(2));
    }
    public void SetSpawnTrans(int num)
    {
        Time.timeScale = 1f;
        player.transform.position = spawnPoints[num].position;
        gameObject.SetActive(false);
    }

    private IEnumerator WaitTime(int num)
    {
        yield return new WaitForSeconds(num);
        Time.timeScale = 0f;

    }
}

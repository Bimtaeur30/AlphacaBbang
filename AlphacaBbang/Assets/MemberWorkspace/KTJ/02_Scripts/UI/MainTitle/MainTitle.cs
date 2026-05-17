using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class MainTitle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startGameTxt;
    [Inject] private DataManager dataManager;

    private void Start()
    {
        bool hasPlayed = dataManager.HasSaveData();
        startGameTxt.text = hasPlayed ? "이어하기" : "시작하기";
    }
}

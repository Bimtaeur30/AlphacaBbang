// =============================================================
// StageMapButton.cs
// 위치: Assets/Scripts/UI/StageMapButton.cs
// =============================================================
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StageMapButton : MonoBehaviour
{
    [SerializeField] private int stageId;
    [SerializeField] private string methodName;
    [SerializeField] private GameObject eventReceiver;

    [Space(6)]
    public UnityEvent<int> onStageClicked = new();

    private Button _btn;

    public int StageId => stageId;
    public string MethodName => methodName;
    public GameObject EventReceiver => eventReceiver;

    public void Init(int id, string method, GameObject receiver)
    {
        stageId = id;
        methodName = method;
        eventReceiver = receiver;
    }

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (_btn != null) _btn.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        onStageClicked?.Invoke(stageId);

        if (eventReceiver != null && !string.IsNullOrWhiteSpace(methodName))
            eventReceiver.SendMessage(methodName, stageId,
                SendMessageOptions.DontRequireReceiver);

        Debug.Log($"[StageMap] Stage {stageId} clicked => {methodName}");
    }
}
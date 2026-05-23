using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoutPlayer : MonoBehaviour
{
    [SerializeField] private RoutRecorder routRecorder;
    [SerializeField] private EventChannelSO mapEventChannel;
    [SerializeField] private LineRenderer routLineRenderer;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI recordTimeTxt;
    [SerializeField] private TextMeshProUGUI gotoLobbyTimer;
    [SerializeField] private SceneTeleporter sceneTeleporter;
    [SerializeField] private Button playButton;
    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;
    [SerializeField] private Button lobbyBtn;
    [SerializeField] private LogBar logBar;
    [SerializeField] private RectTransform logBarParent;
    [SerializeField] private Sprite logBarIcon;
    [SerializeField] private CanvasGroup ParentPanel;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float minFov = 30;
    [SerializeField] private float maxFov = 150;
    [SerializeField] private float zoomAmount = 10;

    private float _recordTime = 0f;
    private Coroutine _playCoroutine;
    private Coroutine _lobbyCountdownCoroutine;

    private const int LobbyWaitSeconds = 10;

    private void Awake()
    {
        mapEventChannel.AddListener<RoutRecordEndEvent>(OnRoutRecordEndEvent);

        playButton.onClick.AddListener(() =>
        {
            StopPlayCoroutines();
            StopLobbyCountdown();

            _playCoroutine = StartCoroutine(PlayRout());
        });

        zoomInButton.onClick.AddListener(() =>
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomAmount, minFov, maxFov);
        });

        zoomOutButton.onClick.AddListener(() =>
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomAmount, minFov, maxFov);
        });

        lobbyBtn.onClick.AddListener(() =>
        {
            StopPlayCoroutines();
            StopLobbyCountdown();

            sceneTeleporter.SceneChange();
        });
    }

    private void OnDestroy()
    {
        mapEventChannel.RemoveListener<RoutRecordEndEvent>(OnRoutRecordEndEvent);

        StopPlayCoroutines();
        StopLobbyCountdown();
    }

    private void OnRoutRecordEndEvent(RoutRecordEndEvent @event)
    {
        _recordTime = @event.RecordTime;

        Debug.Log($"[RoutPlayer] RoutRecordEndEvent 수신 | recordTime: {_recordTime:F2}s | Routs 수: {routRecorder.Routs.Count}");

        StopPlayCoroutines();
        StopLobbyCountdown();

        ParentPanel.gameObject.SetActive(true);
        ParentPanel.DOFade(1f, 1f);

        _playCoroutine = StartCoroutine(PlayRout());
    }

    private void StopPlayCoroutines()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
    }

    private void StopLobbyCountdown()
    {
        if (_lobbyCountdownCoroutine != null)
        {
            StopCoroutine(_lobbyCountdownCoroutine);
            _lobbyCountdownCoroutine = null;
        }
    }

    private IEnumerator PlayRout()
    {
        ResetLogs();

        if (gotoLobbyTimer != null)
            gotoLobbyTimer.text = "리플레이 재생중";

        var routs = new List<(Vector3 point, List<ActionInfo> actions)>(routRecorder.Routs);

        Debug.Log($"[RoutPlayer] PlayRout 시작 | 스냅샷 Routs 수: {routs.Count} | 예상 총 시간: {routs.Count * MapRoutData.RECORD_INTERVAL:F2}s");

        if (routs.Count == 0)
        {
            _playCoroutine = null;
            StartLobbyCountdown();
            yield break;
        }

        routLineRenderer.positionCount = 0;

        for (int i = 0; i < routs.Count; i++)
        {
            routLineRenderer.positionCount++;
            routLineRenderer.transform.position = routs[i].point;
            routLineRenderer.SetPosition(i, routs[i].point);

            float originalTime = i * MapRoutData.RECORD_INTERVAL;
            UpdateTimerText(originalTime);

            for (int j = 0; j < routs[i].actions.Count; j++)
            {
                string actionText = routs[i].actions[j].Action;

                Debug.Log($"[RoutPlayer] i={i} | 시간={originalTime:F2}s | 액션: '{actionText}'");

                LogBar bar = Instantiate(logBar, logBarParent);
                bar.Init(logBarIcon, $"[{FormatTime(originalTime)}] {actionText}");
            }

            yield return new WaitForSeconds(MapRoutData.PLAY_INTERVAL);
        }

        Debug.Log($"[RoutPlayer] 재생 완료 | _recordTime: {_recordTime:F2}s");

        UpdateTimerText(_recordTime);

        _playCoroutine = null;
        StartLobbyCountdown();
    }

    private void StartLobbyCountdown()
    {
        StopLobbyCountdown();
        _lobbyCountdownCoroutine = StartCoroutine(LobbyCountdownRoutine());
    }

    private IEnumerator LobbyCountdownRoutine()
    {
        for (int i = LobbyWaitSeconds; i > 0; i--)
        {
            if (gotoLobbyTimer != null)
                gotoLobbyTimer.text = $"{i}초 뒤에 로비로 이동합니다";

            yield return new WaitForSeconds(1f);
        }

        if (gotoLobbyTimer != null)
            gotoLobbyTimer.text = "로비로 이동합니다";

        _lobbyCountdownCoroutine = null;
        sceneTeleporter.SceneChange();
    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int centiseconds = (int)(time * 100) % 100;

        return $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
    }

    private void UpdateTimerText(float time)
    {
        recordTimeTxt.text = FormatTime(time);
    }

    private void ResetLogs()
    {
        for (int i = logBarParent.childCount - 1; i >= 0; i--)
        {
            Destroy(logBarParent.GetChild(i).gameObject);
        }
    }
}
using DG.Tweening;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class RoutPlayer : MonoBehaviour
{
    [SerializeField] private RoutRecorder routRecorder;
    [SerializeField] private EventChannelSO mapEventChannel;
    [SerializeField] private LineRenderer routLineRenderer;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI recordTimeTxt;
    [SerializeField] private Button playButton;
    [SerializeField] private Button zoomInButton;
    [SerializeField] private Button zoomOutButton;
    [SerializeField] private LogBar logBar;
    [SerializeField] private RectTransform logBarParent;
    [SerializeField] private Sprite a;
    [SerializeField] private CanvasGroup ParentPanel;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float minFov = 30;
    [SerializeField] private float maxFov = 150;
    [SerializeField] private float zoomAmount = 10;

    private float _recordTime = 0f;
    private Coroutine _playCoroutine;

    private void Awake()
    {
        mapEventChannel.AddListener<RoutRecordEndEvent>(OnRoutRecordEndEvent);
        playButton.onClick.AddListener(() =>
        {
            if (_playCoroutine != null)
                StopAllCoroutines();
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
    }

    private void OnDestroy()
    {
        mapEventChannel.RemoveListener<RoutRecordEndEvent>(OnRoutRecordEndEvent);
    }

    private void OnRoutRecordEndEvent(RoutRecordEndEvent @event)
    {
        _recordTime = @event.RecordTime;

        if (_playCoroutine != null)
            StopAllCoroutines();

        ParentPanel.DOFade(1f, 1f);
        _playCoroutine = StartCoroutine(PlayRout());
    }

    private IEnumerator PlayRout()
    {
        ResetLogs();
        var routs = new List<(Vector3 point, List<ActionInfo> actions)>(routRecorder.Routs);

        if (routs.Count == 0)
            yield break;

        routLineRenderer.positionCount = 0;

        var timerCoroutine = StartCoroutine(TickTimer());

        for (int i = 0; i < routs.Count; i++)
        {
            routLineRenderer.positionCount++;
            routLineRenderer.gameObject.transform.position = routs[i].point;
            routLineRenderer.SetPosition(i, routs[i].point);

            for (int j = 0; j < routs[i].actions.Count; j++)
            {
                LogBar bar = Instantiate(logBar, logBarParent);
                float logTime = i * MapRoutDataSO.RECORD_INTERVAL;
                bar.Init(a, $"[{FormatTime(logTime)}] {routs[i].actions[j].Action}");
            }
            yield return new WaitForSeconds(MapRoutDataSO.PLAY_INTERVAL);
        }

        StopCoroutine(timerCoroutine);
        UpdateTimerText(_recordTime); // 재생 완료 후 실제 녹화 시간으로 고정

        _playCoroutine = null;
    }

    private IEnumerator TickTimer()
    {
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime / MapRoutDataSO.PLAY_SPEED; // 실제 경과 시간 기준으로 환산
            UpdateTimerText(elapsed);
            yield return null;
        }
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
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int centiseconds = (int)(time * 100) % 100;

        recordTimeTxt.text = $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";
    }

    private void ResetLogs()
    {
        for (int i = logBarParent.transform.childCount - 1; i >= 0; i--) // 로그 초기화
        {
            Destroy(logBarParent.transform.GetChild(i).gameObject);
        }
    }
}
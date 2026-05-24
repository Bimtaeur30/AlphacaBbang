using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// 클릭할 때마다 텍스트가 순서대로 표시되는 시퀀스 플레이어.
/// 인스펙터에서 TextEntry 리스트에 텍스트를 추가하세요.
/// DOTween 패키지가 필요합니다.
///
/// 동작 구조:
///   시작 → 첫 텍스트 표시(홀드 대기)
///   클릭  → 페이드 아웃 → 다음 텍스트 페이드 인 → 홀드 대기 → 반복
/// </summary>
public class TextSequencePlayer : MonoBehaviour, IPointerClickHandler
{
    // ─────────────────────────────────────────────
    //  인스펙터 설정
    // ─────────────────────────────────────────────

    [System.Serializable]
    public class TextEntry
    {
        [TextArea(2, 5)]
        public string text;
    }
    [SerializeField] private UnityEvent onEnd;

    [Header("텍스트 목록 (순서대로 표시됨)")]
    [SerializeField] private List<TextEntry> textEntries = new List<TextEntry>();

    [Header("표시할 TMP 텍스트 컴포넌트")]
    [SerializeField] private TextMeshProUGUI displayText;

    [Header("애니메이션 설정")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;

    [Header("마지막 텍스트 이후 동작")]
    [SerializeField] private bool loopSequence = false;  // true: 처음으로 돌아감

    // ─────────────────────────────────────────────
    //  내부 상태
    // ─────────────────────────────────────────────

    private int currentIndex = 0;
    private bool isAnimating = false;

    // ─────────────────────────────────────────────
    //  초기화 — 첫 텍스트를 alpha 1로 바로 표시
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (displayText == null)
        {
            Debug.LogError("[TextSequencePlayer] displayText가 인스펙터에 연결되지 않았습니다.");
            return;
        }

        if (textEntries != null && textEntries.Count > 0)
        {
            currentIndex = 0;
            displayText.text = textEntries[0].text;
            SetAlpha(1f);   // 처음부터 홀드 상태
        }
        else
        {
            displayText.text = "";
            SetAlpha(0f);
        }
    }

    // ─────────────────────────────────────────────
    //  클릭 처리 (IPointerClickHandler)
    // ─────────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAnimating) return;
        if (textEntries == null || textEntries.Count == 0) return;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= textEntries.Count)
        {
            if (loopSequence)
                nextIndex = 0;
            else
                onEnd?.Invoke();
                return;
        }

        StartCoroutine(PlayTransition(nextIndex));
    }

    // ─────────────────────────────────────────────
    //  트랜지션 코루틴
    //  홀드(대기) → 페이드 아웃 → 텍스트 교체 → 페이드 인 → 홀드(대기)
    // ─────────────────────────────────────────────

    private IEnumerator PlayTransition(int nextIndex)
    {
        isAnimating = true;

        // 1) 현재 텍스트 페이드 아웃 (alpha 1 → 0)
        bool done = false;
        displayText.DOFade(0f, fadeOutDuration)
                   .SetEase(fadeOutEase)
                   .OnComplete(() => done = true);
        yield return new WaitUntil(() => done);

        // 2) 텍스트 교체
        currentIndex = nextIndex;
        displayText.text = textEntries[currentIndex].text;

        // 3) 다음 텍스트 페이드 인 (alpha 0 → 1)
        done = false;
        displayText.DOFade(1f, fadeInDuration)
                   .SetEase(fadeInEase)
                   .OnComplete(() => done = true);
        yield return new WaitUntil(() => done);

        // 4) 홀드 — 다음 클릭까지 대기
        isAnimating = false;
    }

    // ─────────────────────────────────────────────
    //  유틸리티
    // ─────────────────────────────────────────────

    private void SetAlpha(float alpha)
    {
        Color c = displayText.color;
        c.a = alpha;
        displayText.color = c;
    }

    /// <summary>
    /// 외부에서 시퀀스를 처음부터 다시 시작할 때 호출.
    /// </summary>
    public void ResetSequence()
    {
        StopAllCoroutines();
        DOTween.Kill(displayText);
        isAnimating = false;

        if (textEntries != null && textEntries.Count > 0)
        {
            currentIndex = 0;
            displayText.text = textEntries[0].text;
            SetAlpha(1f);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        fadeOutDuration = Mathf.Max(0.01f, fadeOutDuration);
        fadeInDuration = Mathf.Max(0.01f, fadeInDuration);
    }
#endif
}
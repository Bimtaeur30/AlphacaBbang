using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class FadeTarget
{
    public CanvasGroup canvasGroup;   // UI용 (Image, Text 등)
    public SpriteRenderer spriteRenderer; // 스프라이트용
    public float targetAlpha = 1f;
    public float transitionTime = 1f;
}

[System.Serializable]
public class FadeGroup
{
    public string groupName;
    public List<FadeTarget> targets = new List<FadeTarget>();
}

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private List<FadeGroup> fadeGroups = new List<FadeGroup>();
    [SerializeField] private UnityEvent OnEnd;

    private int currentGroupIndex = 0;
    private bool isFading = false;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isFading)
        {
            TryPlayNextGroup();
        }
    }

    private void TryPlayNextGroup()
    {
        if (currentGroupIndex >= fadeGroups.Count)
        {
            Debug.Log("컷신 끝");
            return;
        }

        FadeGroup group = fadeGroups[currentGroupIndex];
        currentGroupIndex++;

        StartCoroutine(PlayFadeGroup(group));
    }

    private IEnumerator PlayFadeGroup(FadeGroup group)
    {
        isFading = true;

        // 그룹 내 모든 Fade를 병렬 실행
        float maxDuration = 0f;
        var tweens = new List<Tween>();

        foreach (FadeTarget target in group.targets)
        {
            float clampedAlpha = Mathf.Clamp01(target.targetAlpha);
            Tween tween = null;

            if (target.canvasGroup != null)
            {
                tween = target.canvasGroup
                    .DOFade(clampedAlpha, target.transitionTime)
                    .SetEase(Ease.InOutSine);
            }
            else if (target.spriteRenderer != null)
            {
                tween = target.spriteRenderer
                    .DOFade(clampedAlpha, target.transitionTime)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                Debug.LogWarning($"[{group.groupName}] FadeTarget에 CanvasGroup 또는 SpriteRenderer가 없습니다.");
                continue;
            }

            tweens.Add(tween);
            if (target.transitionTime > maxDuration)
                maxDuration = target.transitionTime;
        }

        // 가장 긴 트윈이 끝날 때까지 대기
        yield return new WaitForSeconds(maxDuration);

        isFading = false;

        // 마지막 그룹까지 완료됐으면 종료 로그
        if (currentGroupIndex >= fadeGroups.Count)
        {
            Debug.Log("컷신 끝");
            OnEnd?.Invoke();
        }
    }
}
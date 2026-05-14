using DG.Tweening;
using TMPro;
using UnityEngine;
using System;

public class DamageTextItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText;
    private Vector3 startLocalPos;

    public void Play(float damage, Action onComplete)
    {
        gameObject.SetActive(true);
        damageText.text = Mathf.CeilToInt(damage).ToString();
        damageText.transform.localScale = Vector3.one;
        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, 1f);
        transform.localPosition = startLocalPos;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(startLocalPos.y + 100f, 1f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DOScale(Vector3.one * 1.5f, 1f).SetEase(Ease.OutBack));
        sequence.Join(damageText.DOFade(0f, 1f).SetEase(Ease.InQuart));
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void SetStartPosition(Vector3 localPos)
    {
        startLocalPos = localPos;
    }
}
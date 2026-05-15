using DG.Tweening;
using TMPro;
using UnityEngine;
using System;

public class DamageTextItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText;

    public void Play(float damage, Vector3 worldPosition, Action onComplete)
    {
        transform.DOKill();
        damageText.transform.DOKill();

        gameObject.SetActive(true);
            
        transform.position = worldPosition;

        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        damageText.transform.localRotation = Quaternion.identity;
        damageText.transform.localScale = Vector3.one;

        damageText.text = Mathf.CeilToInt(damage).ToString();

        Color c = damageText.color;
        c.a = 1f;
        damageText.color = c;

        float randomXDir = UnityEngine.Random.Range(-1.5f, 1.5f);

        float rotDir = UnityEngine.Random.value > 0.5f ? 1f : -1f;
        float rotAmount = 35f;

        Vector3 targetPos = new Vector3(
            worldPosition.x + randomXDir,
            worldPosition.y + 2f,
            worldPosition.z);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            damageText.transform.DOLocalRotate(
                new Vector3(0, 0, rotDir * rotAmount),
                0.1f).SetEase(Ease.OutQuad));
        sequence.Append(
            damageText.transform.DOLocalRotate(
                new Vector3(0, 0, -rotDir * rotAmount),
                0.1f).SetEase(Ease.InOutQuad));
        sequence.Append(
            damageText.transform.DOLocalRotate(
                Vector3.zero,
                0.1f).SetEase(Ease.OutQuad));
        sequence.Join(
            transform.DOMove(targetPos, 1.2f)
            .SetEase(Ease.OutCubic));
        sequence.Join(
            transform.DOScale(Vector3.one * 1.5f, 0.5f)
            .SetEase(Ease.OutBack));
        sequence.AppendInterval(0.2f);
        sequence.Append(
            damageText.DOFade(0f, 0.35f)
            .SetEase(Ease.InQuart));
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}
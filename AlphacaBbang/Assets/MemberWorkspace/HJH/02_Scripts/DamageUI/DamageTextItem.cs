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
        transform.localRotation = Quaternion.identity;

        float randomXDir = UnityEngine.Random.Range(-1.5f, 1.5f); // 날아갈 X 방향 랜덤
        float randomRotDir = UnityEngine.Random.Range(-0.1f, 0.1f); // 회전 방향 랜덤

        Sequence sequence = DOTween.Sequence();

        // 왼쪽으로 기울기
        sequence.Append(damageText.transform.DOLocalRotate(new Vector3(0, 0, randomRotDir * 100), 0.1f).SetEase(Ease.OutQuad));
        sequence.Append(damageText.transform.DOLocalRotate(new Vector3(0, 0, -randomRotDir * 100), 0.1f).SetEase(Ease.InOutQuad));
        // 오른쪽으로 기울기
        sequence.Join(damageText.transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
        // 랜덤 방향
        sequence.Join(transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
        sequence.Join(transform.DOLocalMove(
            new Vector3(startLocalPos.x + randomXDir * 0.5f, startLocalPos.y + 3f, startLocalPos.z),
            1.2f).SetEase(Ease.OutCubic));
        sequence.Join(transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(damageText.DOFade(0f, 0.7f).SetEase(Ease.InQuart));
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
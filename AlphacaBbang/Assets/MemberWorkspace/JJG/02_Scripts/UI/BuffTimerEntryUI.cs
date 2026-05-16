using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffTimerEntryUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _timerText;

    public void Init(Sprite icon, string buffName, float duration)
    {
        if (_iconImage != null) _iconImage.sprite = icon;
        if (_nameText != null) _nameText.text = buffName;
        StartCoroutine(Countdown(duration));
    }

    private IEnumerator Countdown(float duration)
    {
        float remaining = duration;
        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            remaining = Mathf.Max(remaining, 0f);

            if (_fillImage != null) _fillImage.fillAmount = remaining / duration;
            if (_timerText != null) _timerText.text = $"{remaining:F1}s";

            yield return null;
        }

        Destroy(gameObject);
    }
}

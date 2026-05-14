using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxOpeningUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _timeText;

    private void Awake()
    {
        if (_slider != null)
        {
            _slider.minValue = 0f;
            _slider.maxValue = 1f;
            _slider.value    = 0f;
            _slider.interactable = false;
        }

        _root.SetActive(false);
    }

    public void Show()
    {
        if (_slider != null) _slider.value = 0f;
        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }

    public void SetProgress(float current, float total)
    {
        if (_slider != null)
            _slider.value = total > 0f ? Mathf.Clamp01(current / total) : 1f;

        if (_timeText != null)
            _timeText.text = $"{current:F1} / {total:F1}s";
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CameraZoomSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Slider sizeSlider;

    [Header("Size Settings")]
    [SerializeField] private float minSize = 3f;
    [SerializeField] private float maxSize = 10f;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        InitializeSlider();
    }

    private void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(ChangeCameraSize);
    }

    private void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveListener(ChangeCameraSize);
    }

    private void InitializeSlider()
    {
        sizeSlider.minValue = 0f;
        sizeSlider.maxValue = 1f;

        float normalizedValue = Mathf.InverseLerp(
            minSize,
            maxSize,
            targetCamera.orthographicSize
        );

        sizeSlider.SetValueWithoutNotify(normalizedValue);
    }

    private void ChangeCameraSize(float value)
    {
        float targetSize = Mathf.Lerp(minSize, maxSize, value);

        targetCamera.orthographicSize = targetSize;
    }
}

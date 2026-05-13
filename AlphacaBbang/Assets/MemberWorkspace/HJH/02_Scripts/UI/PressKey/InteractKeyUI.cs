using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class InteractKeyUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject rootCanvas;
    public Image circleBackground;
    public Image darkOverlay;
    public CanvasGroup contentGroup;
    public TextMeshProUGUI objectText;
    public TextMeshProUGUI actionText;

    [Header("Settings")]
    public Key pressKey = Key.E;
    public float fillDuration = 1.5f;
    public float restoreSpeed = 8f;

    [Range(0f, 1f)]
    public float backgroundAlpha = 0.45f;
    [Range(0f, 1f)]
    public float contentAlpha = 0.15f;

    private float fillAmount = 0f;
    private bool isCompleted = false;
    private IInteractable currentTarget;


    private float pressedTime = 0f;
    void Start()
    {
        darkOverlay.type = Image.Type.Filled;
        darkOverlay.fillMethod = Image.FillMethod.Vertical;
        darkOverlay.fillOrigin = (int)Image.OriginVertical.Bottom;
        darkOverlay.fillAmount = 0f;
        darkOverlay.raycastTarget = false;
        rootCanvas.SetActive(false);
    }

    public void Show(IInteractable target)
    {
        currentTarget = target;
        objectText.text = target.ObjectText;
        actionText.text = target.ActionText;
        fillAmount = 0f;
        isCompleted = false;
        rootCanvas.SetActive(true);
    }

    public void Hide()
    {
        currentTarget = null;
        fillAmount = 0f;
        isCompleted = false;
        rootCanvas.SetActive(false);
    }

    void Update()
    {
        if (currentTarget == null) return;
        bool isPressed = Keyboard.current[pressKey].isPressed;

        if (isPressed && !isCompleted)
        {
            pressedTime += Time.deltaTime;   // 누르는 동안만 누적
            fillAmount = Mathf.Clamp01(pressedTime / fillDuration);

            if (fillAmount >= 1f)
            {
                isCompleted = true;
                currentTarget.Interact();
            }
        }
        else if (!isPressed)
        {
            isCompleted = false;
            pressedTime -= Time.deltaTime * restoreSpeed;   // 복귀
            pressedTime = Mathf.Clamp(pressedTime, 0f, fillDuration);
            fillAmount = Mathf.Clamp01(pressedTime / fillDuration);
        }

        float currentAlpha = Mathf.Lerp(0f, backgroundAlpha, fillAmount);
        darkOverlay.color = new Color(0f, 0f, 0f, currentAlpha);
        darkOverlay.fillAmount = fillAmount;
        contentGroup.alpha = Mathf.Lerp(1f, 1f - contentAlpha, fillAmount);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractKeyUI : MonoBehaviour
{
    [Header("UI References")]
    public Image keyBackground;

    [Header("Colors")]
    public Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float transSpeed = 10f;

    private Color targetColor;

    void Start()
    {
        targetColor = normalColor;
        keyBackground.color = normalColor;
    }

    void Update()
    {
        if (Keyboard.current.insertKey.isPressed)
        {
            targetColor = pressedColor;
        }
        else
        {
            targetColor = normalColor;
        }

        keyBackground.color = Color.Lerp(
            keyBackground.color,    
            targetColor,
            Time.deltaTime * transSpeed
        );
    }
}
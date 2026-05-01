using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogBar : MonoBehaviour
{
    [SerializeField] private Image FirstIconImage;
    [SerializeField] private Image SecondIconImage;
    [SerializeField] private TextMeshProUGUI MessagTxt;
    
    public void SetLogBar(Sprite firstIcon, Sprite secondIcon, string message)
    {
        FirstIconImage.sprite = firstIcon;
        SecondIconImage.sprite = secondIcon;
        MessagTxt.text = message;
    }
}

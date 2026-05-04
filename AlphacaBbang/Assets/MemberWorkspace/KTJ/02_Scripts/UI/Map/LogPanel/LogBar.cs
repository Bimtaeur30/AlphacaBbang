using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogBar : MonoBehaviour
{
    [SerializeField] private Image FirstIconImage;
    [SerializeField] private TextMeshProUGUI MessagTxt;
    
    public void Init(Sprite Icon, string message)
    {
        FirstIconImage.sprite = Icon;
        MessagTxt.text = message;
    }
}

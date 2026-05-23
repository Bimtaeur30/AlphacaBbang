using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemberWorkspace.CHG._02_Scripts.PlayerStat
{
    public class PlayerStatUpUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI beforeStatText;
        [SerializeField] private TextMeshProUGUI afterStatText;
        [SerializeField] private Button statUpButton;
        
        public void StatTextChange(string before, string after)
        {
            beforeStatText.text = before;
            afterStatText.text = after;
        }
        
        public void StatUpButtonInteractableChange(bool value) => statUpButton.interactable = value;
        
    }
}
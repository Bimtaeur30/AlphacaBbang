using TMPro;
using UnityEngine;

public class TalkBox : MonoBehaviour
{
    [SerializeField] private GameObject talkBox;
    //[SerializeField] private 
    
    public void TalkStart()
    {
        //플레이어 이동 금지.
        talkBox.SetActive(true);
    }

    public void TalkEnd()
    {
        
    }
    
}

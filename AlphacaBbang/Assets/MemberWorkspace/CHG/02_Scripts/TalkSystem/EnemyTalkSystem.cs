using Febucci.UI;
using MemberWorkspace.CHG._02_Scripts.TextBoxSystem;
using System.Collections;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.TalkSystem
{
    public class EnemyTalkSystem : MonoBehaviour
    {
        [SerializeField] private DialogueNodeSO[] battleTexts;
        [SerializeField] private float showTime = 1.5f;
        [SerializeField] private float writeTime = 0.8f;
        [SerializeField] private GameObject mainTalkBox;
        private TypewriterByCharacter _mainTypewriter;
        private Coroutine _typingCoroutine;

        private void Awake()
        {
            _mainTypewriter = mainTalkBox.GetComponentInChildren<TypewriterByCharacter>();

            Debug.Assert(_mainTypewriter != null, $"{gameObject.name}: TypewriterByCharacter not found");

            _mainTypewriter.waitForNormalChars = writeTime;
        }

        [ContextMenu("ShowText")]
        public void ShowText()
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }
            _mainTypewriter.StopShowingText();
             _typingCoroutine = StartCoroutine(BattleTalk());
        }

        private IEnumerator BattleTalk()
        {
            mainTalkBox.SetActive(true);

            int rand = Random.Range(0, battleTexts.Length);

            _mainTypewriter.ShowText(battleTexts[rand].Text);

            yield return new WaitForSeconds(showTime);
            mainTalkBox.SetActive(false);
        }
    }
}
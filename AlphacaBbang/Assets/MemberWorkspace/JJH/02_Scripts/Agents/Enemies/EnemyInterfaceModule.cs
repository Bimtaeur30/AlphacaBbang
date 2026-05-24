using MemberWorkspace.CHG._02_Scripts.TalkSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJH._02_Scripts.Agents.Enemies
{
    public class EnemyInterfaceModule : MonoBehaviour, IModule, IEnemyInterface, IAfterInitModule
    {
        [Header("UIs")]
        [field: SerializeField] public EnemyTalkSystem EnemyTalk { get; private set; }
        [SerializeField] private Image _enemyIcon;
        [SerializeField] private TextMeshPro _nameText;

        [Header("Canvas Disable")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _disableDistance = 30f;

        private AbstractEnemy _owner;

        public void Initialize(ModuleOwner owner)
        {
            _owner = (AbstractEnemy)owner;
        }

        private void Update()
        {
            if (_owner.Sensor.IsTargetInRange(_disableDistance, out Collider hitCollider))
            {
                SetInterfaceShow(true);
            }
            else
            {
                SetInterfaceShow(false);
            }
        }

        public void AfterInitalize()
        {
            SetInterfaceShow(true);
            _nameText.text = _owner.EnemyData.EnemyName;
        }

        public void SetInterfaceShow(bool value)
        {
            _nameText.gameObject.SetActive(value);
            _enemyIcon.gameObject.SetActive(_owner.EnemyData.IsBoss && value);
        }
    }
}
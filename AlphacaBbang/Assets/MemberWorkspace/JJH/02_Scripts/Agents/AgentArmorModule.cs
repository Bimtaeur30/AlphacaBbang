using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Weapons;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents
{
    public class AgentArmorModule : MonoBehaviour, IModule, IAgentArmor
    {
        [SerializeField] private Transform HelmetModelParent;
        [SerializeField] private Transform BodyModelParent;
        private GameObject _currentHelmetModel;
        private GameObject _currentBodyModel;

        //[Header("TEST")]
        //[SerializeField] private ArmorSO test_helmet;
        //[SerializeField] private ArmorSO test_body;



        public Dictionary<ArmorType, ArmorSO> armors { get; private set; } = new();
        public bool HeadArmorEquiped { get; private set; } = false;
        public bool BodyArmorEquiped { get; private set; } = false;

        private Agent _owner;
        private EventChannelSO _agentEventChannel;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner as Agent;
            _agentEventChannel = _owner.AgentEventChannel;
        }

        //[ContextMenu("TestEquip_HELMET")]
        //public void TestEquip_HELMET()
        //{
        //    ArmorEquip(true, ArmorType.Helmet, test_helmet);
        //}

        //[ContextMenu("TestEquip_BODY")]
        //public void TestEquip_BODY()
        //{
        //    ArmorEquip(true, ArmorType.Body, test_body);
        //}

        public void ArmorEquip(bool value, ArmorType armorType, ArmorSO armorSO)
        {
            Debug.Assert(armorSO.ArmorModel != null, "armorSO에 모델 오브젝트가 존재하지 않습니다.");

            if (value)
            {
                if(armors.ContainsKey(armorType))
                    armors[armorType] = armorSO;
                else
                    armors.Add(armorType, armorSO);

                switch (armorType)
                {
                    case ArmorType.Helmet:
                        if (_currentHelmetModel != null)
                            Destroy(_currentHelmetModel); // 기존 모델 제거
                        _currentHelmetModel = Instantiate(armorSO.ArmorModel, HelmetModelParent);
                        _currentHelmetModel.gameObject.transform.localScale = Vector3.one;
                        _currentHelmetModel.gameObject.transform.localPosition = Vector3.zero;
                        _currentHelmetModel.gameObject.transform.localRotation = Quaternion.identity;

                        break;
                    case ArmorType.Body:
                        if (_currentBodyModel != null)
                            Destroy(_currentBodyModel); // 기존 모델 제거
                        _currentBodyModel = Instantiate(armorSO.ArmorModel, BodyModelParent);
                        _currentBodyModel.gameObject.transform.localScale = Vector3.one;
                        _currentBodyModel.gameObject.transform.localPosition = Vector3.zero;
                        _currentBodyModel.gameObject.transform.localRotation = Quaternion.identity;

                        break;
                }
            }
            else
            {
                armors.Remove(armorType);
                switch (armorType)
                {
                    case ArmorType.Helmet:
                        if (_currentHelmetModel != null)
                            Destroy(_currentHelmetModel);
                        break;
                    case ArmorType.Body:
                        if (_currentBodyModel != null)
                            Destroy(_currentBodyModel);
                        break;
                }
            }

            _agentEventChannel.RaiseEvent(AgentEvents.AgentArmorEquip.Init(_owner, armors.Values.ToArray()));
        }
    }
}
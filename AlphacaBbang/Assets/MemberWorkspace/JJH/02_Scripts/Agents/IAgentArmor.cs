using JJH._02_Scripts.Weapons;
using System.Collections.Generic;

namespace JJH._02_Scripts.Agents
{
    public interface IAgentArmor
    {
        Dictionary<ArmorType, ArmorSO> armors { get; }
        bool BodyArmorEquiped { get; }
        bool HeadArmorEquiped { get; }

        void ArmorEquip(bool value, ArmorType armorType, ArmorSO armorSO);
        void Initialize(ModuleOwner owner);
    }
}
using System;
using Unity.Behavior;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.BT.Conditions
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "CheckNull", story: "[Object] is null", category: "Conditions", id: "11f29ab574c5e2df70aabc1002ea25d4")]
    public partial class CheckNullCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Object;

        public override bool IsTrue()
        {
            if (Object == null)
                return false;

            return Object.Value == null;
        }
    }
}


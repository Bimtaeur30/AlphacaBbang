using UnityEngine;

namespace JJH._02_Scripts.Weapons
{
    public class Armor : MonoBehaviour
    {
        [field: SerializeField] public ArmorSO ArmorSO { get; private set; }
        [SerializeField] private Transform armorModelTrans;

        private void Awake()
        {
            Instantiate(ArmorSO.ArmorModel, armorModelTrans);
        }
    }
}
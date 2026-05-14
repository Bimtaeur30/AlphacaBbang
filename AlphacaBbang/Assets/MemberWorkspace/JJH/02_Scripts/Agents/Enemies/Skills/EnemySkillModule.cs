using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.Skills
{
    public class EnemySkillModule : MonoBehaviour, IModule, IEnemySkillModule, IAfterInitModule
    {
        protected Dictionary<Type, IEnemySkill> _skillDict = new Dictionary<Type, IEnemySkill>();

        private AbstractEnemy _owner;

        public void Initialize(ModuleOwner owner)
        {
            _owner = (AbstractEnemy)owner;

            _skillDict = GetComponentsInChildren<IEnemySkill>().ToDictionary(module => module.GetType());
        }

        public void AfterInitalize()
        {
            InitializeComponents();
            AfterInitializeComponents();
        }

        protected virtual void InitializeComponents()
        {
            foreach (IEnemySkill skill in _skillDict.Values)
            {
                skill.Initialize(_owner);
            }
        }

        protected virtual void AfterInitializeComponents()
        {
            foreach (IAfterInitModule module in _skillDict.Values.OfType<IAfterInitModule>())
            {
                module.AfterInitalize();
            }
        }

        public IEnemySkill GetSkill<T>() where T : IEnemySkill
        {
            _skillDict.TryGetValue(typeof(T), out IEnemySkill skill);
            return skill;
        }

        public void UseSkill<T>() where T : IEnemySkill
        {
            if (_skillDict.TryGetValue(typeof(T), out IEnemySkill skill))
            {
                skill.UseSkill();
                return;
            }

            IEnemySkill findSkill = _skillDict.Values.FirstOrDefault(skillType => skillType is T);
            if (findSkill != null)
                findSkill.UseSkill();
        }
    }
}
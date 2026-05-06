using UnityEngine;

namespace JJH._02_Scripts.Agents.Enemies.NavMeshs
{
    public interface INavMeshAgent
    {
        public void MoveTo(Vector3 targetPosition);
        public void KeepChase(bool value);
    }
}
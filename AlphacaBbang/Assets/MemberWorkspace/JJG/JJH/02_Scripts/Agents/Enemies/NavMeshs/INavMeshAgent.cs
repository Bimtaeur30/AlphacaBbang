using UnityEngine;
using UnityEngine.AI;

namespace JJH._02_Scripts.Agents.Enemies.NavMeshs
{
    public interface INavMeshAgent
    {
        public NavMeshAgent NavMeshAgent { get; }
        public void MoveTo(Vector3 targetPosition);
        public void KeepChase(bool value);
        public void StopImmediately();
    }
}
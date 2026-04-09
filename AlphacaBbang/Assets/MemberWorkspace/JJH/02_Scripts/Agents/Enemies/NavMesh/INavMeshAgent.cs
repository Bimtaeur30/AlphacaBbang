using UnityEngine;

namespace MemberWorkspace.JJH._02_Scripts.Agents.Enemies.NavMesh
{
    public interface INavMeshAgent
    {
        public void MoveTo(Vector3 targetPosition);
        public void KeepChase(bool value);
    }
}
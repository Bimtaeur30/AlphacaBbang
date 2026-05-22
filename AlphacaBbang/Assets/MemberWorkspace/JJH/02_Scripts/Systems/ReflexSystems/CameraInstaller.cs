using Reflex.Core;
using UnityEngine;

namespace JJH._02_Scripts.Systems.ReflexSystems
{
    public class CameraInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Camera _mainCamera;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(_mainCamera);
        }
    }
}
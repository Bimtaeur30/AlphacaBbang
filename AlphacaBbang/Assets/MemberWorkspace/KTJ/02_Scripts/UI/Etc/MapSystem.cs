using UnityEngine;
using UnityEngine.InputSystem;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private GameObject firstObject;
    [SerializeField] private GameObject secondObject;

    private bool isFirstActive = true;

    private void Start()
    {
        UpdateObjects();
    }

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            isFirstActive = !isFirstActive;
            UpdateObjects();
        }
    }

    private void UpdateObjects()
    {
        firstObject.SetActive(isFirstActive);
        secondObject.SetActive(!isFirstActive);
    }
}

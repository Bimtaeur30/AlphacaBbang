using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    void Update()
    {
        DetectInteractable();
    }

    void DetectInteractable()
    {
        foreach (var mono in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mono is IInteractable target)
            {
                float dist = Vector3.Distance(transform.position, mono.transform.position);
                bool inRange = dist <= target.InteractRange;

                var ui = mono.GetComponentInChildren<InteractKeyUI>();

                if (ui == null) continue;

                if (inRange)
                    ui.Show(target);
                else
                    ui.Hide();
            }
        }
    }
}
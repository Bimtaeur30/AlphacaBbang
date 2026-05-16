using UnityEngine;
using System.Collections.Generic;

public class InteractDetector : MonoBehaviour
{
    private static readonly List<InteractKeyUI> _registered = new();

    public static void Register(InteractKeyUI ui) => _registered.Add(ui);
    public static void Unregister(InteractKeyUI ui) => _registered.Remove(ui);

    void Update()
    {
        DetectInteractable();
    }

    void DetectInteractable()
    {
        InteractKeyUI closest = null;
        float closestDist = float.MaxValue;

        foreach (var target in _registered)
        {
            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= target.InteractRange && dist < closestDist)
            {
                closestDist = dist;
                closest = target;
            }
        }

        foreach (var target in _registered)
        {
            if (target == null) continue;
            if (target == closest)
                target.Show();
            else
                target.Hide();
        }
    }
}
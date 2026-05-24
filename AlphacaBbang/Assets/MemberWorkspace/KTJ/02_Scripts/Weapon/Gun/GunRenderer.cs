using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GunRenderer : MonoBehaviour
{
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0, float speed = 1)
    {
        //Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        //Animator.SetFloat("SPEED", speed);
    }
}

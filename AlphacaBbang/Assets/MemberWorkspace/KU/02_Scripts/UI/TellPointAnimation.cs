using UnityEngine;

public class TellPointAnimation : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void PlayerOpenAnimation()
    {
        _animator.SetTrigger("Play");
        Debug.Log("PlayClip");
    }
}

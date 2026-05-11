using System.Collections;
using UnityEngine;

public class TeleportController : MonoSingleton<TeleportController>
{
    public enum TeleportState { Idle, FadingOut, Teleporting, FadingIn }

    [SerializeField] Transform[] teleportPositions;
    [SerializeField] Transform player;
    [SerializeField] float fadeDuration = 0.5f;

    private IFadeConversion _fadeStrategy;
    private TeleportState _state = TeleportState.Idle;

    protected override void Awake()
    {
        base.Awake();
        _fadeStrategy = GetComponent<IFadeConversion>();

        if (_fadeStrategy == null)
            Debug.LogError("IFadeConversion 구현체가 이 GameObject에 없습니다!");
    }

    public void TeleportTo(int index)
    {
        if (_state != TeleportState.Idle) return;
        if (index < 0 || index >= teleportPositions.Length) return;

        StartCoroutine(TeleportSequence(index));
    }

    private IEnumerator TeleportSequence(int index)
    {
        _state = TeleportState.FadingOut;
        yield return StartCoroutine(_fadeStrategy.FadeOut(fadeDuration));

        _state = TeleportState.Teleporting;
        MovePlayer(teleportPositions[index]);

        _state = TeleportState.FadingIn;
        yield return StartCoroutine(_fadeStrategy.FadeIn(fadeDuration));

        _state = TeleportState.Idle;
    }

    private void MovePlayer(Transform target)
    {
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.SetPositionAndRotation(target.position, target.rotation);

        if (cc != null) cc.enabled = true;
    }
}
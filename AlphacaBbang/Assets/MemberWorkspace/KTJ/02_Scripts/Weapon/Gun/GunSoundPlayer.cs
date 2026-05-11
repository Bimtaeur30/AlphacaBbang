using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts.Systems.SoundSystems;
using UnityEngine;

public class GunSoundPlayer : MonoBehaviour
{
    [SerializeField] private PoolManagerSO poolManagerSO;
    [SerializeField] private PoolItemSO soundPoolSO;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundClipSO audio)
    {
        if (audio == null) return;
        //Debug.Assert(audio != null, "오디오가 널입니다.");
        SoundPlayer sp = poolManagerSO.Pop<SoundPlayer>(soundPoolSO);
        sp.PlaySound(audio);
    }
}

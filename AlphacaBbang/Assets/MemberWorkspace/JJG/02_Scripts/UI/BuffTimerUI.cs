using UnityEngine;

public class BuffTimerUI : MonoSingleton<BuffTimerUI>
{
    [SerializeField] private BuffTimerEntryUI _entryPrefab;
    [SerializeField] private Transform _entryContainer;

    public void AddBuff(Sprite icon, string buffName, float duration)
    {
        if (_entryPrefab == null || _entryContainer == null) return;

        BuffTimerEntryUI entry = Instantiate(_entryPrefab, _entryContainer);
        entry.Init(icon, buffName, duration);
    }
}

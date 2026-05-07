public interface ISaveable
{
    SaveIdData SaveId { get; }
    string GetSaveData();
    void RestoreData(string data);
}
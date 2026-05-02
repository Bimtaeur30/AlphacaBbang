using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class TestLogFilePath : MonoBehaviour
    {
        public string dataPath =>  Application.dataPath;
        public string streamingAssetsPath  =>  Application.streamingAssetsPath;
        public string persistentDataPath =>  Application.persistentDataPath;
        
        [ContextMenu("Log")]
        public void Log()
        {
            Debug.Log(dataPath);
            Debug.Log(streamingAssetsPath);
            Debug.Log(persistentDataPath);
        }
        
    }
}
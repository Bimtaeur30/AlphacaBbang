using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class TestLog : MonoBehaviour
    {
        public string dataPath =>  Application.dataPath;
        public string streamingAssetsPath  =>  Application.streamingAssetsPath;
        public string persistentDataPath =>  Application.persistentDataPath;
        
        [ContextMenu("DataPath")]
        public void DataPath()
        {
            Debug.Log(dataPath);
            Debug.Log(streamingAssetsPath);
            Debug.Log(persistentDataPath);
        }

    }
}
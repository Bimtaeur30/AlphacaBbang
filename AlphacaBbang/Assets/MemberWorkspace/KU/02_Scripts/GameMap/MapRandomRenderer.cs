using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MapRandomRenderer : MonoBehaviour
{
    [Header("Size Random")]
    [SerializeField] private float _minSize = 250f;
    [SerializeField] private float _maxSize = 350f;

    [Header("Height Random")]
    [SerializeField] private float _minHeight = 250f;
    [SerializeField] private float _maxHeight = 300f;

    [Header("Move Position")]
    [SerializeField] private bool MoveTransform = false;

    [SerializeField] private float _moveRangeX = 1f;
    [SerializeField] private float _moveRangeZ = 1f;

    private void Start()
    {
        RandomizeTree();

        //enabled = false;
    }
        
    private void RandomizeTree()
    {
        Vector3 scale = transform.localScale;
        Vector3 pos = transform.position;

        float randomSize = Random.Range(_minSize, _maxSize);

        scale.x = randomSize;
        scale.y = randomSize;

        float randomHeight = Random.Range(_minHeight, _maxHeight);
        scale.z = randomHeight;

        transform.localScale = scale;

        pos.z = CalculateHeight(randomHeight);
        Debug.LogError($"헤이헤이"+ CalculateHeight(randomHeight));
        transform.position = pos;


        if (MoveTransform)
        {

            pos.x += Random.Range(-_moveRangeX, _moveRangeX);
            pos.z += Random.Range(-_moveRangeZ, _moveRangeZ);

            transform.position = pos;
        }
    }

    private float CalculateHeight(float value)
    {
        float baseValue = 300f;
        float baseHeight = 1.5f;

        float diff = baseValue - value;

        // 50 줄어들 때마다 0.85 감소
        float decrease = (diff / 50f) * 0.85f;

        return baseHeight - decrease;
    }
}

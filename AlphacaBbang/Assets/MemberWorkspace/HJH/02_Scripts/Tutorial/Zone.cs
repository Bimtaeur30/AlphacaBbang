using UnityEngine;

public class Zone : MonoBehaviour
{
    public Color borderColor = Color.cyan;
    public Vector2 size = new Vector2(3f, 3f);

    void Start()
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = 4;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = borderColor;
        lr.endColor = borderColor;

        float hw = size.x * 0.5f;
        float hd = size.y * 0.5f;

        lr.SetPositions(new Vector3[]
        {
            new Vector3(-hw, 0,  hd),
            new Vector3( hw, 0,  hd),
            new Vector3( hw, 0, -hd),
            new Vector3(-hw, 0, -hd),
        });
    }
}

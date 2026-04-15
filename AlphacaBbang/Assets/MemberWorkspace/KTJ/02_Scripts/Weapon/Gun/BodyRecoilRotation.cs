using UnityEngine;

public class BodyRecoilRotation : MonoBehaviour
{
    [SerializeField] private Transform bodyRoot;
    [SerializeField] private float recoilRecoverSpeed = 10f;

    private Quaternion _originRot;
    private Quaternion _recoilRot;

    private void Awake()
    {
        if (bodyRoot == null)
            bodyRoot = transform;

        _originRot = bodyRoot.localRotation;
        _recoilRot = Quaternion.identity;
    }

    private void Update()
    {
        Quaternion targetRot = _originRot * _recoilRot;
        bodyRoot.localRotation = Quaternion.Slerp(bodyRoot.localRotation, targetRot, Time.deltaTime * 20f);

        _recoilRot = Quaternion.Slerp(_recoilRot, Quaternion.identity, Time.deltaTime * recoilRecoverSpeed);
    }

    public void ApplyRecoil(float spreadAngle)
    {
        float randomAngle = Random.Range(-spreadAngle, spreadAngle);
        Quaternion recoil = Quaternion.Euler(0, randomAngle, 0f);
        _recoilRot *= recoil;
    }
}

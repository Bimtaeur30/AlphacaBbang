using UnityEngine;

public class ExGun : Gun
{
    [SerializeField] private Transform firePos;
    [SerializeField] private float rayDistance = 10f;
    private RecoilController _recoilController;

    private void Awake()
    {
        _recoilController = GetComponentInChildren<RecoilController>();
        _recoilController.Init(GunDataSO);
        Debug.Assert( _recoilController != null , "리코일 컨트롤러가 자식으로 붙어있지 않습니다.");
    }
    public override void Fire() // y축 반동 아직 안됨.
    {
        _recoilController.AddRecoil();

        Vector3 finalDirection = Quaternion.Euler(0, 90, 0) * _recoilController.ApplyRecoilToDirection(firePos.forward);
        Vector3 origin = firePos.position;
        //Vector3 direction = transform.right;

        Debug.DrawRay(origin, finalDirection * rayDistance, Color.red);

        if (Physics.Raycast(origin, finalDirection, out RaycastHit hit, rayDistance))
        {
            Debug.Log("맞은 오브젝트 : " + hit.collider.name);
        }

    }
}
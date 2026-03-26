using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_TJ : ModuleOwner
{
    #region 직렬화
    [SerializeField] private PlayerInputSO_KTJ playerInputSO;
    #endregion

    #region 모듈
    private GunHandleModule gunHandleModule;
    #endregion

    #region 퍼블릭변수
    public Camera MainCam { get; private set; }
    public Vector2 MousePos { get; private set; }
    #endregion


    protected override void InitializeComponents()
    {
        gunHandleModule = GetModule<GunHandleModule>();
        Debug.Assert(gunHandleModule != null, "gunHandleModule is null");
        MainCam = Camera.main;
    }

    private void OnEnable()
    {
        playerInputSO.OnAimEvent += HandleAimKey;
        playerInputSO.OnFireEvent += HandleFireKey;
    }
    private void OnDisable()
    {
        playerInputSO.OnAimEvent -= HandleAimKey;
        playerInputSO.OnFireEvent -= HandleFireKey;
    }

    private void HandleAimKey(bool v)
    {
        gunHandleModule.Aim(v);
        if (v == true) // 조준시작
        {
        }
        else // 조준해제
        {
        }
    }

    private void HandleFireKey(bool v)
    {
        gunHandleModule.Fire(v);
        if (v == true) // 조준시작
        {

        }
        else // 조준해제
        {

        }
    }

    private void Update()
    {
        if (gunHandleModule.onAim)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Ray ray = MainCam.ScreenPointToRay(mousePos);

            Plane plane = new Plane(Vector3.up, transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);

                Vector3 direction = point - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
                }
            }
        }
    }

}

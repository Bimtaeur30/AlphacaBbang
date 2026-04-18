using UnityEngine;

public class Player_TJ : ModuleOwner
{
    #region 직렬화
    [SerializeField] private PlayerInputSO_KTJ playerInputSO;
    #endregion

    #region 모듈
    public GunHandleModule GunHandleModule { get; private set; }
    public CrossHairModule CrossHairModule { get; private set; }
    #endregion

    #region 퍼블릭 변수
    public Camera MainCam { get; private set; }
    public Vector2 Forward { get; private set; }
    #endregion

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        GunHandleModule = GetModule<GunHandleModule>();
        Debug.Assert(GunHandleModule != null, "GunHandleModule is null");

        CrossHairModule = GetModule<CrossHairModule>();
        Debug.Assert(CrossHairModule != null, "CrossHairModule is null");

        MainCam = Camera.main;
        Debug.Assert(MainCam != null, "MainCam is null");
    }

    private void OnEnable()
    {
        if (playerInputSO == null)
            return;

        playerInputSO.OnAimEvent += HandleAimKey;
        playerInputSO.OnFireEvent += HandleFireKey;
    }

    private void OnDisable()
    {
        if (playerInputSO == null)
            return;

        playerInputSO.OnAimEvent -= HandleAimKey;
        playerInputSO.OnFireEvent -= HandleFireKey;
    }

    private void HandleAimKey(bool isPressed)
    {
        if (GunHandleModule == null)
            return;

        GunHandleModule.Aim(isPressed);
    }

    private void HandleFireKey(bool isPressed)
    {
        if (GunHandleModule == null)
            return;

        GunHandleModule.Fire(isPressed);
    }

    private void Update()
    {
        RotateToCrosshair();
    }

    private void RotateToCrosshair()
    {
        if (GunHandleModule == null || !GunHandleModule.IsInputAim)
            return;

        if (MainCam == null || CrossHairModule == null)
            return;

        Ray ray = MainCam.ScreenPointToRay(CrossHairModule.CHMousePos);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (!plane.Raycast(ray, out float distance))
            return;

        Vector3 point = ray.GetPoint(distance);
        Vector3 direction = point - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Forward = new Vector2(direction.x, direction.z).normalized;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, -90f, 0f);
    }
}
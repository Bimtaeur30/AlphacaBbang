using UnityEngine;

public class RecoilController : MonoBehaviour
{
    private Vector2 _currentRecoil;
    private Vector2 _targetRecoil;
    private GunDataSO _gunDataSO;
    private float _snappiness = 14f;
    private float _returnSpeed = 10f;

    public void Init(GunDataSO gunDataSO)
    {
        this._gunDataSO = gunDataSO;
    }

    private void Update()
    {
        _currentRecoil = Vector2.Lerp(_currentRecoil, _targetRecoil, _snappiness * Time.deltaTime);
        _targetRecoil = Vector2.Lerp(_targetRecoil, Vector2.zero, _returnSpeed * Time.deltaTime);
    }

    public void AddRecoil()
    {
        //float x = Random.Range(_gunDataSO.RecoilForceX * -1, _gunDataSO.RecoilForceX);
        //float y = Random.Range(_gunDataSO.RecoilForceY * -1, _gunDataSO.RecoilForceY);

        //_targetRecoil += new Vector2(x, y);
    }

    public Vector3 ApplyRecoilToDirection(Vector3 baseDirection)
    {

        Quaternion recoilRotation = Quaternion.Euler(-_currentRecoil.y, _currentRecoil.x, 0f);
        return recoilRotation * baseDirection;
    }

    public Vector2 GetCurrentRecoil()
    {
        return _currentRecoil;
    }
}
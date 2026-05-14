// =============================================================
// StageMapEventTest.cs
// 위치: Assets/Scripts/UI/StageMapEventTest.cs
//
// 사용법:
//   1. 씬의 아무 오브젝트에 이 스크립트를 붙인다.
//   2. StageMapEditor의 "이벤트 수신 오브젝트" 슬롯에 해당 오브젝트를 드래그한다.
//   3. 에디터에서 각 노드의 "클릭 메소드" 를 OnStageClick 으로 설정한다.
//   4. 씬에 생성 → 버튼 클릭 시 OnStageClick(stageId) 가 호출된다.
// =============================================================
using UnityEngine;

public class StageMapEventTest : MonoBehaviour
{
    // ── StageMapButton이 SendMessage로 호출하는 메소드
    //    인자: 클릭된 스테이지의 id (int)
    public void OnStageClick(int stageId)
    {
        Debug.Log($"[StageMapEventTest] 스테이지 {stageId} 클릭됨!");

        // 여기에 실제 로직 작성
        // 예) 씬 전환, UI 팝업 열기, 스테이지 데이터 로드 등
        HandleStage(stageId);
    }

    private void HandleStage(int stageId)
    {
        // 스테이지별 분기 예시
        switch (stageId)
        {
            case 0:
                Debug.Log("스테이지 0 처리 → 튜토리얼");
                break;
            case 1:
                Debug.Log("스테이지 1 처리 → 1-1 스테이지 로드");
                break;
            default:
                Debug.Log($"스테이지 {stageId} 처리 → 공통 로직");
                break;
        }

        // 실제 씬 전환 예시 (주석 해제해서 사용)
        // UnityEngine.SceneManagement.SceneManager.LoadScene($"Stage_{stageId}");
    }
}

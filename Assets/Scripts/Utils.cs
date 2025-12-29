using UnityEngine;
using UnityEngine.AI;

public static class Utils
{
    /// <summary>
    /// 대상 위치에서 가장 가까운 NavMesh 좌표를 반환합니다.
    /// </summary>
    /// <param name="targetPos">대상(타겟)의 현재 위치</param>
    /// <param name="range">검색할 반경 (보통 1.0f ~ 2.0f)</param>
    /// <returns>투사된 좌표 (찾지 못하면 원래 좌표 반환)</returns>
    public static Vector3 SampleNavMesh(Vector3 targetPos, float range)
    {
        NavMeshHit hit;
        
        // NavMesh.SamplePosition(검색 시작점, 결과 저장 변수, 검색 반경, 레이어 마스크)
        if (NavMesh.SamplePosition(targetPos, out hit, range, NavMesh.AllAreas))
        {
            return hit.position; // NavMesh 위로 투사된 좌표
        }

        return targetPos; // 근처에 NavMesh가 없으면 원래 위치 반환
    }
}

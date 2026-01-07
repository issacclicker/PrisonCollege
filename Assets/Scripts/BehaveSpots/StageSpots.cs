using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageSpots : MonoBehaviour
{
    [SerializeField] private Transform _spotParent;
    private List<BehaveSpot> _allBehaveSpots;
    private Dictionary<BehaviorType, List<BehaveSpot>> _behaveSpotsMap = new();



    private void Awake()
    {
        _allBehaveSpots = _spotParent.GetComponentsInChildren<BehaveSpot>().ToList();
        InitializeBehaveSpotsMap();
    }



    private void InitializeBehaveSpotsMap()
    {
        _behaveSpotsMap.Clear();

        // Enum에 정의된 모든 값을 순회
        var allTypes = System.Enum.GetValues(typeof(BehaviorType));

        foreach (BehaviorType type in allTypes)
        {
            if (type == BehaviorType.None) continue;

            foreach (var spot in _allBehaveSpots)
            {
                if (spot.HasBehavior(type))
                {
                    if (!_behaveSpotsMap.ContainsKey(type))
                    {
                        //Debug.Log($"new type created : {type}");
                        _behaveSpotsMap[type] = new List<BehaveSpot>();
                    }

                    _behaveSpotsMap[type].Add(spot);
                }
            }
        }
    }



    private List<BehaveSpot> GetSpotsByType(BehaviorType type)
    {
        if (_behaveSpotsMap.TryGetValue(type, out List<BehaveSpot> spots))
        {
            return spots;
        }
        return new List<BehaveSpot>(); // 없으면 빈 리스트 반환
    }



    public BehaveSpot GetRandomSpotByType(BehaviorType type)
    {
        List<BehaveSpot> spots = GetSpotsByType(type);
        List<BehaveSpot> availableSpots = spots.FindAll(s => s.IsUsable);

        if (availableSpots.Count == 0)
        {
            Debug.LogWarning($"{type} 타입의 사용 가능한 스팟이 없습니다.");
            return null;
        }
        int randomIndex = Random.Range(0, availableSpots.Count);
        return availableSpots[randomIndex];
    }
}

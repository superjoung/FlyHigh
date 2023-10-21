using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<UnitInfo> allUnits = new List<UnitInfo>();   // 게임 내 모든 유닛
    public List<UnitInfo> acquiredUnits = new List<UnitInfo>(); // 획득한 유닛

    public void AcquireUnit(UnitInfo unit)
    {
        if (!acquiredUnits.Contains(unit))
        {
            acquiredUnits.Add(unit);
        }
    }
}


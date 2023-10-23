using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightFriend : MonoBehaviour
{
    public List<GameObject> playerUnit = new List<GameObject>(); // 획득한 유닛

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AcquireUnit(GameObject unit)
    {
        if (!playerUnit.Contains(unit))
        {
            playerUnit.Add(unit);
        }
    }
}

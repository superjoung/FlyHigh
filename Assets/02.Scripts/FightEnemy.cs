using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEnemy : MonoBehaviour
{
    public int idx;
    public GameObject[] enemyUnit = new GameObject[5];

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void MakeDontDestroy()
    {
        if(Eagle.nowe != null)
        {
            DialogueInfo di1 = Eagle.nowe.GetComponent<DialogueInfo>();
        }
            
    }
     
}

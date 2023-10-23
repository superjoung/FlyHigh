using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEnemy : MonoBehaviour
{
    public int idx;
    public GameObject[] enemyUnit = new GameObject[5];

    public void MakeDontDestroy()
    {
        if(Eagle.nowe != null)
        {
            DialogueInfo di1 = Eagle.nowe.GetComponent<DialogueInfo>();

            //if (di1.DialogueId == idx)
                //DontDestroyOnLoad(gameObject);
        }
            
    }
     
}

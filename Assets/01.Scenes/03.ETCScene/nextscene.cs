using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextscene : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void next()
    {
        SceneManager.LoadScene("pau_duck_1"); // 다음 씬으로 전환
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneChanger : MonoBehaviour
{
    public void MainChange()
    {
        SceneManager.LoadScene("Main"); // 여기 씬 이름 입력
    }
}

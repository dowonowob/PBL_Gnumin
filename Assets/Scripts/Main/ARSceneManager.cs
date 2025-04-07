using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManaver : MonoBehaviour
{
    // Start is called before the first frame update
    public void GotoMain()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void GotoScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}

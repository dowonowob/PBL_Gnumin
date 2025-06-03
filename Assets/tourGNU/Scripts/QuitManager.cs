using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitManager : MonoBehaviour
{   
    public GameObject quitPanel;
    public Button confirmBtn;
    public Button cancelBtn;

    void Confirm() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            UnityEngine.Application.Quit();
        #endif
    }
    void Cancel() {
        quitPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {   
        quitPanel.SetActive(false);
        confirmBtn.onClick.AddListener(Confirm);
        cancelBtn.onClick.AddListener(Cancel);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {   
            quitPanel.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainmenuManager : MonoBehaviour
{

    public Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() => SceneManager.LoadScene("tourGNU"));    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

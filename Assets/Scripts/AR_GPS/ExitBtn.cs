using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBtn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {    //안드로이드일때
            if (Input.GetKey(KeyCode.Escape))
            {      // 뒤로가기키를 누르면
                Application.Quit();  //앱 종료
            }

        }
    }
}

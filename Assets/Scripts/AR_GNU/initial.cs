using UnityEngine;
using UnityEngine.SceneManagement;

public class initial : MonoBehaviour
{
    void Start()
    {
        // Android 플랫폼에서 실행 중이며 Unity 에디터가 아닐 경우에만 실행
#if UNITY_ANDROID && !UNITY_EDITOR
        // UnityPlayer 클래스 접근
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // 현재 Activity 객체 가져오기
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Java의 getSceneName 메서드를 호출하여 씬 이름을 가져옴
            string sceneName = activity.Call<string>("getSceneName");

            // 씬 이름이 비어있지 않으면 해당 씬을 로드
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
#endif
    }
}

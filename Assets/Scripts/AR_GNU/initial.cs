using UnityEngine;
using UnityEngine.SceneManagement;

public class initial : MonoBehaviour
{
    void Start()
    {
        // Android �÷������� ���� ���̸� Unity �����Ͱ� �ƴ� ��쿡�� ����
#if UNITY_ANDROID && !UNITY_EDITOR
        // UnityPlayer Ŭ���� ����
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // ���� Activity ��ü ��������
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Java�� getSceneName �޼��带 ȣ���Ͽ� �� �̸��� ������
            string sceneName = activity.Call<string>("getSceneName");

            // �� �̸��� ������� ������ �ش� ���� �ε�
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
#endif
    }
}

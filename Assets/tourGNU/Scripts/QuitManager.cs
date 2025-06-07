using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuitManager : MonoBehaviour, BackInputAction.INavigationActions
{
    [Header("UI Components")]
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private BackInputAction inputActions;

    void Awake()
    {
        inputActions = new BackInputAction();
        inputActions.Navigation.SetCallbacks(this);
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Start()
    {
        // ���� â�� ó���� ��Ȱ��ȭ
        quitPanel.SetActive(false);

        // ��ư ����
        confirmButton.onClick.AddListener(QuitApp);
        cancelButton.onClick.AddListener(() => quitPanel.SetActive(false));
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("�ڷΰ��� ��ư ���� (InputSystem)");
            quitPanel.SetActive(true);
        }
    }

    private void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

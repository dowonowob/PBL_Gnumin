using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // New Input System namespace (참고용)
using System;

public class ButtonManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private TourManager tourManager;
    [SerializeField] private Button btn;

    private void Start()
    {
        if (tourManager == null)
        {
            tourManager = FindObjectOfType<TourManager>();
            if (tourManager == null)
                Debug.LogError("[ButtonManager] TourManager가 연결되지 않았습니다.");
        }

        if (btn == null)
        {
            btn = GetComponent<Button>();
            if (btn == null)
                Debug.LogError("[ButtonManager] Button 컴포넌트를 찾을 수 없습니다.");
        }

        btn.onClick.AddListener(OnButtonPressed);
    }

    private void Update()
    {
        if (tourManager == null) return;

        if (tourManager.min == -1)
        {
            btn.gameObject.SetActive(false);
            return;
        }

        btn.gameObject.SetActive(tourManager.min <= 50f);
    }

    private void OnButtonPressed()
    {
        if (tourManager.currentPlaceObject!= null && tourManager.currentPlaceObject.activeSelf)
        {
            tourManager.positionAdjustment();
        }
    }
}

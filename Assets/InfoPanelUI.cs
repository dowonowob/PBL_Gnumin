using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    public static InfoPanelUI Instance { get; private set; }

    public GameObject panel;
    public Text titleText;
    public Text descriptionText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowInfo(string title, string description, Vector3 worldPos)
    {
        titleText.text = title;
        descriptionText.text = description;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up * 0.2f);
        panel.transform.position = screenPos;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}


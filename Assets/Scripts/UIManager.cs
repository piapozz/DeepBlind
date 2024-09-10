using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject intractUI;
    [SerializeField] GameObject miniMapUI;
    [SerializeField] GameObject compassUI;

    TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = intractUI.GetComponentInChildren<TextMeshProUGUI>();
        DisableIntractUI();
    }

    public void DisplayIntractUI(string text)
    {
        intractUI.SetActive(true);
        textMesh.SetText(text);
    }

    public void DisableIntractUI()
    {
        textMesh.SetText("");
        intractUI.SetActive(false);
    }

    public void DisplayMap()
    {
        miniMapUI.SetActive(true);
    }

    public void DisplayCompass()
    {
        compassUI.SetActive(true);
    }
}

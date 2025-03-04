using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject intractUI;
    [SerializeField] GameObject miniMapUI;
    [SerializeField] GameObject compassUI;

    static public UIManager instance = null;

    TextMeshProUGUI textMesh;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        textMesh = intractUI.GetComponentInChildren<TextMeshProUGUI>();
        DisableIntractUI();
    }

    private void Update()
    {
        
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

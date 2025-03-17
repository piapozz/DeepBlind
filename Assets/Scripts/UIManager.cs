using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SystemObject
{
    static public UIManager instance = null;

    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject interactUIPrefab;
    [SerializeField] private GameObject mainSlotUIPrefab;
    private GameObject interactUI = null;
    public GameObject canvas = null;
    public Image mainSlotUI = null;

    private TextMeshProUGUI textMesh;

    public override void Initialize()
    {
        instance = this;

        canvas = Instantiate(canvasPrefab);
        interactUI = Instantiate(interactUIPrefab, canvas.transform);
        GameObject mainSlotBasePrefab = Instantiate(mainSlotUIPrefab, canvas.transform);
        GameObject mainSlotChild = mainSlotBasePrefab.transform.GetChild(0).gameObject;
        mainSlotUI = mainSlotChild.GetComponent<Image>();
        textMesh = interactUI.GetComponentInChildren<TextMeshProUGUI>();

        DisableIntractUI();
    }

    public void DisplayIntractUI(string text)
    {
        interactUI.SetActive(true);
        textMesh.SetText(text);
    }

    public void DisableIntractUI()
    {
        textMesh.SetText("");
        interactUI.SetActive(false);
    }
}

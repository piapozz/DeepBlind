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
    [SerializeField] public Sprite noneItemIcon;
    private GameObject interactUI = null;
    public GameObject canvas = null;
    private Image mainSlotUI = null;
    private TextMeshProUGUI mainTextUI = null;

    private TextMeshProUGUI textMesh;

    public override void Initialize()
    {
        instance = this;

        canvas = Instantiate(canvasPrefab);
        interactUI = Instantiate(interactUIPrefab, canvas.transform);
        GameObject mainSlotBasePrefab = Instantiate(mainSlotUIPrefab, canvas.transform);
        GameObject mainSlotImage = mainSlotBasePrefab.transform.GetChild(0).gameObject;
        GameObject mainSlotCount = mainSlotBasePrefab.transform.GetChild(1).gameObject;
        mainSlotUI = mainSlotImage.GetComponent<Image>();
        mainTextUI = mainSlotCount.GetComponent<TextMeshProUGUI>();
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

    public void ChangeSlotImage(Sprite sprite) { mainSlotUI.sprite = sprite; }
    public void ViewItemSlot(Sprite sprite, int value)
    {
        mainSlotUI.sprite = sprite;
        if (value > 0) mainTextUI.text = value.ToString();
        else mainTextUI.text = ("").ToString();
    }
}

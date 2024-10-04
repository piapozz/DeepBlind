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
    [SerializeField] Image staminaBar;

    [SerializeField] Player player;

    TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = intractUI.GetComponentInChildren<TextMeshProUGUI>();
        DisableIntractUI();
    }

    private void Update()
    {
        // Playerクラスの GetStamina() で「現在のスタミナ / スタミナの最大値」を受け取って代入
        // それに応じてゲージを増減させる
        staminaBar.GetComponent<Image>().fillAmount = player.GetStamina();
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

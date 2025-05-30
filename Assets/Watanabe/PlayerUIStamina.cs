using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIStamina : MonoBehaviour
{
    [SerializeField] private Image staminaBar = null;
    private Player player = null;

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        // それに応じてゲージを増減させる
        staminaBar.fillAmount = player.GetStamina();
    }
}

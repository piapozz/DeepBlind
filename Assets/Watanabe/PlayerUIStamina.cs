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
        // ‚»‚ê‚É‰‚¶‚ÄƒQ[ƒW‚ğ‘Œ¸‚³‚¹‚é
        staminaBar.fillAmount = player.GetStamina();
    }
}

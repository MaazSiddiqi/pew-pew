using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundPlayerUI : PlayerUI
{
    [Header("Round UI")]
    [SerializeField]
    private TextMeshProUGUI roundText;
    [SerializeField]
    private TextMeshProUGUI enemyCountText;
    [SerializeField]
    private TextMeshProUGUI roundCountdownText;

    public void UpdateRoundText(int round)
    {
        if (roundText != null) roundText.text = "Round: " + round;
    }

    public void UpdateEnemyCountText(int count)
    {
        if (enemyCountText != null) enemyCountText.text = "Enemies: " + count;
    }

    public void UpdateRoundCountdown(float time)
    {
        if (roundCountdownText != null)
        {
            if (time > 0)
            {
                roundCountdownText.text = "Next Round: " + Mathf.CeilToInt(time);
                roundCountdownText.gameObject.SetActive(true);
            }
            else
            {
                roundCountdownText.gameObject.SetActive(false);
            }
        }
    }
}

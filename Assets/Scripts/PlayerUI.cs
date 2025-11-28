using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText;
    [SerializeField]
    private UnityEngine.UI.Image damageOverlay;

    // Start is called before the first frame update
    void Start()
    {
        if (damageOverlay != null)
        {
            damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0f);
        }
    }

    public void UpdateText(string promptMessage)
    {
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
        else
        {
            Debug.LogError("Prompt Text is NULL in PlayerUI!");
        }
    }

    public void ShowDamageOverlay()
    {
        if (damageOverlay != null)
        {
            StartCoroutine(FadeDamageOverlay());
        }
    }

    private IEnumerator FadeDamageOverlay()
    {
        // Set to fully visible (or desired max alpha)
        Color color = damageOverlay.color;
        color.a = 0.5f; 
        damageOverlay.color = color;

        float fadeSpeed = 2f;

        while (damageOverlay.color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            damageOverlay.color = color;
            yield return null;
        }
    }
}

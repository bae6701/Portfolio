using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image foregroundImage; 
    [SerializeField] private Image backgroundImage; 
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float lerpSpeed = 2f; 

    private Character targetCharacter;

    public void Setup(Character character)
    {
        targetCharacter = character;

        targetCharacter.OnHealthChanged += UpdateHealthBar;

        UpdateHealthBar(character.currentHealth, character.maxHealth);
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float fillAmount = currentHealth / maxHealth;
        foregroundImage.fillAmount = fillAmount;
        healthText.text = $"{Mathf.RoundToInt(currentHealth)}";
    }

    private void Update()
    {
        if (!Mathf.Approximately(backgroundImage.fillAmount, foregroundImage.fillAmount))
        {
            backgroundImage.fillAmount = Mathf.Lerp(backgroundImage.fillAmount, foregroundImage.fillAmount, Time.deltaTime * lerpSpeed);
        }
    }

    private void OnDestroy()
    {
        if (targetCharacter != null)
        {
            targetCharacter.OnHealthChanged -= UpdateHealthBar;
        }
    }
}

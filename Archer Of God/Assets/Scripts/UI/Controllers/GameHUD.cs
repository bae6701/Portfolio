using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButtonWin;
    [SerializeField] private Button restartButtonLose;

    [Header("Health Bar Setups")]
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private Character playerCharacter;
    [SerializeField] private HealthBar botHealthBar;
    [SerializeField] private Character botCharacter;

    void Start()
    {
        startButton?.onClick.AddListener(() => GameManager.instance.StartGame());
        restartButtonWin?.onClick.AddListener(() => GameManager.instance.RestartGame());
        restartButtonLose?.onClick.AddListener(() => GameManager.instance.RestartGame());

        if (playerHealthBar != null && playerCharacter != null)
        {
            playerHealthBar.Setup(playerCharacter);
        }
        if (botHealthBar != null && botCharacter != null)
        {
            botHealthBar.Setup(botCharacter);
        }
    }
}

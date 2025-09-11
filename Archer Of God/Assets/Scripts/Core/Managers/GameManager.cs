using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Ready, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentState;

    [Header("Character References")]
    [SerializeField] private Character playerCharacter;
    [SerializeField] private Character botCharacter;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 90f;
    [SerializeField] private float fastTimeScale = 1.5f; 
    private float currentTime = 0;
    private bool isSpedUp = false;

    [Header("UI Panels")]
    public GameObject gameStartPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    public event Action<float> OnTimeUpdated;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SetupCharacterReferences();
    }

    private void SetupCharacterReferences()
    {
        if (playerCharacter != null && botCharacter != null)
        {
            playerCharacter.SetTarget(botCharacter);
            botCharacter.SetTarget(playerCharacter);
        }
        else
        {
            Debug.LogError("GameManager에 플레이어 또는 봇 참조가 설정되지 않았습니다.");
        }
    }

    void Start()
    {
        currentState = GameState.Ready;
        Time.timeScale = 0; 
        gameStartPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        StartGame();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            currentTime -= Time.deltaTime;
            OnTimeUpdated?.Invoke(currentTime);

            if (currentTime <= 0 && !isSpedUp)
            {
                isSpedUp = true;
                Time.timeScale = fastTimeScale;
                Debug.Log("SUDDEN DEATH!");
            }
        }
    }

    public void StartGame()
    {
        currentTime = gameDuration;
        currentState = GameState.Playing;
        isSpedUp = false;
        Time.timeScale = 1;
        gameStartPanel.SetActive(false);
    }

    public void PlayerWin()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.GameOver;
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    public void PlayerLose()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.GameOver;
        Time.timeScale = 0;
        losePanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

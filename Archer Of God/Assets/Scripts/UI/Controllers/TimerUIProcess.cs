using TMPro;
using UnityEngine;

public class TimerUIProcess : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnTimeUpdated += UpdateTimer;
        }
    }

    private void UpdateTimer(float time)
    {
        if (time < 0) time = 0;
        timerText.text = Mathf.FloorToInt(time).ToString();
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnTimeUpdated -= UpdateTimer;
        }
    }
}

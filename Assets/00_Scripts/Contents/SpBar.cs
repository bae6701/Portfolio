using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    private Image m_Fill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Fill = Utils.FindChild<Image>(gameObject, "Fill_01", true);
    }

    private void Update()
    {
        
    }

    public void SetHpBar(float ratio)
    {        
        m_Fill.fillAmount = ratio;
    }
}

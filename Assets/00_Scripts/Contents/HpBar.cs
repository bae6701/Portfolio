using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private Image m_Fill, m_Fill_Deco;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_Fill = Utils.FindChild<Image>(gameObject, "Fill_01", true);
        m_Fill_Deco = Utils.FindChild<Image>(gameObject, "Fill_02",true);
    }

    private void Update()
    {
        m_Fill_Deco.fillAmount = Mathf.Lerp(m_Fill_Deco.fillAmount, m_Fill.fillAmount, Time.deltaTime * 2.0f);
    }

    public void SetHpBar(float ratio)
    {        
        m_Fill.fillAmount = ratio;
    }
}

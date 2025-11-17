using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// [2주차 v2.7] (지적 3 해결)
/// '상점 아이템 프리팹'에 부착되어,
/// StoreManager가 데이터를 주입할 때 UI를 갱신합니다.
/// </summary>
public class UI_StoreItem : MonoBehaviour
{
    // --- 인스펙터 연결 (프리팹 내부) ---
    [Header("UI Components")]
    public Image previewImage; // (목업의 'preview-window')
    public TMP_Text itemNameText; // (목업의 '기본 사각형')
    public Button buyButton; // (목업의 '구매 버튼')
    public TMP_Text buttonText; // (목업의 '100' 또는 'Equipped')
    public Image coinIcon; // (목업의 '코인 아이콘')
    
    private int _itemIndex;
    private int _categoryIndex; // 0=Block, 1=BadBlock, 2=Background

    /// <summary>
    /// StoreManager가 생성 시 호출
    /// </summary>
    public void Init(int category, int index)
    {
        _categoryIndex = category;
        _itemIndex = index;
        
        buyButton.onClick.AddListener(OnButtonPressed);
    }
    
    /// <summary>
    /// StoreManager가 호출: "Buy 100", "Equip" 등
    /// </summary>
    public void SetState(string text, bool interactable, bool showCoinIcon)
    {
        buttonText.text = text;
        buyButton.interactable = interactable;
        coinIcon.gameObject.SetActive(showCoinIcon);
    }
    
    /// <summary>
    /// StoreManager가 호출: 프리뷰 이미지/텍스트 설정
    /// </summary>
    public void SetData(Sprite preview, string name)
    {
        if (preview != null)
        {
            previewImage.sprite = preview;
            previewImage.color = Color.white; // (혹시 모르니)
        }
        else
        {
            previewImage.color = Color.clear; // (스프라이트가 없으면 투명하게)
        }
        
        itemNameText.text = name;
    }

    private void OnButtonPressed()
    {
        // 자신의 부모(UI_StorePopup)에 있는 함수를 호출합니다.
        GetComponentInParent<UI_StorePopup>()?.OnStoreItemPressed(_categoryIndex, _itemIndex);
    }
}
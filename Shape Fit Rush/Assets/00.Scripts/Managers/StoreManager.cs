using UnityEngine;

/// <summary>
/// [3주차 v2.19 - 현업 방식]
/// (GameManager에서 '상점/경제/인벤토리' 책임을 분리)
/// </summary>
public class StoreManager : MonoBehaviour
{
    private GameData _data { get { return Managers.Instance.Data.GameData; } }

    public void Init()
    {
        // (StoreManager 초기화 로직)
    }
    
    // --- 1. 경제 API (GameManager에서 이관) ---
    public int GetTotalCoins() { return _data.totalCoins; }
    
    public void AddCoins(int amount)
    {
        _data.totalCoins += amount;
        Managers.Instance.Data.SaveGame();
        // (UI 갱신은 GameData.totalCoins를 바라보는 UIManager가 담당)
    }

    // --- 2. 블록 스킨 API (GameManager에서 이관) ---
    public bool IsSkinUnlocked(int index) { return (_data.unlockedSkinsBitmask & (1 << index)) != 0; }
    public bool BuySkin(int index)
    {
        int price = Managers.Instance.SkinDB.skins[index].price; 
        if (_data.totalCoins >= price && !IsSkinUnlocked(index)) {
            _data.totalCoins -= price; 
            _data.unlockedSkinsBitmask |= (1 << index);
            EquipSkin(index); // (구매 시 자동 장착)
            return true;
        }
        return false;
    }
    public void EquipSkin(int index)
    {
        if (IsSkinUnlocked(index)) {
            _data.currentSkinIndex = index;
            Managers.Instance.Game.OnSkinEquipped(); // (GameManager에게 '시각적' 갱신 요청)
            Managers.Instance.Data.SaveGame();
        }
    }
    
    // --- 3. 방해 블록 스킨 API (GameManager에서 이관) ---
    public bool IsBadBlockSkinUnlocked(int index) { return (_data.unlockedBadSkinsBitmask & (1 << index)) != 0; }
    public bool BuyBadBlockSkin(int index)
    {
        int price = Managers.Instance.BadBlockDB.badBlocks[index].price;
        if (_data.totalCoins >= price && !IsBadBlockSkinUnlocked(index))
        {
            _data.totalCoins -= price;
            _data.unlockedBadSkinsBitmask |= (1 << index);
            EquipBadBlockSkin(index);
            return true;
        }
        return false;
    }
    public void EquipBadBlockSkin(int index)
    {
        if (IsBadBlockSkinUnlocked(index))
        {
            _data.currentBadBlockSkinIndex = index;
            Managers.Instance.Data.SaveGame();
        }
    }

    // --- 4. 배경 스킨 API (GameManager에서 이관) ---
    public bool IsBackgroundSkinUnlocked(int index) { return (_data.unlockedBackgroundSkinsBitmask & (1 << index)) != 0; }
    public bool BuyBackgroundSkin(int index) 
    {
        int price = Managers.Instance.BackgroundDB.backgrounds[index].price;
        if (_data.totalCoins >= price && !IsBackgroundSkinUnlocked(index))
        {
            _data.totalCoins -= price;
            _data.unlockedBackgroundSkinsBitmask |= (1 << index);
            EquipBackgroundSkin(index);
            return true;
        }
        return false;
    }
    public void EquipBackgroundSkin(int index) 
    {
        if (IsBackgroundSkinUnlocked(index))
        {
            _data.currentBackgroundSkinIndex = index;
            Managers.Instance.Game.OnBackgroundEquipped(); // (GameManager에게 '시각적' 갱신 요청)
            Managers.Instance.Data.SaveGame();
        }
    }
}
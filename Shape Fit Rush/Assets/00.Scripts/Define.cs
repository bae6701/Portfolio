using UnityEngine;

public class Define
{
    public enum Sound 
    { 
        Bgm, 
        Effect, 
        MaxCount 
    }
    
    public enum Scene
    {
        Unknown,
        LoadingScene,
        MainGameScene
    }

    public enum SoundID
    {
        // BGM
        BGM_Lobby,
        BGM_InGame,
        BGM_Fever,
        
        // SFX
        SFX_Tap,
        SFX_Perfect,
        SFX_Good,
        SFX_Miss,
        SFX_Coin,
        
        // Voice
        Voice_GameOver,
        Voice_FeverTime,
        Voice_Combo10,
        Voice_Combo30,
    }

    // [신규 v2.10] 블록 스킨 ID
    public enum SkinID
    {
        Skin_Square_Blue,
        Skin_Star_Yellow,
        Skin_Pentagon_Pink,
        // (스킨 추가 시 여기에 Skin_Diamond 등을 추가)
    }
    
    // [신규 v2.10] 방해 블록 스킨 ID
    public enum BadBlockID
    {
        Bad_Triangle_Red,
        Bad_Circle_Purple,
        // (방해 블록 추가 시 여기에 추가)
    }
    
    // [신규 v2.10] 배경 스킨 ID
    public enum BackgroundID
    {
        BG_Default,
        BG_01,
        BG_02,
        BG_03,
        BG_04,
        BG_05,
        BG_06,
        BG_07,
        BG_08,
        BG_09,
        // (배경 추가 시 여기에 추가)
    }
}
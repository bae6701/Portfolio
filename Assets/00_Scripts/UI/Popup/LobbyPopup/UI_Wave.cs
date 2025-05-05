using Data;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_Wave : UI_Base
{
    private GameObject Wave_Bar;
    private GameObject BossWaveTimer_Bar;
    private TextMeshProUGUI MonsterCount_T;
    private TextMeshProUGUI Timer_T;
    private TextMeshProUGUI Wave_T;
    private TextMeshProUGUI Wave_Bar_Text;
    private TextMeshProUGUI BossWaveTimer_Text;

    private Image MonsterCount_Fill;


    enum GameObjects
    {
        Wave_Bar,
        BossWaveTimer_Bar
    }

    enum Images
    { 
        Monster_Count_Fill
    }
    enum Texts
    { 
        Monster_Count_Text,
        Wave_Text,
        Time,
        Wave_Bar_Text,
        BossName_Text,
        BossWaveTimer_Text,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        Wave_Bar = Get<GameObject>((int)GameObjects.Wave_Bar);
        Wave_Bar.SetActive(false);
        BossWaveTimer_Bar = Get<GameObject>((int)GameObjects.BossWaveTimer_Bar);
        BossWaveTimer_Bar.SetActive(false);

        MonsterCount_T = GetText((int)Texts.Monster_Count_Text);
        Timer_T = GetText((int)Texts.Time);
        Wave_T = GetText((int)Texts.Wave_Text);
        Wave_Bar_Text = Get<TextMeshProUGUI>((int)Texts.Wave_Bar_Text);
        BossWaveTimer_Text = Get<TextMeshProUGUI>((int)Texts.BossWaveTimer_Text);

        MonsterCount_Fill = GetImage((int)Images.Monster_Count_Fill);
    }                        

    void Update()
    {      
        MonsterCount_T.text = Managers.Game.MonsterCount.ToString() + " / " + Managers.Game.MonsterMaxCount.ToString();
        MonsterCount_Fill.fillAmount = (float)Managers.Game.MonsterCount / Managers.Game.MonsterMaxCount;
        Timer_T.text = Managers.Game.BossWave() ? "IN BOSS" : Managers.Game.WavePoint();
        BossWaveTimer_Text.text = Managers.Game.WavePoint();
        Wave_T.text = "WAVE " + Managers.Game.Wave.ToString();      
    } 

    public void WaveNotiBarOpen(string waveText)
    {
        BossWaveTimer_Bar.SetActive(false);
        Wave_Bar.SetActive(true);
        Wave_Bar.GetComponent<Animator>().SetTrigger("MOB");
        Wave_Bar_Text.text = "WAVE " + waveText;
    }

    public void BossWaveBarOpen()
    {
        BossWaveTimer_Bar.SetActive(true);
        Wave_Bar.SetActive(true);
        Wave_Bar.GetComponent<Animator>().SetTrigger("BOSS");
        Wave_Bar_Text.text = "BOSS WAVE";
        //TODO 보스 개수의 따른 변화
        Get<TextMeshProUGUI>((int)Texts.BossName_Text).text = Managers.Data.bossDict[0].monsterName;
    }
}

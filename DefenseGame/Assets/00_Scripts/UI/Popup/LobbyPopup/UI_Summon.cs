using Data;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Summon : UI_Base
{
    [UnityEngine.Range(0.0f, 30.0f)]
    [SerializeField] private float trailSpeed;
    [SerializeField] private float yPosMin, yPosMax;
    [SerializeField] private float xPos;

    enum Texts
    { 
        Summon_Text,
    }

    enum Buttons
    {
        Summon_Button,
    }

    private TextMeshProUGUI Summon_T; 

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        Summon_T = GetText((int)Texts.Summon_Text);
        BindEvent(GetButton((int)Buttons.Summon_Button).gameObject, OnClickSummon);
    }

    private void Update()
    {
        Summon_T.text = Managers.Game.SummonCount.ToString();
        Summon_T.color = Managers.Game.Money >= Managers.Game.SummonCount ? Color.white : Color.red;
    }

    public void OnClickSummon(PointerEventData evt)
    {
        ClickSummon();
    }

    private void ClickSummon()
    {
        if (Managers.Game.Money < Managers.Game.SummonCount) return;
        if (Managers.Spawn.SelectSpawnHeroList(Net_Utils.LocalId() == 0).Count >= Managers.Game.HeroMaxCount) return;

        Managers.Game.Money -= Managers.Game.SummonCount;
        Managers.Game.SummonCount += 2;

        StartCoroutine(SummonCoroutine());
    }

    IEnumerator SummonCoroutine()
    {
        HeroType heroType = Net_Utils.RandomsSpawn(Rarity.COMMON);

        Vector3 buttonWorldPosition = Camera.main.ScreenToWorldPoint(GetButton((int)Buttons.Summon_Button).gameObject.transform.position);
        GameObject trailInstance = Managers.Resource.Instantiate("Effects/Trail");

        trailInstance.transform.position = buttonWorldPosition;

        Vector3 endPos = Managers.Spawn.Spawner.HeroSpawnPos(heroType);

        Vector3 startPoint = buttonWorldPosition;
        Vector3 endPoint = endPos;

        Vector3 controlPoint = GenerateRandomControlPoint(startPoint, endPoint);

        float elapsedTime = 0.0f;

        while (elapsedTime < trailSpeed)
        {
            float t = elapsedTime / trailSpeed;

            Vector3 curvePosition = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

            trailInstance.transform.position = new Vector3(curvePosition.x, curvePosition.y, 0.0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Managers.Resource.Destroy(trailInstance);
        Managers.Spawn.Spawner.Summon(Rarity.COMMON, heroType);
    }

    private Vector3 GenerateRandomControlPoint(Vector3 start, Vector3 end)
    {
        // 시작점과 끝점의 중간 위치
        Vector3 midPoint = (start + end) / 2f;

        // Y축 방향으로 랜덤한 높이를 추가하여 곡선을 만듦
        float randomHeight = Random.Range(yPosMin, yPosMax);
        midPoint += Vector3.up * randomHeight;

        // X 방향으로도 약간의 랜덤 변화를 추가
        midPoint += new Vector3(Random.Range(-xPos, xPos), 0.0f);

        return midPoint;
    }
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 베지어 곡선 공식 : (1-t)^2 & p0 + 2 * (1-t) * t * p1 + t^2 * p2
        return Mathf.Pow(1 - t, 2) * p0 + 2
            * (1 - t) * t * p1 +
            Mathf.Pow(t, 2) * p2;
    }
}

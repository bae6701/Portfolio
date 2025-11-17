using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (리빌드 v2.5 최종본)
/// (MonoBehaviour - 씬에 배치되어야 함)
/// </summary>
public class JudgmentZone : MonoBehaviour 
{
    public Block currentBlockInZone = null;
    private SpriteRenderer sr;

    void Awake() 
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (Managers.Instance.Game != null)
        {
            // 게임이 시작되면, OnGameStart(상태 리셋) 함수를 호출
            Managers.Instance.Game.OnGameStart += OnGameStart;
        }
    }

    void OnDestroy()
    {
        if (Managers.Instance != null && Managers.Instance.Game != null)
        {
            Managers.Instance.Game.OnGameStart -= OnGameStart;
        }
    }

    private void OnGameStart()
    {
        currentBlockInZone = null;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 피버 모드가 아니거나, 처리할 블록(currentBlockInZone)이 없으면 무시
        if (!Managers.Instance.Game.isFeverMode || currentBlockInZone == null)
            return;
            
        // Stay 이벤트가 currentBlockInZone이 아닌 다른 콜라이더에서 발생하면 무시
        if (other.gameObject != currentBlockInZone.gameObject)
            return;

        // GameManager.CheckJudgment()의 거리 계산 로직 사용
        float distance = 0f;
        if (currentBlockInZone.direction == Block.MoveDirection.Down) { distance = Mathf.Abs(currentBlockInZone.transform.position.y - this.transform.position.y); }
        else { distance = Mathf.Abs(currentBlockInZone.transform.position.x - this.transform.position.x); }

        // 거리가 'Perfect' 범위 안에 들어왔을 때만 자동 판정
        if (distance <= Managers.Instance.Settings.perfectThreshold)
        {
            // "자동 퍼펙트" 호출
            Managers.Instance.Game.TriggerFeverPerfect(currentBlockInZone);
            
            // [중요] 1회만 처리하고, 즉시 null로 변경 (다음 프레임에 중복 호출 방지)
            currentBlockInZone = null; 
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Block block = other.GetComponent<Block>();
        if (block != null) 
        { 
            currentBlockInZone = block; 
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Block block = other.GetComponent<Block>();
        
        if (block != null && block == currentBlockInZone) 
        { 
            if (Managers.Instance == null || Managers.Instance.Game.isFeverMode || block.wasSpawnedDuringFever)
            {
                currentBlockInZone = null;
                return;
            }

            // (v2.15) 착한 블록이 판정(null)되지 않고 그냥 빠져나갔다면 Miss
            if (block.isGoodBlock)
            {
                Debug.Log("--- Missed by Pass ---");
                Managers.Instance.Game.TriggerMiss();
            }
            
            currentBlockInZone = null; 
        }
    }

    /// <summary>
    /// GameManager가 '스프라이트'를 직접 전달
    /// </summary>
    public void SetSkin(Sprite frameSprite)
    {
        if (sr != null)
        {
            sr.sprite = frameSprite;
        }
    }
}
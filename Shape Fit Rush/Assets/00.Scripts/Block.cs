using UnityEngine;

/// <summary>
/// (리빌드 v2.5 최종본 - 버그 수정)
/// (v2.5: 유저님의 Managers.cs 아키텍처와 호환)
/// (MonoBehaviour - 프리팹에 부착)
/// </summary>
[RequireComponent(typeof(Poolable))] // [버그 수정] Poolable을 필수로 요구
public class Block : MonoBehaviour
{
    public bool isGoodBlock = true;
    public float moveSpeed = 5f;
    public MoveDirection direction;
    private Transform despawnLeft, despawnRight, despawnBottom;
    public bool wasSpawnedDuringFever = false;
    public enum MoveDirection { Left, Right, Down }
    
    public void Init(MoveDirection dir, Transform despawnL, Transform despawnR, Transform despawnB, bool isFeverBlock)
    {
        this.direction = dir;
        this.despawnLeft = despawnL;
        this.despawnRight = despawnR;
        this.despawnBottom = despawnB;

        this.wasSpawnedDuringFever = isFeverBlock;
    }
    
    void Update()
    {
        switch (direction)
        {
            case MoveDirection.Left:  transform.Translate(Vector2.left * moveSpeed * Time.deltaTime); break;
            case MoveDirection.Right: transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); break;
            case MoveDirection.Down:  transform.Translate(Vector2.down * moveSpeed * Time.deltaTime); break;
        }

        if (CheckIfOutOfScreen())
        {          
            Managers.Instance.Pool.Push(this.GetComponent<Poolable>());
        }
    }

    private bool CheckIfOutOfScreen()
    {
        switch (direction)
        {
            case MoveDirection.Left:  return (despawnLeft != null && transform.position.x < despawnLeft.position.x);
            case MoveDirection.Right: return (despawnRight != null && transform.position.x > despawnRight.position.x);
            case MoveDirection.Down:  return (despawnBottom != null && transform.position.y < despawnBottom.position.y);
            default: return false;
        }
    }
    
    void OnDisable()
    {
        moveSpeed = 5f;
        wasSpawnedDuringFever = false;
    }
}
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// [2주차 v2.7 - 최종본]
/// (BadBlock 생성 시 Sprite 교체)
/// </summary>
public class BlockSpawner : MonoBehaviour 
{
    // --- 2. 서비스 참조 (Start에서 초기화) ---
    private GameManager gameManager;
    private SkinDatabase skinDB;
    private ResourceManager resourceManager;
    private GameObject badBlockPrefab; 

    // --- 3. 내부 변수 ---
    private CancellationTokenSource _spawnCancelToken;
    
    // (인스펙터 참조 - 씬(Scene) 위치)
    [Header("Spawn Positions")]
    public Transform spawnPosRight;
    public Transform spawnPosLeft;
    public Transform spawnPosTop;
    public Transform despawnPosLeft;
    public Transform despawnPosRight;
    public Transform despawnPosBottom;
    
    // [신규] v2.7: Start()에서 매니저 참조 초기화
    async UniTaskVoid Start()
    {
        gameManager = Managers.Instance.Game;
        skinDB = Managers.Instance.SkinDB;
        resourceManager = Managers.Instance.Resource;

        badBlockPrefab = await resourceManager.LoadAsync<GameObject>("Block_Triangle");

        if (gameManager != null)
        {
            gameManager.OnGameStart += StartSpawning;
            gameManager.OnGameOver += StopSpawning;
        }
    }
    void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStart -= StartSpawning;
            gameManager.OnGameOver -= StopSpawning;
        }

        StopSpawning();
    }

    public void StartSpawning() 
    {
        // (v3.2) 시작 전, 이전 작업을 취소
        if (_spawnCancelToken != null)
            StopSpawning();
        
        // (v3.2) 새 취소 토큰을 만들고, 비동기 작업 시작
        _spawnCancelToken = new CancellationTokenSource();
        Co_SpawnBlocks(_spawnCancelToken.Token).Forget(); // UniTask 비동기 시작
    }
    public void StopSpawning() 
    {
        if (_spawnCancelToken != null)
        {
            _spawnCancelToken.Cancel();
            _spawnCancelToken.Dispose();
            _spawnCancelToken = null;
        }
    }

    // --- 4. 스폰 로직 (v2.7) ---
    async UniTaskVoid Co_SpawnBlocks(CancellationToken token)
    { 
        // (v3.2) CancellationTokenSource가(이) Cancel()을(를) 호출하면
        // 이 루프는 'OperationCanceledException'을(를) 발생시키고 종료됨
        while (true)
        {
            float currentSpawnInterval = gameManager.CurrentSpawnInterval;
            float currentBadBlockChance = gameManager.CurrentBadBlockChance;
            float currentBlockSpeed = gameManager.CurrentBlockSpeed;
            bool isCurrentlyFever = gameManager.isFeverMode;

            // [수정 v3.2] WaitForSeconds -> UniTask.Delay
            await UniTask.Delay(System.TimeSpan.FromSeconds(currentSpawnInterval), cancellationToken: token);
            
            int skinIndex = gameManager.currentSkinIndex;
            if (skinDB == null || skinIndex >= skinDB.skins.Count) skinIndex = 0;
            
            GameObject goodPrefab = skinDB.skins[skinIndex].blockPrefab;
            
            bool isBadBlock = (Random.Range(0f, 1f) < currentBadBlockChance);
            
            // [수정 v3.2] Addressable "Key"를(을) 사용
            string keyToSpawn = isBadBlock ? "Block_Triangle" : goodPrefab.name;
            
            // ... (스폰 위치/방향 로직) ...
            Vector3 spawnPos = Vector3.zero;
            Block.MoveDirection spawnDir = Block.MoveDirection.Left;
            int spawnSide = Random.Range(0, 3);
            switch (spawnSide)
            {
                case 0: spawnPos = spawnPosTop.position; spawnDir = Block.MoveDirection.Down; break;
                case 1: spawnPos = spawnPosLeft.position; spawnDir = Block.MoveDirection.Right; break;
                case 2: spawnPos = spawnPosRight.position; spawnDir = Block.MoveDirection.Left; break;
            }
            
            // [수정 v3.2] resourceManager.Instantiate -> await InstantiateAsync (CS0176 에러 해결)
            GameObject blockGO = await resourceManager.InstantiateAsync(keyToSpawn, null);

            if (blockGO != null)
            {
                if (isBadBlock)
                {
                    int badSkinIndex = gameManager.currentBadBlockSkinIndex;
                    if (Managers.Instance.BadBlockDB != null && badSkinIndex < Managers.Instance.BadBlockDB.badBlocks.Count)
                    {
                        Sprite equippedSprite = Managers.Instance.BadBlockDB.badBlocks[badSkinIndex].badBlockSprite;
                        if (equippedSprite != null)
                            blockGO.GetComponent<SpriteRenderer>().sprite = equippedSprite;
                    }
                }

                blockGO.transform.position = spawnPos;
                blockGO.transform.rotation = Quaternion.identity;
                Block block = blockGO.GetComponent<Block>();
                block.moveSpeed = currentBlockSpeed; 
                block.Init(spawnDir, despawnPosLeft, despawnPosRight, despawnPosBottom, isCurrentlyFever);
            }
        }
    }
}
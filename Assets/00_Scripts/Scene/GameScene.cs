using UnityEngine;

public class GameScene : BaseScene
{
    UI_GameScene _sceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        _sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
        Managers.Game.StartGame(_sceneUI);
    }

    public override void Clear()
    {
        
    }

    
}

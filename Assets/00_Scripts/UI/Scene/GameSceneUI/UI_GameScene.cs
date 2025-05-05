using Unity.Netcode;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_GameScene_Top TopUI { get; private set; }
    public UI_GameScene_Middle MiddleUI { get; private set; } 
    public UI_GameScene_Bottom BottomUI { get; private set; } 
    

    public override void Init()
    {
        base.Init();

        TopUI = GetComponentInChildren<UI_GameScene_Top>();
        MiddleUI = GetComponentInChildren<UI_GameScene_Middle>();                   
        BottomUI = GetComponentInChildren<UI_GameScene_Bottom>();  
    }   
}

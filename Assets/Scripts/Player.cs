using Dracon;
using UnityEngine;

public class Player : Sentient
{
    public static Player Instance;
    public PlayerCameraController Camera;
    public PlayerInteraction Interaction;
    public PlayerInput Input;

    [Space, Header("UI")] 
    public CanvasGroup combatUIGroup;
    
    protected override void InternalAwake()
    {
        base.InternalAwake();
        
        Input.keyEvents[PlayerInput.KeyMap.Test].onKeyDown += OnTest;
        Input.keyEvents[PlayerInput.KeyMap.Zoom].onKeyDown += () => OnZoom(true);
        Input.keyEvents[PlayerInput.KeyMap.Zoom].onKeyUp += () => OnZoom(false);
        Instance = this;
    }

    public override void EnterCombat()
    {
        base.EnterCombat();
        Camera.SetCombat(false);
        combatUIGroup.alpha = 1;
    }

    public override void ExitCombat()
    {
        base.ExitCombat();
        Camera.SetStandard();
        combatUIGroup.alpha = 0;
    }

    public void OnZoom(bool zoomed)
    {
        if (InCombat)
        {
            Camera.SetCombat(zoomed);
        }
        else
        {
            if (Camera.standardCam.Priority != 10)
            {
                Camera.SetStandard();
            }
        }
    }

    public void OnTest()
    {
        if (InCombat)
        {
            ExitCombat();
        }
        else
        {
            EnterCombat();
        }
    }
}

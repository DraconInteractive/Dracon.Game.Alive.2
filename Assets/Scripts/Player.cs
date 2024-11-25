using UnityEngine;

public class Player : Sentient
{
    public static Player Instance;
    public PlayerCameraController Camera;
    public PlayerInteraction Interaction;
    
    protected override void InternalAwake()
    {
        base.InternalAwake();
        Interaction.onTestKey += OnTest;
        Interaction.onZoomChange += OnZoom;
        Instance = this;
    }

    public override void EnterCombat()
    {
        base.EnterCombat();
        Camera.SetCombat(false);
    }

    public override void ExitCombat()
    {
        base.ExitCombat();
        Camera.SetStandard();
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

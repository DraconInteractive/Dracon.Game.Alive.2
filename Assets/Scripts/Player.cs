using System;
using System.Linq;
using Dracon;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : Sentient
{
    [Header("Player")]
    public static Player Instance;
    public PlayerCameraController Camera;
    public PlayerInteraction Interaction;
    public PlayerInput Input;

    [Space, Header("Movement")] 
    public StarterAssets.ThirdPersonController MovementController;
    
    [Space, FoldoutGroup("Movement Settings")] 
    public float standardWalkSpeed = 2f;
    [FoldoutGroup("Movement Settings")] 
    public float combatWalkSpeed = 2f;
    [FoldoutGroup("Movement Settings")] 
    public float standardRunSpeed = 5.3f;
    [FoldoutGroup("Movement Settings")] 
    public float combatRunSpeed = 5.3f;
    [FoldoutGroup("Movement Settings")] 
    public float standardRotationSmoothTime = 0.12f;
    [FoldoutGroup("Movement Settings")] 
    public float combatRotationSmoothTime = 0.12f;

    [Header("Combat")] 
    public GameObject gunObj;
    
    [Space, Header("UI")] 
    public CanvasGroup combatUIGroup;
    
    private static readonly int A_Combat = Animator.StringToHash("InCombat");

    protected override void InternalAwake()
    {
        base.InternalAwake();
        
        Input.keyEvents[PlayerInput.KeyMap.Test].onKeyDown += OnTest;
        Input.keyEvents[PlayerInput.KeyMap.Zoom].onKeyDown += () => OnZoom(true);
        Input.keyEvents[PlayerInput.KeyMap.Zoom].onKeyUp += () => OnZoom(false);
        Instance = this;
    }

    private void Start()
    {
        
    }

    public override void EnterCombat()
    {
        base.EnterCombat();
        Camera.SetCombat(false);
        combatUIGroup.alpha = 1;
        primaryAnimator.SetBool(A_Combat, true);
        MovementController.MoveSpeed = combatWalkSpeed;
        MovementController.SprintSpeed = combatRunSpeed;
        MovementController.RotationSmoothTime = combatRotationSmoothTime;
        MovementController.StrafingMode = true;
        gunObj.SetActive(true);
    }

    public override void ExitCombat()
    {
        base.ExitCombat();
        Camera.SetStandard();
        combatUIGroup.alpha = 0;
        primaryAnimator.SetBool(A_Combat, false);
        MovementController.MoveSpeed = standardWalkSpeed;
        MovementController.SprintSpeed = standardRunSpeed;
        MovementController.RotationSmoothTime = standardRotationSmoothTime;
        MovementController.StrafingMode = false;
        gunObj.SetActive(false);

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

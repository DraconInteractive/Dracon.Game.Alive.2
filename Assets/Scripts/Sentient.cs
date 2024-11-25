using System;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Sentient : SerializedMonoBehaviour
{
    [Header("Sentient")]
    public Inventory inventory;
    public SentientStats stats;
    public bool InCombat;
    
    private void Awake()
    {
        InternalAwake();
    }

    private void Update()
    {
        InternalUpdate();
    }

    private void OnValidate()
    {
        InternalValidate();
    }

    protected virtual void InternalAwake()
    {
        
    }
    
    protected virtual void InternalUpdate()
    {
        
    }
    
    protected virtual void InternalValidate()
    {
        if (inventory == null)
        {
            if (TryGetComponent<Inventory>(out var inv))
            {
                inventory = inv;
            }
            else
            {
                inventory = gameObject.AddComponent<Inventory>();
            }
        }
    }

    [FoldoutGroup("Combat"), Button]
    public virtual void EnterCombat()
    {
        InCombat = true;
    }

    [FoldoutGroup("Combat"), Button]
    public virtual void ExitCombat()
    {
        InCombat = false;
    }
    
    [ContextMenu("Assign Core Stats - Random")]
    public void AssignRandomCoreStats ()
    {
        stats.AssignRCoreDistForLevel();
        stats.Update();
        stats.RestoreActiveStats();
    }
}



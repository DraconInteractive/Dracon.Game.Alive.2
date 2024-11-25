using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Stat
{
    public float Current = 1;
}

[Serializable]
public class ActiveStat : Stat // health, mana, stamina
{
    public float BaseMax = 10;
    public float Max = 10;
    public float RegenRate = 0.1f;

    [ProgressBar(0, "$Max"), ShowInInspector, ReadOnly, HideLabel]
    public float ValueBar => Current;
    
    [HideInInspector]
    public Action onEmpty = new(() => { });
    
    public virtual void Tick(Sentient sentient)
    {
        Current += RegenRate;
        Current = Mathf.Clamp(Current, 0, Max);
    }

    public virtual void Modify(float amount)
    {
        Current += amount;
        Current = Mathf.Clamp(Current, 0, Max);
        if (Current == 0)
        {
            onEmpty.Invoke();
        }
    }

    private Color GetBarColor(float value)
    {
        return new Color(0.3f, 0.3f, 0.5f);
    }
}

[Serializable]
public class CoreStat : Stat // str, dex, wis
{ }

[Serializable]
public class CalcStat : Stat
{
    public float Base;
}

[Serializable]
public class SentientStats
{
    private int[] _corePointsPerLevel = new[] { 12, 13, 14, 15, 16, 18, 19, 20, 22, 23, 24, 25 };
    private int _maxLevel => _corePointsPerLevel.Length;
    
    public int level;
    [ShowInInspector, ReadOnly, PropertyOrder(0)]
    public int corePoints => (level > _maxLevel) ? _corePointsPerLevel[_maxLevel - 1] : _corePointsPerLevel[level];
    
    [Header("Core")]
    public CoreStat strength = new();
    public CoreStat dexterity = new();
    public CoreStat wisdom = new();
    public CoreStat intelligence = new();
    public CoreStat constitution = new();
    public CoreStat luck = new();
    
    [Header("Active")]
    public ActiveStat health;
    public ActiveStat stamina;
    public ActiveStat mana;

    [Header("Calculated")]
    public CalcStat damage;
    public CalcStat atkSpeed;
    public CalcStat moveSpeed;

    public void Update()
    {
        // TODO: Final iteration should look like:
        // 1. Update CoreStats
        // 2. Update CalcStats
        // 3. Update ActiveStats

        damage.Current = damage.Base + level * (strength.Current * 1.5f + dexterity.Current * 1.2f);
        atkSpeed.Current = atkSpeed.Base + level * (dexterity.Current * 1.5f + strength.Current * 0.5f);
        moveSpeed.Current = moveSpeed.Base + level * (dexterity.Current * .05f);

        health.Max = health.BaseMax + level * (constitution.Current * 5);
        stamina.Max = stamina.BaseMax + level * (constitution.Current * 2.5f + strength.Current * 2.5f);
        mana.Max = mana.BaseMax + level * (wisdom.Current * 5 + intelligence.Current * 1);
    }
    
    public void AssignRCoreDistForLevel()
    {
        var assigned = new int[6];
        
        for (var i = 0; i < corePoints; i++)
        {
            var r = Random.Range(0, 6);
            assigned[r]++;
        }

        strength.Current = assigned[0];
        dexterity.Current = assigned[1];
        wisdom.Current = assigned[2];
        intelligence.Current = assigned[3];
        constitution.Current = assigned[4];
        luck.Current = assigned[5];
    }

    public void RestoreActiveStats()
    {
        health.Current = health.Max;
        stamina.Current = stamina.Max;
        mana.Current = mana.Max;
    }
}

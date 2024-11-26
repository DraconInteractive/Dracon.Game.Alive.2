using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Dracon/Item")]
public class Item : ScriptableObject
{
    [ReadOnly]
    public string ID;
    public string Name;
    public Sprite Icon;
    public bool Stackable;
    
    [SerializeReference]
    public List<ItemComponent> Components = new(); // This array holds the components for the item

    [HideInInspector]
    public string ComponentJSON;
    
    [Button]
    public void GenerateID()
    {
        ID = System.Guid.NewGuid().ToString();
    }

    [Button]
    public void Save()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All  // Include type information when serializing and deserializing
        };
        ComponentJSON = JsonConvert.SerializeObject(Components, Formatting.Indented, settings);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [Button]
    public void Load()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All  // Include type information when serializing and deserializing
        };
        var components = JsonConvert.DeserializeObject<List<ItemComponent>>(ComponentJSON, settings);
        Components = components;
    }

    private void OnValidate()
    {
        if ((Components == null || Components.Count == 0) && !string.IsNullOrEmpty(ComponentJSON))
        {
            Load();
        }
    }

    [Button]
    public void Debug()
    {
        UnityEngine.Debug.Log(
            $"ID: {ID}\nIcon: {((Icon == null) ? "None" : Icon.name)}\nComponents: {Components.Count}: {string.Join("\n -", Components.Select(x => x.GetType().FullName))}");
    }
}

[System.Serializable]
public class ItemInstance
{
    private string _id;

    public string ID
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
            }

            return _id;
        }
    }
    public Item archetype;
    public int StackSize = 1; // For non-stackables, will default to 1
    [SerializeReference]
    public List<ItemComponent> Components = new(); // This array holds the components for the item

    public static ItemInstance FromItem(Item item)
    {
        item.Load();
        var instance = new ItemInstance
        {
            archetype = item,
            Components = new List<ItemComponent>()
        };

        foreach (var component in item.Components)
        {
            var clone = component.Clone();
            Debug.Log("Clone created: " + clone.GetType());
            instance.Components.Add(clone);
        }

        return instance;
    }
}

#region Components
public abstract class ItemComponent
{
    public abstract ItemComponent Clone();
}

public class EquippableComponent : ItemComponent
{
    public enum EquipSlot
    {
        Weapon,
        Head
    }

    public EquipSlot slot;


    public override ItemComponent Clone()
    {
        return new EquippableComponent() { slot = slot };
    }
}

public class WeaponComponent : ItemComponent
{
    public int damage;
    public int range;
    public override ItemComponent Clone()
    {
        return new WeaponComponent() { damage = damage, range = range };
    }
}

public class ShieldComponent : ItemComponent
{
    public int defense;
    public override ItemComponent Clone()
    {
        return new ShieldComponent() { defense = defense };
    }
}
#endregion
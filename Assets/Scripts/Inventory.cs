using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : SentientModule
{
    public List<ItemInstance> items = new();
    public List<EquipmentSlot> slots = new();

    public override void SentientAwake()
    {
        base.SentientAwake();
        foreach (var item in items)
        {
            item.Components = item.archetype.Components;
        }
    }

    [Button]
    public void AddToInventory(Item baseItem)
    {
        if (baseItem.Stackable)
        {
            var existingItem = items.FirstOrDefault(x => x.archetype == baseItem);
            if (existingItem != null)
            {
                existingItem.StackSize++;
                return;
            }
        }
        items.Add(ItemInstance.FromItem(baseItem));
    }

    public void AddToInventory(ItemInstance itemInstance)
    {
        if (itemInstance.archetype.Stackable)
        {
            var existingItem = items.FirstOrDefault(x => x.archetype == itemInstance.archetype);
            if (existingItem != null)
            {
                existingItem.StackSize += itemInstance.StackSize;
                return;
            }
        }
        items.Add(itemInstance);
    }

    public ItemInstance GetItemByIndex(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return null;
        }
        return items[index];
    }
    
    [Serializable]
    public class EquipmentSlot
    {
        public string SlotName;
        public string EquippedItemID;
    }
}
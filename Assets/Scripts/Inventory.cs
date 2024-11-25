using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemInstance> items = new();

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
}
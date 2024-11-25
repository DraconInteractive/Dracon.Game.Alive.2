using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ItemContainer : Interactable
{
    private Inventory inventory;

    [Header("UI")] 
    public GameObject uiBaseObject;
    public GameObject uiPrefab;
    public Transform uiContainer;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }
 
    public override void Interact()
    {
        base.Interact();
        
        // Can either add items from this inventory to players inventory,
        // or show inventory window. That works too. 
        
        // Running subtractive approach for now
        var item = inventory.items[0];
        inventory.items.RemoveAt(0);
        
        Player.Instance.inventory.AddToInventory(item);
        
        CleanUI();
        PopulateUI();
    }

    public override bool CanInteract()
    {
        return inventory.items.Count > 0;
    }

    public override void Focus()
    {
        base.Focus();
        if (uiBaseObject == null) return;
        
        uiBaseObject.SetActive(true);
        PopulateUI();   
    }

    public override void Defocus()
    {
        base.Defocus();
        if (uiBaseObject == null) return;

        CleanUI();
        uiBaseObject.SetActive(false);

    }

    private void PopulateUI()
    {
        foreach (var item in inventory.items)
        {
            var obj = Instantiate(uiPrefab, uiContainer);
            var txt = obj.GetComponentInChildren<TMP_Text>();
            txt.text = $"{item.StackSize}x {item.archetype.Name}";
        }
    }

    private void CleanUI()
    {
        foreach (Transform child in uiContainer)
        {
            Destroy(child.gameObject);
        }
    }
}

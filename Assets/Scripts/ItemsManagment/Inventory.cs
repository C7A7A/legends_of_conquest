using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private List<ItemsManager> itemsList;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        itemsList = new List<ItemsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItems(ItemsManager item) {
        if (item.isStackable) {

            bool itemAlreadyinInventory = false;

            foreach (ItemsManager itemInInventory in itemsList) {
                if (itemInInventory.itemName == item.itemName) {
                    itemInInventory.amount += item.amount;
                    itemAlreadyinInventory = true;
                }
            }

            if (!itemAlreadyinInventory) {
                itemsList.Add(item);
            }

        } else {
            itemsList.Add(item);
        }
    }

    public void RemoveItem(ItemsManager item) {
        if (item.isStackable) {
            ItemsManager inventoryItem = null;
            
            foreach (ItemsManager itemInInventory in itemsList) {
                if (itemInInventory.itemName == item.name) {
                    itemInInventory.amount--;
                    inventoryItem = itemInInventory;
                }
            }

            if (inventoryItem != null && inventoryItem.amount <= 0) {
                itemsList.Remove(item);
            }

        } else {
            itemsList.Remove(item);
        }

        MenuManager.instance.UpdateItemsInventory();
    }

    public List<ItemsManager> GetItemsList() {
        return itemsList;
    }
}

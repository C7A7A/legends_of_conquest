using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public static ShopManager instance;

    public GameObject shopMenu, buyPanel, sellPanel;

    [SerializeField] TextMeshProUGUI currentGoldText;

    public List<ItemsManager> itemsForSale;

    [SerializeField] GameObject itemSlotContainer;
    [SerializeField] RectTransform itemSlotBuyContainerParent;
    [SerializeField] RectTransform itemSlotSellContainerParent;

    [SerializeField] ItemsManager selectedItem;
    [SerializeField] TextMeshProUGUI buyItemName, buyItemDescription, buyItemValue;
    [SerializeField] TextMeshProUGUI sellItemName, sellItemDescription, sellItemValue;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenShopMenu() {
        shopMenu.SetActive(true);
        GameManager.instance.shopOpened = true;

        currentGoldText.text = "Gold: " + GameManager.instance.currentGold;
        buyPanel.SetActive(true);
    }

    public void CloseShopMenu() {
        shopMenu.SetActive(false);
        GameManager.instance.shopOpened = false;
    }

    public void OpenBuyPanel() {
        buyPanel.SetActive(true);
        sellPanel.SetActive(false);

        UpdateItemsInShop(itemSlotBuyContainerParent, itemsForSale);
    }

    public void OpenSellPanel() {
        buyPanel.SetActive(false);
        sellPanel.SetActive(true);

        UpdateItemsInShop(itemSlotSellContainerParent, Inventory.instance.GetItemsList());
    }

    private void UpdateItemsInShop(RectTransform itemSlotContainerParent, List<ItemsManager> itemsToLoopThrough) {
        foreach (RectTransform itemSlot in itemSlotContainerParent) {
            Destroy(itemSlot.gameObject);
        }

        foreach (ItemsManager item in itemsToLoopThrough) {
            RectTransform itemSlot = Instantiate(itemSlotContainer, itemSlotContainerParent).GetComponent<RectTransform>();

            Image itemImage = itemSlot.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = item.itemsImage;

            TextMeshProUGUI itemAmountText = itemSlot.Find("ItemAmountText").GetComponent<TextMeshProUGUI>();

            if (item.amount > 1) {
                itemAmountText.text = "";
            } else {
                itemAmountText.text = "";
            }

            itemSlot.GetComponent<ItemButton>().itemOnButton = item;
        }
    }

    public void SelectedBuyItem(ItemsManager itemToBuy) {
        selectedItem = itemToBuy;
        buyItemName.text = selectedItem.itemName;
        buyItemDescription.text = selectedItem.itemDescription;
        buyItemValue.text = "Value: " + selectedItem.valueInCoins.ToString();
    }

    public void SelectedSellItem(ItemsManager itemToSell) {
        selectedItem = itemToSell;
        sellItemName.text = selectedItem.itemName;
        sellItemDescription.text = selectedItem.itemDescription;

        int value = (int)(selectedItem.valueInCoins * 0.4f);
        sellItemValue.text = "Value: " + value.ToString();
    }

    public void BuyItem() {
        if (GameManager.instance.currentGold >= selectedItem.valueInCoins) {
            GameManager.instance.currentGold -= selectedItem.valueInCoins;
            Inventory.instance.AddItems(selectedItem);

            currentGoldText.text = "Gold: " + GameManager.instance.currentGold;
        }
    }

    public void SellItem() {
        if (selectedItem) {
            int value = (int)(selectedItem.valueInCoins * 0.4f);

            GameManager.instance.currentGold += value;
            Inventory.instance.RemoveItem(selectedItem);

            currentGoldText.text = "Gold: " + GameManager.instance.currentGold;

            selectedItem = null;
        }

        OpenSellPanel();
    }
}

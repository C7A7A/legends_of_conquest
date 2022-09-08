using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{

    private bool canOpenShop;

    [SerializeField] List<ItemsManager> shopKeeperItemsForSale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpenShop && Input.GetButtonDown("Fire1") && Player.instance.IsMovementActive && !ShopManager.instance.shopMenu.activeInHierarchy) {
            ShopManager.instance.itemsForSale = shopKeeperItemsForSale;
            ShopManager.instance.OpenShopMenu();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            canOpenShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            canOpenShop = false;
        }
    }
}

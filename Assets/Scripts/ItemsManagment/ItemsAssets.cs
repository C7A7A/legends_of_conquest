using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsAssets : MonoBehaviour
{
    public static ItemsAssets instance;
    [SerializeField] ItemsManager[] itemsAvailable;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ItemsManager GetItemAsset(string itemToGetName) {
        foreach (ItemsManager item in itemsAvailable) {
            if (item.itemName == itemToGetName) {
                return item;
            }
        }

        return null;
    }
}

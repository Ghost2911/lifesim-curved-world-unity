using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    public string storeName;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hit.transform.GetComponent<PlayerInput>().inventory.gameObject.GetComponent<InventoryShop>().Open(storeName);
        Debug.Log("Shop trigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.GetComponent<PlayerInput>().inventory.gameObject.GetComponent<InventoryShop>().Open(storeName);
    }
    
}

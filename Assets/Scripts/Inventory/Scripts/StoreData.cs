using UnityEngine;

[CreateAssetMenu(fileName = "Store", menuName = "Scriptable Object/Store")]
public class StoreData : ScriptableObject
{
    [Header("Настройки магазина:")]
    public string storeName;
    [Header("Настройки товаров:")]
    public StoreItem[] items;
}

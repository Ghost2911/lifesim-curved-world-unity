
[System.Serializable]
public class StoreItem
{
    public string name; // имя товара, которое будет отображаться для игрока
    public InventoryComponent prefab; // сам префаб
    public int buy, sell, count; // покупка, продажа, количество (если count = 0, то по умолчанию этого товара не будет в наличии)
}

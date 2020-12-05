using UnityEngine;

public class InventoryComponent : MonoBehaviour {

	[SerializeField] private int _limit = 99; // сколько предметов данного типа может быть в инвентаре
	[SerializeField] private Sprite _icon; // иконка предмета для инвентаря
	[SerializeField] private string _item = "myItem"; // идентификатор (имя предмета)
    [SerializeField] private InventoryComponent[] _loot;
    [SerializeField] private toolType _requiredTool;
    [SerializeField] private int _id;
    [SerializeField] private InventoryEnum _size; // выбираем размер иконки


    public int limit => _limit;
    public InventoryComponent[] loot => _loot;
    public Sprite icon => _icon;
    public int id => _id;
    public string item => _item;
    public toolType tool => _requiredTool;
    public InventoryEnum size => _size;
}

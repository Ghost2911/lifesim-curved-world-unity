using UnityEngine;

public class InventoryDrop : MonoBehaviour {

	private static InventoryDrop _internal;
    public WorldChanger wc;
 
	void Awake()
	{
		_internal = this;
	}

	public static void DropItem(InventoryComponent item, int count)
	{
		if(item == null || count <= 0) return;
		_internal.DropItem_internal(item, count);
	}

	void DropItem_internal(InventoryComponent item, int count)
	{
		for(int i = 0; i < count; i++)
		{
            //сброс предметов в точке transform
            //сброс предметов не поместившихся в инвентарь через AddItem
            Vector3 placePos = transform.position;
            wc.PutObject(item.id, Mathf.RoundToInt(placePos.x), Mathf.RoundToInt(placePos.z));
		}
	}
}
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{

    public string Item { get; set; }
    public InventoryEnum Size { get; set; }
    public int Counter { get; set; }
    public SurrogateVector2 Position { get; set; }

    public InventorySaveData() { }

    public InventorySaveData(string item, InventoryEnum size, int counter, Vector2 position)
    {
        this.Item = item;
        this.Size = size;
        this.Counter = counter;
        this.Position = position;
    }
}

[System.Serializable]
public class InventoryDataState
{

    public List<InventorySaveData> items = new List<InventorySaveData>();
    public string[,] Field { get; set; }
    public string Money { get; set; }

    public InventoryDataState() { }

    public void AddItem(InventorySaveData item)
    {
        items.Add(item);
    }
}

[System.Serializable]
public struct SurrogateVector2
{
    public float x, y;

    public SurrogateVector2(float rX, float rY)
    {
        x = rX;
        y = rY;
    }

    public static implicit operator Vector2(SurrogateVector2 rValue)
    {
        return new Vector2(rValue.x, rValue.y);
    }

    public static implicit operator SurrogateVector2(Vector2 rValue)
    {
        return new SurrogateVector2(rValue.x, rValue.y);
    }
}

public class InventorySerializator
{

    public static void SaveBinary(InventoryDataState state, string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Create);
        binary.Serialize(stream, state);
        stream.Close();
    }

    public static InventoryDataState LoadBinary(string dataPath)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream stream = new FileStream(dataPath, FileMode.Open);
        InventoryDataState state = (InventoryDataState)binary.Deserialize(stream);
        stream.Close();
        return state;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class WorldChanger : MonoBehaviour
{
   int worldSize = 64;
   public Inventory inventory;
   static private GameObject[,] worldObjects;
   public GameObject[,] worldEnviroment;

    void Awake()
    {
        LoadMain();
    }

    private void LoadMain()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main":
                {
                    if (File.Exists(Path()))
                    {
                        WorldInfo.matrix = new int[worldSize, worldSize];
                        WorldInfo.matrix = WorldSerializator.LoadBinary(WorldInfo.matrix, Path());
                    }
                    else
                        WorldInfo.matrix = Maze.GenerateMap(worldSize, worldSize);
                    break;
                }
            case "Cave":
                {
                    WorldInfo.matrix = Maze.GenerateCave(worldSize, worldSize);
                    break;
                }
            default:
                return;
        }

        worldObjects = new GameObject[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                if (WorldInfo.matrix[x, y] != 0)
                {
                    worldObjects[x, y] = Instantiate(Resources.Load("Prefab/" + WorldInfo.matrix[x, y]) as GameObject, new Vector3(x, 0f, y), new Quaternion(), gameObject.transform);
                    worldObjects[x, y].name = WorldInfo.matrix[x, y].ToString();
                }
            }
        }
    }

    public void PutObject(int idItem,int x, int y)
    {
        worldObjects[x, y] = Instantiate(Resources.Load("Prefab/" + idItem) as GameObject, new Vector3(x, 0f, y), new Quaternion(), gameObject.transform);
        WorldInfo.matrix[x, y] = Convert.ToInt32(idItem);
    }

    public void TakeObject(toolType handTool, int x, int y)
    {
        if (worldObjects[x, y] != null)
        {
            InventoryComponent ic = worldObjects[x, y].GetComponent<InventoryComponent>();
            if (ic.tool == handTool)
            {
                inventory.AddQuickly(ic, 1);
                WorldInfo.matrix[x, y] = 0;
                Destroy(worldObjects[x, y].gameObject);
                if (ic.loot.Length != 0)
                {
                    Debug.Log("спаун");
                    PutObject(Convert.ToInt32(ic.loot[0].name), x, y);
                }
               
            }
        }
    }

    string Path()
    {
        return Application.persistentDataPath + "/Map.bin";
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (SceneManager.GetActiveScene().name == "Main")
                WorldSerializator.SaveBinary(WorldInfo.matrix, Path());
        }
    }
    private void OnApplicationQuit()
    { 
        if (SceneManager.GetActiveScene().name == "Main")
            WorldSerializator.SaveBinary(WorldInfo.matrix,Path());
    }

    //FOR DEBUGGING
    public void DeleteWorld()
    {
        File.Delete(Path());
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                Destroy(worldObjects[x, y]);
            }
        }

        WorldInfo.matrix = Maze.GenerateMap(worldSize, worldSize);
        worldObjects = new GameObject[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                if (WorldInfo.matrix[x, y] != 0)
                {
                    worldObjects[x, y] = Instantiate(Resources.Load("Prefab/" + WorldInfo.matrix[x, y]) as GameObject, new Vector3(x, 0f, y), new Quaternion(), gameObject.transform);
                    worldObjects[x, y].name = WorldInfo.matrix[x, y].ToString();
                }
            }
        }
    }
}


public class WorldSerializator
{
    public static void SaveBinary(int[,] world, string dataPath)
    {
        File.WriteAllBytes(dataPath, WorldInfo.ToBytes<int>(world));
    }

    public static int[,] LoadBinary(int[,] matrix, string dataPath) 
    {
        WorldInfo.FromBytes(matrix,File.ReadAllBytes(dataPath));
        return matrix;
    }
}

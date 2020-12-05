using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maze
{
    public static int[,] GenerateMap(int width, int height)
    {
        int[,] data = new int[width, height];
        
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                int rnd =  Random.Range(0,20);
                data[i, j] = (rnd > 10)?0:rnd;
            }

        return data;
    }

    public static int[,] GenerateCave(int width, int height)
    {
        int[,] data = new int[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                int rnd = Random.Range(11, 40);
                data[i, j] = (rnd > 15) ? 0 : rnd;
            }

        return data;
    }

    struct Node
    {
        public bool path, check;
    }

    public static int Round(int value)
    {
        if (value % 2 == 0) return value + 1; else return value;
    }

    public static int[,] GenerateMaze(int width, int height)
    {
        if (width < 10 || height < 10) return null;

        width = Round(width);
        height = Round(height);

        int x, y;
        bool finish = false;

        List<string> dir = new List<string>();
        Node[,] field = new Node[width, height];

        int j = Round(Random.Range(0, height - 1));

        field[1, j].path = true; 
        field[1, j].check = true; 

        while (true)
        {
            finish = true;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    if (field[x, y].path) // ищем флажок развилки
                    {
                        finish = false;

                        if (x - 2 >= 0)
                            if (!field[x - 2, y].check) dir.Add("Left"); // если путь свободен, добавляем направление

                        if (y - 2 >= 0)
                            if (!field[x, y - 2].check) dir.Add("Down");

                        if (x + 2 < width)
                            if (!field[x + 2, y].check) dir.Add("Right");

                        if (y + 2 < height)
                            if (!field[x, y + 2].check) dir.Add("Up");

                        if (dir.Count > 0)
                        {
                            switch (dir[Random.Range(0, dir.Count)]) // выбираем случайное направление
                            {
                                case "Left":
                                    field[x - 1, y].check = true;
                                    field[x - 2, y].check = true;
                                    field[x - 2, y].path = true;
                                    break;

                                case "Down":
                                    field[x, y - 1].check = true;
                                    field[x, y - 2].check = true;
                                    field[x, y - 2].path = true;
                                    break;

                                case "Right":
                                    field[x + 1, y].check = true;
                                    field[x + 2, y].check = true;
                                    field[x + 2, y].path = true;
                                    break;

                                case "Up":
                                    field[x, y + 1].check = true;
                                    field[x, y + 2].check = true;
                                    field[x, y + 2].path = true;
                                    break;
                            }
                        }
                        else // если направление добавить невозможно, убираем флажок развилки
                        {
                            field[x, y].path = false;
                        }

                        dir.Clear(); // чистим массив
                    }
                }
            }

            if (finish) break; 
        }

        int[,] result = new int[width, height];

        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                if (field[x, y].check)
                    result[x, y] = 1;
                else
                    result[x, y] = 2;
            }
        }

        return result;
    }
}
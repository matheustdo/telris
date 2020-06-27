using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchLogic : MonoBehaviour
{
    static public int rowsAmount = 20;
    static public int columnsAmount = 10;
    private int[,] grid = new int[rowsAmount, columnsAmount];
    private int[,] lonely = new int[rowsAmount, columnsAmount];
    public GameObject blockObject;
    public Material blockMaterial;
    private Block fallingBlock;
    private GameObject[] fallingBlockObjects;
    private int[] fallingBlockPosition = new int[2];
    private float delay = 1f;
    private float time = 0f;
    private bool falling = false;

    void Update()
    {
        time += Time.deltaTime;

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
        {
            if (MoveBlockInX(1))
                UpdateLonelyPrint();
        }
        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
        {
            if (MoveBlockInX(-1))
                UpdateLonelyPrint();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Rotate(-1))
                UpdateLonelyPrint();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Rotate(1))
                UpdateLonelyPrint();
        }
        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
        {
            if (MoveBlockInY(-1))
            {
                UpdateLonelyPrint();
            }
            else
            {
                StampToGrid();
                falling = false;
            }

            time = 0f;
        }

        if (falling && time >= delay)
        {
            if (MoveBlockInY(-1))
            {
                UpdateLonelyPrint();
            }
            else
            {
                StampToGrid();
                falling = false;
            }

            time = 0f;
        }
        else if (!falling)
        {
            Block block = new Block(new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 0, 0 } }, blockMaterial);
            AddBlock(block);
            falling = true;
            time = 0f;
        }
    }

    bool MoveBlockInX(int move)
    {
        int[,] movedLonely = new int[rowsAmount, columnsAmount];

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (lonely[i, j] == 1)
                {
                    if (j + move >= 0 && j + move < columnsAmount && grid[i, j + move] == 0)
                    {
                        movedLonely[i, j + move] = lonely[i, j];
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        fallingBlockPosition[1] = fallingBlockPosition[1] + move;
        lonely = movedLonely;
        return true;
    }

    bool MoveBlockInY(int move)
    {
        int[,] movedLonely = new int[rowsAmount, columnsAmount];

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (lonely[i, j] == 1)
                {
                    if (i + move >= 0 && i + move < rowsAmount && grid[i + move, j] == 0)
                    {
                        movedLonely[i + move, j] = lonely[i, j];
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        fallingBlockPosition[0] = fallingBlockPosition[0] + move;
        lonely = movedLonely;
        return true;
    }

    bool AddBlock(Block block)
    {
        int[,] auxLonely = new int[rowsAmount, columnsAmount];
        List<GameObject> auxFallingBlockObjects = new List<GameObject>();

        for (int i = 0; i < block.Structure.GetLength(0); i++)
        {
            for (int j = 0; j < block.Structure.GetLength(1); j++)
            {
                if (block.Structure[i, j] == 1)
                {
                    if (grid[rowsAmount - 1 - i, (columnsAmount / 2) - (block.Structure.GetLength(1) / 2) + j] == 1)
                    {
                        return false;
                    }
                    else
                    {
                        auxLonely[rowsAmount - 1 - i, (columnsAmount / 2) - (block.Structure.GetLength(1) / 2) + j] = 1;
                        GameObject newBlock = Instantiate(blockObject, new Vector3((columnsAmount / 2) - (block.Structure.GetLength(1) / 2) + j, rowsAmount - 1 - i, 0), new Quaternion(0, 0, 0, 0));
                        newBlock.GetComponent<Renderer>().material = block.Material;
                        auxFallingBlockObjects.Add(newBlock);
                    }
                }
            }
        }

        fallingBlockPosition[0] = rowsAmount - 1;
        fallingBlockPosition[1] = (columnsAmount / 2) - (block.Structure.GetLength(1) / 2);
        fallingBlock = block;
        fallingBlockObjects = auxFallingBlockObjects.ToArray();
        lonely = auxLonely;
        return true;
    }

    void StampToGrid()
    {
        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (lonely[i, j] == 1)
                {
                    grid[i, j] = lonely[i, j];
                }
            }
        }
    }

    bool Rotate(int direction)
    {
        int[,] auxBlockStructure = new int[fallingBlock.Structure.GetLength(0), fallingBlock.Structure.GetLength(1)];
        int[,] auxLonely = new int[rowsAmount, columnsAmount];

        if (direction < 0)
        {
            for (int i = 0; i < fallingBlock.Structure.GetLength(0); i++)
            {
                for (int j = 0; j < fallingBlock.Structure.GetLength(1); j++)
                {
                    auxBlockStructure[fallingBlock.Structure.GetLength(0) - j - 1, i] = fallingBlock.Structure[i, j];
                }
            }
        }
        else
        {
            for (int i = 0; i < fallingBlock.Structure.GetLength(0); i++)
            {
                for (int j = 0; j < fallingBlock.Structure.GetLength(1); j++)
                {
                    auxBlockStructure[j, fallingBlock.Structure.GetLength(0) - i - 1] = fallingBlock.Structure[i, j];
                }
            }
        }

        for (int i = 0; i < fallingBlock.Structure.GetLength(0); i++)
        {
            for (int j = 0; j < fallingBlock.Structure.GetLength(1); j++)
            {
                if (auxBlockStructure[i, j] == 1)
                {
                    if (fallingBlockPosition[0] - i >= 0
                        && fallingBlockPosition[1] + j >= 0
                        && fallingBlockPosition[0] - i < rowsAmount
                        && fallingBlockPosition[1] + j < columnsAmount
                        && grid[fallingBlockPosition[0] - i, fallingBlockPosition[1] + j] == 0)
                    {
                        auxLonely[fallingBlockPosition[0] - i, fallingBlockPosition[1] + j] = 1;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        fallingBlock = new Block(auxBlockStructure, fallingBlock.Material);
        lonely = auxLonely;
        return true;
    }

    void UpdateLonelyPrint()
    {
        int count = 0;

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (lonely[i, j] == 1)
                {
                    fallingBlockObjects[count].GetComponent<Transform>().position = new Vector3(j, i, 0);
                    count++;
                }
            }
        }
    }
}

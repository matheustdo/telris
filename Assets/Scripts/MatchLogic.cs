using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchLogic : MonoBehaviour
{
    static public int rowsAmount = 20;
    static public int columnsAmount = 10;
    public int[,] grid = new int[rowsAmount, columnsAmount];
    private int[,] lonely = new int[rowsAmount, columnsAmount];
    public GameObject blockObject;
    private GameObject[,] blockObjects = new GameObject[rowsAmount, columnsAmount];
    private int[,] blockStructure = new int[,] { { 0, 1, 0, }, { 1, 1, 1 }, { 0, 0, 0 } };
    private int[,] fallingBlock;
    private int[] fallingBlockPosition = new int[2];
    public Material Material1;
    public Material Material2;
    private float delay = 1f;
    private float time = 0f;
    private bool falling = false;

    void Start()
    {
        grid[0, 5] = 1;
        grid[0, 6] = 1;
        grid[1, 6] = 1;
        grid[5, 5] = 1;

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                blockObjects[i, j] = Instantiate(blockObject, new Vector3(j, i, 0), new Quaternion(0, 0, 0, 0));
                if (grid[i, j] == 1)
                {
                    blockObjects[i, j].GetComponent<Renderer>().material = Material2;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
        {
            MoveBlockInX(1);
        }
        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
        {
            MoveBlockInX(-1);
        }
        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0)
        {
            Rotate(-1);
        }
        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
        {
            if (!MoveBlockInY(-1))
            {
                StampToGrid();
                falling = false;
            }
        }

        time += Time.deltaTime;

        if (falling && time >= delay)
        {
            if (!MoveBlockInY(-1))
            {
                StampToGrid();
                falling = false;
            }

            time = 0f;
        }
        else if (!falling)
        {
            AddBlock(blockStructure);
            falling = true;
            time = 0f;
        }


        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (grid[i, j] == 1 || lonely[i, j] == 1)
                {
                    blockObjects[i, j].GetComponent<Renderer>().material = Material2;
                }
                else
                {
                    blockObjects[i, j].GetComponent<Renderer>().material = Material1;
                }
            }
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

    bool AddBlock(int[,] block)
    {
        int[,] auxLonely = new int[rowsAmount, columnsAmount];

        for (int i = 0; i < block.GetLength(0); i++)
        {
            for (int j = 0; j < block.GetLength(1); j++)
            {
                if (block[i, j] == 1)
                {
                    if (grid[rowsAmount - 1 - i, (columnsAmount / 2) - (block.GetLength(1) / 2) + j] == 1)
                    {
                        return false;
                    }
                    else
                    {
                        auxLonely[rowsAmount - 1 - i, (columnsAmount / 2) - (block.GetLength(1) / 2) + j] = 1;
                    }
                }
            }
        }

        fallingBlockPosition[0] = rowsAmount - 1;
        fallingBlockPosition[1] = (columnsAmount / 2) - (block.GetLength(1) / 2);
        fallingBlock = block;
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
        int[,] auxBlock = new int[fallingBlock.GetLength(0), fallingBlock.GetLength(1)];
        int[,] auxLonely = new int[rowsAmount, columnsAmount];

        if (direction < 0)
        {
            for (int i = 0; i < fallingBlock.GetLength(0); i++)
            {
                for (int j = 0; j < fallingBlock.GetLength(1); j++)
                {
                    auxBlock[fallingBlock.GetLength(0) - j - 1, i] = fallingBlock[i, j];
                }
            }
        }
        else
        {
            for (int i = 0; i < fallingBlock.GetLength(0); i++)
            {
                for (int j = 0; j < fallingBlock.GetLength(1); j++)
                {
                    auxBlock[j, fallingBlock.GetLength(0) - i - 1] = fallingBlock[i, j];
                }
            }
        }

        for (int i = 0; i < fallingBlock.GetLength(0); i++)
        {
            for (int j = 0; j < fallingBlock.GetLength(1); j++)
            {
                if (auxBlock[i, j] == 1)
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

        fallingBlock = auxBlock;
        lonely = auxLonely;
        return true;
    }
}

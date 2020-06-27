using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int[,] Structure { get; }

    public Material Material { get; set; }

    public Block(int[,] structure, Material material)
    {
        this.Structure = structure;
        this.Material = material;
    }
}

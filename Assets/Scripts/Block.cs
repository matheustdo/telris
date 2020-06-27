using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : ScriptableObject
{
    private int[,] Structure { get; }
    private GameObject Model { get; set; }
    private int Rotation { get; set; }

    public Block(int[,] structure, GameObject model, int rotation)
    {
        this.Structure = structure;
        this.Model = model;
        this.Rotation = rotation;
    }
}

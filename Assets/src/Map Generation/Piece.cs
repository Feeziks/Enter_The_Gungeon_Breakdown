using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapGeneration;

public class Piece
{
    //The pieces prefab
    public GameObject prefab;

    public int rotation;

    public PieceSet[] legalNeighbors;
    public Piece[][] legalNeighborsArray;

    public int index;
    public float PLogP;

    //Constructor
    public Piece(GameObject prefab, int rotation, int index)
    {
        this.prefab = prefab;
        this.rotation = rotation;
        this.index = index;
    }

    //Check if another Piece fits in a specific direction
    public bool Fits(Direction dir, Piece piece)
    {
        //TODO: Actually check what does and does not fit as a neighbor
        return true;
    }

    //
}

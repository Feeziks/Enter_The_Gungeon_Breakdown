using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapGeneration;

//Data container for each piece. 
//A piece is a potential choice for a slot while it collapses

public class Piece
{

    //Public Members
    //Prefab
    public GameObject prefab;

    //Valid Neighbor list
    public int[,] validNeighbors;

    //name
    public string name;

    //Private Members

    //Constructors
    public Piece(string n, GameObject p, int[,] v)
    {
        this.name = n;
        this.prefab = p;
        this.validNeighbors = v;
    }

    public Piece(Piece copy)
    {
        this.name = copy.name;
        this.prefab = copy.prefab;
        this.validNeighbors = copy.validNeighbors;
    }

    //Public methods
    
    //Private methods

}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MapGeneration;

//Data container for each piece. 
//A piece is a potential choice for a slot while it collapses

public class Piece
{

    //Public Members
    //Prefab
    public GameObject prefab;

    //prefab location
    public string prefabPath;

    //Valid Neighbor list
    public int[,] validNeighbors;

    //name
    public string name;

    //Private Members

    //Constructors
    public Piece()
    {

    }
    
    public Piece(string n, GameObject p, int[,] v)
    {
        this.name = n;
        this.prefab = p;
        this.validNeighbors = v;
    }

    public Piece(string n, string pp, int [,] v)
    {
        this.name = n;
        this.prefabPath = pp;
        this.validNeighbors = v;
    }

    public Piece(Piece copy)
    {
        this.name = copy.name;
        this.prefab = copy.prefab;
        this.validNeighbors = copy.validNeighbors;
    }

    //Public methods
    public bool LoadPrefab()
    {
        if(String.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Null or empty prefab path when attempting to load prefab in piece: " + this.name);
            return false;
        }

        this.prefab = Resources.Load(prefabPath, typeof(GameObject)) as GameObject;
        if(this.prefab == null)
        {
            Debug.LogError("Unable to load prefab from path " + prefabPath + " for piece " + this.name);
            return false;
        }

        return true;
    }
    
    //Private methods

}

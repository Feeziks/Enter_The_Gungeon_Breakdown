using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using MapGeneration;

//A Slot is a position within the map that will contain a piece of the map/room
public class Slot
{
    //This slots position in world space
    public Vector2Int position;

    //List of possible pieces that can exist in this slot
    public PieceSet pieceSet;

    //The actual piece that exists within this slot
    public Piece piece;

    //The gameobject for this slot
    public GameObject go;

    //Check if this slot has already been collapsed (which means that the piece that will fill in this slot has already been determined)
    public bool IsCollapsed()
    {
        return this.piece != null;
    }

    //Check if the slot has already been collapsed and we have already assigned the game object to it
    public bool IsConstructed()
    {
        return this.go != null;
    }

    //Class constructor
    public Slot(Vector2Int position)
    {
        this.position = position;
        this.pieceSet = new PieceSet();
    }

    //Get this slots neighbors in all 8 directions
    public Slot GetNeighbor(Direction dir)
    {
        //TODO figure out how to get a slots neighbor
        return null;
    }

    //Collapse this slot to a specific piece, meaning, determine the piece that will be chosen to be placed here
    public void Collapse(Piece piece)
    {
        if(this.IsCollapsed())
        {
            Debug.Log("Attempting to collapse the slot at " + this.position + " which has already been collapsed!");
            throw new Exception("Slot is already collapsed");
        }

        this.piece = piece;

        //Empty the piece set since we have decided what piece this will be
        this.pieceSet.Clear();
    }

    //Collapse to a random piece, use this if our slot happens to be the first slot chosen (Or maybe other instances too? unsure)
    public void CollapseRandom()
    {
        if(!this.pieceSet.Any())
        {
            Debug.Log("Attempted to collapse slot at " + this.position + " with an empty piece set!");
            throw new Exception("Slot cannot collapse with empty piece set");
        }

        if(this.IsCollapsed())
        {
            Debug.Log("Attempting to collapse the slot at " + this.position + " which has already been collapsed!");
            throw new Exception("Slot is already collapsed");
        }

        //TODO: Randomly select a piece from the piece set
    }

    //Remove a piece from the slots piece set. This occurs when another slot has been collapsed and the options available to this slot are changed
    public void RemovePieces(PieceSet toRemove)
    {
        PieceSet removable = toRemove.Intersect(this.pieceSet);
        this.pieceSet.Remove(removable);

        if(this.pieceSet.Empty)
        {
            Debug.Log("Empty piece set after remove at slot in position " + this.position + "!");
            throw new Exception("Slot cannot collapse with empty piece set");
        }
    }

}

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
    public List<Piece> possiblePieces;

    //List of the valid pieces than can exist in this slot
    public List<Piece> validPieces;

    //The actual piece that exists within this slot
    public Piece piece;

    //The gameobject for this slot
    public GameObject go;

    private bool active;

    //Check if this slot has already been collapsed (which means that the piece that will fill in this slot has already been determined)
    public bool IsCollapsed()
    {
        return piece != null;
    }

    //Check if the slot has already been collapsed and we have already assigned the game object to it
    public bool IsConstructed()
    {
        return go != null;
    }

    //Class constructor
    public Slot(Vector2Int p, List<Piece> pieceSet)
    {
        position = p;
        active = false;
        possiblePieces = pieceSet;
        validPieces = new List<Piece>(); //Empty list to start, until set as active slot
    }

    public void SetActive()
    {
        active = true;
        validPieces = new List<Piece>(possiblePieces);
    }

    public void SetInactive()
    {
        active = false;
        validPieces.Clear();
    }

    public bool GetActive()
    {
        return active;
    }

    //Collapse this slot to a specific piece, meaning, determine the piece that will be chosen to be placed here
    public void Collapse(Piece p)
    {
        if(IsCollapsed())
        {
            Debug.Log("Attempting to collapse the slot at " + position + " which has already been collapsed!");
            throw new Exception("Slot is already collapsed");
        }

        piece = p;

        Debug.Log("Collapsed slot at position : " + position + " to piece: " + piece.name);

        //Empty the piece set since we have decided what piece this will be
        validPieces.Clear();
        validPieces.Add(piece);
    }

    //Collapse to a random piece, use this if our slot happens to be the first slot chosen (Or maybe other instances too? unsure)
    public void CollapseRandom()
    {
        if(!validPieces.Any())
        {
            Debug.Log("Attempted to collapse slot at " + position + " with an empty piece set!");
            throw new Exception("Slot cannot collapse with empty piece set");
        }

        if(IsCollapsed())
        {
            Debug.Log("Attempting to randomly collapse the slot at " + position + " which has already been collapsed!");
            throw new Exception("Slot is already collapsed");
        }

        int idx = (int)Randomness.Instance.RandomUniformFloat(0, validPieces.Count);
        piece = validPieces[idx];
        if(piece == null)
        {
            Debug.Log("Attempted to randomly choose possiblePiece " + idx + " which was a null piece!");
            throw new Exception("Slot cannot collapse to null piece");
        }

        Debug.Log("Randomly Collapsed slot at position : " + position + " to piece: " + piece.name);

        //Clear the valid piece list since we already picked our own piece
        validPieces.Clear();
        validPieces.Add(piece);
    }

    public bool Construct()
    {
        if(piece == null)
        {
            Debug.LogError("Attempted to construct a slot with null piece!");
            return false;
        }

        //Set the gameobject to the prefab for the piece
        float pieceWidth = piece.prefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float pieceHeight = piece.prefab.GetComponent<SpriteRenderer>().bounds.size.y;
        go = UnityEngine.Object.Instantiate(piece.prefab, new Vector3(position.x * pieceWidth, position.y * pieceHeight, 0), Quaternion.identity);

        return go != null;
    }

    public int GetEntropy()
    {
        if(active && !IsCollapsed())
            return validPieces.Count;
        
        return -1;
    }

    public bool Constrain(List<Piece> newValidPieces)
    {
        bool success = true;
        //First check if any of the passed pieces exist within our current set of pieces
        //https://stackoverflow.com/questions/11092930/check-if-listt-contains-any-of-another-list
        success &= validPieces.Any(x => newValidPieces.Any(y => y == x));

        //If none of those pieces exist in our current set then we are over constrained
        //If that occurs we need to restart the WFC on this room. We could add some kind of "undo" feature here to improve performance
        if(!success)
            return false;

        //Otherwise constrain the remaining valid pieces to the intersection between the two lists
        //validPieces = (List<Piece>) validPieces.AsQueryable().Intersect(newValidPieces);
        validPieces = validPieces.Intersect(newValidPieces).ToList();

        //If there is only a single valid piece remaining, collapse to that piece
        if(validPieces.Count == 1 && !IsCollapsed())
        {
            Collapse(validPieces[0]);
        }

        return success;

    }

}

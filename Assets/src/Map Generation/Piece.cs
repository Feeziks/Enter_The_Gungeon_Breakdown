using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapGeneration;

public class Piece
{
    //Public members
    
    //TODO: Need someway to contain the data for valid neighbors
    //public SomeClassOrSomething validNeighbors[][]; //will be an 8 x number of prototypes array so this could be quite mem heavy
    //hopefully we can store that information in a smaller/more compact method like an int that is an index into the array of prototypes

    //TODO: Need someway to contain the valid options for this piece
    //public SomeClassOrSomething validPieces[]; //will be an array(list?) of number of prototypes. remove / add options as WFC
    //iterates over the slots

    //Private members
    private GameObject prefab;

    //Constructor
    public Piece()
    {

    }

    //Public methods

    //Check if another Piece fits in a specific direction
    public bool Fits(Direction dir, Piece piece)
    {
        //TODO: Actually check what does and does not fit as a neighbor
        return true;
    }

    //private methods

}

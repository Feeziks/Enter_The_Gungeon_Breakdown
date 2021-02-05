using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{

    //public members

    //private members
    private static Slot[,] roomSlots;

    #region SINGLETON
    RoomGenerator()
    {

    }
    private static readonly object padlock = new object();
    private static RoomGenerator instance = null;

    public static RoomGenerator Instance
    {
        get
        {   
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new RoomGenerator();
                }
                return instance;
            }
        }
    }
    #endregion

    //public methods
    public Room GenerateRoom(int level, int difficulty)
    {
        //Generate a new room based on the given level and difficulty

        // First create the outline of the room - this will be some number of slots in some shape

        // determine the number of slots by the current level and difficulty
        int numSlots = GetNumSlots(level, difficulty);

        //Determine which piece set to use
        int pieceSetIdx = (int)Randomness.Instance.RandomUniformFloat(0, MapGeneration.Globals.NUM_FLOOR_TYPES);
        List<Piece> pieceSet = null;

        switch(pieceSetIdx)
        {
            case 0: //Test Pieces 
                pieceSet = new List<Piece>(TestPieces.all_TestPieces_pieces);
                break;
            case 1: //Forest Pieces
                pieceSet = new List<Piece>(ForestPieces.all_ForestPieces_pieces);
                break;
            default: //Error
                Debug.LogError("This shouldnt happen");
                break;

        }

        //Create the slots
        roomSlots = new Slot[numSlots, numSlots];
        for(int x = 0; x < numSlots; x++)
        {
            for(int y = 0; y < numSlots; y++)
            {
                roomSlots[x, y] = new Slot(new Vector2Int(x, y), pieceSet);
            }
        }

        // determine which slots will be "active" and which will be "inactive"
        DetermineSlotActivity(numSlots);

        //Next we collapse the active slots following the WFC algo
        WaveFunctionCollapse.SetSlots(ref roomSlots);

        while(!WaveFunctionCollapse.IsCollapsed())
        {
            WaveFunctionCollapse.Iterate();
        }

        return null;
    }

    //private methods
    private int GetNumSlots(int level, int difficulty)
    {
        int numSlots = (int)Mathf.Max(9, level * (difficulty + 1));
        numSlots = (int)Mathf.Sqrt(numSlots);
        return numSlots;
    }

    private void DetermineSlotActivity(int numSlots)
    {
        int numActiveSlots = 0;
        List<Vector2Int> activeSlots = new List<Vector2Int>();

        //Randomly select a starting point in the array
        int startX = (int)Randomness.Instance.RandomUniformFloat(1, numSlots);
        int startY = (int)Randomness.Instance.RandomUniformFloat(1, numSlots);

        roomSlots[startX, startY].active = true;
        activeSlots.Add(new Vector2Int(startX, startY));
        numActiveSlots++;

        Vector2Int thisSlot;

        while(numActiveSlots != numSlots)
        {
            //Randomly "walk" from an active edge slot and activate that new slot

            //Find an active slot on the edge
            do
            {
                int index = (int)Randomness.Instance.RandomUniformFloat(0, activeSlots.Count);
                thisSlot = activeSlots[index];
            }while(!IsSlotActiveEdge(thisSlot.x, thisSlot.y, numSlots));

            //Randomly choose a direction to move
            int xfloor = thisSlot.x == 0 ? 0 : -1;
            int xceil = thisSlot.x == (numSlots - 1) ? 0 : 1;

            int yfloor = thisSlot.y == 0 ? 0 : -1;
            int yceil = thisSlot.y == (numSlots -1) ? 0 : 1;

            int x = 0;
            int y = 0;
            Vector2Int nextSlot = new Vector2Int(0, 0);

            do{
                x = (int)Randomness.Instance.RandomUniformFloat(xfloor, xceil);
                y = (int)Randomness.Instance.RandomUniformFloat(yfloor, yceil);
                nextSlot = new Vector2Int(thisSlot.x + x, thisSlot.y + y);
            }while(roomSlots[nextSlot.x, nextSlot.y].active);

            roomSlots[nextSlot.x, nextSlot.y].active = true;
            activeSlots.Add(nextSlot);
            numActiveSlots++;
        }
        
    }

    private bool IsSlotActiveEdge(int thisSlotx, int thisSloty, int numSlots)
    {
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                    continue;

                if(thisSlotx + x < 0 || thisSlotx + x >= numSlots)
                    continue;
                
                if(thisSloty + y < 0 || thisSloty + y >= numSlots)
                    continue;

                if(roomSlots[thisSlotx + x, thisSloty + y].active == false) //If there is at least 1 space adjacent to this piece that is inactive then we are on the edge
                    return true;
            }
        }

        return false;
    }

    private void PrintSlots(int numSlots)
    {
        //Debugging tool to print out active and inactive slots
        string toPrint = "\n";
        for(int y = 0; y < numSlots; y++)
        {
            for(int x = 0; x < numSlots; x++)
            {
                toPrint += roomSlots[x,y].active + "\t";
            }
            toPrint += "\n";
        }

        Debug.Log(toPrint);
    }
}

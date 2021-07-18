using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
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
    public Room GenerateRoom(int level, int difficulty, List<Piece> pieceSet)
    {
        Room toReturn = new Room(1, "test name");

        //Generate a new room based on the given level and difficulty
        // First create the outline of the room - this will be some number of slots in some shape
        // determine the number of slots by the current level and difficulty
        int numSlots = GetNumSlots(level, difficulty);

        //Create the slots
        roomSlots = new Slot[numSlots, numSlots];
        ResetSlots(numSlots, pieceSet);

        // determine which slots will be "active" and which will be "inactive"
        DetermineSlotActivity(numSlots);

        //Next we collapse the active slots following the WFC algo
        WaveFunctionCollapse.SetSlots(ref roomSlots, pieceSet);

        while(!WaveFunctionCollapse.IsCollapsed())
        {
            PrintSlots(numSlots, true);
            WaveFunctionCollapse.Iterate();

            if(WaveFunctionCollapse.Failed())
            {
                //reset the slots
                ResetSlots(numSlots, pieceSet);
                DetermineSlotActivity(numSlots);
                //Set the WFC slots again to restart the process
                WaveFunctionCollapse.SetSlots(ref roomSlots, pieceSet);
            }
        }

        //Construct the active slots
        ConstructRoom(numSlots);

        //The wave function is complete and our room is ready
        //Add the active slots with prefabs instantiated into the room
        AddCollapsedSlotsToRoom(ref toReturn);

        return toReturn;
    }

    //private methods
    private void ResetSlots(int numSlots, List<Piece> pieceSet)
    {
        for(int x = 0; x < numSlots; x++)
        {
            for(int y = 0; y < numSlots; y++)
            {
                roomSlots[x, y] = new Slot(new Vector2Int(x, y), pieceSet);
            }
        }
    }

    private int GetNumSlots(int level, int difficulty)
    {
        int numSlots = (int)Mathf.Max(9, level * (difficulty + 1));
        numSlots = (int)Mathf.Sqrt(numSlots);

        //TODO: Reset this
        numSlots = 3;
        return numSlots;
    }

    private void DetermineSlotActivity(int numSlots)
    {
        /*
        int numActiveSlots = 0;
        List<Vector2Int> activeSlots = new List<Vector2Int>();

        //Randomly select a starting point in the array
        int startX = (int)Randomness.Instance.RandomUniformFloat(1, numSlots);
        int startY = (int)Randomness.Instance.RandomUniformFloat(1, numSlots);

        roomSlots[startX, startY].SetActive();
        activeSlots.Add(new Vector2Int(startX, startY));
        numActiveSlots++;

        Vector2Int thisSlot;

        while(numActiveSlots != (int) (numSlots * numSlots) * (3/4))
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
            }while(roomSlots[nextSlot.x, nextSlot.y].GetActive());

            roomSlots[nextSlot.x, nextSlot.y].SetActive();
            activeSlots.Add(nextSlot);
            numActiveSlots++;
        }
        */
        for(int x = 0; x < numSlots; x++)
        {
            for(int y = 0; y < numSlots; y++)
            {
                roomSlots[x, y].SetActive();
            }
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

                if(roomSlots[thisSlotx + x, thisSloty + y].GetActive() == false) //If there is at least 1 space adjacent to this piece that is inactive then we are on the edge
                    return true;
            }
        }

        return false;
    }

    private void AddCollapsedSlotsToRoom(ref Room r)
    {
        for(int x = 0; x < roomSlots.GetLength(0); x++)
        {
            for(int y = 0; y < roomSlots.GetLength(1); y++)
            {
                if(roomSlots[x,y].IsCollapsed())
                {
                    roomSlots[x,y].go.transform.parent = r.Container.transform;
                }
            }
        }
    }

    private void ConstructRoom(int numSlots)
    {
        bool test = false;

        for (int i = 0; i < numSlots; i++)
        {
            for(int j = 0; j < numSlots; j++)
            {
                if(roomSlots[i,j].GetActive())
                {
                    test = roomSlots[i,j].Construct();
                    if(test == false)
                    {
                        Debug.LogError("Could not construct the slot at position: " + i + ", " + j);
                    }
                }
            }
        }
    }

    private void PrintSlots(int numSlots, bool toFile = false)
    {
        //Debugging tool to print out active and inactive slots
        string toPrint = "\n";
        string toWrite = "\n";
        string logFilePath = "";

        if(toFile)
        {
            logFilePath = "Logs\\MapGeneration_" + ".txt";// + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ".txt";
            if(!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "Start");
            }
        }

        for(int y = 0; y < numSlots; y++)
        {
            for(int x = 0; x < numSlots; x++)
            {
                toPrint += roomSlots[x,y].GetActive() + "\t";
                
                if(toFile)
                {
                    toWrite += "position: " + roomSlots[x,y].position;
                    toWrite += "\tActive: " + roomSlots[x,y].GetActive();

                    if(roomSlots[x,y].piece != null)
                    {
                        toWrite += "\tPiece: " + roomSlots[x,y].piece.name;    
                    }

                    toWrite += "\tValid Pieces: ";
                    if(roomSlots[x,y].validPieces.Any())
                    {
                        for(int i = 0; i < roomSlots[x,y].validPieces.Count; i++)
                        {
                            toWrite += roomSlots[x,y].validPieces[i].name + ", ";
                        }
                    }
                    else
                    {
                        toWrite += "None";
                    }

                    toWrite += "\n";
                }
            }
            
            toPrint += "\n";
        }

        Debug.Log(toPrint);
        if(toFile)
        {
            File.AppendAllText(logFilePath, toWrite);
        }
    }
}

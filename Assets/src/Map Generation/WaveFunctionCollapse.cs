using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class WaveFunctionCollapse
{

    //public members

    //private members
    private static Slot[,] m_slots;
    private static bool collapsed;
    private static List<Piece> pieceSet;

    //public methods
    public static void SetSlots(ref Slot[,] s, List<Piece> ps)
    {
        m_slots = s;
        collapsed = false;
        pieceSet = ps;
    }

    public static void Iterate()
    {
        //Do things
        //Get the minimum entropy slot
        Vector2Int minEntropy = GetMinimumEntropy();
        //Collapse that slot
        m_slots[minEntropy.x, minEntropy.y].CollapseRandom();
        //Propagate the collapse through the remaining slots
        Propagate(minEntropy);
    }

    public static bool IsCollapsed()
    {
        return collapsed;
    }

    //Private methods
    private static Vector2Int GetMinimumEntropy()
    {
        int min = 1000000000;
        Vector2Int ret = new Vector2Int(-1, -1);
        for(int i = 0; i < m_slots.GetLength(0); i++)
        {
            for(int j = 0; j < m_slots.GetLength(1); j++)
            {
                int temp = m_slots[i,j].GetEntropy();

                if(temp < min && temp != -1)
                {
                    min = temp;
                    ret = new Vector2Int(i, j);
                }
            }
        }

        return ret;
    }

    private static Vector2Int[] GetNeighbors(Vector2Int thisSlotCoords)
    {
        //Get all valid neighbors for this slot, return them in a way that you can access with the directions enum
        Vector2Int[] neighbors = new Vector2Int[(int)MapGeneration.Direction.DIRECTION_LENGTH];
        int neighborsIdx = 5;

        Vector2Int[] neighborOffsets = new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(1, -1),
                                       new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1),
                                       new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1) };

        //Get the valid neighbors for the pieces next to our starting slot
        for(int i = 0; i < neighborOffsets.Length; i++)
        {
            int x = neighborOffsets[i].x;
            int y = neighborOffsets[i].y;

            neighbors[i] = new Vector2Int(-1, -1);

            if(thisSlotCoords.x + x < 0 || thisSlotCoords.x + x >= m_slots.GetLength(0)) //Do not check for neighbors that dont exist
                continue;
            
            if(thisSlotCoords.y + y < 0 || thisSlotCoords.y + y >= m_slots.GetLength(1)) //Do not check for neighbors that dont exist
                continue;

            if(!m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].GetActive()) //Do not check inactive slots
                continue;
            

            if(m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].IsCollapsed()) //Skip if the slot is already collapsed
                continue;

            //Add this valid neighboring slot into our return array
            neighbors[i] = new Vector2Int(thisSlotCoords.x + x, thisSlotCoords.y + y);
        }
        
        return neighbors;
    }

    private static void Propagate(Vector2Int startingSlot)
    {
        Stack myStack = new Stack();
        myStack.Push(startingSlot);

        string logFilePath = "Logs\\Propagation.txt";
        File.WriteAllText(logFilePath, "");

        while(myStack.Count != 0)
        {
            Vector2Int thisSlotCoords = (Vector2Int)myStack.Pop();

            File.AppendAllText(logFilePath, "Propagating from slot at position : " + thisSlotCoords.x + ", " + thisSlotCoords.y + "\n");

            //Get all the neighbors
            Vector2Int[] neighbors = GetNeighbors(thisSlotCoords);

            //Iterate through the list of valid neighbors
            for(int neighborIdx = 0; neighborIdx < neighbors.Length; neighborIdx++)
            {
                //Is this neighbor valid?
                if(neighbors[neighborIdx] != new Vector2Int(-1, -1))
                {
                    //For each valid neighbor in our list, constrain its list of valid neighbors to the list of valid neighbors for this slot

                    //TODO: Create a list that includes all the valid neighbors for each possible piece at this location?
                    //First get the list of valid neighbors for the current piece(s) in the current direction
                    List<Piece> theseValidNeighbors = new List<Piece>();

                    for(int validPieceThisSlot = 0; validPieceThisSlot < m_slots[thisSlotCoords.x, thisSlotCoords.y].validPieces.Count; validPieceThisSlot++)
                    {
                        //Get the valid piece for this iteration through this slots valid pieces
                        Piece thisValidPiece = m_slots[thisSlotCoords.x, thisSlotCoords.y].validPieces[validPieceThisSlot];

                        //Get the valid neighbors in the current direction for this valid piece
                        for(int i = 0; i < thisValidPiece.validNeighbors.GetLength(1); i++)
                        {
                            //Get the index into the piece set list for the neighbor
                            int thisValidPieceNeighborIdx = thisValidPiece.validNeighbors[neighborIdx, i];
                            //Check if that neighbor is valid or not
                            if(thisValidPieceNeighborIdx == -1)
                                continue;

                            //Get the valid piece from the piece set
                            Piece thisValidNeighbor = pieceSet[thisValidPieceNeighborIdx];
                            //Check if that piece is not already in our list to constrain to
                            if(!theseValidNeighbors.Contains(thisValidNeighbor))
                            {
                                //Add that piece to the list
                                theseValidNeighbors.Add(thisValidNeighbor);
                            }
                        }
                    }

                    //Constrain the neighbor to the pieces that are valid to it
                    bool pass = m_slots[neighbors[neighborIdx].x, neighbors[neighborIdx].y].Constrain(theseValidNeighbors);

                    //if we have over constrained a slot we cannot continue
                    if(!pass)
                    {
                        Debug.LogError("Over constrained the slot " + neighbors[neighborIdx] + " while attempting to propagate slot " + thisSlotCoords);
                    }

                    //Add this neighbor to the stack if it isnt there already
                    File.AppendAllText(logFilePath, "Adding " + neighbors[neighborIdx] + " to the stack.\n");

                    if(!myStack.Contains(neighbors[neighborIdx]))
                    {
                        myStack.Push(neighbors[neighborIdx]);
                    }

                    File.AppendAllText(logFilePath, "Current stack contents: ");
                    foreach(Vector2Int obj in myStack)
                    {
                        File.AppendAllText(logFilePath, "(" + obj.x + ", " + obj.y + ") ");
                    }

                }
            }
            
        }


        collapsed = true;
    }

}

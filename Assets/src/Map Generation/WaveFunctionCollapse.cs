using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveFunctionCollapse
{

    //public members

    //private members
    private static Slot[,] m_slots;
    private static bool collapsed;

    //public methods
    public static void SetSlots(ref Slot[,] s)
    {
        m_slots = s;
        collapsed = false;
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

        //init the neighbors array
        for(int i = 0; i < (int)MapGeneration.Direction.DIRECTION_LENGTH; i++)
        {
            neighbors[i] = new Vector2Int(-1, -1);
        }

        //Get the valid neighbors for the pieces next to our starting slot
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) //Skip when looking at ourselves
                    continue;

                if(thisSlotCoords.x + x < 0 || thisSlotCoords.x + x >= m_slots.GetLength(0)) //Do not check for neighbors that dont exist
                    continue;
                
                if(thisSlotCoords.y + y < 0 || thisSlotCoords.y + y >= m_slots.GetLength(1)) //Do not check for neighbors that dont exist
                    continue;

                if(!m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].active) //Do not check inactive slots
                    continue;

                if(m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].IsCollapsed()) //Skip if the slot is already collapsed
                    continue;

                //Add this valid neighboring slot into our return array
                neighbors[neighborsIdx % (int)MapGeneration.Direction.DIRECTION_LENGTH] = new Vector2Int(thisSlotCoords.x + x, thisSlotCoords.y + y);
                neighborsIdx++;
            }

        }
        
        return neighbors;
    }

    private static void Propagate(Vector2Int startingSlot)
    {
        Debug.Log("Propagating");

        Stack myStack = new Stack();
        myStack.Push(startingSlot);

        while(myStack.Count != 0)
        {
            Vector2Int thisSlotCoords = (Vector2Int)myStack.Pop();

            //Get all the neighbors
            Vector2Int[] neighbors = GetNeighbors(thisSlotCoords);

            //Now begin updating the neighbors, constrain their possible pieces list to the valid neighbors list of our current piece
            for(int i = 0; i < (int)MapGeneration.Direction.DIRECTION_LENGTH; i++)
            {
                //Check for neighbor validity
                if(neighbors[i] != new Vector2Int(-1, -1))
                {
                    //Constrain the neighbors to the valid neighbor list for our collapsed slot
                    //Get the valid neighbor list for this piece in this direction
                    List<Piece> theseValidNeighbors = new List<Piece>();
                    for(int j = 0; j < m_slots[thisSlotCoords.x, thisSlotCoords.y].possiblePieces.Count; j++)
                    {
                        //Get the valid neighbors for the piece?
                        if(m_slots[thisSlotCoords.x, thisSlotCoords.y].piece.validNeighbors[i, j] != -1)
                        {
                            theseValidNeighbors.Add(m_slots[thisSlotCoords.x, thisSlotCoords.y].possiblePieces[j]);
                        }
                    }

                    bool pass = m_slots[neighbors[i].x, neighbors[i].y].Constrain(theseValidNeighbors);

                    //Add this to the stack if its not already there
                    if(!myStack.Contains(neighbors[i]))
                        myStack.Push(neighbors[i]);
                }
            }
        }


        collapsed = true;
    }

}

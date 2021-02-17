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
    private static int[,] propCounts;

    private static int MAX_PROP_COUNT = 3;

    private static Vector2Int invalidNeighbor = new Vector2Int(-1, -1);
    private static Vector2Int inactiveNeighbor = new Vector2Int(-2, -2);
    private static Vector2Int collapsedNeighbor = new Vector2Int(-3, -3);

    //public methods
    public static void SetSlots(ref Slot[,] s, List<Piece> ps)
    {
        m_slots = s;
        collapsed = false;
        pieceSet = ps;

        propCounts = new int[m_slots.GetLength(0), m_slots.GetLength(1)];
        ResetPropCounts();
        
        //Enforce Active "edge" constraints
        EnforceEdgeConstraints();
    }

    public static void Iterate()
    {
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
    private static void ResetPropCounts()
    {
        for(int x = 0; x < m_slots.GetLength(0); x++)
        {
            for(int y = 0; y < m_slots.GetLength(1); y++)
            {
                propCounts[x,y] = 0;
            }
        }
    }

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

        Vector2Int[] neighborOffsets = new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(1, -1),
                                       new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1),
                                       new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1) };

        //Get the valid neighbors for the pieces next to our starting slot
        for(int i = 0; i < neighborOffsets.Length; i++)
        {
            int x = neighborOffsets[i].x;
            int y = neighborOffsets[i].y;

            neighbors[i] = invalidNeighbor;

            if(thisSlotCoords.x + x < 0 || thisSlotCoords.x + x >= m_slots.GetLength(0)) //Do not check for neighbors that dont exist
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }
            
            if(thisSlotCoords.y + y < 0 || thisSlotCoords.y + y >= m_slots.GetLength(1)) //Do not check for neighbors that dont exist
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }

            if(!m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].GetActive()) //Do not check inactive slots
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }
            
            if(m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].IsCollapsed()) //Skip if the slot is already collapsed
            {
                neighbors[i] = collapsedNeighbor;
                continue;
            }

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

            propCounts[thisSlotCoords.x, thisSlotCoords.y]++;

            File.AppendAllText(logFilePath, "Propagating from slot at position : " + thisSlotCoords.x + ", " + thisSlotCoords.y + "\n");

            //Get all the neighbors
            Vector2Int[] neighbors = GetNeighbors(thisSlotCoords);

            bool success = EnforceConstraints(thisSlotCoords, ref neighbors, ref myStack);            
            if(!success)
            {
                //Restart WFC
            }
        }

        collapsed = CheckCollapsed();
    }

    private static bool EnforceConstraints(Vector2Int slotCoords, ref Vector2Int[] neighbors, ref Stack stack)
    {   
        string logFilePath = "Logs\\Propagation.txt";
        //Check if any of our neighbors are inactive
        List<Piece> inactiveConstraints = new List<Piece>();
        for(int i = 0; i < neighbors.Length; i++)
        {
            //If they are inactive we need to constrain our current slot to ensure that we only use a piece 
            //that can border an inactive one
            if(neighbors[i] == inactiveNeighbor)
            {
                Piece inactivePiece = pieceSet[pieceSet.Count - 1];
                for(int j = 0; j < inactivePiece.validNeighbors.GetLength(1); j++)
                {
                    Piece thisInactiveConstraint;
                    //Get the list of allowable neighbors from the inactive piece's piece set in the opposite direction
                    //I.E. N -> S, E -> W etc
                    //inverse direction is equal to direction + 1/2direction length % direction length

                    if(inactivePiece.validNeighbors[(i + ((int)MapGeneration.Direction.DIRECTION_LENGTH / 2)) % (int)MapGeneration.Direction.DIRECTION_LENGTH, j] != -1)
                    {
                        thisInactiveConstraint = pieceSet[j];
                        if(!inactiveConstraints.Contains(thisInactiveConstraint))
                            inactiveConstraints.Add(thisInactiveConstraint);
                    }
                }

            }
        }

        File.AppendAllText(logFilePath, "Valid pieces for slot: " + slotCoords.x + ", " + slotCoords.y + " Before inactive constraints");
        for(int i = 0; i < m_slots[slotCoords.x, slotCoords.y].validPieces.Count; i++)
        {
            File.AppendAllText(logFilePath, m_slots[slotCoords.x, slotCoords.y].validPieces[i].name + " ");
        }
        File.AppendAllText(logFilePath, "\n");

        //Constrain our current piece based on the inactive piece constraints we just set above
        bool success = m_slots[slotCoords.x, slotCoords.y].Constrain(inactiveConstraints);
        if(!success)
        {
            Debug.LogError("Overconstrained the slot at position: " + slotCoords);
            //Somehow restart our WFC algo
            return false;
        }

        File.AppendAllText(logFilePath, "Valid pieces for slot: " + slotCoords.x + ", " + slotCoords.y + " After inactive constraints");
        for(int i = 0; i < m_slots[slotCoords.x, slotCoords.y].validPieces.Count; i++)
        {
            File.AppendAllText(logFilePath, m_slots[slotCoords.x, slotCoords.y].validPieces[i].name + " ");
        }
        File.AppendAllText(logFilePath, "\n");


        //Now we constrain all the active and non-collapsed neighbors
        for(int i = 0; i < neighbors.Length; i++)
        {
            List<Piece> activeConstraints = new List<Piece>();
            if(neighbors[i] != inactiveNeighbor && neighbors[i] != invalidNeighbor && neighbors[i] != collapsedNeighbor)
            {
                //Constrain the neighbor to the list of valid neighbors from our current piece(s) in that neighbors direction
                Piece thisActiveConstraint;
                for(int j = 0; j < m_slots[slotCoords.x, slotCoords.y].validPieces.Count; j++)
                {
                    Piece thisValidPiece = m_slots[slotCoords.x, slotCoords.y].validPieces[j];
                    for(int k = 0; k < thisValidPiece.validNeighbors.GetLength(1); k++)
                    {
                        if(thisValidPiece.validNeighbors[i, k] != -1)
                        {
                            thisActiveConstraint = pieceSet[j];
                            if(!activeConstraints.Contains(thisActiveConstraint))
                            {
                                activeConstraints.Add(thisActiveConstraint);
                            }
                        }
                    }
                }

                //Constrain the neighboring slot
                success = m_slots[neighbors[i].x, neighbors[i].y].Constrain(activeConstraints);
                if(!success)
                {
                    Debug.LogError("Overconstrained the slot at position: " + slotCoords);
                    //Somehow restart our WFC algo
                    return false;
                }

                //Add that slot onto the stack
                if(!stack.Contains(neighbors[i]) && propCounts[neighbors[i].x, neighbors[i].y] != MAX_PROP_COUNT)
                    stack.Push(neighbors[i]);
            }
        }

        return true;
    }

    private static void EnforceEdgeConstraints()
    {
        //Get list of active edge slots
        List<Vector2Int> activeEdgeSlots = new List<Vector2Int>();
        for(int i = 0; i < m_slots.GetLength(0); i++)
        {
            for(int j = 0; j < m_slots.GetLength(1); j++)
            {
                //Check if this slot is active
                if(m_slots[i,j].GetActive() == false)
                {
                    //Skip if inactive
                    continue;
                }

                //Check the neighbors to see if any are inactive
                bool edge = CheckForInactiveNeighbors(new Vector2Int(i,j));
                if(edge)
                {
                    activeEdgeSlots.Add(new Vector2Int(i, j));
                }
            }
        }

        //Constrain all the active edge slots
        for(int i = 0; i < activeEdgeSlots.Count; i++)
        {
            Vector2Int[] neighbors = GetNeighbors(activeEdgeSlots[i]);
            Stack dummyStack = new Stack();
            EnforceConstraints(activeEdgeSlots[i], ref neighbors, ref dummyStack);
        }
    }

    private static bool CheckForInactiveNeighbors(Vector2Int coords)
    {
        Vector2Int[] neighborOffsets = new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(1, -1),
                                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1),
                                new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1) };

        //Get the valid neighbors for the pieces next to our starting slot
        for(int i = 0; i < neighborOffsets.Length; i++)
        {
            int x = neighborOffsets[i].x;
            int y = neighborOffsets[i].y;

            Vector2Int coordsPlusOffset = new Vector2Int(coords.x + x, coords.y + y);

            if(coordsPlusOffset.x < 0 || coordsPlusOffset.x >= m_slots.GetLength(0))
                continue;
            else if(coordsPlusOffset.y < 0 || coordsPlusOffset.y >= m_slots.GetLength(1))
                continue;
            else if(m_slots[coordsPlusOffset.x, coordsPlusOffset.y].GetActive() == false)
                return true;
        }
        return false;
    }

    private static bool CheckCollapsed()
    {
        for(int x = 0; x < m_slots.GetLength(0); x++)
        {
            for(int y = 0; y < m_slots.GetLength(1); y++)
            {
                //If any slot is NOT collapsed and is active then the function has not collapsed
                if(m_slots[x,y].IsCollapsed() == false && m_slots[x,y].GetActive())
                {
                    return false;
                }
            }
        }

        return true;
    }

}

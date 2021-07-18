using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class WaveFunctionCollapse
{

    //public members

    //private members
    private static Slot[,] m_slots;
    private static bool collapsed;
    private static List<Piece> pieceSet;
    private static int[,] propCounts;
    private static List<Vector2Int> edgeSlots;

    private static int MAX_PROP_COUNT = 3;
    private static int MAX_ITERATION_COUNT = 100;
    private static int curr_iteration_count;

    private static Vector2Int invalidNeighbor = new Vector2Int(-1, -1);
    private static Vector2Int inactiveNeighbor = new Vector2Int(-2, -2);
    private static Vector2Int collapsedNeighbor = new Vector2Int(-3, -3);

    private static bool failure_status;

    //public methods
    public static void SetSlots(ref Slot[,] s, List<Piece> ps)
    {
        m_slots = s;
        collapsed = false;
        pieceSet = ps;

        propCounts = new int[m_slots.GetLength(0), m_slots.GetLength(1)];
        ResetPropCounts();

        curr_iteration_count = 0;

        edgeSlots = new List<Vector2Int>();
        ResetEdgeSlots();
        
        //Enforce Active "edge" constraints
        bool success = EnforceEdgeConstraints();
        if(!success)
        {
            Debug.LogError("Error in setting WFC slots! edges of the slots cannot be constrained properly!");
            failure_status = true;
        }

        failure_status = false;
    }

    public static void Iterate()
    {
        if(curr_iteration_count >= MAX_ITERATION_COUNT)
        {
            collapsed = true;
            return;
        }
        curr_iteration_count++;
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

    public static bool Failed()
    {
        return failure_status;
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

    private static void ResetEdgeSlots()
    {
        edgeSlots.Clear();
    }

    private static Vector2Int GetMinimumEntropy()
    {
        int min = 1000000000;
        bool min_set = false;
        Vector2Int ret = new Vector2Int(-1, -1);
        for(int i = 0; i < m_slots.GetLength(0); i++)
        {
            for(int j = 0; j < m_slots.GetLength(1); j++)
            {
                int temp = m_slots[i,j].GetEntropy();

                if(temp <= min && temp != -1)
                {
                    if(min_set)
                    {
                        if(Randomness.Instance.RandomUniformFloat(0, 1) > 0.5f)
                        {
                            min = temp;
                            ret = new Vector2Int(i, j);
                        }
                    }
                    else
                    {
                        min = temp;
                        ret = new Vector2Int(i, j);
                        min_set = true;
                    }
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

            if(thisSlotCoords.x + x < 0 || thisSlotCoords.x + x >= m_slots.GetLength(0)) 
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }
            
            if(thisSlotCoords.y + y < 0 || thisSlotCoords.y + y >= m_slots.GetLength(1)) 
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }

            if(!m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].GetActive()) 
            {
                neighbors[i] = inactiveNeighbor;
                continue;
            }
            
            if(m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].IsCollapsed()) 
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
                failure_status = true;
            }
        }

        collapsed = CheckCollapsed();
    }

    private static bool EnforceConstraints(Vector2Int slotCoords, ref Vector2Int[] neighbors, ref Stack stack)
    {   
        string logFilePath = "Logs\\Propagation.txt";
        
        /*
        bool success = EnforceInactiveConstraints(slotCoords, neighbors);

        if(!success)
        {
            //Exit early if we have already failed
            failure_status = true;
            return success;
        } 
        */

        bool success = false;       

        //Now we constrain all the active and non-collapsed neighbors
        for(int i = 0; i < neighbors.Length; i++)
        {
            List<Piece> activeConstraints = new List<Piece>();
            //Inactive, Invalid, and Collapsed neighbors are all a negative value. So this should filter those out
            if(neighbors[i].x >= 0 && neighbors[i].y >= 0)
            {
                //Constrain the neighbor to the list of valid neighbors from our current piece(s) in that neighbors direction
                Piece thisActiveConstraint;
                for(int j = 0; j < m_slots[slotCoords.x, slotCoords.y].validPieces.Count - 1; j++)
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

                
                if(m_slots[neighbors[i].x, neighbors[i].y].IsCollapsed())
                {
                    if(!activeConstraints.Contains(m_slots[neighbors[i].x, neighbors[i].y].piece))
                    {
                        Debug.LogError("Overconstrained an already collapsed slot at position: " + neighbors[i]);
                        failure_status = true;
                        return false;
                    }
                }
                success = m_slots[neighbors[i].x, neighbors[i].y].Constrain(activeConstraints);
                if(!success)
                {
                    Debug.LogError("Overconstrained the slot at position: " + neighbors[i].x + ","  + neighbors[i].y);
                    failure_status = true;
                    return false;
                }

                //Add that slot onto the stack
                if(!stack.Contains(neighbors[i]) && propCounts[neighbors[i].x, neighbors[i].y] < MAX_PROP_COUNT)
                {
                    File.AppendAllText(logFilePath, "Pushing slot at position: " + neighbors[i].x + ", " + neighbors[i].y + " on to the stack\n");
                    stack.Push(neighbors[i]);
                }
            }
        }

        return true;
    }

    private static bool EnforceInactiveConstraints(Vector2Int slotCoords, Vector2Int[] neighbors)
    {
        //Ensure that we arent over constraining a slot
        if(m_slots[slotCoords.x, slotCoords.y].IsCollapsed())
        {
            return true;
        }

        //Enforce constraints on the passed slot based on its inactive neighbors
        string logFilePath = "Logs\\Propagation.txt";
        //Check if any of our neighbors are inactive
        for(int i = 0; i < (int)MapGeneration.Direction.DIRECTION_LENGTH; i++)
        {
            if(neighbors[i] != inactiveNeighbor)
            {
                continue;
            }

            List<Piece> inactiveConstraints = new List<Piece>();
            //If they are inactive we need to constrain our current slot to ensure that we only use a piece 
            //that can border an inactive one
            //Ensure that the inactive piece for each piece set is emplaced at the END of the list of all those pieces
            Piece inactivePiece = pieceSet[pieceSet.Count - 1];
            for(int j = 0; j < inactivePiece.validNeighbors.GetLength(1) - 1; j++)
            {
                Piece thisInactiveConstraint;
                //Get the list of allowable neighbors from the inactive piece's piece set in the opposite direction
                //I.E. N -> S, E -> W etc
                //inverse direction is equal to direction + 1/2direction length % direction length
                int inverseDirection = (i + ((int)MapGeneration.Direction.DIRECTION_LENGTH / 2)) % (int)MapGeneration.Direction.DIRECTION_LENGTH;
                if(inactivePiece.validNeighbors[inverseDirection, j] != -1)
                {
                    thisInactiveConstraint = pieceSet[j];
                    if(!inactiveConstraints.Contains(thisInactiveConstraint))
                        inactiveConstraints.Add(thisInactiveConstraint);
                }
            }

            //Constrain our current piece based on the inactive piece constraints we just set above
            bool success = m_slots[slotCoords.x, slotCoords.y].Constrain(inactiveConstraints);
            if(!success)
            {
                Debug.LogError("Overconstrained the slot at position: " + slotCoords);
                failure_status = true;
                return false;
            }

        }

        //Return pass
        return true;
    }

    private static bool EnforceActiveConstraints()
    {

        return true;
    }

    private static bool EnforceEdgeConstraints()
    {
        //Get list of active edge slots
        
        edgeSlots = GetEdgeSlots();

        //Constrain all the active edge slots
        for(int i = 0; i < edgeSlots.Count; i++)
        {
            Vector2Int[] neighbors = GetNeighbors(edgeSlots[i]);
            
            bool success = EnforceInactiveConstraints(edgeSlots[i], neighbors);
            if(!success)
            {
                failure_status = true;
                return false;
            }
        }

        return true;
    }

    private static List<Vector2Int> GetEdgeSlots()
    {
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
        return activeEdgeSlots;
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
                return true;
            else if(coordsPlusOffset.y < 0 || coordsPlusOffset.y >= m_slots.GetLength(1))
                return true;
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

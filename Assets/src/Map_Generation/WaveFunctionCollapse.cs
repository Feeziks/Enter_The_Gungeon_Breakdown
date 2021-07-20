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

  private static int numDirections = 8; //8 Total directions each slot needs to check for neighbors -> see values in neighborIdxToDirectionString for directions
  private static Dictionary<Vector2Int, string> neighborIdxToDirectionString = new Dictionary<Vector2Int, string>
  {
    {new Vector2Int(0,  1), "N"  },
    {new Vector2Int(1,  1), "NE" },
    {new Vector2Int(1,  0), "E"  },
    {new Vector2Int(1, -1), "SE" },
    {new Vector2Int(0, -1), "S"  },
    {new Vector2Int(-1,-1), "SW" },
    {new Vector2Int(-1, 0), "W"  },
    {new Vector2Int(-1, 1), "NW" }
  };
  private static Dictionary<string, string> inverseDirections = new Dictionary<string, string>
  {
    {"N" , "S" },
    {"NE", "SW"},
    {"E" , "W" },
    {"SE", "NW"},
    {"S" , "N" },
    {"SW", "NE"},
    {"W" , "E" },
    {"NW", "SE"},
  };

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
    if (!success)
    {
      Debug.LogError("Error in setting WFC slots! edges of the slots cannot be constrained properly!");
      failure_status = true;
    }

    failure_status = false;
  }

  public static void Iterate()
  {
    if (curr_iteration_count >= MAX_ITERATION_COUNT)
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
    for (int x = 0; x < m_slots.GetLength(0); x++)
    {
      for (int y = 0; y < m_slots.GetLength(1); y++)
      {
        propCounts[x, y] = 0;
      }
    }
  }

  private static void ResetEdgeSlots()
  {
    edgeSlots.Clear();
  }

  private static Vector2Int GetMinimumEntropy()
  {
    int min = int.MaxValue;
    bool min_set = false;
    Vector2Int ret = new Vector2Int(-1, -1);
    for (int i = 0; i < m_slots.GetLength(0); i++)
    {
      for (int j = 0; j < m_slots.GetLength(1); j++)
      {
        int temp = m_slots[i, j].GetEntropy();

        if (temp <= min && temp != -1)
        {
          if (min_set)
          {
            if (Randomness.Instance.RandomUniformFloat(0, 1) > 0.5f)
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
    Vector2Int[] neighbors = new Vector2Int[numDirections];

    Vector2Int[] neighborOffsets = neighborIdxToDirectionString.Keys.ToArray();

    //Get the valid neighbors for the pieces next to our starting slot
    for (int i = 0; i < neighborOffsets.Length; i++)
    {
      int x = neighborOffsets[i].x;
      int y = neighborOffsets[i].y;

      neighbors[i] = invalidNeighbor;

      if (thisSlotCoords.x + x < 0 || thisSlotCoords.x + x >= m_slots.GetLength(0))
      {
        neighbors[i] = inactiveNeighbor;
        continue;
      }

      if (thisSlotCoords.y + y < 0 || thisSlotCoords.y + y >= m_slots.GetLength(1))
      {
        neighbors[i] = inactiveNeighbor;
        continue;
      }

      if (!m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].GetActive())
      {
        neighbors[i] = inactiveNeighbor;
        continue;
      }

      if (m_slots[thisSlotCoords.x + x, thisSlotCoords.y + y].IsCollapsed())
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

    while (myStack.Count != 0)
    {
      Vector2Int thisSlotCoords = (Vector2Int)myStack.Pop();

      propCounts[thisSlotCoords.x, thisSlotCoords.y]++;

      File.AppendAllText(logFilePath, "Propagating from slot at position : " + thisSlotCoords.x + ", " + thisSlotCoords.y + "\n");

      //Get all the neighbors
      Vector2Int[] neighbors = GetNeighbors(thisSlotCoords);

      bool success = EnforceConstraints(thisSlotCoords, ref neighbors, ref myStack);
      if (!success)
      {
        failure_status = true;
      }
    }

    collapsed = CheckCollapsed();
  }

  private static bool EnforceConstraints(Vector2Int slotCoords, ref Vector2Int[] neighbors, ref Stack stack)
  {
    string logFilePath = "Logs\\Propagation.txt";

    bool success = false;

    /*
    success= EnforceInactiveConstraints(slotCoords, neighbors);

    if(!success)
    {
        //Exit early if we have already failed
        failure_status = true;
        return success;
    } 
    */

    success = EnforceActiveConstraints(slotCoords, neighbors, ref stack);

    if (!success)
    {
      //Exit early if we have already failed
      failure_status = true;
      return success;
    }

    //We passed both constraints successfully
    return success;
  }

  private static bool EnforceInactiveConstraints(Vector2Int slotCoords, Vector2Int[] neighbors)
  {
    //Ensure that we arent over constraining a slot
    if (m_slots[slotCoords.x, slotCoords.y].IsCollapsed())
    {
      return true;
    }

    //Enforce constraints on the passed slot based on its inactive neighbors
    string logFilePath = "Logs\\Propagation.txt";
    //Check if any of our neighbors are inactive
    for (int i = 0; i < numDirections; i++)
    {
      if (neighbors[i] != inactiveNeighbor)
      {
        continue;
      }

      List<Piece> inactiveConstraints = new List<Piece>();
      //If they are inactive we need to constrain our current slot to ensure that we only use a piece 
      //that can border an inactive one
      //Ensure that the inactive piece for each piece set is emplaced at the END of the list of all those pieces
      Piece inactivePiece = pieceSet[pieceSet.Count - 1];
      //Vector2Int offset = new Vector2Int(neighbors[i].x - slotCoords.x, neighbors[i].y - slotCoords.y);
      //string direction = neighborIdxToDirectionString[offset];
      string inverseDirection = inverseDirections[inverseDirections.Keys.ToList()[i]];
      //Constrain our piece based on the inactive pieces valid neighbors in the inverse direction (Pointing towards us)
      bool success = m_slots[slotCoords.x, slotCoords.y].Constrain(inactivePiece.validNeighbors[inverseDirection]);
      if (!success)
      {
        Debug.LogError("Overconstrained the slot at position: " + slotCoords);
        failure_status = true;
        return false;
      }

    }

    //Return pass
    return true;
  }

  private static bool EnforceActiveConstraints(Vector2Int slotCoords, Vector2Int[] neighbors, ref Stack stack)
  {
    //Loop over the passed neighbors
    //One potential optimization is to pass a list of only valid neighbors so we no longer need to check, but thats for the future

    for (int neighbor = 0; neighbor < neighbors.Length; neighbor++)
    {
      //Check if the neighbor is valid
      if (neighbors[neighbor].x < 0 || neighbors[neighbor].y < 0)
        continue; //Skip

      //Get the direction of this neighbor
      Vector2Int offset = new Vector2Int(neighbors[neighbor].x - slotCoords.x, neighbors[neighbor].y - slotCoords.y);
      string direction = neighborIdxToDirectionString[offset];
      List<Piece> validNeighborsThisDirection = new List<Piece>();

      //Loop through the potentially valid pieces in this slot and find their valid neighbors in this direction.
      //Append them to the list if they are not already there
      //We can then constrain the neighbor in this direction to that list of valid neighbors
      for (int validPiece = 0; validPiece < m_slots[slotCoords.x, slotCoords.y].validPieces.Count; validPiece++)
      {
        Piece thisPiece = m_slots[slotCoords.x, slotCoords.y].validPieces[validPiece];
        //Get valid neighbors based on the direction string from earlier
        List<Piece> thisPieceValidNeighborsThisDirection = thisPiece.validNeighbors[direction];
        foreach (Piece p in thisPieceValidNeighborsThisDirection)
        {
          if (!validNeighborsThisDirection.Contains(p))
          {
            //Build the constraining list
            validNeighborsThisDirection.Add(p);
          }
        }
      }

      //We now have the list to constrain the neighbor in this direction to, enforce it on the slot
      if (m_slots[neighbors[neighbor].x, neighbors[neighbor].y].IsCollapsed())
      {
        if (!validNeighborsThisDirection.Contains(m_slots[neighbors[neighbor].x, neighbors[neighbor].y].piece))
        {
          Debug.LogError("Overconstrained an already collapsed slot at position: " + neighbors[neighbor]);
          failure_status = true;
          return false;
        }
      }
      bool success = m_slots[neighbors[neighbor].x, neighbors[neighbor].y].Constrain(validNeighborsThisDirection);
      if (!success)
      {
        Debug.LogError("Overconstrained the slot at position: " + neighbors[neighbor].x + "," + neighbors[neighbor].y);
        failure_status = true;
        return false;
      }

      //Add that slot onto the stack
      if (!stack.Contains(neighbors[neighbor]) && propCounts[neighbors[neighbor].x, neighbors[neighbor].y] < MAX_PROP_COUNT)
      {
        stack.Push(neighbors[neighbor]);
      }
    }

    return true;
  }

  private static bool EnforceEdgeConstraints()
  {
    //Get list of active edge slots

    edgeSlots = GetEdgeSlots();

    //Constrain all the active edge slots
    for (int i = 0; i < edgeSlots.Count; i++)
    {
      Vector2Int[] neighbors = GetNeighbors(edgeSlots[i]);

      bool success = EnforceInactiveConstraints(edgeSlots[i], neighbors);
      if (!success)
      {
        failure_status = true;
        return false;
      }
    }

    Propagate(edgeSlots[0]);
    return !failure_status;
  }

  private static List<Vector2Int> GetEdgeSlots()
  {
    List<Vector2Int> activeEdgeSlots = new List<Vector2Int>();
    for (int i = 0; i < m_slots.GetLength(0); i++)
    {
      for (int j = 0; j < m_slots.GetLength(1); j++)
      {
        //Check if this slot is active
        if (m_slots[i, j].GetActive() == false)
        {
          //Skip if inactive
          continue;
        }

        //Check the neighbors to see if any are inactive
        bool edge = CheckForInactiveNeighbors(new Vector2Int(i, j));
        if (edge)
        {
          activeEdgeSlots.Add(new Vector2Int(i, j));
        }
      }
    }
    return activeEdgeSlots;
  }

  private static bool CheckForInactiveNeighbors(Vector2Int coords)
  {
    Vector2Int[] neighborOffsets = neighborIdxToDirectionString.Keys.ToArray();

    //Get the valid neighbors for the pieces next to our starting slot
    for (int i = 0; i < neighborOffsets.Length; i++)
    {
      int x = neighborOffsets[i].x;
      int y = neighborOffsets[i].y;

      Vector2Int coordsPlusOffset = new Vector2Int(coords.x + x, coords.y + y);

      if (coordsPlusOffset.x < 0 || coordsPlusOffset.x >= m_slots.GetLength(0))
        return true;
      else if (coordsPlusOffset.y < 0 || coordsPlusOffset.y >= m_slots.GetLength(1))
        return true;
      else if (m_slots[coordsPlusOffset.x, coordsPlusOffset.y].GetActive() == false)
        return true;
    }
    return false;
  }

  private static bool CheckCollapsed()
  {
    for (int x = 0; x < m_slots.GetLength(0); x++)
    {
      for (int y = 0; y < m_slots.GetLength(1); y++)
      {
        //If any slot is NOT collapsed and is active then the function has not collapsed
        if (m_slots[x, y].IsCollapsed() == false && m_slots[x, y].GetActive())
        {
          return false;
        }
      }
    }

    return true;
  }

}

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
  private static List<Piece> pieceSet;
  private static Transform parentTransform;

  private static bool collapsed;
  private static bool failed;

  private static Vector2Int inactiveNeighbor = new Vector2Int(-1, -1);
  private static Vector2Int outOfBoundsNeighbor = new Vector2Int(-2, -2);
  private static Vector2Int collapsedNeighbor = new Vector2Int(-3, -3);

  private static Vector2Int[] neighborOffsets = new Vector2Int[8]
  {
    new Vector2Int( 0,  1),
    new Vector2Int( 1,  1),
    new Vector2Int( 1,  0),
    new Vector2Int( 1, -1),
    new Vector2Int( 0, -1),
    new Vector2Int(-1, -1),
    new Vector2Int(-1,  0),
    new Vector2Int(-1,  1)
  };

  private static Dictionary<string, Vector2Int> directionToNeighborOffset = new Dictionary<string, Vector2Int>
  {
    { "N", neighborOffsets[0]},
    {"NE", neighborOffsets[1]},
    { "E", neighborOffsets[2]},
    {"SE", neighborOffsets[3]},
    { "S", neighborOffsets[4]},
    {"SW", neighborOffsets[5]},
    { "W", neighborOffsets[6]},
    {"NW", neighborOffsets[7]}
  };

  private static Dictionary<string, string> directionToInverseDirection = new Dictionary<string, string>
  {
    { "N",  "S"},
    {"NE", "SW"},
    { "E",  "W"},
    {"SE", "NW"},
    { "S",  "N"},
    {"SW", "NE"},
    { "W",  "E"},
    {"NW", "SE"}
  };

  public static bool InitSlots(int width, int height, List<Piece> ps, Transform t)
  {
    bool success = true;
    pieceSet = ps;
    parentTransform = t;
    m_slots = new Slot[width, height];

    for(int x = 0; x < width; x++)
    {
      for(int y = 0; y < height; y++)
      {
        m_slots[x, y] = new Slot(new Vector2Int(x, y), pieceSet, parentTransform);
      }
    }

    DetermineSlotActivity();
    CacheNeighbors();
    success &= ConstrainEdgeSlots();
    return success;
  }

  public static void DetermineSlotActivity()
  {
    //For now set every single slot as active
    //In the future look up algo's to determine more "natural" looking map
    foreach(Slot s in m_slots)
    { 
      s.SetActive();
    }
  }

  public static bool ConstrainEdgeSlots()
  {
    bool success = true;
    List<Vector2Int> edgeSlots = GetEdgeSlots();

    foreach(Vector2Int edge in edgeSlots)
    {
      success &= EnforceInactiveConstraints(edge);
    }

    if(!success)
    {
      Debug.Log("Error in Edge Constraints when applying constraints to edge slots!");
      return success;
    }

    //Propagate these constraints to our active neighbors
    success &= Propagate(edgeSlots);
    if(!success)
    {
      Debug.Log("Error in Edge Constraints when propagating!");
      return success;
    }

    return success;
  }

  public static bool Iterate()
  {
    bool success = true;
    Vector2Int minEntropy = GetMinimumEntropy();
    if (minEntropy.x < 0 && minEntropy.y < 0)
    {
      success = false;
      Debug.Log("Error in GetMinimumEntropy!");
      return success;
    }

    success &= m_slots[minEntropy.x, minEntropy.y].CollapseRandom();
    success &= Propagate(minEntropy);
    return success;
  }

  public static bool IsCollapsed()
  {
    return CheckCollapsedStatus();
  }

  public static Slot[,] GetSlots()
  {
    return m_slots;
  }

  private static bool EnforceInactiveConstraints(Vector2Int pos)
  {
    //Enforce inactive constraints on this slot
    //Do not propagate these effects here, that will be handled later on

    //Loop over the slots neighbors and if the slot is inactive or out of bounds
    //Add its constraints on THIS slot
    bool success = true;
    foreach(var item in m_slots[pos.x, pos.y].neighbors)
    {
      if(item.Value == inactiveNeighbor || item.Value == outOfBoundsNeighbor)
      {
        string inverseDirection = directionToInverseDirection[item.Key];
        success &= m_slots[pos.x, pos.y].Constrain(pieceSet[pieceSet.Count - 1].validNeighbors[inverseDirection]);
      }
    }
    return success;
  }

  private static bool EnforceActiveConstraints(Vector2Int pos, ref Stack<Vector2Int> myStack, List<Vector2Int> alreadyTouched)
  {
    bool success = true;
    foreach(var item in m_slots[pos.x, pos.y].neighbors)
    {
      if((item.Value.x < 0 && item.Value.y < 0) || m_slots[item.Value.x, item.Value.y].IsCollapsed())
      {
        continue; //Skip any slots that are inactive, out of bounds, or collapsed. We already constrained those slots
      }

      List<Piece> constraints = new List<Piece>();
      foreach(Piece p in m_slots[pos.x, pos.y].validPieces)
      {
        success &= m_slots[item.Value.x, item.Value.y].Constrain(p.validNeighbors[item.Key]);
      }

      if(!alreadyTouched.Contains(item.Value))
      {
        myStack.Push(item.Value);
      }
    }

    if(!success)
    {
      Debug.Log("Error in EnforceActiveConstraints!");
    }

    return success;
  }

  private static bool Propagate(Vector2Int slots)
  {
    List<Vector2Int> tempList = new List<Vector2Int>{ slots };
    return Propagate(tempList);
  }

  private static bool Propagate(List<Vector2Int> slots)
  {
    bool success = true;
    Stack<Vector2Int> myStack = new Stack<Vector2Int>();
    List<Vector2Int> alreadyTouched = new List<Vector2Int>();
    foreach(Vector2Int s in slots)
    {
      myStack.Push(s);
    }

    while(myStack.Count != 0)
    {
      Vector2Int slotPos = myStack.Pop();
      success &= EnforceActiveConstraints(slotPos, ref myStack, alreadyTouched);

      if(!alreadyTouched.Contains(slotPos))
      {
        alreadyTouched.Add(slotPos);
      }
    }

    return success;
  }

  private static List<Vector2Int> GetEdgeSlots()
  {
    List<Vector2Int> edges = new List<Vector2Int>();
    foreach (Slot s in m_slots)
    {
      foreach (var item in s.neighbors)
      {
        if (item.Value.x < 0 && item.Value.y < 0)
        {
          if (!edges.Contains(s.position))
          {
            edges.Add(s.position);
            break;
          }
        }
      }
    }

    return edges;
  }

  private static bool CheckCollapsedStatus()
  {
    foreach(Slot s in m_slots)
    {
      if(s.IsCollapsed() == false)
      {
        return false;
      }
    }
    return true;
  }

  private static Vector2Int GetMinimumEntropy()
  {
    int minimum = int.MaxValue;
    Vector2Int minSlot = inactiveNeighbor;
    foreach(Slot s in m_slots)
    {
      int thisEntropy = s.GetEntropy();
      if (thisEntropy == -1)
        continue;

      if(thisEntropy < minimum)
      {
        minimum = thisEntropy;
        minSlot = s.position;
      }

      else if(thisEntropy == minimum)
      {
        //Randomly decide between the two pieces when equal entropy
        if(Randomness.Instance.RandomUniformFloat(0, 100) > 50.0f)
        {
          minimum = thisEntropy;
          minSlot = s.position;
        }
      }
    }

    return minSlot;
  }

  private static void CacheNeighbors()
  {
    for(int x = 0; x < m_slots.GetLength(0); x++)
    {
      for(int y = 0; y < m_slots.GetLength(1); y++)
      {
        foreach(var item in directionToNeighborOffset)
        {
          Vector2Int neighborPosition = new Vector2Int(x, y) + item.Value;
          Vector2Int thisNeighbor = inactiveNeighbor;

          if(neighborPosition.x < 0 || neighborPosition.x >= m_slots.GetLength(0))
          {
            thisNeighbor = outOfBoundsNeighbor;
          }
          else if(neighborPosition.y < 0 || neighborPosition.y >= m_slots.GetLength(1))
          {
            thisNeighbor = outOfBoundsNeighbor;
          }
          else
          {
            if(m_slots[neighborPosition.x, neighborPosition.y].GetActive())
            {
              thisNeighbor = neighborPosition;
            }
            else
            {
              thisNeighbor = inactiveNeighbor;
            }
          }

          m_slots[x, y].neighbors[item.Key] = thisNeighbor;
        }
      }
    }
  }
}

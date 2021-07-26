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
      lock (padlock)
      {
        if (instance == null)
        {
          instance = new RoomGenerator();
        }
        return instance;
      }
    }
  }
  #endregion

  //public methods
  public Room GenerateRoom(int level, int difficulty, List<Piece> pieceSet, Transform mapTransform)
  {
    Room toReturn = new Room(1, mapTransform.gameObject.name + "_Room", mapTransform);

    //Generate a new room based on the given level and difficulty
    // First create the outline of the room - this will be some number of slots in some shape
    // determine the number of slots by the current level and difficulty
    int numSlots = GetNumSlots(level, difficulty);

    //Next we collapse the active slots following the WFC algo
    WaveFunctionCollapse.InitSlots(numSlots, numSlots, pieceSet, mapTransform);
    bool WFC_Success = true;
    while (!WaveFunctionCollapse.IsCollapsed())
    {
      WFC_Success &= WaveFunctionCollapse.Iterate();

      if (WFC_Success == false)
      {
        //Set the WFC slots again to restart the process
        WaveFunctionCollapse.InitSlots(numSlots, numSlots, pieceSet, mapTransform);
        WFC_Success = true;
      }
    }
    //Create the slots
    roomSlots = new Slot[numSlots, numSlots];
    roomSlots = WaveFunctionCollapse.GetSlots();
    //Construct the active slots
    ConstructRoom(numSlots);

    //The wave function is complete and our room is ready
    //Add the active slots with prefabs instantiated into the room
    AddCollapsedSlotsToRoom(ref toReturn);

    return toReturn;
  }

  //private methods

  private int GetNumSlots(int level, int difficulty)
  {
    int numSlots = (int)Mathf.Max(9, level * (difficulty + 1));
    numSlots = (int)Mathf.Sqrt(numSlots);

    //TODO: Reset this
    numSlots = 3;
    return numSlots;
  }

  private void AddCollapsedSlotsToRoom(ref Room r)
  {
    for (int x = 0; x < roomSlots.GetLength(0); x++)
    {
      for (int y = 0; y < roomSlots.GetLength(1); y++)
      {
        if (roomSlots[x, y].IsCollapsed())
        {
          roomSlots[x, y].go.transform.parent = r.container.transform;
        }
      }
    }
  }

  private void ConstructRoom(int numSlots)
  {
    bool test = false;

    for (int i = 0; i < numSlots; i++)
    {
      for (int j = 0; j < numSlots; j++)
      {
        if (roomSlots[i, j].GetActive())
        {
          test = roomSlots[i, j].Construct();
          if (test == false)
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

    if (toFile)
    {
      logFilePath = "Logs\\MapGeneration_" + ".txt";// + DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + ".txt";
      if (!File.Exists(logFilePath))
      {
        File.WriteAllText(logFilePath, "Start");
      }
    }

    for (int y = 0; y < numSlots; y++)
    {
      for (int x = 0; x < numSlots; x++)
      {
        toPrint += roomSlots[x, y].GetActive() + "\t";

        if (toFile)
        {
          toWrite += "position: " + roomSlots[x, y].position;
          toWrite += "\tActive: " + roomSlots[x, y].GetActive();

          if (roomSlots[x, y].piece != null)
          {
            toWrite += "\tPiece: " + roomSlots[x, y].piece.name;
          }

          toWrite += "\tValid Pieces: ";
          if (roomSlots[x, y].validPieces.Any())
          {
            for (int i = 0; i < roomSlots[x, y].validPieces.Count; i++)
            {
              toWrite += roomSlots[x, y].validPieces[i].name + ", ";
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
    if (toFile)
    {
      File.AppendAllText(logFilePath, toWrite);
    }
  }
}

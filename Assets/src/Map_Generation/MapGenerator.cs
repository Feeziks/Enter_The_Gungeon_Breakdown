using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
  //This class generates a map and ensures that the map is "solvable"
  //The map that is created should be affected by the following
  //  The level (how far has the player gotten)
  //  The previous level (Try to not use the same textures as the previus level)
  //  Game difficulty setting
  //  Probably some other things too

  #region SINGLETON
  MapGenerator()
  {

  }
  private static readonly object padlock = new object();
  private static MapGenerator instance = null;

  public static MapGenerator Instance
  {
    get
    {
      lock (padlock)
      {
        if (instance == null)
        {
          instance = new MapGenerator();
        }
        return instance;
      }
    }
  }
  #endregion

  //public members

  //private members
  private float[] growthRates = new float[] { 0.1618f, 0.2427f, 0.3236f };

  //public methods
  public void GenerateNewMap(int level, int difficulty, ref Map retMap)
  {
    if (retMap == null)
    {
      retMap = new Map(level + "_map");
    }

    //Get a number of points for this map
    int totalPoints = CalculateMapPoints(level, difficulty);
    int currPoints = totalPoints;

    //TODO: Remove this once testing is fleshed out
    currPoints = 1;

    //Determine which piece set to use
    int numLevels = GetNumberOfLevelTypes();
    Debug.Log(numLevels);
    int pieceSetIdx = (int)Randomness.Instance.RandomUniformFloat(0, numLevels);
    List<Piece> pieceSet = null;

    //TODO: Remove this when done with testing
    pieceSetIdx = 0;
    switch (pieceSetIdx)
    {
      case 0:
        pieceSet = MapPieces.Kamen.allKamenPieces;
        MapPieces.Kamen.Load();
        break;
      default:
        Debug.Log("Idk how this happened");
        break;
    }
    //Generate rooms until we are out of points
    do
    {
      Room thisRoom = RoomGenerator.Instance.GenerateRoom(level, difficulty, pieceSet, retMap.GetTransform());
      retMap.AddRoom(thisRoom);
      currPoints -= thisRoom.cost;
    }
    while (currPoints > 0);
  }

  //private methods
  private int CalculateMapPoints(int level, int difficulty)
  {
    //More points means a harder/bigger map
    //Current thoughts are to have the points grow at a "slow" exponential rate
    // Y = a(1 + r)^x  a -> difficulty; r -> growth rate in %; x -> current level
    //TODO: Work on balancing this equation
    int points = (int)Mathf.Ceil(10 * Mathf.Pow((1 + growthRates[difficulty]), (float)level));

    return points;
  }

  private int GetNumberOfLevelTypes()
  {
    //Get the number of level types that exist within the MapPieces namespace via reflection
    //https://stackoverflow.com/questions/79693/getting-all-types-in-a-namespace-via-reflection/34869091
    string nspace = "MapPieces";
    var q = from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.Namespace == nspace
            select t;
    q.ToList();
    return q.Count();
  }
}


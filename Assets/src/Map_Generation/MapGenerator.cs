using System.Collections;
using System.Collections.Generic;
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
            lock(padlock)
            {
                if(instance == null)
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
    private float[] growthRates = new float[] {0.1618f, 0.2427f, 0.3236f};

    //public methods
    public Map GenerateNewMap(int level, int difficulty)
    {
        Map myMap = new Map();

        //Get a number of points for this map
        int totalPoints = CalculateMapPoints(level, difficulty);
        int currPoints = totalPoints;

        //TODO: Remove this once testing is fleshed out
        currPoints = 1;

        //Determine which piece set to use
        int pieceSetIdx = (int)Randomness.Instance.RandomUniformFloat(0, MapGeneration.Globals.NUM_FLOOR_TYPES);
        List<Piece> pieceSet = null;

        //TODO: Remove this when done with testing
        pieceSetIdx = 2;
        switch(pieceSetIdx)
        {
            case 0: //Test Pieces 
                pieceSet = new List<Piece>(TestPieces.all_TestPieces_pieces);
                TestPieces.Load();
                break;
            case 1: //Forest Pieces
                pieceSet = new List<Piece>(ForestPieces.all_ForestPieces_pieces);
                ForestPieces.Load();
                break;
            case 2:
                pieceSet = new List<Piece>(Kamen.all_Kamen_pieces);
                Kamen.Load();
                break;
            default: //Error
                Debug.LogError("This shouldnt happen");
                break;

        }
        //Generate rooms until we are out of points
        do
        {
            Room thisRoom = RoomGenerator.Instance.GenerateRoom(level, difficulty, pieceSet);
            myMap.AddRoom(thisRoom);
            currPoints -= thisRoom.cost;
        }
        while(currPoints > 0);


        return myMap;
    }

    //private methods
    private int CalculateMapPoints(int level, int difficulty)
    {
        //More points means a harder/bigger map
        //Current thoughts are to have the points grow at a "slow" exponential rate
        // Y = a(1 + r)^x  a -> difficulty; r -> growth rate in %; x -> current level
        //TODO: Work on balancing this equation
        int points = (int) Mathf.Ceil( 10 * Mathf.Pow((1 + growthRates[difficulty]), (float)level));

        return points;
    }
}


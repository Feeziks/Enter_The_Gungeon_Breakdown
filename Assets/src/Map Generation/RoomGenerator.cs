using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{

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

    //public members

    //private members

    //public methods
    public Room GenerateRoom(int level, int difficulty)
    {
        //Generate a new room based on the given level and difficulty
        //For now lets just say there are only 3 types of rooms to make this easy
        int roomType = (int) Randomness.Instance.RandomUniformFloat(0, 3);
        Debug.Log(roomType);

        Room newRoom = new Room(1);
        return newRoom;
    }

    //private methods

}

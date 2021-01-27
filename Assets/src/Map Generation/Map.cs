using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    //This class holds the data that describes the map of any given level
    //This class does not perform any actions relating to generating/deleting/checking etc. maps it is just a data holder

    //Constructor
    public Map()
    {

    }

    //public members

    //private members
    private List<Room> rooms = new List<Room>();

    //public methods
    public void AddRoom(Room newRoom)
    {
        rooms.Add(newRoom);
    }

    public Texture2D GetRoomTexture(int roomNumber)
    {
        if(roomNumber < 0 || roomNumber >= rooms.Count)
        {
            return null;
        }

        return rooms[roomNumber].texture;
    }

    public void ClearMap()
    {
        rooms.Clear();
    }

    //private methods
}

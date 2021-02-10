using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    //This class holds the data that describes the map of any given level
    //This class does not perform any actions relating to generating/deleting/checking etc. maps it is just a data holder

    //Messed up the git history by accident so making a change ehre so I can make a new commit

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

    public void ClearMap()
    {
        rooms.Clear();
    }

    //private methods
}

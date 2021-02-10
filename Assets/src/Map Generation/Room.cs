using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    //This class is a data container for a single room inside of a Map
    //A room is a collection of "tiles" that are assembled together in a way that is "solvable"
    //A room has a cost associated with it based on its size and what tiles it contains
    //For example a large room with several traps will "cost" more than a smaller room with fewer traps
    //TODO: This might cause issues with a small room and a large number of traps. Perhaps rather than raw size and raw #traps
    //A percentage of walkable terrain vs trap terrain could be used?

    //Constructors
    public Room()
    {
        this.cost = 1;
        this.name = "Unamed Room";

        this.Container = new GameObject(this.name);
    }

    public Room(int c, string n)
    {
        this.cost = c;
        this.name = n;

        this.Container = new GameObject(this.name);
    }

    //public members
    public int cost { get; }
    public string name {get; }
    public GameObject Container {get; }

    //private members
    
    //public methods

    //private methods
}

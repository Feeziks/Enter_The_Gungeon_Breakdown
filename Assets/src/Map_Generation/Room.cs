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
  public Room(int c, string n, Transform p)
  {
    cost = c;
    name = n;
    parent = p;
    parent = p;

    container = new GameObject(this.name);
    container.transform.SetParent(parent);
  }

  //public members
  public int cost { get; }
  public string name {get; }
  public GameObject container {get; }

  //private members
  private Transform parent; // The parent of any room should be a map object
    
  //public methods

  //private methods
  }

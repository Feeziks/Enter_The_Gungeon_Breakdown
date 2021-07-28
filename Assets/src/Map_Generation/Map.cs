using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
  //This class holds the data that describes the map of any given level
  //This class does not perform any actions relating to generating/deleting/checking etc. maps it is just a data holder

  //Constructor
  public Map(string n)
  {
    name = n;
    container = new GameObject(name);
  }

  //public members
  public string name { get; }

  //private members
  private List<Room> rooms = new List<Room>();
  private GameObject container;

  //public methods
  public void AddRoom(Room newRoom)
  {
    rooms.Add(newRoom);
  }

  public void ClearMap()
  {
    foreach(Room r in rooms)
    {
      GameObject.Destroy(r.container);
    }
    rooms.Clear();
  }

  public Transform GetTransform()
  {
    return container.transform;
  }

  //private methods
}

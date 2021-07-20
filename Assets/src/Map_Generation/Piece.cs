﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MapGeneration;

//Data container for each piece. 
//A piece is a potential choice for a slot while it collapses

public class Piece
{

  //Public Members
  //name
  public string name;

  //Prefab
  public GameObject prefab;

  //prefab location
  public string prefabPath;

  //Valid Neighbor list
  public Dictionary<string, List<Piece>> validNeighbors;

  //Private Members

  //Constructors
  public Piece()
  {

  }

  public Piece(string n, string pp)
  {
    name = n;
    prefabPath = pp;
    validNeighbors = null;
    prefab = null;
  }

  public Piece(string n, GameObject p, Dictionary<string, List<Piece>> v)
  {
    name = n;
    prefab = p;
    validNeighbors = v;
  }

  public Piece(string n, string pp, Dictionary<string, List<Piece>> v)
  {
    name = n;
    prefabPath = pp;
    validNeighbors = v;
  }

  public Piece(Piece copy)
  {
    name = copy.name;
    prefab = copy.prefab;
    validNeighbors = copy.validNeighbors;
  }

  //Public methods
  public bool LoadPrefab()
  {
    if (String.IsNullOrEmpty(prefabPath))
    {
      Debug.LogError("Null or empty prefab path when attempting to load prefab in piece: " + name);
      return false;
    }

    prefab = Resources.Load(prefabPath, typeof(GameObject)) as GameObject;
    if (prefab == null)
    {
      Debug.LogError("Unable to load prefab from path " + prefabPath + " for piece " + name);
      return false;
    }

    return true;
  }

  //Private methods

}

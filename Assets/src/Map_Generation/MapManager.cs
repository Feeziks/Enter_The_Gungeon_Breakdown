using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MapGeneration;

public class MapManager : MonoBehaviour
{
  //This class manages maps. I.E. Generates and stores the map data

  //Public members
  public GameObject display;

  //Private members
  private Map m_map;

  [SerializeField] private TMP_InputField levelInput;
  [SerializeField] private TMP_Dropdown difficultyInput;

  //Public methods
  public void Start()
  {
    m_map = new Map("My_Map");
  }

  public void GetNewMap()
  {
    if(m_map != null)
    {
      m_map.ClearMap();
    }
    int level = UI_Conversions.UserInputToInt(levelInput);
    if (level < 0)
    {
      Debug.Log("Enter a positive integer value for level");
      return;
    }
    int difficulty = difficultyInput.value;
    MapGenerator.Instance.GenerateNewMap(level, difficulty, ref m_map);
  }

  //Private methods
}

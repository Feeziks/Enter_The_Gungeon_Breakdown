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
    private Map m_map = new Map();

    [SerializeField] private TMP_InputField levelInput;
    [SerializeField] private TMP_Dropdown difficultyInput;

    //Public methods
    public void GetNewMap()
    {
        int level = UI_Conversions.UserInputToInt(levelInput);
        if(level < 0)
        {
            Debug.Log("Enter a positive integer value for level");
            return;
        }
        int difficulty = difficultyInput.value;
        m_map = MapGenerator.Instance.GenerateNewMap(level, difficulty);
    }

    //Private methods
}

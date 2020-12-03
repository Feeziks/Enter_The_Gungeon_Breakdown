using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestRarityTest : MonoBehaviour
{
    // Chest rarity UI Elements
    [SerializeField] private TMP_InputField commonRarityInput;
    [SerializeField] private TMP_InputField uncommonRarityInput;
    [SerializeField] private TMP_InputField rareRarityInput;
    [SerializeField] private TMP_InputField legendaryRarityInput;
    [SerializeField] private TMP_InputField totalRarityInput;

    public Color errorColor;
    public Color goodColor;

    [SerializeField] private Toggle logFileToggle;
    [SerializeField] private TMP_InputField iterationsInput;
    [SerializeField] private Button goButton;
    [SerializeField] private TMP_Text resultsText;

    [SerializeField] private TMP_InputField floorInput;
    [SerializeField] private TMP_InputField chestsPerFloorInput;
    [SerializeField] private TMP_InputField luckInput;

    private float[] rarities;

    public void Start()
    {
        rarities = new float[4];
    }

    //Chest rarity input callbacks
    public void UpdateTotalRarity()
    {
        //Get all input values
        float commonRarity      = UI_Conversions.UserInputToFloat(commonRarityInput);
        float uncommonRarity    = UI_Conversions.UserInputToFloat(uncommonRarityInput);
        float rareRarity        = UI_Conversions.UserInputToFloat(rareRarityInput);
        float legendaryRarity   = UI_Conversions.UserInputToFloat(legendaryRarityInput);

        //Check values
        if(commonRarity == -1.0f ||
           uncommonRarity == -1.0f ||
           rareRarity == -1.0f ||
           legendaryRarity == -1.0f)
        {
            //Disable the GO button
            goButton.interactable = false;
            //Exit early
            return;
        }

        //There are values in all inputs, update the total value
        float totalValue = commonRarity + uncommonRarity + rareRarity + legendaryRarity;
        totalRarityInput.text = totalValue.ToString();

        if(totalValue != 100.0f && totalValue != 1.0f)
        {
            goButton.interactable = false;
        }
        else
        {
            goButton.interactable = true;
            if(totalValue == 100.0f)
            {
                commonRarity /= 100.0f;
                uncommonRarity /= 100.0f;
                rareRarity /= 100.0f;
                legendaryRarity /= 100.0f;
            }

            rarities[0] = commonRarity;
            rarities[1] = uncommonRarity;
            rarities[2] = rareRarity;
            rarities[3] = legendaryRarity;
        }
    }

    //GO! Generate all the chests!
    public void ChestTest()
    {
        //Clear the results text
        resultsText.text = "";
        //See if we want to make a text file with results
        bool log = logFileToggle.isOn;
        string logFileName = "";
        if(log == true)
        {
            logFileName = @"Logs/ChestRarityTest_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_log.txt";
        }

        int iterations = UI_Conversions.UserInputToInt(iterationsInput);
        if(iterations == -1)
        {
            resultsText.text = "Please specify a number of iterations";
            return;
        }

        //Get modifier inputs
        //TODO: Probably better practice to have an "on end edit" for each one that updates these values rather than check here?
        int floor = UI_Conversions.UserInputToInt(floorInput);
        int chestsPerFloor = UI_Conversions.UserInputToInt(chestsPerFloorInput);
        int luck = UI_Conversions.UserInputToInt(luckInput);

        if(floor == -1 ||
           chestsPerFloor == -1 ||
           luck == -1)
        {
            resultsText.text += "Please fill in the \"Modifiers\" inputs";
            return;
        }

        List<int> chests = new List<int>();
        //run for the requested number of iterations
        for(int i = 0; i < iterations; i++)
        {
            //Generate chest/floor chests
            for(int j = 0; j < chestsPerFloor; j++)
            {
                chests.Add(Randomness.Instance.GenerateChest(rarities, floor, luck));
            }
        }

        //Count the number of chests of each rarity
        int[] chestCounts = new int[4];
        for(int i = 0; i < chests.Count; i++)
        {
            chestCounts[chests[i]]++;
        }

        //Create an expected output
        //Print results
        int[] expectedChests = new int[4];
        string output = "";
        for(int i = 0; i < 4; i++)
        {
            int totalChests = iterations * chestsPerFloor;
            expectedChests[i] = (int)Mathf.Ceil(totalChests * rarities[i]);

            string chestRarityName = "";
            if(i == 0)
                chestRarityName = "common";
            else if(i == 1)
                chestRarityName = "uncommon";
            else if(i == 2)
                chestRarityName = "rare";
            else
                chestRarityName = "legendary";

            output += "Number of " + chestRarityName + " chests generated: " + chestCounts[i];
            output += "\r\nNumber of " + chestRarityName + " chests expected: " + expectedChests[i];
            output += "\r\nDifference of " + Mathf.Abs(chestCounts[i] - expectedChests[i]);
            output += "\r\n\r\n";
        }

        resultsText.text = output;

        if(log)
        {
            File.WriteAllText(logFileName, output);
        }

    }
}

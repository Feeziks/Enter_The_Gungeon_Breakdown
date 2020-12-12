using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestLootTest : MonoBehaviour
{
    //Inputs panel text mesh pro drop down fields
    [SerializeField] private TMP_Dropdown chestTypeInput;

    //Inputs panel text mesh pro input fields
    [SerializeField] private TMP_InputField lootPoolInput;
    [SerializeField] private TMP_InputField iterationsInput;

    //Inputs panel toggle fields
    [SerializeField] private Toggle inclusiveToggle;

    //Generate panel toggle fields
    [SerializeField] private Toggle logFileToggle;

    //Results panel text fields
    [SerializeField] private TMP_Text resultsText;


    //Enable all panels
    public void EnableAllPanels()
    {
        gameObject.SetActive(true);
    }

    //Disable all panels
    public void DisableAllPanels()
    {
        gameObject.SetActive(false);
    }

    public void LootTest()
    {
        //Get the value from all of the inputs
        int chestType = chestTypeInput.value;
        int lootPoolSize = UI_Conversions.UserInputToInt(lootPoolInput);
        int iterations = UI_Conversions.UserInputToInt(iterationsInput);
        bool inclusive = inclusiveToggle.isOn;

        //Check the values
        if(lootPoolSize <= 0 ||
           iterations <= 0)
        {
            //exit early
            resultsText.text = "Please fill in the inputs!";
            return;
        }

        int[] loot = new int[lootPoolSize];

        for(int i = 0; i < iterations; i++)
        {
            int thisLoot = -1;
            if(inclusive)
            {
                thisLoot = (int)Randomness.Instance.RandomUniformFloat(0, lootPoolSize);
            }
            else
            {
                do
                {
                    thisLoot = (int)Randomness.Instance.RandomUniformFloat(0, lootPoolSize);    
                } while (thisLoot == 0 || thisLoot == lootPoolSize);
            }
            loot[thisLoot]++;
        }

        int sum = 0;
        int average = 0;
        int mode = 0;
        int modeCount = 0;
        int min = iterations + 1;
        int minCount = iterations + 1;
        
        for(int i = (inclusive == true ? 0 : 1); i < (inclusive == true ? lootPoolSize : lootPoolSize - 1); i++)
        {
            sum += loot[i];
            if(loot[i] > modeCount)
            {
                modeCount = loot[i];
                mode = i;
            }

            if(loot[i] < minCount)
            {
                minCount = loot[i];
                min = i;
            }
        }

        average = sum / (inclusive == true ? lootPoolSize : lootPoolSize - 2);

        //Calculate the ideal value for each item
        int idealValue = 0;
        if(inclusive)
        {
            idealValue = iterations / lootPoolSize;
        }
        else
        {
            idealValue = iterations / (lootPoolSize - 2);
        }

        //Output the results
        resultsText.text = "";
        resultsText.text += "Generated " + iterations + " items\n";
        resultsText.text += "mean: " + average + "\n";
        resultsText.text += "mode: item_" + mode + " with " + modeCount + " occurences\n";
        resultsText.text += "min: item_" + min + " with " + minCount + " occurences\n";
        resultsText.text += "ideally each value would be " + idealValue + "\n";
        resultsText.text += "\n***All values***\n";

        for(int i = (inclusive == true ? 0 : 1); i < (inclusive == true ? lootPoolSize : lootPoolSize - 1); i++)
        {
             resultsText.text += "item_" + i + " " + loot[i] + " occurences\n";
        }
    }
}

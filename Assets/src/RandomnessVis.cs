using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class RandomnessVis : MonoBehaviour
{
    public TMP_InputField numberToGenerate;
    public TMP_InputField rangeMinimum;
    public TMP_InputField rangeMaximum;

    public Toggle roundingToggle;
    public TMP_Dropdown roundingPlaces;

    public GameObject windowGraph;
    private Window_Graph wg;

    public void ReturnToSceneSelection()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    public void PrintList<T>(IEnumerable<T> l)
    {
        foreach(var item in l)
            Debug.Log(item);
    }

    public void Clear(Window_Graph wg)
    {
        wg.Clear();
    }

    private void RoundToDecimalPlaces(ref List<float> randomNumbers, int decimalPlaces)
    {
        for(int i = 0; i < randomNumbers.Count; i++)
        {
            randomNumbers[i] = (float)System.Math.Round(randomNumbers[i], decimalPlaces);
        }
    }

    private List<Vector2> CountOccurences(List<float> randomNumbers, ref int minCount, ref int maxCount)
    {
        //Vector2 X will be the value of the random number
        //Vector2 Y will be the number of occurences of that value
        List<Vector2> counts = new List<Vector2>();
        int thisCount;
        float lastSearchValue = -1;
        float thisSearchValue = -1;

        for(int i = 0; i < randomNumbers.Count; i++)
        {
            //Initialize the count for this value
            thisCount = 1;

            //Get the value to search for
            thisSearchValue = randomNumbers[i];

            //If we are still on the same value (i.e. list[i-1] and list[i] are the same)
            if(thisSearchValue == lastSearchValue)
                continue; //No need to keep checking we already have counted this values cocurences
            
            //Start at the next index and begin counting the occurences of that value
            for(int j = 1 + 1; j < randomNumbers.Count; j++)
            {
                if(randomNumbers[j] == thisSearchValue)
                    thisCount += 1;
            }

            if(thisCount < minCount)
                minCount = thisCount;

            if(thisCount > maxCount)
                maxCount = thisCount;

            //Add the occurences to our count list and update the last search value to be this search value
            counts.Add(new Vector2(thisSearchValue, thisCount));
            lastSearchValue = thisSearchValue;
        }

        return counts;
    }

    public void UnityRandomRange()
    {
        wg = windowGraph.GetComponent<Window_Graph>();
        if(wg == null)
        {
            Debug.Log("wg is null");
            return;
        }

        Clear(wg);

        int numToGen = UI_Conversions.UserInputToInt(numberToGenerate);
        float minRange = UI_Conversions.UserInputToFloat(rangeMinimum);
        float maxRange = UI_Conversions.UserInputToFloat(rangeMaximum);

        if(numToGen == -1 || minRange == -1 || maxRange == -1)
            return;

        List<float> unityRandomRangeNumbers = new List<float>();
        Randomness.Instance.UnityRandom(numToGen, minRange, maxRange, ref unityRandomRangeNumbers);
        
        if(roundingToggle.isOn)
        {
            //Mild rounding to make it easier
            int decimalPlaceRound = UI_Conversions.UserInputToInt(roundingPlaces.options[roundingPlaces.value].text);
            RoundToDecimalPlaces(ref unityRandomRangeNumbers, decimalPlaceRound);
        }
        //Sort the list
        unityRandomRangeNumbers.Sort();

        //Count the number of occurences of each value
        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(unityRandomRangeNumbers, ref minCount, ref maxCount);

        //plot that count vs the value
        wg.Graph(counts, new Vector2(minRange, maxRange), new Vector2(minCount, maxCount));
    }

    public void CSharpRandom()
    {
        wg = windowGraph.GetComponent<Window_Graph>();
        if(wg == null)
        {
            Debug.Log("wg is null");
            return;
        }

        Clear(wg);

        int numToGen = UI_Conversions.UserInputToInt(numberToGenerate);
        float minRange = UI_Conversions.UserInputToFloat(rangeMinimum);
        float maxRange = UI_Conversions.UserInputToFloat(rangeMaximum);

        if(numToGen == -1 || minRange == -1 || maxRange == -1)
            return;

        List<float> cSharpRandomNumbers = new List<float>();
        Randomness.Instance.CSharpRandom(numToGen, minRange, maxRange, ref cSharpRandomNumbers);

        if(roundingToggle.isOn)
        {
            int decimalPlaceRound = UI_Conversions.UserInputToInt(roundingPlaces.options[roundingPlaces.value].text);
            RoundToDecimalPlaces(ref cSharpRandomNumbers, decimalPlaceRound);
        }

        cSharpRandomNumbers.Sort();

        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(cSharpRandomNumbers, ref minCount, ref maxCount);

        wg.Graph(counts, new Vector2(minRange, maxRange), new Vector2(minCount, maxCount));
    }

    public void ChiSquareRandom()
    {
        wg = windowGraph.GetComponent<Window_Graph>();
        if(wg == null)
        {
            Debug.Log("wg is null");
            return;
        }

        Clear(wg);

        int numToGen = UI_Conversions.UserInputToInt(numberToGenerate);
        float minRange = UI_Conversions.UserInputToFloat(rangeMinimum);
        float maxRange = UI_Conversions.UserInputToFloat(rangeMaximum);

        if(numToGen == -1 || minRange == -1 || maxRange == -1)
            return;

        List<float> chiSquareRandomNumbers = new List<float>();
        Randomness.Instance.UnityRandom(numToGen, 0.0f, 1.0f, ref chiSquareRandomNumbers);

        //Square the values 
        for(int i = 0; i < chiSquareRandomNumbers.Count; i++)
        {
            chiSquareRandomNumbers[i] *= chiSquareRandomNumbers[i];
            //Values are still between 0.0f and 1.0f, need to scale them to min and max values
            chiSquareRandomNumbers[i] = minRange + (chiSquareRandomNumbers[i] * (( maxRange - minRange )));
        }

        if(roundingToggle.isOn)
        {
            int decimalPlaceRound = UI_Conversions.UserInputToInt(roundingPlaces.options[roundingPlaces.value].text);
            RoundToDecimalPlaces(ref chiSquareRandomNumbers, decimalPlaceRound);
        }
        chiSquareRandomNumbers.Sort();

        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(chiSquareRandomNumbers, ref minCount, ref maxCount);

        wg.Graph(counts, new Vector2(minRange, maxRange), new Vector2(minCount, maxCount));
    }

    public void GaussianRandom()
    {
        wg = windowGraph.GetComponent<Window_Graph>();
        if(wg == null)
        {
            Debug.Log("wg is null");
            return;
        }

        Clear(wg);

        int numToGen = UI_Conversions.UserInputToInt(numberToGenerate);
        float mean = UI_Conversions.UserInputToFloat(rangeMinimum);
        float stdDev = UI_Conversions.UserInputToFloat(rangeMaximum);

        if(numToGen == -1 || mean == -1 || stdDev == -1)
            return;

        List<float> standardDistributionRandomNumbers = new List<float>();
        Randomness.Instance.GaussianRandom(numToGen, mean, stdDev, ref standardDistributionRandomNumbers);

        if(roundingToggle.isOn)
        {
            int decimalPlaceRound = UI_Conversions.UserInputToInt(roundingPlaces.options[roundingPlaces.value].text);
            RoundToDecimalPlaces(ref standardDistributionRandomNumbers, decimalPlaceRound);
        }

        standardDistributionRandomNumbers.Sort();

        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(standardDistributionRandomNumbers, ref minCount, ref maxCount);

        wg.Graph(counts, new Vector2(mean + stdDev * -4.0f, mean + stdDev * 4.0f), new Vector2(minCount, maxCount));


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RandomnessVis : MonoBehaviour
{
    public TMP_InputField numberToGenerate;
    public GameObject windowGraph;
    private Window_Graph wg;

    public int decimalPlaceRound;

    public void PrintList<T>(IEnumerable<T> l)
    {
        foreach(var item in l)
            Debug.Log(item);
    }

    public void Clear(Window_Graph wg)
    {
        wg.Clear();
    }

    private int GetUserInput()
    {
        //Get the user input number
        string str = numberToGenerate.text;
        if(string.IsNullOrEmpty(str))
        {
            Debug.Log("No number was input");
            return -1;
        }
        int numToGen = System.Convert.ToInt32(str);
        return numToGen;
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

        int numToGen = GetUserInput();
        if(numToGen == -1)
            return;

        List<float> unityRandomRangeNumbers = new List<float>();
        Randomness.Instance.UnityRandom(numToGen, 0.0f, 1.0f, ref unityRandomRangeNumbers);
        //Mild rounding to make it easier
        RoundToDecimalPlaces(ref unityRandomRangeNumbers, decimalPlaceRound);
        //Sort the list
        unityRandomRangeNumbers.Sort();

        //Count the number of occurences of each value
        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(unityRandomRangeNumbers, ref minCount, ref maxCount);

        //plot that count vs the value
        wg.Graph(counts, new Vector2(0.0f, 1.0f), new Vector2(minCount, maxCount));
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

        int numToGen = GetUserInput();
        if(numToGen == -1)
            return;

        List<float> cSharpRandomNumbers = new List<float>();
        Randomness.Instance.CSharpRandom(numToGen, 0.0f, 1.0f, ref cSharpRandomNumbers);
        RoundToDecimalPlaces(ref cSharpRandomNumbers, decimalPlaceRound);
        cSharpRandomNumbers.Sort();

        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(cSharpRandomNumbers, ref minCount, ref maxCount);

        wg.Graph(counts, new Vector2(0.0f, 1.0f), new Vector2(minCount, maxCount));
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

        int numToGen = GetUserInput();
        if(numToGen == -1)
            return;

        List<float> chiSquareRandomNumbers = new List<float>();
        Randomness.Instance.CSharpRandom(numToGen, 0.0f, 1.0f, ref chiSquareRandomNumbers);

        //Square the values 
        for(int i = 0; i < chiSquareRandomNumbers.Count; i++)
        {
            chiSquareRandomNumbers[i] *= chiSquareRandomNumbers[i];
        }

        RoundToDecimalPlaces(ref chiSquareRandomNumbers, decimalPlaceRound);
        chiSquareRandomNumbers.Sort();

        int minCount = System.Int32.MaxValue;
        int maxCount = System.Int32.MinValue;
        List<Vector2> counts = CountOccurences(chiSquareRandomNumbers, ref minCount, ref maxCount);

        wg.Graph(counts, new Vector2(0.0f, 1.0f), new Vector2(minCount, maxCount));
    }

}

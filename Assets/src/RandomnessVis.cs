using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RandomnessVis : MonoBehaviour
{
    public TMP_InputField numberToGenerate;
    public GameObject windowGraph;
    private Window_Graph wg;

    void Start()
    {

    }

    public void PrintList<T>(IEnumerable<T> l)
    {
        foreach(var item in l)
            Debug.Log(item);
    }

    public void UnityRandomRange()
    {
        wg = windowGraph.GetComponent<Window_Graph>();
        if(wg == null)
        {
            Debug.Log("wg is null");
            return;
        }

        wg.Clear();

        //Get the user input number
        string str = numberToGenerate.text;
        if(string.IsNullOrEmpty(str))
        {
            Debug.Log("No number was input");
            return;
        }
        int numToGen = System.Convert.ToInt32(str);

        List<float> unityRandomRangeNumbers = new List<float>();
        Randomness.Instance.UnityRandom(numToGen, 0.0f, 1.0f, ref unityRandomRangeNumbers);
        //Mild rounding to make it easier
        for(int i = 0; i < unityRandomRangeNumbers.Count; i++)
        {
            unityRandomRangeNumbers[i] = (float)System.Math.Round(unityRandomRangeNumbers[i], 2);
        }
        //Sort the list
        unityRandomRangeNumbers.Sort();
        //Count the number of occurences of each value
        List<Vector2> counts = new List<Vector2>();
        int thisCount = 1;
        float minCount = Mathf.Infinity;
        float maxCount = Mathf.NegativeInfinity;

        float lastSearchVal = 0;

        for(int i = 0; i < unityRandomRangeNumbers.Count; i++)
        {
            //Get the value to search for
            float searchVal = unityRandomRangeNumbers[i];
            if(searchVal == lastSearchVal)
                continue;
            
            thisCount = 1;

            for(int j = i + 1; j < unityRandomRangeNumbers.Count; j++)
            {
                if(unityRandomRangeNumbers[j] == searchVal)
                    thisCount += 1;
            }

            if(thisCount < minCount)
                minCount = thisCount;

            if(thisCount > maxCount)
                maxCount = thisCount;

            counts.Add(new Vector2(searchVal, thisCount));
            lastSearchVal = unityRandomRangeNumbers[i];
        }
        //plot that count vs the value
        wg.Graph(counts, new Vector2(0.0f, 1.0f), new Vector2(minCount, maxCount));
    }

}

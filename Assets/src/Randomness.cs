using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomness
{
    #region SINGLETON


    Randomness()
    {

    }
    private static readonly object padlock = new object();
    private static Randomness instance = null;
    public static Randomness Instance
    {
        get
        {   
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new Randomness();
                }
                return instance;
            }
        }
    }
    #endregion

    public void UnityRandom(int num, float min, float max, ref List<float> randomNumbers)
    {
        //Clear the list of any data from before
        randomNumbers.Clear();

        for(int i = 0; i < num; i++)
        {
            randomNumbers.Add(Random.Range(min, max));
        }
    }

    public void CSharpRandom(int num, float min, float max, ref List<float> randomNumbers)
    {
        //Clear the list of any data from before
        randomNumbers.Clear();

        //Create an instance of the random class
        System.Random rand = new System.Random();

        for(int i = 0; i < num; i++)
        {
            //Get a random floating value between 0.0 and 1.0
            float thisNum = (float)rand.NextDouble();
            
            //Shift the value to fit between the passed min and max
            thisNum = min + (thisNum * (( max - min ) + 1 ));
            randomNumbers.Add(thisNum);
        }
    }
}

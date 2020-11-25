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
}

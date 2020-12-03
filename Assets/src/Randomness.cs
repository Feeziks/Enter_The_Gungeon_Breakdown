using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Randomness
{
    #region SINGLETON


    Randomness()
    {

    }
    private static readonly object padlock = new object();
    private static Randomness instance = null;
    private static System.Random SysRand = null;

    public static Randomness Instance
    {
        get
        {   
            lock(padlock)
            {
                if(instance == null)
                {
                    instance = new Randomness();
                    SysRand = new System.Random();
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
            randomNumbers.Add(UnityEngine.Random.Range(min, max));
        }
    }

    public void CSharpRandom(int num, float min, float max, ref List<float> randomNumbers)
    {
        //Clear the list of any data from before
        randomNumbers.Clear();

        for(int i = 0; i < num; i++)
        {
            //Get a random floating value between 0.0 and 1.0
            float thisNum = (float)SysRand.NextDouble();
            
            //Shift the value to fit between the passed min and max
            thisNum = min + (thisNum * (( max - min )));
            randomNumbers.Add(thisNum);
        }
    }

    private float NextGaussian()
    {
        float point1 = 0.0f;
        float point2 = 0.0f;
        float sum = 0.0f;

        do
        {
            point1 = 2.0f * (float)SysRand.NextDouble() - 1.0f;
            point2 = 2.0f * (float)SysRand.NextDouble() - 1.0f;
            sum = point1 * point1 + point2 * point2;
        } while( sum >= 1.0f || sum == 0.0f);

        float result = Mathf.Sqrt((-2.0f * Mathf.Log(sum)) / sum);
        return point1 * result;
    }

    public void GaussianRandom(int num, float mean, float stdDev, ref List<float> randomNumbers)
    {
        //Box-Mueller Transform on uniform random distribution
        //https://en.wikipedia.org/wiki/Marsaglia_polar_method

        //Clear the list
        randomNumbers.Clear();

        for(int i = 0; i < num; i++)
        {
            float thisNum = mean;
            //Get a gaussian value
            do
            {
                thisNum = NextGaussian();
                thisNum = mean + thisNum * stdDev;
            }while(thisNum < mean + stdDev * -3 || thisNum > mean + stdDev * 3); //And check that it is within 3 std deviations!
            randomNumbers.Add(thisNum);
        }
    }

    public float RandomUniformFloat(float min, float max)
    {
        float thisNum = (float)SysRand.NextDouble();
        thisNum = min + (thisNum * (( max - min )));
        return thisNum;
    }

    public float RandomSquaredFloat(float min, float max)
    {
        float thisNum = (float)SysRand.NextDouble();
        thisNum = thisNum * thisNum;
        thisNum = min + (thisNum * (( max - min )));
        return thisNum;
    }

    public float RandomGaussianFloat(float mean, float stdDev)
    {
        float result = 0.0f;
        do{
            result = NextGaussian();
            result = mean + result * stdDev;
        }while(result <= mean + stdDev * -3.5f || result > mean + stdDev * 3.5f);

        return result;
    }

    //Generate a chest based on chest rarities and appropriate modifiers
    //(For now return an int that represents the chest rarity 0 - common, 1 - uncommon etc)
    //TODO: This needs to be refactored
    //TODO: We need to create an enum for chest types
    //TODO: We need to create a static array with floor modifiers for chest types
    public int GenerateChest(float[] rarities, int floorNum, int luck)
    {
        //As the floor number increases the chance of getting a rarer chest should increase as well
        float[] floorModifiers = new float[4];
        floorModifiers[0] = -0.01f;
        floorModifiers[1] = -0.005f;
        floorModifiers[2] =  0.01f;
        floorModifiers[3] =  0.005f;

        float[] modifiedRarities = new float[4];
        Array.Copy(rarities, modifiedRarities, 4);

        for(int i = 0; i < floorNum - 1; i++)
        {
            modifiedRarities[0] += floorModifiers[0];
            modifiedRarities[1] += floorModifiers[1];
            modifiedRarities[2] += floorModifiers[2];
            modifiedRarities[3] += floorModifiers[3];
        }

        float[] cumulativeRarities = new float[4];
        cumulativeRarities[0] = modifiedRarities[0];
        cumulativeRarities[1] = modifiedRarities[0] + modifiedRarities[1];
        cumulativeRarities[2] = modifiedRarities[0] + modifiedRarities[1] + modifiedRarities[2];
        cumulativeRarities[3] = modifiedRarities[0] + modifiedRarities[1] + modifiedRarities[2] + modifiedRarities[3];

        //Reroll for each point of luck the user has, take the best result of all rolls
        int chest = 0;
        int bestResult = 0;
        for(int i = 0; i <= luck; i++)
        {
            float randomValue = RandomSquaredFloat(0.0f, 1.0f);

            //Compare the result to the cumulative values of each chest type
            for(int j = 3; j >= 0; j--)
            {
                if(j == 3)
                {
                    if(randomValue >= cumulativeRarities[j - 1])
                    {
                        chest = j;
                        break;
                    }
                }
                else if(j == 0)
                {
                    chest = j;
                }
                else
                {
                    if(randomValue >= cumulativeRarities[j - 1] && randomValue < cumulativeRarities[j])
                    {
                        chest = j;
                        break;
                    }
                }
            }

            if(chest > bestResult)
                bestResult = chest;

        }

        return bestResult;
    }
}

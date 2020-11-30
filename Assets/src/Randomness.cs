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
            randomNumbers.Add(Random.Range(min, max));
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
}

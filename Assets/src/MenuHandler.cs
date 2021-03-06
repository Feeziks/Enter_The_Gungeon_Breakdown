﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    public void LoadRNGGraphsScene()
    {
        SceneManager.LoadScene("RNG_Graphs", LoadSceneMode.Single);
    }

    public void LoadRNGTestsScene()
    {
        SceneManager.LoadScene("RNG_Tests", LoadSceneMode.Single);
    }

    public void LoadMapGeneration()
    {
        SceneManager.LoadScene("Map_Generation", LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

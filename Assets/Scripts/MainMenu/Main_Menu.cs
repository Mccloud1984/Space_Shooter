using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class Main_Menu : MonoBehaviour
{
    [SerializeField]
    public int _singlePlayerMode = 1; //Game Scene
    [SerializeField]
    public int _coopMode = 2;
    public void LoadSinglePlayer()
    {
        Debug.Log("Single Player Loading...");
        SceneManager.LoadScene(_singlePlayerMode);
        GameConfig.SetCoOpMode(false);
    }

    public void LoadCoOpMode()
    {
        Debug.Log("Co-Op Mode Loading");
        SceneManager.LoadScene(_coopMode);
        GameConfig.SetCoOpMode(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;
    [SerializeField]
    private int _defaultSceneName = 0; //"Game" scene is scene 0;
    [SerializeField]
    public int CurrentRound = 0;
    [SerializeField]
    private int _rountsLimit = 10;
    //[SerializeField]
    //private bool _coopModeEnabled = false;
    [SerializeField]
    private GameObject _playerPrefab;

    private void Start()
    {
        CurrentRound = 0;
        SpawnPlayers();
        GameConfig.GetUIManager().ShowShootAstroidText();
    }


    private void Update()
    {
        if(_isGameOver == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
                SpawnPlayers();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                ToMainMenu();
        }
        if(_isGameOver == false)
        {
//            if (Input.GetKeyDown(KeyCode.Escape))
//            {
//                Debug.Log("Escape key hit.");
//                Application.Quit();
//#if UNITY_EDITOR
//                UnityEditor.EditorApplication.isPlaying = false;
//#endif
//            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                if(Time.timeScale == 0)
                {
                    ResumePlay();
                }
                else
                {
                    PausePlay();
                }
            }
        }

    }

    public void ResumePlay()
    {
        GameConfig.GetUIManager().HidePauseMenu();
        Time.timeScale = 1;
    }
    public void PausePlay()
    {
        GameConfig.GetUIManager().ShowPauseMenu();
        Time.timeScale = 0;
    }

    public void ToMainMenu()
    {
        ResumePlay();
        SceneManager.LoadScene(_defaultSceneName, LoadSceneMode.Single);
    }
    private void SpawnPlayers()
    {
        StartCoroutine(LoadPlayersRoutine());
    }

    IEnumerator LoadPlayersRoutine()
    {
        yield return new WaitForSeconds(1f);
        Vector3 startLocation = new Vector3(0, GameDimentions.BottomOfScreen);
        GameObject objToInst = _playerPrefab;
        Instantiate(objToInst, startLocation, Quaternion.identity);
    }


    public void OnPlayerDeath()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players?.Length >= 2)
            return;
        GameOver();
    }

    public void GameOver()
    {
        GameConfig.GetUIManager().SetGameOver();
        GameConfig.GetSpawnManager().StopSpawning();
        _isGameOver = true;
    }

    public void StartNextRound()
    {
        CurrentRound++;
        if (CurrentRound >= _rountsLimit)
            GameOver(); //GameConfig.GetUIManager().ShowEndGameText();
        else
            GameConfig.GetUIManager().ShowNextRoundUI(CurrentRound);
    }

    public void StartNewGame()
    {
        _isGameOver = false;
    }

    public void StartGame()
    {
        GameConfig.GetUIManager().HideShootAstroidText();
        StartNextRound();
    }
}

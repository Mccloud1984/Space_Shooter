using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemysPrefabs = new GameObject[2];
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerupContainer;
    [SerializeField]
    List<GameObject> _availablePowerUps = new List<GameObject>();
    [SerializeField]
    private RangeList _spawnEnemiesRange = new RangeList(2, 5);
    //[SerializeField]
    //private float _startSpawningDelay = 2f;
    [SerializeField]
    private RangeList _spawnPowerupsDelayRange = new RangeList( 8f, 20f );
    [SerializeField]
    private int _currentSpawnCount = 0;
    [SerializeField]
    private int _startingSpawnLimit = 10;
    [SerializeField]
    private int _roundSpawnLimit = 10;
    [SerializeField]
    private int _roundSpawnMultiplyer = 2;

    private Transform _laserContainerObj;

    private bool _stopSpawning = false;

    IEnumerator _spawnEnemiesRoutine;
    IEnumerator _spawnPowerupsRoutine;
    // Start is called before the first frame update
    public void Start()
    {
        _currentSpawnCount = 0;
        _roundSpawnLimit = _startingSpawnLimit;
        _laserContainerObj = transform.Find("Laser_Container");
        if (_laserContainerObj == null)
            Debug.LogError("Failed to find Laser Container.");
    }
    public void StartSpawning()
    {
        _stopSpawning = false;
        _currentSpawnCount = 0;
        _spawnEnemiesRoutine = SpawnEnemies();
        StartCoroutine(_spawnEnemiesRoutine);
        _spawnPowerupsRoutine = SpawnPowerup();
        StartCoroutine(_spawnPowerupsRoutine);
        
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
    }



    IEnumerator SpawnEnemies()
    {
        //yield return new WaitForSeconds(_startSpawningDelay);
        while (_stopSpawning == false && ++_currentSpawnCount <= _roundSpawnLimit)
        {
            if(_enemysPrefabs.Length > 0)
            {
                yield return new WaitForSeconds(_spawnEnemiesRange.GetRandomNumber());
                int enemyToLoad = UnityEngine.Random.Range(0, _enemysPrefabs.Length);
                GameObject newEnemy = Instantiate(_enemysPrefabs[enemyToLoad], GameDimentions.GetRandomEnemyStartPos(), Quaternion.identity);
                newEnemy.transform.SetParent(_enemyContainer.transform, false);
            } 
            else
            {
                Debug.LogError("Missing enemy prefabs.");
            }
        }
        if (_currentSpawnCount >= _roundSpawnLimit)
        {
            yield return new WaitForSeconds(2f);
            _stopSpawning = true;
            _roundSpawnLimit *= _roundSpawnMultiplyer;
            if ((GameConfig.GetGameManager().CurrentRound + 1) % 2 == 0)
                _spawnEnemiesRange = new RangeList(_spawnEnemiesRange.MinValue, _spawnEnemiesRange.MaxValue / 2);
            else                
                _spawnEnemiesRange = new RangeList(_spawnEnemiesRange.MinValue/2, _spawnEnemiesRange.MaxValue);
            GameConfig.GetGameManager().StartNextRound();
        }
    }

    IEnumerator SpawnPowerup()
    {
        //float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        //yield return new WaitForSeconds(_startSpawningDelay);
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(_spawnPowerupsDelayRange.GetRandomNumber());
            if(_availablePowerUps.Count >= 1)
            {
                int powerupIndex = UnityEngine.Random.Range(0, _availablePowerUps.Count);
                GameObject powerUpToSpawn = _availablePowerUps[powerupIndex];
                if (powerUpToSpawn != null)
                {
                    GameObject newPowerup = Instantiate(powerUpToSpawn, GameDimentions.GetRandomEnemyStartPos(), Quaternion.identity);
                    newPowerup.transform.parent = _powerupContainer.transform;

                }
                else
                {
                    Debug.LogError($"Failed to pull up powerup index of {powerupIndex}");
                }
            }
            //waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        }

    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public GameObject SpawnLaser(GameObject laser, Vector3 startLocation)
    {
        GameObject laserObj = Instantiate(laser, startLocation, Quaternion.identity);
        laserObj.transform.SetParent(_laserContainerObj, false);
        return laserObj;
    }
}

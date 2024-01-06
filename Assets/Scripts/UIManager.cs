using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private string _scoreTextDefaultValue = "Score: ";
    [SerializeField]
    private Text _player1ScoreTextObject;
    [SerializeField]
    private int _player1Score;
    [SerializeField]
    private Text _player2ScoreTextObject;
    [SerializeField]
    private int _player2Score;
    [SerializeField]
    private int _currentHighScore;
    [SerializeField]
    private string _highScoreDefaultValue = "High:";
    [SerializeField]
    private Text _highScoreTextObject; 
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _player1LivesImage;
    [SerializeField]
    private Image _player2LivesImage;
    [SerializeField]
    private Text _gameOverTextObject;
    [SerializeField]
    private float _flickerRate = .35f;
    [SerializeField]
    private Text _restartTextObject;
    [SerializeField]
    private GameObject _pauseMenuPrefab;
    [SerializeField]
    private GameObject _startTextPrefab;
    [SerializeField]
    private float _startTextDelay = 2f;
    [SerializeField]
    private float _startTextSecondsLimit = 10;
    [SerializeField]
    private GameObject _nextRoundTextPrefab;
    [SerializeField]
    private string _nextRoundDefaultValue = "Round: ";
    private RoundText _currentRoundText;
    [SerializeField]
    private string _startGameDefaultValue = "Start!";
    private float _startTextFlickerSpeed = .4f;
    private float _startRoundDelaySpeed = 3f;


    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        _player1Score = 0;
        _player2Score = 0;
        _gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Failed to find game manager.");
        _currentRoundText = transform.Find("Round_Container").GetComponent<RoundText>();
        if (_currentRoundText == null)
            Debug.LogError("Failed to find current round text script.");
        StartGame();
        if(_pauseMenuPrefab == null)
        {
            Debug.LogError("Missing pause menu prefab");
        }
        GetHighScore();
    }


    GameObject _pauseMenuObj = null;
    public void ShowPauseMenu()
    {
        _pauseMenuObj = Instantiate(_pauseMenuPrefab);
        _pauseMenuObj.transform.SetParent(gameObject.transform, false);
     }

    public void HidePauseMenu()
    {
        if(_pauseMenuObj != null)
            GameObject.Destroy(_pauseMenuObj);
    }

    GameObject _startTextObj = null;
    public void ShowShootAstroidText()
    {
        flickerStartTextRoutine = flickerShootAstroidText(_flickerRate);
        StartCoroutine(flickerStartTextRoutine);
    }
    
    public void HideShootAstroidText()
    {
        if(flickerStartTextRoutine != null)
        {
            StopCoroutine(flickerStartTextRoutine);
            GameObject.Destroy(_startTextObj);
            flickerStartTextRoutine = null;
        }
    }


    IEnumerator flickerShootAstroidTextRoutine;
    IEnumerator flickerShootAstroidText(float flickerRate)
    {
        yield return new WaitForSeconds(_startTextDelay);
        _startTextObj = Instantiate(_startTextPrefab);
        _startTextObj.transform.SetParent(gameObject.transform, false);
        int times = 0;
        while (times <= _startTextSecondsLimit)
        {
            _startTextObj.SetActive(true);
            yield return new WaitForSeconds(flickerRate);
            _startTextObj.SetActive(false);
            yield return new WaitForSeconds(flickerRate);
            times += 2;
        }
        HideShootAstroidText();
    }

    //internal void ShowEndGameText()
    //{
    //    throw new NotImplementedException();
    //}

    GameObject _nextRoundTextObj;
    IEnumerator flickerStartTextRoutine;
    IEnumerator StartFlickerStartText(float flickerSpeed)
    {
        while (true)
        {
            _nextRoundTextObj.SetActive(true);
            yield return new WaitForSeconds(flickerSpeed);
            _nextRoundTextObj.SetActive(false);
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    IEnumerator ShowStartRoundText(float startRoundDelay, float flickerSpeed)
    {
        yield return new WaitForSeconds(startRoundDelay);
        var nextRoundText = _nextRoundTextObj.GetComponent<Text>();
        nextRoundText.text = $"{_startGameDefaultValue}";
        GameConfig.GetSpawnManager().StartSpawning();
        yield return new WaitForSeconds(flickerSpeed* 3);
        StopCoroutine(flickerStartTextRoutine);
        _nextRoundTextObj.SetActive(false);
    }

    public void ShowNextRoundUI(int currentRound)
    {
        _nextRoundTextObj = GameObject.Instantiate(_nextRoundTextPrefab);
        _nextRoundTextObj.transform.SetParent(transform, false);
        var nextRoundText = _nextRoundTextObj.GetComponent<Text>();
        nextRoundText.text = $"{_nextRoundDefaultValue} {currentRound}";
        
        flickerStartTextRoutine = StartFlickerStartText(_startTextFlickerSpeed);
        StartCoroutine(flickerStartTextRoutine);
        StartCoroutine(ShowStartRoundText(_startRoundDelaySpeed, _startTextFlickerSpeed));
        _currentRoundText.UpdateRoundText(currentRound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int currentScore, bool isPlayer2)
    {
        if(isPlayer2 == false)
        {
            _player1Score = currentScore;
            _player1ScoreTextObject.text = $"{_scoreTextDefaultValue}{currentScore}";
        }
        else if(isPlayer2 == true)
        {
            _player2Score = currentScore;
            if (_player2ScoreTextObject.gameObject.activeSelf == false)
                _player2ScoreTextObject.gameObject.SetActive(true);
            _player2ScoreTextObject.text = $"{_scoreTextDefaultValue}{currentScore}";

        }

    }

    public void GetHighScore()
    {
        _currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreText();
    }

    public void CheckForHighScore()
    {
        if (_player1Score > _currentHighScore)
            _currentHighScore = _player1Score;
        else if (_player2Score > _currentHighScore)
            _currentHighScore = _player2Score;
        UpdateHighScoreText();
        PlayerPrefs.SetInt("HighScore", _currentHighScore);
        PlayerPrefs.Save();
    }

    public void UpdateHighScoreText()
    {
        if (_highScoreTextObject != null)
            _highScoreTextObject.text = $"{_highScoreDefaultValue} {_currentHighScore}";
    }

    public void UpdateLives(int currentLives, bool playerTwo)
    {
        if (currentLives > 3)
            Debug.LogError("Cannot assign more than 3 lives.");
        else if (currentLives < 0)
            Debug.LogError("Cannot assign less than 0 lives.");
        else if (playerTwo == false)
            _player1LivesImage.sprite = _livesSprites[currentLives];
        else if (playerTwo == true)
        {
            if (_player2LivesImage.gameObject.activeSelf == false)  
                _player2LivesImage.gameObject.SetActive(true);
            _player2LivesImage.sprite = _livesSprites[currentLives];
        }
    }

    IEnumerator flickerGameOverRoutine;
    IEnumerator flickerGameOver(float flickerRate)
    {
        while (true)
        {
            _gameOverTextObject.gameObject.SetActive(true);
            yield return new WaitForSeconds(flickerRate);
            _gameOverTextObject.gameObject.SetActive(false);
            yield return new WaitForSeconds(flickerRate);
        }
    }
    public void SetGameOver()
    {
        CheckForHighScore();
        flickerGameOverRoutine = flickerGameOver(_flickerRate);
        StartCoroutine(flickerGameOverRoutine);
        _restartTextObject.gameObject.SetActive(true);
        //_gameManager.GameOver();
    }

    public void StartGame()
    {
        if (flickerGameOverRoutine != null)
            StopCoroutine(flickerGameOverRoutine);
        flickerGameOverRoutine = null;
        _gameManager.StartNewGame();
        _gameOverTextObject.gameObject.SetActive(false);
        _restartTextObject.gameObject.SetActive(false);
        
    }
}

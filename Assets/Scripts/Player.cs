using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Utilities;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float _initialSpeed = 3.5f;
    [SerializeField]
    private float _currentSpeed;
    [SerializeField]
    private int _speedLevel = 1;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _currentActiveLaser;
    [SerializeField]
    private float _laserOffset = 1f;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _currentShield;
    [SerializeField]
    private int _shieldStrength = 0;
    [SerializeField]
    private int _score;
    [SerializeField]
    private bool _isPlayer2;
    [SerializeField]
    private LocationObject _startingLocation;
    [SerializeField]
    private Animator _thusterAnim;
    private string _anim_increase_thruster = "Increase_Thruster";
    private bool _canMove = false;
    private string playerString => _isPlayer2 ? "Player2" : "";

    //private SpawnManager _spawnManager;
    //private UIManager _uiManager;
    [SerializeField]
    private GameObject[] _damageSprites = new GameObject[2];

    //private AudioSource _audioSource;
    //private AudioClipsManager _audioClips;

    private string _anim_turning_left_bool = "Turning_Left";
    private string _anim_turning_right_bool = "Turning_Right";
    private string _anim_player_hit_bool = "Player_Hit";
    private Animator _animator;

    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private float _hitInvincibleTime = 5f;
    [SerializeField]
    private bool _invincible = false;
    [SerializeField]
    private float _sideScreenOffset = .7f;


    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            Debug.LogError($"Failed to find mesh renderer of player {name}");
        

        _animator = gameObject.GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError($"Player, {gameObject.name}, missing animator object");

        _currentActiveLaser = _laserPrefab;
        _currentSpeed = _initialSpeed;
        foreach (GameObject ds in _damageSprites)
            if(ds != null)
                ds.SetActive(false);
        StartCoroutine(ResetPlayer());
    }

    IEnumerator ResetPlayer()
    {
        yield return new WaitForSeconds(1f);
        GameConfig.GetUIManager().UpdateLives(3, _isPlayer2);
        GameConfig.GetUIManager().UpdateScore(0, _isPlayer2);

        Vector3 startingLocationPos = Vector3.up; 
        //Debug.Log($"{playerString}BeginingPosition ${this.transform.position}");
        while (this.transform.position.y < _startingLocation.Y)
        {
            //Debug.Log($"{playerString}Position ${this.transform.position}");
            Vector3 move = _currentSpeed * Time.deltaTime * startingLocationPos;
            //Debug.Log($"{playerString}Moving ${move}");
            this.transform.Translate(move);
            increaseThruster(move.y);
            yield return new WaitForEndOfFrame();
        }
        _canMove = true;
        increaseThruster(0);
    }
    // Update is called once per frame
    void Update()
    {
        if (_canMove == false)
            return;
        CalculateMovment();
        float shouldFire = 0;
#if UNITY_ANDROID || UNITY_IOS
        shouldFire = Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Fire") ? 1f : 0f;
#else
        //shouldFire = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) ? 1f : 0f;
        shouldFire = Input.GetAxis($"{playerString}Fire1");
#endif
        if (shouldFire > 0f && Time.time > _canFire)
        {

            FireLaser();
        }
    }

    void CalculateMovment()
    {
#if UNITY_ANDROID || UNITY_IOS
        float horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");// Input.GetAxis("Horizontal");
        float verticalInput = CrossPlatformInputManager.GetAxis("Vertical"); // Input.GetAxis("Vertical");
#else
        float horizontalInput =  Input.GetAxis($"{playerString}Horizontal");
        float verticalInput =  Input.GetAxis($"{playerString}Vertical");
#endif
        runTurningAnimation(horizontalInput);
        increaseThruster(verticalInput);

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        _currentSpeed = _speedLevel * _initialSpeed;
        transform.Translate(_currentSpeed * Time.deltaTime * direction);

        /*Clamp prevents the value from going above or below the values given */
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, GameDimentions.BottomOfScreen + _spriteRenderer.bounds.size.y, GameDimentions.PlayerScreenLimit));

        if (transform.position.x >= GameDimentions.RightSideOfScreen - _sideScreenOffset)
            transform.position = new Vector3(GameDimentions.LeftSideOfScreen + _sideScreenOffset, transform.position.y, 0);
        else if (transform.position.x <= GameDimentions.LeftSideOfScreen + _sideScreenOffset)
            transform.position = new Vector3(GameDimentions.RightSideOfScreen - _sideScreenOffset, transform.position.y, 0);
    }

    private void increaseThruster(float verticalInput)
    {
        if (verticalInput > 0f)
        {
            _thusterAnim.SetBool(_anim_increase_thruster, true);
        }
        else if (verticalInput <= 0f)
        {
            _thusterAnim.SetBool(_anim_increase_thruster, false);
        }
    }
    private void runTurningAnimation(float horizontalInput)
    {
        if (horizontalInput > 0f)
        {
            _animator.SetBool(_anim_turning_right_bool, true);
            _animator.SetBool(_anim_turning_left_bool, false);
        }
        else if (horizontalInput < 0f)
        {

            _animator.SetBool(_anim_turning_left_bool, true);
            _animator.SetBool(_anim_turning_right_bool, false);
        }
        else
        {
            _animator.SetBool(_anim_turning_right_bool, false);
            _animator.SetBool(_anim_turning_left_bool, false);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        Vector3 startLocation = transform.position + new Vector3(0, _laserOffset, 0);
        GameObject laserObj = GameConfig.GetSpawnManager().SpawnLaser(_currentActiveLaser, startLocation);
        
        MasterLaser mlaser = laserObj.GetComponent<MasterLaser>();
        if (mlaser)
        {
            mlaser.SetPlayer(this);
            GameConfig.GetAudioClipsManager()?.PlayClip(this.gameObject, mlaser.GetLaserSound());
        }
        else
        {
            Debug.LogError($"Laser {laserObj.name} missing sound.");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            Damage();
        if(other.CompareTag("Enemy_Laser"))
        {
            Damage();
            other.GetComponent<ChildLaser>()?.DestroyMe();
        }
    }

    public void Damage()
    {
        if(_shieldStrength > 0)
        {
            _shieldStrength--;
            if(_shieldStrength == 0)
                RemoveShield();
            if (_invincible)
                return;
        }
        else
        {
            if (_invincible)
                return;
            GameConfig.GetUIManager().UpdateLives(--_lives, _isPlayer2);
            switch (_lives)
            {
                case 2:
                    GameObject ds = _damageSprites[Random.Range(0, _damageSprites.Length)];
                    if(ds != null)
                        ds.SetActive(true);
                    break;
                case 1:
                    foreach(GameObject ds2 in _damageSprites)
                        if(ds2 != null)
                            ds2.SetActive(true);
                    break;
                case 3:
                    break;
                default:
                    GameConfig.GetGameManager().OnPlayerDeath();
                    Object.Destroy(this.gameObject);
                    break;
            }
        }
        StartCoroutine(HitInvincibleTimeout(_hitInvincibleTime));
    }

    IEnumerator HitInvincibleTimeout(float timeOutTime)
    {
        _invincible = true;
        _animator.SetBool(_anim_player_hit_bool, true);
        yield return new WaitForSeconds(timeOutTime);
        _animator.SetBool(_anim_player_hit_bool, false);
        _invincible = false;
    }

    IEnumerator laserPowerUpRoutine;
    IEnumerator LaserPowerUpTimeout(float timeOutTime)
    {
        yield return new WaitForSeconds(timeOutTime);
        _currentActiveLaser = _laserPrefab;
        laserPowerUpRoutine = null;
    }

    IEnumerator speedPowerpURoutine;
    IEnumerator SpeedPowerUpTimeout(float timeOutTime)
    {
        _speedLevel++;
        while (_speedLevel > 1)
        {
            yield return new WaitForSeconds(timeOutTime);
            _speedLevel--;
        }
    }

    IEnumerator shieldPowerupRoutine;
    IEnumerator ShieldPowerupTimeout(float timeOutTime)
    {
        if(_shieldStrength > 0)
        {
            yield return new WaitForSeconds(timeOutTime);
            if(_shieldStrength > 0)
            {
                RemoveShield();
            }
        }
    }

    public void RemoveShield()
    {
        GameObject.Destroy(_currentShield);
        _currentShield = null;
        _shieldStrength = 0;
    }

    public void CollectPowerup(GameObject powerUpObj, float timeout)
    {
        Powerup powerup = powerUpObj.GetComponent<Powerup>();
        if(powerup != null)
            switch(powerup.GetPowerupType())
            {
                case PowerupType.Laser:
                    GameObject laserObject = powerup.GetPowerupObject();
                    if(laserObject != null)
                    {
                        _currentActiveLaser = laserObject;
                        if (laserPowerUpRoutine != null)
                            StopCoroutine(laserPowerUpRoutine);
                        laserPowerUpRoutine = LaserPowerUpTimeout(timeout);
                        StartCoroutine(laserPowerUpRoutine);
                    } 
                    else
                    {
                        Debug.LogError($"Powerup {powerUpObj.name} is missing laser object.");
                    }
                    break;
                case PowerupType.Speed:
                    if (speedPowerpURoutine != null)
                        StopCoroutine(speedPowerpURoutine);
                    speedPowerpURoutine = SpeedPowerUpTimeout(timeout);
                    StartCoroutine(speedPowerpURoutine);
                    break;
                case PowerupType.Shield:
                    if (shieldPowerupRoutine != null)
                        StopCoroutine(shieldPowerupRoutine);
                    if (_currentShield != null)
                        RemoveShield();
                    GameObject shieldObject = powerup?.GetPowerupObject();
                    if(shieldObject != null)
                    {
                        _currentShield = Instantiate(shieldObject, this.transform, false);
                        _shieldStrength = powerup.GetPowerupStrength();
                        shieldPowerupRoutine = ShieldPowerupTimeout(timeout);
                        StartCoroutine(shieldPowerupRoutine);
                    }
                    else
                    {
                        Debug.LogError($"Shield powerup {powerUpObj.name} is missing powerup object.");
                    }
                    break;
                default:
                    Debug.LogError($"Power up type of {powerUpObj.name} is unknown.");
                    break;
            }
    }

    private IEnumerator UpdateScoreRoutine(int enemyScore)
    {
        _score += enemyScore;
        GameConfig.GetUIManager()?.UpdateScore(_score, _isPlayer2);
        yield return null;
    }
    public void UpdateScore(int enemyScore)
    {
        StartCoroutine(UpdateScoreRoutine(enemyScore));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4;
    private float _bottomOfScreen = GameDimentions.BottomOfScreen;
    private float _topOfScreen = GameDimentions.TopOfScreen;
    private float _xSidesLimiter = GameDimentions.RightSideOfScreen;
    [SerializeField]
    private int _scoreValue = 10;
    
    private bool _destroyed;
    private bool _hit;
    private AudioClipsManager _audioClips;

    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private float _currentFireRate;
    [SerializeField]
    private RangeList _fireRateRange = new RangeList(3f, 7f);

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Failed to find animator.");
        _audioClips = GameConfig.GetAudioClipsManager();
        _currentFireRate = _fireRateRange.GetRandomNumber();
        enemyFireRoutine = RunEnemyFireRoutine();
        StartCoroutine(enemyFireRoutine);
    }

    IEnumerator enemyFireRoutine;
    IEnumerator RunEnemyFireRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        FireLaser();
        while (true)
        {
            yield return new WaitForSeconds(_currentFireRate);
            FireLaser();
        }
    }
    void FireLaser()
    {
        MasterLaser laser = _laser.GetComponent<MasterLaser>();
        if(laser != null)
            GameConfig.GetAudioClipsManager().PlayClip(this.gameObject, laser.GetLaserSound());
        if(!_hit && !_destroyed)
        {
            //var obj = Instantiate(_laser, transform.position, Quaternion.identity);
            //obj.transform.SetParent(transform, false);
            GameConfig.GetSpawnManager().SpawnLaser(_laser, transform.position);
        }
    }
    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);
        if (transform.position.y < _bottomOfScreen)//&& !_destroyed)
        {
            //if (_hit || _destroyed)
                GameObject.Destroy(gameObject);
            //transform.position = GameDimentions.GetRandomEnemyStartPos();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"Hit: {other.transform.name}");
        //if (_destroyed)
        //    return;
        
        //Debug.Log($"Hit: {other.transform.tag}");
        if(other.CompareTag("Player"))
        {
            _hit = true;
        }
        if (!_hit)
        {
            if(other.CompareTag("Laser"))
            {
                ChildLaser cLaser = other.gameObject.GetComponent<ChildLaser>();
                if(cLaser != null)
                {
                    cLaser.UpdateScore(_scoreValue);
                    cLaser.DestroyMe();
                }
                _hit = true;
            }
         }
        if (_hit)
            DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        DestroyChildren();
        _animator.SetTrigger(_onDeathAnim);
        //_speed = 0;
        StartCoroutine(OnEnemyDeath());
        _audioClips.PlayExplosionClip(gameObject);
    }

    private void DestroyChildren()
    {
        //Debug.Log($"Enemy has {this.transform.childCount} children");
        if(this.transform.childCount > 0)
        {
            for(int i = 0; i < this.transform.childCount; i++)
            {
                Transform child =  this.transform.GetChild(i);
                //Debug.Log($"")
                if(!child.CompareTag("Enemy_Laser"))
                    GameObject.Destroy(child.gameObject, .5f);
            }
        }
    }

    private string _onDeathAnim = "OnEnemyDeath";
    private IEnumerator OnEnemyDeath()
    {
        yield return new WaitForSeconds(1f);
        Destroy(GetComponent<Collider2D>());
        while(_animator.GetCurrentAnimatorStateInfo(0).IsName(_onDeathAnim))
            yield return new WaitForEndOfFrame();

        //Debug.Log("End of animation");
        //GameObject.Destroy(this.gameObject);
        _destroyed = true;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Astroid : MonoBehaviour
{
    [SerializeField]
    private RangeList _rotateSpeedRange = new RangeList(20f, 100f);
    [SerializeField]
    private float _currentRotateSpeed;
    [SerializeField]
    private RangeList _moveSpeedRange = new RangeList(1f, 8f);
    [SerializeField]
    private float _currentMoveSpeed;
    [SerializeField]
    private RangeList _sizeRange = new RangeList(0.2f, 1f);
    [SerializeField]
    private float _currentSize;
    [SerializeField]
    private int _scoreValue = 5;
    private const string MASTER_ASTROID = "Master_Astroid";

    [SerializeField]
    private GameObject _explosionPrefab;

    private bool _destroyed = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _currentRotateSpeed = Random.Range(_rotateSpeedRange.MinValue, _rotateSpeedRange.MaxValue);
        _currentMoveSpeed = Random.Range(_moveSpeedRange.MinValue, _moveSpeedRange.MaxValue);
        _currentSize = Random.Range(_sizeRange.MinValue, _sizeRange.MaxValue);
        transform.localScale = new Vector3(_currentSize, _currentSize, _currentSize);
    }

    // Update is called once per frame
    void Update()
    {
        if(name != MASTER_ASTROID)
        {
            transform.Translate(_currentMoveSpeed * Time.deltaTime * Vector3.down, Space.World);
            if (transform.position.y < GameDimentions.BottomOfScreen)//&& !_destroyed)
            {
                //if (_hit || _destroyed)
                GameObject.Destroy(gameObject);
                //transform.position = GameDimentions.GetRandomEnemyStartPos();

            }
        }

        transform.Rotate(new Vector3(0, 0, _currentRotateSpeed * Time.deltaTime), Space.World);

    }
    private GameObject _explosionObj;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_destroyed == false)
        {
            if(other.CompareTag("Laser") || other.CompareTag("Player"))
            {
                _destroyed = true;
                gameObject.GetComponent<SpriteRenderer>().enabled =false;
                _explosionObj = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                _explosionObj.transform.localScale = this.transform.localScale;
                ChildLaser cLaser = other.gameObject.GetComponent<ChildLaser>();
                if (cLaser != null)
                {
                    cLaser.UpdateScore(_scoreValue);
                    GameObject.Destroy(other.gameObject);
                }
                if (other.CompareTag("Player"))
                {
                    Player player = other.GetComponent<Player>();
                    player.Damage();
                }
                if(name == MASTER_ASTROID)
                    GameConfig.GetGameManager()?.StartGame();

                GameObject.Destroy(this.gameObject, 0.25f);
            }
        }
    }

}

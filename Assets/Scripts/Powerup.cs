using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum PowerupType
{
    Shield,
    Speed,
    Laser
}
public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private GameObject _powerUp;

    [SerializeField]
    private float _powerUpTimeout = 5f;
    [SerializeField]
    private int _strength = 1;
    [SerializeField]
    private AudioClip _powerupSound;
    private AudioClipsManager _audioClipsManager;
    // Start is called before the first frame update
    [SerializeField]
    private PowerupType _powerupType;
    
    void Start()
    {
        _audioClipsManager = GameConfig.GetAudioClipsManager();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.down);
        
        if(transform.position.y <= GameDimentions.BottomOfScreen)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Player>()?.CollectPowerup(this.gameObject, _powerUpTimeout);
            _audioClipsManager.PlayClip(other.gameObject, _powerupSound);
            GameObject.Destroy(this.gameObject);
        }
    }

    public PowerupType GetPowerupType()
    {
        return _powerupType;
    }
    public GameObject GetPowerupObject()
    {
        return _powerUp;
    }

    public int GetPowerupStrength()
    {
        return _strength;
    }
}

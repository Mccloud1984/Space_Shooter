using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserDirection
{
    forward,
    left,
    right,
    down,
}

public class ChildLaser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private LaserDirection _laserDirection = LaserDirection.forward;
    [SerializeField]
    private float _directionDivergenceOffset = .1f;
    private Player _player;

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (GameDimentions.IsOutsideScreenBoundries(transform))
            DestroyMe();
    }
    private void CalculateMovement()
    {
        Vector3 direction = Vector3.up;
        if (_laserDirection == LaserDirection.left)
            direction += Vector3.left * _directionDivergenceOffset;
        if (_laserDirection == LaserDirection.right)
            direction += Vector3.right * _directionDivergenceOffset;
        if (_laserDirection == LaserDirection.down)
            direction = Vector3.down;

        Vector3 move = _speed * Time.deltaTime * direction;
        transform.Translate(move);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void DestroyMe()
    {
        StartCoroutine(DestoryMe(Random.Range(0f, .1f), transform.parent?.gameObject));
    }

    IEnumerator DestoryMe(float randomTime, GameObject parent)
    {
        yield return new WaitForSeconds(randomTime);

        if (parent != null && parent.GetComponent<MasterLaser>() != null)
            if (parent.transform.childCount <= 1)
                GameObject.Destroy(parent);

        GameObject.Destroy(gameObject);
    }

    public void UpdateScore(int scoreValue)
    {
        if (_player != null)
            _player.UpdateScore(scoreValue);
    }
}

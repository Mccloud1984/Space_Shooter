using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterLaser : MonoBehaviour
{
    [SerializeField]
    private AudioClip _laserSound;
    private Player _player;

    public AudioClip GetLaserSound()
    {
        return _laserSound;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        ChildLaser[] children = gameObject.GetComponentsInChildren<ChildLaser>();
        if (children != null)
            foreach (ChildLaser child in children)
                child.SetPlayer(player);
    }
}

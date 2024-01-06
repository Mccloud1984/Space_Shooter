using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipsManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionClip;

    public void PlayExplosionClip(GameObject sprite)
    {
        PlayClip(sprite, _explosionClip);
    }

    internal void PlayClip(GameObject sprite, AudioClip clip)
    {
        AudioSource source = sprite.GetComponent<AudioSource>();
        if (source == null)
            Debug.LogError($"Failed to find audio source on {sprite.name}");
        source.PlayOneShot(clip);
    }
}

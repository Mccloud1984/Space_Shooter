using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(AudioSource))]
public class Explosion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RemoveExplosion());
        GameConfig.GetAudioClipsManager().PlayExplosionClip(gameObject);
    }

    IEnumerator RemoveExplosion()
    {
        Animator anim = GetComponent<Animator>();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

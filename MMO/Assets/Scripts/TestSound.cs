using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    int i = 0;
    private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();
        //audio.PlayOneShot(audioClip1);
        //audio.PlayOneShot(audioClip2);

        //float lifeTime = Mathf.Max(audioClip1.length, audioClip2.length);
        //GameObject.Destroy(gameObject, lifeTime);

        i++;

        if (i % 2 == 0)
            Managers.Sound.Play(audioClip1, Define.Sound.Bgm);
        else
            Managers.Sound.Play(audioClip2, Define.Sound.Bgm);
    }
}

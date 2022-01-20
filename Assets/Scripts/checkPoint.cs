using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPoint : MonoBehaviour
{
    public bool sudahPernah = false; // Menampung variabel apakah sudah pernah membuat spawn di cek poin tersebut
    public AudioSource sfx;

    void Awake()
    {
        sfx = gameObject.GetComponent<AudioSource>();
    }

    public void playMusic()
    {
        sfx.Play();
    }
}

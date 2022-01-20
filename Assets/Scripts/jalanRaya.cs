using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jalanRaya : MonoBehaviour, Levers
{
    public GameObject player; // Referensi objek player
    private player pr; // Referensi skrip player

    public GameObject jalanan;
    public SpriteRenderer lever;
    public Sprite[] leverImages = null; // Menampung sprite lever
    public SpriteRenderer lampu;
    public Sprite[] lampuImages = null; // Menampung sprite lever
    
    public GameObject soal = null; // Referensi objek soal
    

    // Start is called before the first frame update
    void Start()
    {
        pr = player.GetComponent<player>(); // Menentukan komponen skrip player
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy"){
            pr.mati(); // Mati
        }
    }

    public void toggleLever()
    {
        if(jalanan.tag == "Ground")
        {
            gameObject.GetComponent<AudioSource>().Play();
            jalanan.tag = "Spike";
            lever.sprite = leverImages[0]; // Mengubah sprite lever menajadi mati
            lampu.sprite = lampuImages[0];
        } else {
            if(!soal.GetComponent<pertanyaan>().pilihanBenar) // Jika pilihan belum benar
            {
                soal.SetActive(true); // Menampilkan soal
            } else {
                gameObject.GetComponent<AudioSource>().Play();
                jalanan.tag = "Ground";
                lever.sprite = leverImages[1]; // Mengubah sprite lever menajadi mati
                lampu.sprite = lampuImages[1];
            }
        }
    }

    public GameObject getKomponen()
    {
        return gameObject;
    }
}

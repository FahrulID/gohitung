using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;

public interface Levers
{
    GameObject getKomponen();
}

public class player : MonoBehaviour
{
    // Untuk menyimpan setting menu
    public pengaturMenu menu;

    // Variabel untuk nyawa player
    private int maxNyawa = 3;
    private int nyawa;
    private int lastNyawa;

    // Gambar hati untuk nyawa
    public Image[] spriteNyawa;

    private float ms = 20f; // Kekuatan gerak
    private float jp = 70f; // Kekuatan loncat

    private float gravity = -6.81f; // Gravitasi
    private float gravityMultiplier = 1.8f; // Meningkatkan kecepatan saat jatuh
    private float groundDrag = 30f; // Berhenti perlahan di tanah
    private float airDrag = 45f; // Berhenti perlahan di udara

    public GameObject sprite; // Gambar player
    public Transform GroundCheck; // Objek untuk mengecek di atas tanah atau tidak
    public LayerMask groundLayer; // Layer untuk mengecek di atas tanah atau tidak
    public LayerMask finishLayer; // Layer untuk mengecek di atas tanah atau tidak

    public Rigidbody2D rb; // Komponen rigidbody2d player
    public Animator anim; // Animasi player
    private float h; // Input gerak horizontal
    private float v; // Input gerak vertikal
    public bool isGrounded; // Variabel untuk menyimpan apakah player di atas tanah

    private bool nearLever; // Jika berada di dekat lever
    private bool onPlatform; // Variabel untuk menyimpan apakah player berada di atas platform
    public Button[] toggleButton; // Objek tombol untuk toggle platform
    private Levers lever; // Referensi Script platform

    private Vector3 spawnPoint = new Vector3(); // Titik respawn

    private Vector2 Velocity = new Vector2(0, 0); // Velocity player ( dihitung dari input horizontal dan vertikal )
    private Vector2 ExtraVeloc = new Vector2(0, 0); // Kecepatan tambahan ( Untuk di atas platform agar bergerak bersama dengan platform )
    private Vector2 Speed = new Vector2(0, 0); // Kecepatan pergerakan player
    private Vector2 lastPos = new Vector2(0, 0); // Posisi terakhir player sebelum fixedUpdate()

    private bool died = false; // Variabel untuk menyimpan apakah player sudah mati 

    private AudioSource suaraLoncat;
    public AudioSource suaraHurt;
    public AudioSource suaraKalah;
    public AudioSource suaraMenang;
    public AudioSource BGM;

    public bool isFreeze = false;

    public GameObject kalahScreen;
    public GameObject overScreen;
    public GameObject menangScreen;


    public void Start()
    {
        suaraLoncat = gameObject.GetComponent<AudioSource>();
        if(GameObject.Find("PengaturMenu") != null)
        {
            menu = GameObject.Find("PengaturMenu").GetComponent<pengaturMenu>();
        }
        nyawa = lastNyawa = maxNyawa; // Mendeklarasikan nyawa
        setCheckPoint(transform.position.x, transform.position.y, transform.position.z); // Menentukan titik spawn player ketika game dimulai
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        die(); // Mengecek untuk game over maupun jatuh ke bawah

        if(!isFreeze)
        {
            isGrounded = Physics2D.OverlapCircle(GroundCheck.position, 0.08f, groundLayer); // Mengecek apakah posisi obyek GroundCheck mendekati layer "Ground" jika iya maka isGrounded = true

            // if(Physics2D.OverlapCircle(GroundCheck.position, 0.5f, finishLayer))
            // {
            //     BGM.Pause();
            //     menangScreen.SetActive(true);
            //     StartCoroutine(PlayMusic(suaraMenang, menang));
            // }

            horizMovement(); // Menangani pergerakkan horizontal

            vertMovement(); // Menangani pergerakkan vertikal

            flip(); // Menangani flip sprite player agar berbalik arah kanan dan kiri

            animasi(); // Mengatur animasi

            if(isGrounded && v > 0)
            {
                suaraLoncat.Play();
            }

            movement(); // Penggerakan Player
        }
    }

    public void die()
    {
        
        if(transform.position.y <= -25f) // Jika player berada dibawah titik -25
        {
            mati(); // Mati
        }

        if(nyawa < lastNyawa) // Jika nyawa berkurang
        {
            if(nyawa >= 0) // Jika nyawa lebih besar dari sama dengan 0
            {
                for(int x = maxNyawa-1; x > nyawa-1; x--) // Loop mundur sebanyak nyawa yang hilang
                {
                    spriteNyawa[x].enabled = false; // Menyembunyikan sprite nyawa
                }
                lastNyawa-=1; // Mengurangi nyawa
            }
        }
    }

    void animasi()
    {
        // Animasi gerak kiri kanan
        anim.SetFloat("Speed", Mathf.Abs(h * ms)); // Mengirimkan data kecepatan horizontal ke Animator "Speed"
        // Animasi gerak atas bawah
        anim.SetFloat("Vertical", Velocity.y); // Mengirimkan data kecepatan vertikal ke Animator "Vertical"
    }

    void horizMovement()
    {
        // h = Input.GetAxisRaw("Horizontal"); // Mengambil input pergerakkan horizontal dari player
        h = CrossPlatformInputManager.GetAxis("Horizontal"); // Untuk pergerakkan dengan tombol

        if(h == 0) // Jika H sama dengan 0, yang artinya player sedang tidak menekan tombol gerak
        {
            if(isGrounded && (Speed.x > 0 || Speed.x < 0) && !onPlatform) // Jika player menyentuh tanah dan kecepatannya tidak 0 serta tidak sedang berada di atas platform
            {
                Velocity.x *= groundDrag * Time.deltaTime; // Memperlambat gerakan horizontal ketika di atas tanah
            } else if(!isGrounded && (Speed.x > 0 || Speed.x < 0) && !onPlatform) { // Jika player tidak menyentuh tanah dan kecepatannya tidak 0 serta tidak sedang berada di atas platform
                Velocity.x *= airDrag * Time.deltaTime; // Memperlambat gerakan horizontal ketika di udara
            }

            if(onPlatform) // Jika sedang berada di atas platform
            {
                addVelocityX(ExtraVeloc.x); // Menambah velocity tambahan agar dapat mengikuti pergerakkan platform
            }
        } else {
            // Move Kiri Kanan
            addVelocityX(h * ms * Time.deltaTime + ExtraVeloc.x); // Menambah velocity agar bisa bergerak kiri dan kanan
        }
    }

    void vertMovement()
    {
        // v = Input.GetAxisRaw("Vertical"); // Mengambil input pergerakkan vertikal dari player
        v = CrossPlatformInputManager.GetAxis("Vertical"); // Untuk pergerakkan dengan tombol

        if(onPlatform && v == 0)
        {
            v = -1;
        }

        if(!isGrounded) // Jika sedang tidak berada di atas tanah
        {
            // Menghitung titik balik lompatan / jatuh
            if(Speed.y < 0) // Jika sedang jatuh yang mana ditandai dengan kecepatan vertikal minus
            {
                Velocity.y += gravity * Time.deltaTime * gravityMultiplier; // Menambah kecepatan sesuai gravityMultiplier
            } else if (Speed.y > 0) // Jika sedang loncat yang mana ditandai dengan kecepatan vertikal plus
            {
                Velocity.y += gravity * Time.deltaTime; // Mengurangi kecepatan sesuai gravitasi
            }
        } else {
            addVelocityY(v * jp * Time.deltaTime); // Menambah velocity agar bisa meloncat
        }
    }

    public void addExtraVelocity(float velx, float vely)
    {
        ExtraVeloc.x = velx; // Mengubah variabel extra velocity.x
        ExtraVeloc.y = vely; // Mengubah variabel extra velocity.y
    }

    public void addVelocityX(float velx)
    {
        Velocity.x = velx; // Mengubah variabel velocity.x
    }

    public void addVelocityY(float vely)
    {
        Velocity.y = vely; // Mengubah variabel velocity.y
    }

    void movement()
    {
        rb.MovePosition(new Vector2(transform.position.x + Velocity.x, transform.position.y + Velocity.y)); // Menggerakan player sesuai velocity
        
        // Menghitung kecepatan
        Speed = (rb.position - lastPos); // Kecepatan laju player
        lastPos = rb.position; // Posisi terakhir player
    }

    void flip()
    {
        // Flip character ketika mengarah ke lain arah
        if(h>0) // Jika mengarah ke kanan
        {
            sprite.transform.eulerAngles = new Vector3(0, 0, 0); // Normal, Menampilkan gambar player mengarah ke kanan
        } else if (h<0) // Jika mengarah ke kiri
        {
            sprite.transform.eulerAngles = new Vector3(0, 180, 0); // Flipped, Menampilkan gambar player mengarah ke kiri
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform" || col.gameObject.tag == "CheckPoint"){ // Jika player masuk collision dengan tanah, platform maupun cek poin
            anim.SetBool("Grounded", isGrounded); // Animasi diubah menjadi sedang di atas tanah
        }
        
        if (col.gameObject.tag == "Finish"){ // Jika menyentuh finish
            BGM.Pause();
            menangScreen.SetActive(true);
            StartCoroutine(PlayMusic(suaraMenang, menang));
        }
        
        if (col.gameObject.tag == "CheckPoint"){ // Apabila masuk collision dengan tag cek poin
            if(!col.gameObject.GetComponent<checkPoint>().sudahPernah) // Jika cek poin belum pernah di lewati
            {
                setCheckPointHere(); // Mengatur tempat spawn di tempat sekarang
                col.gameObject.GetComponent<checkPoint>().sudahPernah = true; // Mengubah variabel sudah pernah menjadi true
                col.gameObject.GetComponent<checkPoint>().playMusic();
            }
        }

        if (col.gameObject.tag == "Enemy"){ // Ketika palyer bersentuhan dengan spike maupun musuh
            mati(); // Mati
        }
    }
    
    // Mengecek Collision Exit
    void OnCollisionExit2D(Collision2D col){
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform" || col.gameObject.tag == "CheckPoint"){ // Apabila keluar dari collision dengan tag ground, platform maupun cek poin
            anim.SetBool("Grounded", isGrounded); // Animasi diubah menjadi sedang loncat
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Platform"){ // Jika bersentuhan dengan platform
            lever = other.GetComponent<Levers>();
            if(lever.getKomponen().GetComponent<moving_platform>().toggleable) // Mengecek apakah platform bisa di toggle
            {
                toggleButton[0].gameObject.SetActive(true);
                toggleButton[0].enabled = true; // Menampilan tombol toggle di layar
                toggleButton[0].GetComponent<Image>().enabled = true; // Manmpilkan gambar tombol toggle di layar
            }
            onPlatform = true; // Mengubah variabel onPlatform
        }

        if (other.gameObject.tag == "LampuMerahToggle"){ // Jika bersentuhan dengan platform
            lever = other.transform.parent.gameObject.GetComponent<Levers>();

            toggleButton[1].gameObject.SetActive(true);
            toggleButton[1].enabled = true; // Menampilan tombol toggle di layar
            toggleButton[1].GetComponent<Image>().enabled = true; // Manmpilkan gambar tombol toggle di layar

            onPlatform = true; // Mengubah variabel onPlatform
        }

        if (other.gameObject.tag == "Lever" || other.gameObject.tag == "LampuMerahToggle"){ // Jika keluar bersentuhan dengan Lever
            nearLever = true; // Near lever menjadi true
        }

        if (other.gameObject.tag == "Finish"){ // Jika menyentuh finish
            BGM.Pause();
            menangScreen.SetActive(true);
            StartCoroutine(PlayMusic(suaraMenang, menang));
        }

        if (other.gameObject.tag == "Lever" || other.gameObject.tag == "LampuMerahToggle"){ // Jika bersentuhan dengan Lever
            nearLever = true; // Near lever menjadi true
        }

        if (other.gameObject.tag == "Spike"){ // Ketika palyer bersentuhan dengan spike maupun musuh
            mati(); // Mati
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Platform"){ // ketika keluar dari platform
            v = 0;
            if(lever.getKomponen().GetComponent<moving_platform>().toggleable) // Jika platformnya toggleable
            {
                toggleButton[0].gameObject.SetActive(false);
                toggleButton[0].enabled = false; // Menghapus tombol di layar
                toggleButton[0].GetComponent<Image>().enabled = false; // Menghapus gambar tombol di layar
            }
            lever = null; // Menghapus referensi
            onPlatform = false; // Mengubah value variabel menjadi false
        }
        if (other.gameObject.tag == "LampuMerahToggle"){ // ketika keluar dari platform

            toggleButton[1].gameObject.SetActive(false);
            toggleButton[1].enabled = false; // Menghapus tombol di layar
            toggleButton[1].GetComponent<Image>().enabled = false; // Menghapus gambar tombol di layar

            lever = null; // Menghapus referensi
        }

        if (other.gameObject.tag == "Lever" || other.gameObject.tag == "LampuMerahToggle"){ // Jika bersentuhan dengan Lever
            nearLever = false; // Near lever menjadi false
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // if (other.gameObject.tag == "Platform"){ // Jika bersentuhan dengan platform
        //     lever = other.GetComponent<Levers>();
        //     if(lever.getKomponen().GetComponent<moving_platform>().toggleable) // Mengecek apakah platform bisa di toggle
        //     {
        //         toggleButton[0].gameObject.SetActive(true);
        //         toggleButton[0].enabled = true; // Menampilan tombol toggle di layar
        //         toggleButton[0].GetComponent<Image>().enabled = true; // Manmpilkan gambar tombol toggle di layar
        //     }
        //     onPlatform = true; // Mengubah variabel onPlatform
        // }

        if (other.gameObject.tag == "LampuMerahToggle"){ // Jika bersentuhan dengan platform
            lever = other.transform.parent.gameObject.GetComponent<Levers>();

            toggleButton[1].gameObject.SetActive(true);
            toggleButton[1].enabled = true; // Menampilan tombol toggle di layar
            toggleButton[1].GetComponent<Image>().enabled = true; // Manmpilkan gambar tombol toggle di layar
        }

        if (other.gameObject.tag == "Lever" || other.gameObject.tag == "LampuMerahToggle"){ // Jika keluar bersentuhan dengan Lever
            nearLever = true; // Near lever menjadi true
        }
    }

    public void mati()
    {
        if(!died) // Jika belum mati ( artinya sedang akan mati )
        {
            died = true; // Died jadi true agar tidak mengulang lagi memanggil fungsi ini
            BGM.Pause();
            kalahScreen.SetActive(true);
            transform.position = spawnPoint; // Memindahkan posisi player ke cek poin
            StartCoroutine(PlayMusic(suaraHurt, jalankanMati));
            
        }
    }  

    void jalankanMati()
    {
        nyawa--; // nyawa dikurangi 1
        respawn(); // Respawn
    }

    IEnumerator PlayMusic(AudioSource suara, Action action)
    {
        suara.Play();
        yield return new WaitWhile (()=> suara.isPlaying);
        action();
    }

    void respawn()
    {
        if(nyawa >= 0) // Jika masih memiliki nyawa, maka 
        {
            transform.position = spawnPoint; // Memindahkan posisi player ke cek poin
            died = false; // Mengubah Died menjadi false
            kalahScreen.SetActive(false);
            BGM.UnPause();
        } else {
            kalahScreen.SetActive(false);
            overScreen.SetActive(true);
            StartCoroutine(PlayMusic(suaraKalah, kalah));
        }
    }

    void kalah()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load ulang level
    }

    public void setCheckPoint(float x, float y, float z)
    {
        spawnPoint.x = x; // Set cek poin koordinat x
        spawnPoint.y = y; // Set cek poin koordinat y
        spawnPoint.z = z; // Set cek poin koordinat z
    }

    public void setCheckPointHere()
    {
        spawnPoint = transform.position; // Set cek poin koordinat player sekarang
    }

    public void togglePlatform()
    {
        if(nearLever) // Jika berada di dekat lever
        {
            if(lever!=null) // Jika ada referensi ke platform
            {
                if(lever.getKomponen().GetComponent<moving_platform>().toggleable) // Jika platform bisa di toggle
                {
                    lever.getKomponen().GetComponent<moving_platform>().toggleLever(); // Memanggil fungsi toggle di platform
                    toggleButton[0].gameObject.SetActive(false);
                    toggleButton[0].enabled = false; // Menampilan tombol toggle di layar
                    toggleButton[0].GetComponent<Image>().enabled = false; // Manmpilkan gambar tombol toggle di layar
                }
            }
        }
    }

    public void toggleLampuMerah()
    {
        if(nearLever) // Jika berada di dekat lever
        {
            if(lever!=null) // Jika ada referensi ke platform
            {
                lever.getKomponen().GetComponent<jalanRaya>().toggleLever(); // Memanggil fungsi toggle di jalanRaya
            }
        }
    }

    void menang()
    {
        if(menu != null)
        {
            if(menu.latestLevel < 5)
            {
                menu.levelTerbuka[SceneManager.GetActiveScene().buildIndex] = true;
                menu.latestLevel = SceneManager.GetActiveScene().buildIndex + 1;
            }
        }
        SceneManager.LoadScene(0); // Kembali ke menu utama
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void exit()
    {
        SceneManager.LoadScene(0);
    }
}

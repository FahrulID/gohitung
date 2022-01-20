using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_platform : MonoBehaviour, Levers
{
    public GameObject player; // Referensi objek player
    private player pr; // Referensi skrip player

    public Rigidbody2D rb; // Referensi rigidbody platform

    public bool pertanyaan; // Apakah platform memiliki pertanyaan
    public GameObject soal = null; // Referensi objek soal
    public bool togglePertanyaan; // Variable toggle pertanyaan

    public bool toggleable = false; // Variable yang menentukan apakah platform bisa ditoggle

    public SpriteRenderer lever = null; // Menampung obyek lever
    public Sprite[] leverImages = null; // Menampung sprite lever

    public bool bergerak = false; // Menentukan apakah platform bergerak atau tidak

    public bool wrongDie = false; // Menentukan apakah salah pilihan akan mati atau tidak
    public bool falling = false; // Menentukan apakah platform sedang jatuh
    public bool fallback = false; // Menentukan titik balik ketika platform jatuh

    public Vector3 asal = new Vector3(); // Menentukan asal tempat platform
    public Vector3 awal = new Vector3(); // Menentukan awal tempat platform
    public Vector3 sekarang = new Vector3(); // Menentukan posisi sekarang platform
    public Vector3 tujuan = new Vector3(); // Menentukan posisi tujuan platform
    public Vector3 arah; // Menentukan arah pergerakkan

    public bool toTujuan = true; // Menentukan mengarah ke tujuan / asal
    public bool oneTime = false;
    public float ms = 10f; // Kecepatan platform

    public float tolerance = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if(oneTime)
        {
            oneTime = false;
        }
        asal = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Menentukan posisi asal dari sebuah platform
        pr = player.GetComponent<player>(); // Menentukan komponen skrip player

        if(!pertanyaan) // Jika bukan merupakan platform pertanyaan
        {
            toggleLever(); // Menyalakan platform
            bergerak = true;
        }

        if(pertanyaan) // Jika merupakan paltform pertanyaaan
        {
            togglePertanyaan = false; // Ubah toggle pertanyaan menjadi false
            bergerak = false; // Bergerak = false
        }

        if(soal != null) // Jika referensi soal tidak kosong
        {
            soal.GetComponent<pertanyaan>().setObyek(gameObject); // Mengirimkan obyek referensi platform ini ke skrip soal
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(bergerak) // Jika platformnya bisa bergerak
        {
            gerak(); // Bergerak  
        }

        if(falling && wrongDie)
        {
            falling = false;
        }

        if(falling && !wrongDie) // Jika platformnya sedang jatuh dan jika salah tidak mati
        {
            fall(); // Jatuh
        }
    }

    public void gerak()
    {
        sekarang = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Menentukan posisi sekarang paltform
                
        Vector3 tempat; // Menampung posisi tujuan
        if(toTujuan) // Jika sedang menuju tujuan
        {
            tempat = tujuan; // posisi tujuan sama dengan posisi variabel tujuan
            awal = asal; // Posisi awal menjadi posisi asal
            arah = tujuan - sekarang; // Arahnya adalah tujuan - sekarang
        } else { // Jika sedang menuju awal
            tempat = asal; // Posisi tujuan menjadi posisi asal
            awal = tujuan; // Posisi awal menjadi posisi variabel tujuan
            arah = asal - sekarang; // Arahnya adalah asal - sekarang
        }
        arah = arah.normalized; // Normalisasi variabel arah sehingga maksimal nilanya 1 dan minimalnya -1, cocok untuk menentukan arah kiri atau kanan, atas atau bawah

        rb.MovePosition(new Vector2(sekarang.x + arah.x * ms * Time.deltaTime, sekarang.y + arah.y * ms * Time.deltaTime)); // Menggerakan objek menggunakan move position

        cekUbahArah(tempat); // Mengecek perbuahan arah ketika sampai tempat tujuan
    }

    public void cekUbahArah(Vector3 tempat)
    {
        bool gantiX = false; // Menentukan apakah harus mengganti posisi koordinat x
        bool gantiY = false; // Menentukan apakah harus mengganti posisi koordinat y
        // Menentukan arah
        if(awal.x > tempat.x) // Jika posisi koordinat x dari posisi awal lebih besar dari koordinat x dari tujuan ( Artinya tujuan berada di sebelah kiri )
        {
            // Tujuan berada di kiri
            if(sekarang.x <= tempat.x + tolerance || sekarang.x <= tempat.x - tolerance) // Jika posisi platform sekarang sudah melewati tujuan
            {
                gantiX = true; // Ganti arah koordinat x menjadi true
            }
            
        } else if ( awal.x < tempat.x) // Jika posisi koordinat x dari posisi awal lebih kecil dari koordinat x dari tujuan ( Artinya tujuan berada di sebelah kanan )
        {
            // Tujuan berada di kanan
            if(sekarang.x >= tempat.x + tolerance || sekarang.x >= tempat.x - tolerance) // Jika posisi platform sekarang sudah melewati tujuan
            {
                gantiX = true; // Ganti arah koordinat x menjadi true
            }
        } else { // Artinya posisi koordinat x sudah benar
            // Tujuannya sudah sama
            gantiX = true; // Ganti arah koordinat x menjadi true
        }

        
        if(awal.y > tempat.y) // Jika posisi koordinat y dari posisi awal lebih besar dari koordinat y dari tujuan ( Artinya tujuan berada di bawah )
        {
            // Tujuan berada di bawah
            if(sekarang.y <= tempat.y + tolerance || sekarang.y <= tempat.y - tolerance) // Jika posisi platform sekarang sudah melewati tujuan
            {
                gantiY = true; // Ganti arah koordinat y menjadi true
            }
            
        } else if ( awal.y < tempat.y) // Jika posisi koordinat y dari posisi awal lebih kecil dari koordinat y dari tujuan ( Artinya tujuan berada di atas )
        {
            // Tujuan berada di atas
            if(sekarang.y >= tempat.y + tolerance || sekarang.y >= tempat.y - tolerance) // Jika posisi platform sekarang sudah melewati tujuan
            {
                gantiY = true; // Ganti arah koordinat y menjadi true
            }
        } else {
            // Tujuannya sudah sama
            gantiY = true; // Ganti arah koordinat x menjadi true
        }

        if(gantiX && gantiY) // Jika posisi koordinat x serta y harus di ubah arahnya
        {
            toTujuan = toTujuan ? false : true; // Mengubah arah tujuan
            if(oneTime)
            {
                toggleLever();
            }
        }
        // Menentukan arah
    }

    public void toggleLever()
    {
        if(toggleable && lever != null) // Jika platformnya bisa di toggle dan referensi lever nya tidak kosong
        {
            if(bergerak) // Jika platform sedang bergerak
            {
                gameObject.GetComponent<AudioSource>().Play();
                lever.sprite = leverImages[0]; // Mengubah sprite lever menajadi mati
                bergerak = false; // Berhenti bergerak
            } else { // Jika platform sedang tidak bergerak
                if(pertanyaan && !togglePertanyaan && soal != null) // Jika merupakan platform yang memiliki eprtanyaan, dan toggle pertanyaan false serta referensi obyek soal tidak kosong
                {
                    if(!soal.GetComponent<pertanyaan>().pilihanBenar) // Jika pilihan belum benar
                    {
                        soal.SetActive(true); // Menampilkan soal
                    }
                } else { // Jika tidak
                    gameObject.GetComponent<AudioSource>().Play();
                    lever.sprite = leverImages[1]; // Mengubah sprite lever menjadi hidup
                    bergerak = true; // Bergerak
                    toggleable = false; // Mematikan fitur toggleable
                }
            }
        }
    }

    public void fall()
    {
        if(!fallback) // Jika jatuh kebawah dan bukan sedang mengarah ke atas kembali
        {
            float jatuhY = -45f; // Koordinat y tujuan
            sekarang = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Posisi platform sekarang
            if(sekarang.y > jatuhY) // Jika posisi sekarang belum melewati posisi jatuhY
            {
                Vector3 tujuan = new Vector3(asal.x, jatuhY, asal.z); // Tujuannya adalah jatuhY

                arah = tujuan - sekarang; // Arahnya adalah tujuan - sekarang
                arah = arah.normalized; // Normalize arahnya

                rb.MovePosition(new Vector2(sekarang.x + arah.x * ms * Time.deltaTime * 3f, sekarang.y + arah.y * ms * Time.deltaTime *3f)); // Menggerakan dengan kecepatan 3kali dari kecepatan biasanya
            } else { // Jika sudah melewati
                fallback = true; // Fallback menjadi true
            }
        } else { // Jika fallback = true
            sekarang = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Posisi sekarang 
            if(sekarang.y < asal.y) // Jika posisi sekarang belum melewati tujuannya
            {
                Vector3 tujuan = asal; // tujuannya adalah posisi asal

                arah = tujuan - sekarang; // Arahnya tujuan - seakrang
                arah = arah.normalized; // Normalisasi arah

                rb.MovePosition(new Vector2(sekarang.x + arah.x * ms * Time.deltaTime, sekarang.y + arah.y * ms * Time.deltaTime)); // Menggerakan platform kembali ke atas
            } else { // Jika sudah
                fallback = false; // Fallback jadikan false
                falling = false; // Berhenti jatuh
            }
        }
    }
    
    // Mengecek Collision Exit
    void OnCollisionExit2D(Collision2D col){
        if (col.gameObject.tag == "Player"){ // Jika keluar kolisi dengan player
            pr.addExtraVelocity(0f, 0f); // Ubah velocity player jadi 0 karena sudah tidak berada di atas platform
        }
    }

    // Mengecek Collision Exit
    void OnCollisionStay2D(Collision2D col){
        if (col.gameObject.tag == "Player" && bergerak){ // Jika collision dari player stay serta platformyna bergerak
            pr.addExtraVelocity(arah.x * ms * Time.deltaTime, arah.y * ms * Time.deltaTime); // Menambah velocity ke player sesuai kecepatan platform
        }
        
        if (col.gameObject.tag == "Player" && !bergerak){ // Jika collision dari player stay serta platformyna tidak bergerak
            pr.addExtraVelocity(0f, 0f); // Ubah velocity player jadi 0 karena sudah tidak bergerak paltformnya
        }
    }

    public GameObject getKomponen()
    {
        return gameObject;
    }
}

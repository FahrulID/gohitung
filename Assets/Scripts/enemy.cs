using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float ms = 15f;
    public float jp = 30f;

    private float gravity = -6.81f;
    private float gravityMultiplier = 1.8f;
    public float groundDrag = 30f;
    public float airDrag = 50f;

    public GameObject sprite;
    public Transform GroundCheck;
    public LayerMask groundLayer;

    public Rigidbody2D rb;
    public Animator anim;
    public float h;
    public float v;
    public bool isGrounded;

    public Vector2 Velocity = new Vector2(0, 0);
    public Vector2 Speed = new Vector2(0, 0);
    public Vector2 lastPos = new Vector2(0, 0);
    public Vector3 asal = new Vector3();
    public Vector3 awal = new Vector3();
    public Vector3 sekarang = new Vector3();
    public Vector3 tujuan = new Vector3();

    public bool toTujuan = true;

    public float secElapsed = 0;

    void Start()
    {
        asal = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        sekarang = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, 0.15f, groundLayer);

        Vector3 tempat;
        if(toTujuan)
        {
            tempat = tujuan;
            awal = asal;
        } else {
            tempat = asal;
            awal = tujuan;
        }

        inputMovement(tempat);

        horizMovement();

        vertMovement();

        // Flip character ketika mengarah ke lain arah
        flip();

        // Penggerak
        movement(tempat);

        // Animasi
        animasi();
    }
    
    void animasi()
    {
        // Animasi gerak atas bawah
        anim.SetFloat("Vertical", Velocity.y); // Mengirimkan data kecepatan vertikal ke Animator "Vertical"
    }

    void inputMovement(Vector3 tempat)
    {
        if(isGrounded)
        {
            secElapsed += Time.deltaTime;
            h = 0;
            v = 0;
        }

        if(secElapsed >= 1f)
        {
            secElapsed = 0;

            if(tempat.x >= sekarang.x)
            {
                h = 1;
                v = 1;
            }

            if(tempat.x <= sekarang.x)
            {
                h = -1;
                v = 1;
            }
        }
    }

    void horizMovement()
    {
        // Efek drag untuk memperlambat pergerakkan
        if(h == 0)
        {
            if(isGrounded && (Speed.x > 0 || Speed.x < 0))
            {
                Velocity.x *= groundDrag * Time.deltaTime;
            } else if(!isGrounded && (Speed.x > 0 || Speed.x < 0)) {
                Velocity.x *= airDrag * Time.deltaTime;
            }
        } else {
            // Move Kiri Kanan
            Velocity.x = h * ms * Time.deltaTime;
        }
    }

    void vertMovement()
    {
        // Move atas bawah
        if(!isGrounded)
        {
            // Menghitung titik balik lompatan / jatuh
            if(Speed.y < 0)
            {
                Velocity.y += gravity * Time.deltaTime * gravityMultiplier;
            } else if (Speed.y > 0)
            {
                Velocity.y += gravity * Time.deltaTime;
            }
        } else {
            Velocity.y = v * jp * Time.deltaTime;
        }
    }

    void movement(Vector3 tempat)
    {
        rb.MovePosition(new Vector2(transform.position.x + Velocity.x, transform.position.y + Velocity.y)); // Menggerakan player sesuai velocity
        
        // Menghitung kecepatan
        Speed = (rb.position - lastPos); // Kecepatan laju player
        lastPos = rb.position; // Posisi terakhir player

        cekUbahArah(tempat);
    }

    void cekUbahArah(Vector3 tempat)
    {
        bool gantiX = false;

        // Menentukan arah
        if(awal.x > tempat.x)
        {
            // Tujuan berada di kiri
            if(sekarang.x <= tempat.x)
            {
                gantiX = true;
            }
            
        } else if ( awal.x < tempat.x)
        {
            // Tujuan berada di kanan
            if(sekarang.x >= tempat.x)
            {
                gantiX = true;
            }
        } else {
            // Tujuannya sudah sama
            gantiX = true;
        }

        if(gantiX)
        {
            toTujuan = toTujuan ? false : true;
        }
        // Menentukan arah
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
    
    // Mengecek Collision Enter
    void OnCollisionEnter2D(Collision2D col){
        // Apabila masuk collision dengan tag ground
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform"){
            anim.SetBool("Grounded", isGrounded);
        }
    }
    
    // Mengecek Collision Exit
    void OnCollisionExit2D(Collision2D col){
        // Apabila keluar dari collision dengan tag ground
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Platform"){
            anim.SetBool("Grounded", isGrounded);
        }
    }
    
    void  OnTriggerStay(Collider other){
        if(other.gameObject.tag == "Platform"){
            transform.parent = other.transform;
        }
     }
 
    void  OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Platform"){
            transform.parent = null;
        }
     }  
}

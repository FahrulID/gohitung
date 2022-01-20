using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class pertanyaan : MonoBehaviour
{

    public GameObject player; // Referensi gameobject player
    private player pr; // Referensi script dari obyek player

    public int jumlahAngka; // Berapa banyak angka dalam soal
    public int level; // Level kesulitan

    private int jawaban; // Variabel untuk menyimpan jawaban
    private int[] angka; // Variabel untuk menyimpan daftar angka-angka ( tanpa operasi )
    private int[] angkaJawaban; // Variabel untuk menyimpan angka jawaban ( setelah di operasi , misalnya ada pengurangan maka yang awalnya 5 di variabel angka menjadi -5 di variabel ini )
    private bool[] operasi; // Variabel menampung bentuk operasi

    public bool x; // Untuk menentukan pertanyaannya untuk platform atau untuk lampu merah, true = paltform, false = lampu

    public TextMeshProUGUI soal; // Referensi obyek soal
    public TextMeshProUGUI[] pilihanButton; // Referensi tombol pilihan ganda

    public int[] pilihan; // Nilai-nilai dalam pilihan ganda

    private int[,] range = new int[4,2]{ // Untuk digunakan dalam level
        {
            0,6 // Jika level 1 maka angka yang digunakan hanya dari 0 hingga 5 ( 6 karena 5 + 1, jadi nantinya 6 tidak termasuk )
        },
        {
            0,11 // Jika level 2 maka angka yang digunakan dari 0 hingga 10
        },
        {
            1,16 // Jika level 3 maka angka yang digunakan dari 0 hingga 15
        },
        {
            0,11 // Jika level 4 maka angka yang digunakan dari angka 0 hingga 10
        }
    };

    private string pertanyaanText = "";

    public bool sudahDijawab = false; // Menampung apakah player sudah menjawab pertanyaan
    public bool pilihanBenar = false; // Apakah player sudah menjawab dengan benar

    public GameObject gameObj; // Referensi platform

    void Awake()
    {
        pr = player.GetComponent<player>(); // Referensi komponen skrip player

        angka = new int[jumlahAngka]; // Menentukan seberapa banyak angka  dalam array
        angkaJawaban = new int[jumlahAngka]; // Menentukan seberapa banyak angka dalam array 
        pilihan = new int[4]; // Menentukan berapa banyak pilihan, disini 4
        operasi = new bool[jumlahAngka-1]; // Menentukan seberapa banyak operasi matematika, disini jumlahAngka - 1 karena operasi selalu kurang 1 dari jumlah angka
        // Misal saja, 1 + 1, angkanya 2 dan operasi nya 1. 2 + 3 - 7, angkanya 3 tetapi operasinya hanya 2.

        // Generate angka dan operasi

        for(int x=0;x<jumlahAngka;x++) // Looping sebanyak jumlah angka
        {
            angka[x] = Random.Range(range[level-1, 0], range[level-1, 1]); // Menghasilkan angka random menggunakan Random.next(min, max), dimana min adalah nilai minimum dan max adalah maximum
            if(x < jumlahAngka-1) // Jika x kurang dari jumlah angka - 1, untuk menentukan operasi yang mana kurang 1 dari jumlah angka
            {
                if(Random.value > 0.5) // Menentukan operasi, dimana 50:50, kemungkinan tiap operaso + ( tambah ) dan - ( kurang ) adalah 50%
                {
                    operasi[x] = true; // Operasi pertambahan
                } else {
                    operasi[x] = false; // Operasi pengurangan
                }
            }
        }

        // Mendapatkan Jawaban

        angkaJawaban[0] = angka[0]; // Inisiasi angka jawaban, angka[0] terlebih dahulu karena tidak terpengaruhi operasi
        jawaban += angkaJawaban[0]; // Jawaban dari soal

        pertanyaanText = "" + angka[0]; // Text soal

        for(int x=1;x<jumlahAngka;x++) // Looping sebanyak jumlah angka dan melanggari index pertama
        {
            if(level < 4) // Jika bkn level nya dibawah 4
            {
                if(!operasi[x-1]) // jika operasi pengurangan
                {
                    angkaJawaban[x] = angka[x] * -1; // Angkanya dikali -1 agar berubah jadi minus
                    pertanyaanText += "-" + angka[x]; // Text soal
                } else {
                    angkaJawaban[x] = angka[x]; // Angkanya positif
                    pertanyaanText += "+" + angka[x]; // Text soal
                }
                jawaban += angkaJawaban[x]; // Menambah jawaban
            } else {
                angkaJawaban[x] = angka[x]; // 
                pertanyaanText += "X" + angka[x]; // Text soal
                jawaban *= angkaJawaban[x]; // Menambah jawaban
            }
            
        }
    }

    void Start()
    {
        // Membuat pilihan jawaban
        bool jawabanSudah = false; // Apakah jawaban yang benar sudah mendapat tempat

        for(int x = 0; x < pilihan.Length; x++) // Looping sebanyak pilihan gandanya
        {
            if(!jawabanSudah) // Jika jawaban yang benar belum mendapat tempat
            {
                if(Random.value > 0.75) // Kemungkinan 25% atau 1 : 4
                {
                    pilihan[x] = jawaban; // Pilihan ganda ke-x berisi jawaban
                    jawabanSudah = true; // Jawaban yang benar sudah mendapat tempat
                } else { // Kemungkinan 75% atau 3 : 4
                    pilihan[x] = randomExcept(jawaban - 5, jawaban + 6, pilihan); // Mencari angka random yang mana tidak boleh sama dengan angka yang sudah di dapat, "-5" dan "+6" agar perbedaan jawaban yang salah dan yang benar tidak terlalu beda jauh
                    if(pilihan[x] == jawaban)
                    {
                        jawabanSudah = true;
                    }
                }
            } else { // Jika jawaban yang benar sudah memiliki tempat
                pilihan[x] = randomExcept(jawaban - 5, jawaban + 6, pilihan); // Mencari angka random yang mana tidak boleh sama dengan angka yang sudah di dapat, "-5" dan "+6" agar perbedaan jawaban yang salah dan yang benar tidak terlalu beda jauh
            }

            if(x == pilihan.Length - 1) // Jika sampai dipilihan akhir 
            {
                if(!termasuk(pilihan, jawaban)) // Jika jawaban yang benar tidak memiliki tempat
                {
                    // Debug.Log("Tidak ada jawaban");
                    pilihan[x] = jawaban; // Pilihan terakhir isinya jawaban
                }
            }
            
        }

        // Menampilkan soal

        soal.text = pertanyaanText; // Menampilkan teks soal ke dalam kotak pertanyaan

        for(int x = 0; x < pilihanButton.Length; x++) // Looping sebanyak pilihan ganda
        {
            pilihanButton[x].text = "" + pilihan[x]; // Mengisi pilihan ganda dengan teks jawaban pilihan
        }


    }

    public int randomExcept(int min, int max, int[] except)
    {
        int value; // Value untuk dikembalikan
        value = Random.Range(min, max); // Menghasilkan angka random
        while(termasuk(except, value)) // Looping sampai value yang dihasilkan tidak sama dengan angka yang ada di dalam array "except"
        {
            value = Random.Range(min, max); // Menghasilkan angka random
        }
        // Debug.Log(value);
        return value; // Mengembalikkan hasil angka random
    }

    public bool termasuk(int[] except, int value)
    {
        bool kembali = false;
        for(int x = 0; x < except.Length; x++)
        {
            if(except[x] == value)
            {
                kembali = true;
                x = except.Length;
                // Debug.Log("Angka sama" + value);
            }
        }
        return kembali;
    }

    public void buttonClicked(int i)
    {
        if(pilihan[i] == jawaban) // Jika tombol yang dipilih adalah jawabannya
        {
            sudahDijawab = true; // Sudah dijawab menjadi true
            pilihanBenar = true; // Pilihan benar menjadi true

            if(x)
            {
                gameObj.GetComponent<moving_platform>().togglePertanyaan = true; // Mengganti value toggle pertanyaan di platform menjadi true
                gameObj.GetComponent<moving_platform>().toggleLever(); // Memanggil togglelever pada paltform
            } else {
                gameObj.GetComponent<jalanRaya>().toggleLever(); // Memanggil togglelever pada paltform
            }
            gameObject.SetActive(false); // Menghapus soal dari layar
        } else { // Jika tombol yang dipilih salah
            if(!sudahDijawab) // Jika sudah dijawab bernilai false
            {
                sudahDijawab = true; // Ubah nilainya menjadi true
            }

            gameObject.SetActive(false); // Menghapus tampilan soal di layar

            if(x)
            {
                if(gameObj.GetComponent<moving_platform>().wrongDie)
                {
                    pr.mati();
                } else {
                    gameObj.GetComponent<moving_platform>().falling = true; // Menjatuhkan platform
                }
            } else {
                pr.mati();
            }
        }
    }

    public void setObyek(GameObject obj)
    {
        gameObj = obj; // Mengeset gameObj sesuai dengan parameter obj
    }
}

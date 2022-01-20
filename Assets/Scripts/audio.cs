using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio : MonoBehaviour
{
    public AudioSource musik;

    public bool musikPlaying = true;
    
    public pengaturMenu menu;

    void Start()
    {
        
        if(menu != null)
        {
            musik.volume = menu.volume;
            if(menu.MuteMusic)
            {
                musik.Stop();
            }
        }
    }

    void Awake()
    {
        musik = gameObject.GetComponent<AudioSource>();
        if(GameObject.Find("PengaturMenu") != null)
        {
            menu = GameObject.Find("PengaturMenu").GetComponent<pengaturMenu>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void berhenti()
    {
        musik.Pause();
    }

    public void mainkan()
    {
        musik.Play(0);
    }

    public void toggleMusik()
    {
        if(musikPlaying)
        {
            berhenti();
        } else {
            mainkan();
        }
    }
}

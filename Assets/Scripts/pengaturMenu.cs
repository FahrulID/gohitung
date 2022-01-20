using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pengaturMenu : MonoBehaviour
{
    static pengaturMenu instance;

    public bool[] levelTerbuka = new bool[]
    {
        true, 
        false, 
        false, 
        false, 
        true
    };

    public float volume = 1f;
    public bool MuteMusic = false;

    public int latestLevel = 1;
    
    void Awake()
    {
        if(instance == null)
         {    
             instance = this; // In first scene, make us the singleton.
             DontDestroyOnLoad(gameObject);
         }
         else if(instance != this)
             Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
    }

    public void mute()
    {
        MuteMusic = true;
    }

    public void unmute()
    {
        MuteMusic = false;
    }

    public void changeVolume(float value)
    {
        volume = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

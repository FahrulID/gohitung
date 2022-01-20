using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class muteMusic : MonoBehaviour
{
    // Untuk menyimpan setting menu
    public pengaturMenu menu;

    public Sprite[] images;

    void Start()
    {
        menu = GameObject.Find("PengaturMenu").GetComponent<pengaturMenu>();
        if(menu != null)
        {
            if(menu.MuteMusic){
                gameObject.GetComponent<Image>().sprite = images[0];
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = images[1];
            }
        }
    }

    public void switchImage()
    {
        if(menu != null)
        {
            if(menu.MuteMusic){
                gameObject.GetComponent<Image>().sprite = images[1];
                menu.unmute();
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = images[0];
                menu.mute();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

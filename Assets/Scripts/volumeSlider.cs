using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeSlider : MonoBehaviour
{
    public pengaturMenu menu;

    public Slider volume;

    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("PengaturMenu").GetComponent<pengaturMenu>();
        volume = gameObject.GetComponent<Slider>();
        if(menu!=null)
        {
            volume.value = menu.volume;
        }
        volume.onValueChanged.AddListener(delegate {changeVolume(); });
    }

    public void changeVolume()
    {
        menu.changeVolume(volume.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

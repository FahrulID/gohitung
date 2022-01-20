using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{

    public pengaturMenu menu;

    public Button[] levels;

    void Start()
    {
        menu = GameObject.Find("PengaturMenu").GetComponent<pengaturMenu>();
        for(int x = 0; x < levels.Length; x++)
        {
            levels[x].interactable = menu.levelTerbuka[x];
        }
    }

    public void play()
    {
        SceneManager.LoadScene(menu.latestLevel);
    }

    public void playLevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
    }

    public void quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

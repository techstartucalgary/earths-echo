using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (settingsMenu.activeSelf)
        {
            Time.timeScale = 0;
            settingsMenu.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            settingsMenu.SetActive(false);
            Cursor.visible = false;
        }
    }
}

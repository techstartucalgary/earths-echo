using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsMenu : MonoBehaviour
{
    public GameObject audioSettingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSettingsMenu.activeSelf)
        {
            Time.timeScale = 0;
            audioSettingsMenu.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            audioSettingsMenu.SetActive(false);
            Cursor.visible = false;
        }
    }
}

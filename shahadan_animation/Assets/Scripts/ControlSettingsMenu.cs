using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSettingsMenu : MonoBehaviour
{
    public GameObject controlSettingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlSettingsMenu.activeSelf)
        {
            Time.timeScale = 0;
            controlSettingsMenu.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            controlSettingsMenu.SetActive(false);
            Cursor.visible = false;
        }
    }
}

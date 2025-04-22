using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettingsMenu : MonoBehaviour
{
    public GameObject graphicSettingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (graphicSettingsMenu.activeSelf)
        {
            Time.timeScale = 0;
            graphicSettingsMenu.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            graphicSettingsMenu.SetActive(false);
            Cursor.visible = false;
        }
    }
}

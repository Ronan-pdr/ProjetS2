using System.Collections;
using System.Collections.Generic;
using Script.InterfaceInGame;
using UnityEngine;

public class LauncherManager : MonoBehaviour
{

    [SerializeField] PauseMenu instance;

    [SerializeField] private TabMenu instanced;
    // Update is called once per frame
    void Start()
    {
        PauseMenu.Instance = instance;
        TabMenu.Instance = instanced;
        Debug.Log(instance);
    }
    void Update()
    {
        if (instance.Getdisconnecting())
            return;
            
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (instance.GetIsPaused())
            {
                instance.Resume();
            }
            else
            {
                instance.Pause();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            MenuManager.Instance.OpenMenu("tab");
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            MenuManager.Instance.OpenMenu("InterfaceInGame");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle map1;
    [SerializeField] private Toggle map2;
    
    // Start is called before the first frame update
    void Start()
    {
        map1.isOn = true;
        map2.isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

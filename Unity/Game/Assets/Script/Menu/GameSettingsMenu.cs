using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Script.Manager;
using Script.Zone;
using TMPro;

public class GameSettingsMenu : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private Toggle all;
    [SerializeField] private Toggle inside;
    [SerializeField] private Toggle outside;
    [SerializeField] private Toggle bouffe;
    [SerializeField] private Toggle cours;
    
    [Header("Slider")]
    [SerializeField] private Slider nbChasseurNumber;
    [SerializeField] private Slider nbTimeNumber;
    
    [Header("Texte")]
    [SerializeField] private TMP_Text nbChasseurText;
    [SerializeField] private TMP_Text nbTimeText;
    
    [Header("Reste")]
    [SerializeField] private SettingsGame settingsGame;
    
    
    
    private Dictionary<ZoneManager.EnumZone, Toggle> _dictToggle;
    

    // Start is called before the first frame update
    void Start()
    {
        ZoneManager.EnumZone zone = settingsGame.Zone;
        nbChasseurNumber.maxValue = PhotonNetwork.CurrentRoom.PlayerCount;
        nbTimeNumber.minValue = 1;
        nbTimeNumber.maxValue = 20;
        _dictToggle = new Dictionary<ZoneManager.EnumZone, Toggle>();
        _dictToggle.Add(ZoneManager.EnumZone.All, all);
        _dictToggle.Add(ZoneManager.EnumZone.Inside, inside);
        _dictToggle.Add(ZoneManager.EnumZone.Outside, outside);
        _dictToggle.Add(ZoneManager.EnumZone.Bouffe, bouffe);
        _dictToggle.Add(ZoneManager.EnumZone.Cours, cours);
        foreach (var kvp in _dictToggle)
            kvp.Value.isOn = zone == kvp.Key;
        
    }

    public void SetNbChasseur(float nbChasseur)
    {
        Set(nbChasseur, nbChasseurText, settingsGame.SetNbChasseur);
    }

    public void SetTime(float time)
    {
        Set(time, nbTimeText, settingsGame.SetTimeMax);
    }

    private void Set(float f, TMP_Text text, Action<int> func)
    {
        int nb = (int) f;
        func(nb);
        text.text = nb.ToString();
    }

    public void SetMap(int zone)
    {
        Toggle toggle = _dictToggle[(ZoneManager.EnumZone) zone];
        if (toggle.isOn)
        {
            _dictToggle[settingsGame.Zone].isOn = false;
            settingsGame.SetZone((ZoneManager.EnumZone) zone);
        }
        else
            toggle.isOn = true;
    }
    
}

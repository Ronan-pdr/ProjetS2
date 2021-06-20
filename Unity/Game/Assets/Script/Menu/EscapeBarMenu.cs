using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EscapeBarMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject gameSettingsButton;
    
    
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        gameSettingsButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public void StartGame()
    {
        if (PhotonNetwork.MasterClient.NickName == "Labyrinthe")
        {
            PhotonNetwork.LoadLevel(3);
        }
        else
        {
            PhotonNetwork.LoadLevel(2);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using Object = System.Object;


public class BotManager : MonoBehaviour
{ 
    // Chaque joueur va contrôler un certain nombre de bots,
    // les leurs seront stockés dans le dossier 'BotManager' sur Unity
    // et ceux controlés pas les autres dans 'DossierOtherBot'
    
    
    public static BotManager Instance; //c'est possible puisqu'il y en a qu'un par joueur
    
    // stocké tous les bots
    private List<BotClass> Bots;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Bots = new List<BotClass>();

        int nBot = 5;

        for (int i = 0; i < nBot; i++) // Instancier, ranger (dans la liste) et positionner sur la map tous les bots
        {
            BotRectiligne bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Humanoide", "BotRectiligne"),
                Vector3.zero, Quaternion.identity).GetComponent<BotRectiligne>();
            Bots.Add(bot);

            bot.transform.position += CrossManager.Instance.GetPosition(i);
            bot.SetOwnBotManager(this); // lui indiquer quel est son père
            
            Bots.Add(bot); // les enregistrer dans une liste (cette liste contiendra seulement les bots que l'ordinateur contrôle)
        }
    }

    public void Die(GameObject bot)
    {
        Bots.Remove(bot.GetComponent<BotClass>()); // le supprimer de la liste
        
        PhotonNetwork.Destroy(bot.gameObject); // détruire l'objet
    }
}
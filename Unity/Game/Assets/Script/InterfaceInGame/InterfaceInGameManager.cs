using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.EntityPlayer;
using UnityEngine.UI;

namespace Script.InterfaceInGame
{
    public class InterfaceInGameManager : MonoBehaviourPunCallbacks
    {
        public static InterfaceInGameManager Instance;
        
        // pour instancier sur la toile l'affichage du joueur
        [SerializeField] Transform infoContent;
        [SerializeField] private Transform SpriteContent;
        [SerializeField] GameObject infoItemPrefab;

        // relatif à toi
        [SerializeField] private AffichageInfoJoueur tesInfos;
        private PlayerClass ownPlayerClass;
        
        // relatif aux autres
        private List<AffichageInfoJoueur> infosJoueur;
        
        //Listes des images pour la vie
        [SerializeField] private Sprite[] sprites;

        private void Awake()
        {
            Instance = this;
            infosJoueur = new List<AffichageInfoJoueur>();
        }

        void Update()
        {
            int x = (100 * ownPlayerClass.GetCurrentHealth() / ownPlayerClass.GetMaxHealth()) - 1;
            Image image = SpriteContent.GetComponent<Image>();
            Debug.Log(sprites);
            image.sprite = sprites[x/20];
        }
        public void Set()
        {
            MasterManager masterManager = MasterManager.Instance;
            int hauteur = Screen.height;
            //int largeur = Screen.width;
            int taille = 85;
            
            ownPlayerClass = masterManager.GetOwnPlayer();

            tesInfos.SetUp(ownPlayerClass, 80, -25);

            int index = 0;
            foreach (PlayerClass playerClass in masterManager.GetListPlayer()) // pas grave si la liste est vide (mais elle est forcément instancié)
            {
                if (playerClass != ownPlayerClass)
                {
                    infosJoueur.Add(Instantiate(infoItemPrefab, infoContent).GetComponent<AffichageInfoJoueur>());
                    infosJoueur[index++].SetUp(playerClass, hauteur - index*taille, 90);
                }
            }
        }
    }
}
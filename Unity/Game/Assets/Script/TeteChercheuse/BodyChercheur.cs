using System;
using Photon.Pun;
using UnityEngine;
using Script.Tools;

namespace Script
{
    public class BodyChercheur : TeteChercheuse
    {
        [SerializeField] private CapsuleCollider botCapsuleCollider;
        private CapsuleCollider ownCapsuleCollider;
        
        // Destination
        private Vector3 coordDestination;
        
        //Ecart maximum entre sa destination et sa position pour qu'il soit considéré comme arrivé
        private float ecartDistance;
        
        // Nécessaire pour lui envoyer le résultat des body
        private BotRectiligne botRectiligne;

        private void Start()
        {
            Find = false;

            ownCapsuleCollider = GetComponent<CapsuleCollider>();

            // on récupère toutes les caractéristiques du CapsuleCollider du bot
            ownCapsuleCollider.center = botCapsuleCollider.center;
            ownCapsuleCollider.height = botCapsuleCollider.height - 0.5f;
            ownCapsuleCollider.radius = botCapsuleCollider.radius;

            float rayon = ownCapsuleCollider.radius; // marre d'avoir des warning parce que j'utilise plusieurs fois un transform
            
            moveAmount = new Vector3(0, 0, rayon); // la vitesse est le rayon de la capsule (parce que le diamètre c'est trop)
            ecartDistance = rayon*2;
        }

        // l'instancier de manière static
        public static void InstancierStatic(BotRectiligne lanceur, Vector3 _coordDestination, Vector3 rotation)
        {
            Transform trLanceur = lanceur.transform;
            
            BodyChercheur body = PhotonNetwork.Instantiate("PhotonPrefabs/TeteChercheuse/BodyChercheur",
                trLanceur.position, trLanceur.rotation).GetComponent<BodyChercheur>();
            
            body.Instancier(lanceur, _coordDestination, rotation);
        }

        // l'instancier de manière non-static (est appelé dans 'InstatncierStatic')
        private void Instancier(BotRectiligne lanceur, Vector3 _coordDestination, Vector3 rotation)
        {
            SetRbTr();
            
            botRectiligne = lanceur;
            Lanceur = lanceur.gameObject;
            coordDestination = _coordDestination;

            Tr.Rotate(rotation);
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) // Seul le masterClient controle le bodyChercheur
                return;
            
            Rb.MovePosition(Rb.position + Tr.TransformDirection(moveAmount)); // move entity sans deltaTime

            if (!Find && Calcul.Distance(Tr.position, coordDestination) > ecartDistance)
                return; // il n'a rien touché et est trop loin de sa destination, donc il continue
            
            botRectiligne.FoundResultDestination(!Find, coordDestination); // S'il a trouvé un obstacle, alors la coordonnée n'est pas valide
            
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
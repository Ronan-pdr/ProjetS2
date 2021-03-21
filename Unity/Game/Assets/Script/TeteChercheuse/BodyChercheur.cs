using UnityEngine;
using Script.Tools;
using Script.DossierPoint;
using Script.EntityPlayer;

namespace Script.TeteChercheuse
{
    public class BodyChercheur : TeteChercheuse
    {
        // Nous n'avons pas envie que tous les ordinateurs des joueurs suivent ce script.
        // Mais, comme les body checheur s'instancie localement (pas avec photon),
        // nul besoin de rajouter des conditions.
        
        // les capsules colliders
        [SerializeField] private CapsuleCollider botCapsuleCollider;
        private CapsuleCollider ownCapsuleCollider;
        
        // Destination
        private CrossPoint Destination;
        
        //Ecart maximum entre sa destination et sa position pour qu'il soit considéré comme arrivé
        private float ecartDistance;
        
        // Nécessaire pour lui envoyer le résultat des body
        private CrossPoint crossPoint;

        private float Vitesse = 3;

        private void Start()
        {
            // parenter
            Tr.parent = MasterManager.Instance.GetDossierBodyChercheur();

            Find = false;

            ownCapsuleCollider = GetComponent<CapsuleCollider>();

            // on récupère toutes les caractéristiques du CapsuleCollider du bot
            ownCapsuleCollider.center = botCapsuleCollider.center;
            ownCapsuleCollider.height = botCapsuleCollider.height;
            ownCapsuleCollider.radius = botCapsuleCollider.radius * 0.8f; // 0,8 pour laisser une petite marge

            float rayon = ownCapsuleCollider.radius;

            MoveAmount = new Vector3(0, 0, Vitesse);
            ecartDistance = rayon*2;
        }

        // l'instancier de manière static
        public static void InstancierStatic(CrossPoint lanceur, CrossPoint destination, Vector3 rotation)
        {
            Transform trLanceur = lanceur.transform;

            BodyChercheur original = MasterManager.Instance.GetOriginalBodyChercheur();
            
            BodyChercheur body = Instantiate(original, trLanceur.position, trLanceur.rotation);
            
            body.Instancier(lanceur, destination, rotation);
        }

        // l'instancier de manière non-static (est appelé dans 'InstatncierStatic')
        private void Instancier(CrossPoint lanceur, CrossPoint destination, Vector3 rotation)
        {
            SetRbTr();

            crossPoint = lanceur;
            Lanceur = lanceur.gameObject;
            Destination = destination;

            Tr.Rotate(rotation);

            MoveAmount = new Vector3(0, 0, 3f);
            for (int i = 0; i < 20; i++)
            {
                MoveEntity();
            }
            
            // je fais ça pour qu'il se décale un peu pour que les bodyChercheur qui se croisent ne se cogne pas
            MoveAmount = new Vector3(0, 0, 0);
            for (int i = 0; i < 3; i++)
            {
                MoveEntity();
            }
        }

        private void Update()
        {
            MoveAmount = new Vector3(0, 0, Vitesse); // la vitesse est le rayon de la capsule (parce que le diamètre c'est trop, il traverse les destinations sans s'arrêter)

            MoveEntity();
            //Rb.MovePosition(Rb.position + Tr.TransformDirection(MoveAmount)); // move entity sans deltaTime

            float dist = Calcul.Distance(Tr.position, Destination.transform.position, Calcul.Coord.Y);

            if (Find || dist > 1000) // s'il rentre en contact avec un obstacle ou qu'il est trop loin de sa destination (il s'est perdu) c'est fini
            {
                Destroy(gameObject);
                return;
            }

            if (dist < ecartDistance) // il est arrivé à destination (est ce qu'il est à la bonne altitude ?) 
            {
                if (Calcul.Distance(Tr.position.y, Destination.transform.position.y) < ownCapsuleCollider.height / 2) // c'est que c'est une destination valide
                {
                    crossPoint.AddNeighboors(Destination);
                }

                Destroy(gameObject);
            }

            // else il est trop loin de sa destination, donc il continue
        }
    }
}
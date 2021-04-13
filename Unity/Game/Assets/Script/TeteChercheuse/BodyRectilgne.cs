using System;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Tools;
using UnityEngine;

namespace Script.TeteChercheuse
{
    public class BodyRectilgne : BodyChercheur
    {
        // la vitesse est si faible parce que sinon il fait n'importe quoi dans les escaliers
        private float Vitesse = 4f;
        
        // l'instancier de manière static
        public static void InstancierStatic(GameObject lanceur, GameObject destination)
        {
            BodyRectilgne original = MasterManager.Instance.GetOriginalBodyRectilgne(); // récupérer la préfab

            Vector3 posisionLanceur = lanceur.transform.position;
            float rotation = Calcul.Angle(0, posisionLanceur, destination.transform.position, Calcul.Coord.Y);
            BodyRectilgne body = Instantiate(original, posisionLanceur, lanceur.transform.rotation);
            
            body.Instancier(lanceur, destination, rotation);
        }
        
        // l'instancier de manière non-static (est appelé dans 'InstatncierStatic')
        private void Instancier(GameObject lanceur, GameObject destination, float rotation)
        {
            SetRbTr();
            
            Lanceur = lanceur;
            Destination = destination;

            Tr.Rotate(new Vector3(0, rotation, 0));

            // je fais ça pour qu'il se décale un peu pour que les bodyChercheur qui se croisent ne se cogne pas
            MoveAmount = new Vector3(0, 0, 30*(3-SimpleMath.Mod((int)rotation/6, 2)));
            Tr.position += Tr.TransformDirection(MoveAmount) * Time.fixedDeltaTime;
        }
        
        private void Update()
        {
            MoveAmount = new Vector3(0, 0, Vitesse);

            float dist = Calcul.Distance(Tr.position, Destination.transform.position, Calcul.Coord.Y);

            if (Find || dist > 100) // s'il rentre en contact avec un obstacle ou qu'il est trop loin de sa destination (il s'est perdu) c'est fini
            {
                if (dist > 100)
                    Debug.Log($"WARNING : la distance entre un body chercheur et sa destination était de {dist}");
                
                Destroy(gameObject);
                Lanceur.GetComponent<CrossPoint>().EndResearchBody(null);
                return;
            }

            if (dist < ecartDistance) // il est arrivé à destination (est ce qu'il est à la bonne altitude ?) 
            {
                if (Calcul.Distance(Tr.position.y, Destination.transform.position.y) < ownCapsuleCollider.height / 2) // c'est que c'est une destination valide
                {
                    Lanceur.GetComponent<CrossPoint>().EndResearchBody(Destination.GetComponent<CrossPoint>());
                }
                else // sur la bonne position en x et z mais pas sur y, ça veut dire qu'il dépasserait la destination s'il n'y avait pas ce cas
                {
                    Lanceur.GetComponent<CrossPoint>().EndResearchBody(null);
                }

                Destroy(gameObject);
            }

            // else il est trop loin de sa destination, donc il continue
        }

        private void FixedUpdate()
        {
            MoveEntity();
        }
        
        // Event
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.GetComponent<Entity>())
            {
                ownCapsuleCollider.isTrigger = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other);
        }
        
        private void OnCollisionStay(Collision other)
        {
            OnCollisionAux(other);
        }

        private void OnCollisionAux(Collision other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrête pas
            {
                ownCapsuleCollider.isTrigger = true;
                return;
            }
            
            ContactPoint[] listContact = other.contacts;
            int len = listContact.Length;
            
            int i;
            for (i = 0; i < len && Calcul.Distance(listContact[i].point.y, GetYSol()) < ownCapsuleCollider.radius * 1.2f; i++)
            {}

            if (i < len) // cela signifie qu'un objet (qui n'est pas entity) l'a touché à une hauteur supérieur au rayon 
            {
                Find = true;
                HittenObj = other.gameObject;
            }
        }
        
        private float GetYSol()
        {
            float y = ownCapsuleCollider.transform.position.y;

            return y;
        }
    }
}
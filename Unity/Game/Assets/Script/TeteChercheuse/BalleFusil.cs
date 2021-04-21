using UnityEngine;
using Photon.Pun;
using System.IO;
using Script.Tools;
using Script.EntityPlayer;
using Script.Bot;
using Script.DossierArme;
using Script.Manager;

namespace Script.TeteChercheuse
{
    public class BalleFusil : TeteChercheuse
    {
        // ------------ Attributs ------------
        
        private ArmeInfo armeInfo;
        private PhotonView Pv;
    
        // ------------ Constructeurs ------------
        private void Start()
        {
            SetRbTr();
    
            Pv = GetComponent<PhotonView>();
    
            // parenter
            Tr.parent = MasterManager.Instance.GetDossierBalleFusil();
            
            MoveAmount = new Vector3(0, 0, 50);
        }
        
        
        public static void Tirer(Vector3 coord, GameObject ownObj, Vector3 rotation, ArmeInfo armeInf)
        {
            BalleFusil balleFusil = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse", "Balle" + armeInf.GetName()), coord, Quaternion.identity).GetComponent<BalleFusil>();
            
            balleFusil.VecteurCollision(ownObj, rotation, armeInf);
        }
    
        private void VecteurCollision(GameObject ownObj, Vector3 rotation, ArmeInfo armeInf)
        {
            Lanceur = ownObj;
            armeInfo = armeInf;
            
            transform.Rotate(rotation);
        }
    
        // ------------ Update ------------
        public void Update()
        {
            if (!Pv.IsMine) // Seul le créateur de la balle la contrôle
                return;
    
            // si max distance -> il s'arrête
            if (Calcul.Distance(Lanceur.transform.position, Tr.position) > armeInfo.GetPortéeAttaque())
            {
                enabled = false;
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void FixedUpdate()
        {
            if (!Pv.IsMine) // Seul le créateur de la balle la contrôle
                return;
            
            MoveEntity();
        }
        
        // ------------ Event ------------
        private void OnTriggerEnter(Collider other)
        {
            OnCollisionAux(other);
        }
    
        private void OnTriggerStay(Collider other)
        {
            OnCollisionAux(other);
        }
    
        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other.collider);
        }
    
        private void OnCollisionStay(Collision other)
        {
            OnCollisionAux(other.collider);
        }

        private void OnCollisionAux(Collider other)
        {
            if (!Pv)
                return;
            
            // Seul le créateur de la balle gère les collisions
            if (!Pv.IsMine)
                return;
            
            // Le cas où c'est avec notre propre personnage ou que c'est avec une autre tête chercheuse
            if (other.gameObject == Lanceur || other.gameObject.GetComponent<TeteChercheuse>())
                return;

            GameObject hittenObj = other.gameObject;
            
            // Si c'est pas un humain on s'en fout
            if (hittenObj.GetComponent<Humanoide>())
            {
                Humanoide cibleHumaine = hittenObj.GetComponent<Humanoide>();
    
                if (!(cibleHumaine is Chasseur)) // Si la personne touchée est un chasseur, personne prend de dégât
                {
                    cibleHumaine.TakeDamage(armeInfo.GetDamage()); // Le chassé ou le bot prend des dégâts
    
                    if (cibleHumaine is BotClass)
                    {
                        Lanceur.GetComponent<Chasseur>().TakeDamage(1); // Le chasseur en prend aussi puisqu'il s'est trompé de cible
                    }
                }
            }
    
            PhotonNetwork.Destroy(gameObject);
        }
    }
}


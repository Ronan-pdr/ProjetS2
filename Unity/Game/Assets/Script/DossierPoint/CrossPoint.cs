using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Script.DossierPoint
{
    public class CrossPoint : Point
    {
        // Chaque CrossPoint envoie des 'bodyChercheur' à chaque fois qu'il souhaite trouvé une nouvelle destination
        // il les envoie vers les coordonnées (pas toute),
        // ceux qui sont positifs sont ajoutés dans la liste 'Neighboors'
        private List<CrossPoint> Neighboors;

        private int nAttenduBodyChercher;
        
        // Getter
        public CrossPoint GetNeighboor(int index) => Neighboors[index];
        public int GetNbNeighboor() => Neighboors.Count;
        
        // Setter
        public void AddNeighboor(CrossPoint value)
        {
            Neighboors.Add(value);
        }
        
        // constructeur
        private void Awake()
        {
            Neighboors = new List<CrossPoint>();
        }

        public void EndResearchBody(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            nAttenduBodyChercher -= 1;
            
            if (nAttenduBodyChercher < 0) // tout reçu
            {
                throw new Exception("trop de résultat de body chercheur ont été reçu");
            }

            if (!(neighboor is null)) // est-ce un voisin valide
            {
                Neighboors.Add(neighboor);
            }

            if (nAttenduBodyChercher == 0) // tous reçu
            {
                CrossManager.Instance.EndOfOneResearch(Neighboors);
            }
        }

        public void SearchNeighboors()
        {
            Neighboors = new List<CrossPoint>();

            CrossPoint potentialNeighboor;
            Vector3 ownCoord = transform.position;
            float distanceThisWithDest;
            int len = CrossManager.Instance.GetNumberPoint();
            
            for (int i = 0; i < len; i++)
            {
                potentialNeighboor = CrossManager.Instance.GetPoint(i);
                if (Neighboors.Contains(potentialNeighboor)) // si je l'ai déjà placé dans ma liste, il est inutile de le tester
                    continue;
                
                distanceThisWithDest = Calcul.Distance(ownCoord, potentialNeighboor.transform.position);

                if (0.2f < distanceThisWithDest && distanceThisWithDest < 30) // on ne veut pas lancer un body chercheur la où on se situe et on ne veux pas des voisins trop loins
                {
                    nAttenduBodyChercher += 1;
                    BodyRectilgne.InstancierStatic(gameObject, potentialNeighboor.gameObject);
                }
            } 
            
            if (nAttenduBodyChercher == 0) // aucun voisin potentiel donc c'est la fin de la recherche
            {
                CrossManager.Instance.EndOfOneResearch(Neighboors);
            } 
        }
    }
}
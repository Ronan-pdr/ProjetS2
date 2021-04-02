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
        public void InitialiserNeighboors()
        {
            Neighboors = new List<CrossPoint>();
        }
        public void AddNeighboor(CrossPoint value)
        {
            Neighboors.Add(value);
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
            transform.position += new Vector3(0, 0, 0); // unity surélève le point de 0.3, je compense 
            
            if (Neighboors == null)
                Neighboors = new List<CrossPoint>();

            CrossPoint potentialNeighboor;
            Vector3 ownCoord = transform.position;
            Vector3 destCoord;
            float distanceThisWithDest, amountRotation;
            int len = CrossManager.Instance.GetNumberPoint();
            
            for (int i = 0; i < len; i++)
            {
                potentialNeighboor = CrossManager.Instance.GetPoint(i);
                if (Neighboors.Contains(potentialNeighboor)) // si je l'ai déjà placé dans ma liste, il est inutile de le tester
                    continue;
                
                destCoord = potentialNeighboor.transform.position;
                
                distanceThisWithDest = Calcul.Distance(ownCoord, destCoord);

                if (0.2f < distanceThisWithDest && distanceThisWithDest < 100) // on ne veut pas lancer un body chercheur la où on se situe et on ne veux pas des voisins trop loins
                {
                    nAttenduBodyChercher += 1;
                    
                    amountRotation = Calcul.Angle(0, ownCoord, destCoord, Calcul.Coord.Y);
                    BodyChercheur.InstancierStatic(this, potentialNeighboor, new Vector3(0, amountRotation, 0));
                }
            }
        }

        private void LoadNeighboors()
        {
            
        }
    }
}
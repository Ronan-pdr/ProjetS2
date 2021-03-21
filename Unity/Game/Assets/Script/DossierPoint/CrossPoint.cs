using System.Collections.Generic;
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
        // ceux qui sont positifs sont ajoutés dans la liste 'neighboors'
        [SerializeField] private List<CrossPoint> Neighboors;
        
        // Getter

        public CrossPoint GetNeighboor(int index) => Neighboors[index];
        public int GetNbNeighboor() => Neighboors.Count;
        
        // Setter
        public void AddNeighboors(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            Neighboors.Add(neighboor);
        }
        
        public void Start()
        {
            transform.position += new Vector3(0, 0, 0); // unity sur-élève le point de 0.3, je compense 
            
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
                if (Neighboors.Contains(potentialNeighboor)) // si je l'ai déjà placer dans ma liste, il est inutile de le tester
                    continue;
                
                destCoord = potentialNeighboor.transform.position;
                
                distanceThisWithDest = Calcul.Distance(ownCoord, destCoord);

                if (0.2f < distanceThisWithDest && distanceThisWithDest < 30) // on ne veut pas lancer un body chercheur la où on se situe et on ne veux pas des voisins trop loins
                {
                    amountRotation = Calcul.Angle(0, ownCoord, destCoord, Calcul.Coord.Y);
                    BodyChercheur.InstancierStatic(this, potentialNeighboor, new Vector3(0, amountRotation, 0));
                }
            }
        }
    }
}
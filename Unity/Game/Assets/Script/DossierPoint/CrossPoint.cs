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
        [SerializeField] private List<CrossPoint> Neighboors;

        private int nAttenduBodyChercher;

        private CrossManager _crossManager;
        
        // ------------ Getters ------------
        public CrossPoint GetNeighboor(int index) => Neighboors[index];
        public int GetNbNeighboor() => Neighboors.Count;
        public static int GetIndexToName(string namePoint)
        {
            string s = "";
            for (int i = namePoint.Length - 2; i >= 0 && namePoint[i] != '('; i--)
            {
                s = namePoint[i] + s;
            }

            if (int.TryParse(s, out int index))
            {
                return index;
            }
            
            throw new Exception($"Le nom '{namePoint}' n'est pas homologé (index = {s})");
        }
        
        // ------------ Setter ------------
        public void AddNeighboor(CrossPoint value)
        {
            Neighboors.Add(value);
        }
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            Neighboors = new List<CrossPoint>();
        }

        private void Start()
        {
            _crossManager = CrossManager.Instance;
        }
        
        // ------------ Méthodes ------------

        public void EndResearchBody(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            nAttenduBodyChercher -= 1;
            
            if (nAttenduBodyChercher < 0) // tout reçu
            {
                throw new Exception("trop de résultat de body chercheur ont été reçu");
            }

            // est-ce un voisin valide et n'est il pas déjà dedans (#Serialize Field)
            if (!(neighboor is null))
            {
                if (Neighboors.Contains(neighboor))
                {
                    throw new Exception($"Ca devrait être impossible (Lanceur -> {name}, Dest -> {neighboor})");
                }
                
                Neighboors.Add(neighboor);
                _crossManager.OneNewNeighboorFind();
            }

            if (nAttenduBodyChercher == 0) // tout reçu
            {
                _crossManager.EndOfOneResearch(Neighboors);
            }
        }

        public void SearchNeighboors()
        {
            foreach (CrossPoint potentialNeighboor in GetPotentialNeigboors())
            {
                nAttenduBodyChercher += 1;
                BodyRectilgne.InstancierStatic(gameObject, potentialNeighboor.gameObject);
            }
            
            //Debug.Log($"{name} a envoyé {nAttenduBodyChercher} bodyChercheur(s)");
            
            if (nAttenduBodyChercher == 0) // aucun voisin potentiel donc c'est la fin de la recherche
            {
                CrossManager.Instance.EndOfOneResearch(Neighboors);
            } 
        }

        // va return une liste de maximum 15 éléments
        private List<CrossPoint> GetPotentialNeigboors()
        {
            List<CrossPoint> potentialNeighboors = new List<CrossPoint>(_crossManager.GetCrossPoints());
            Vector3 ownCoord = transform.position;
            float distanceMax = 30;
            
            //Debug.Log(name);

            do
            {
                for (int i = potentialNeighboors.Count - 1; i >= 0; i--)
                {
                    if (Neighboors.Contains(potentialNeighboors[i]))
                    {
                        //Debug.Log($"Déja trouvé -> {potentialNeighboors[i].name}");
                        // déjà repertorié
                        potentialNeighboors.RemoveAt(i);
                        continue;
                    }

                    Vector3 pos = potentialNeighboors[i].transform.position;
                    
                    float distanceThisWithDest = Calcul.Distance(ownCoord, pos);
                    
                    if (!(0.4f < distanceThisWithDest && distanceThisWithDest < distanceMax))
                    {
                        //Debug.Log(potentialNeighboors[i].name);
                        // trop proche ou trop loin
                        potentialNeighboors.RemoveAt(i);
                        continue;
                    }
                    
                    if (Calcul.Distance(ownCoord, pos, Calcul.Coord.Y)*0.8f <= Calcul.Distance(ownCoord.y, pos.y))
                    {
                        //Debug.Log(potentialNeighboors[i].name);
                        // trop de d'altitude pour la potentiel distance de montée
                        potentialNeighboors.RemoveAt(i);
                    }
                }

                distanceMax -= 3;

            } while (potentialNeighboors.Count > 10);

            return potentialNeighboors;
        }
    }
}
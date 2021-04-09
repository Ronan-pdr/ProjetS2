using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Tools;
using UnityEngine;

namespace Script.TeteChercheuse
{
    public class RayGaz
    {
        private class Node
        {
            public Node After { get; }
            public Vector3 Position { get; }

            public Node(Node after, Vector3 p)
            {
                After = after;
                Position = p;
            }
        }
        
        private Vector3 coordOrigin;

        // Quelle est la distance qu'il va parcourir losqu'il va se déplacer ?
        // Sachant qu'il ne se déplace que tout droit 
        private float bond = 1f;

        // Cette matrice indique s'il a déjà parcouru une zone
        private Node[,] Sonde;
        private int heigth;
        private int width;
        
        // calculer la complexité
        private float begin;

        // cette file contient des positions valides (mais il y est peut-être déjà allé)
        // où il devra se répendre autour
        private MyFile<Node> file;
        
        // variable relative à sa capsule
        private (float hauteur, float rayon) capsule;
        
        // destination
        private Vector3 destination;
        
        // constructeur
        public RayGaz(Vector3 posInitiale, Vector3 posDestination)
        {
            // légèrement modifier la position initilae en fonction du bond
            // c'est pour que les positions soit toujours bien calé avec la matrice
            posInitiale -= SimpleMath.Mod(posInitiale, bond) + Vector3.up * 0f;
            
            // Déterminer l'origine de notre matrice ainsi que ses dimensions grâce aux contours
            GameObject[] contour = MasterManager.Instance.GetContour();
            
            Vector3 origin = contour[0].transform.position;
            coordOrigin = new Vector3(origin.x, 0, origin.z);
            
            heigth = (int) ((contour[1].transform.position.z - origin.z) / bond);
            width = (int) ((contour[2].transform.position.x - origin.x) / bond);
            Sonde = new Node[heigth, width];
            
            // vérifiez que la position ininiale est bien comprise dans les bornes
            if (!IsValidPosition(posInitiale))
            {
                throw new Exception("Un RayGaz ne peut-être lancé en dehors des contours");
            }
            
            // récupérer les côtes des bots pour les ray
            CapsuleCollider cap = MasterManager.Instance.GetCapsuleBot();
            float scale = cap.transform.localScale.y;
            capsule.hauteur = cap.height * scale;
            capsule.rayon = cap.radius * scale;
            
            // initialiser destination
            destination = posDestination;

            // enfiler la première position and Let's this party started
            Node first = new Node(null, posInitiale);
            file = new MyFile<Node>();
            file.Enfiler(first);
            CheckPosition(first);
        }

        public static List<Vector3> GetPath(GameObject lanceur, GameObject destination)
        {
            RayGaz rayGaz = new RayGaz(lanceur.transform.position, destination.transform.position);
            
            return rayGaz.Resarch();
        }

        private List<Vector3> Resarch()
        {
            // enreegistrer temps au début de la recherche
            begin = Time.time;
            
            Node node;
            do
            {
                node = file.Defiler();
                
                // temporaire
                Test.TestRayGaz.CreateMarqueur(node.Position);

                // devant
                NewPosition(node, Vector3.forward);
                // derriere
                NewPosition(node, Vector3.back);
                // droite
                NewPosition(node, Vector3.right);
                // gauche
                NewPosition(node, Vector3.left);

            } while (!file.IsEmpty() && !Arrivé(node.Position));

            if (file.IsEmpty())
            {
                Debug.Log("Il existe aucun chemin pour y accéder");

                return null;
            }
            
            // bien arrivé
            Debug.Log($"Le gaz s'est répendu en {Time.time - begin} senconde(s)");
            return GetBestPath(node);
        }

        private void CheckPosition(Node node)
        {
            Vector3 p = node.Position;
            Sonde[GetIndexZ(p), GetIndexX(p)] = node;
        }

        // cette fonction vérifie d'une part si la position "p"
        // se situe sur le terrain (dans les bornes de la matrice)
        // et si l'on ne l'a pas déjà traitée
        private bool IsValidPosition(Vector3 p)
        {
            int x = GetIndexX(p);
            int z = GetIndexZ(p);

            bool inBorne = 0 <= x && x < width && 0 <= z && z < heigth;
            
            return inBorne && Sonde[z, x] == null;
        }

        private void NewPosition(Node after, Vector3 direction)
        {
            Vector3 position = after.Position;
            
            //Debug.Log($"Can I pass ? {CanIPass(position, direction)}");
            //Debug.Log($"Est ce valide ? {IsValidPosition(position)}");

            Vector3 newPos = position + direction * bond;

            if (CanIPass(capsule, position, direction, bond) && IsValidPosition(newPos))
            {
                // nous avons trouvé une position où le gaz va se répendre...
                Node node = new Node(after, newPos);
                
                // ...plus qu'à l'enfiler et ...
                file.Enfiler(node);
                
                // ...indiquer que cette position va être traiter,
                // donc il sera inutile de la refaire
                CheckPosition(node);
            }
        }

        private bool Arrivé(Vector3 p)
        {
            return Calcul.Distance(p, destination, Calcul.Coord.Y) < bond;
        }
        
        public static bool CanIPass((float hauteur, float rayon) cap, Vector3 position, Vector3 direction, float maxDistance)
        {
            // haut du corps (vers les yeux)
            Vector3 hautDuCorps = position + Vector3.up * (cap.hauteur - cap.rayon);
            
            // bas du corps (vers le haut des pieds)
            Vector3 basDuCorps = position + Vector3.up * cap.rayon;
            
            if (Physics.CapsuleCast(hautDuCorps, basDuCorps, cap.rayon, direction, maxDistance))
            {
                return false;
            }

            return true;
        }

        private List<Vector3> GetBestPath(Node node)
        {
            List<Vector3> path = new List<Vector3>();
            
            // c'est la position de la destination
            path.Add(node.Position);

            Node nextNode;
            while (node.After != null)
            {
                nextNode = node.After;
                while (nextNode.After != null && CanIPass(capsule, node.Position, Calcul.Diff(nextNode.After.Position, node.Position),
                    Calcul.Distance(nextNode.After.Position, node.Position)))
                {
                    nextNode = nextNode.After;
                }

                node = nextNode;
                path.Add(node.Position);
            }

            return path;
        }
        
        // sert à récupérer l'index x de la matrice correspond à la position "p"
        private int GetIndexX(Vector3 p)
        {
            return (int)((p.x - coordOrigin.x) / bond);
        }
        
        // sert à récupérer l'index z de la matrice correspond à la position "p"
        private int GetIndexZ(Vector3 p)
        {
            return (int)((p.z - coordOrigin.z) / bond);
        }
    }
}
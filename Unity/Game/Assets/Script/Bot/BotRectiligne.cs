using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Script;
using Script.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotRectiligne : BotClass
{
    //Etat
    private int EtatCheminTournerAttendre; // EnChemin -> 0 ; Tourner -> 1 ; Attendre -> 2

    //Destination
    private Vector3 coordDestination;
    
    //Rotation
    private float rotationSpeed = 200;
    private float amountRotation;
    
    //Ecart maximum entre le point et sa position pour qu'il soit considéré comme arrivé à destination
    private float ecartDistance = 0.5f;
    
    //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
    private float ecartTime = 1;
    private float lastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire
    
    // La liste des coordonnées possible (pour l'instant c'est les crossPoints)
    private Vector3[] listCordonnées;
    private int nResultReceive;
    private List<Vector3> validDestinations;
    
    //Getter

    public int GetEtat() => EtatCheminTournerAttendre;

    public void Start()
    {
        SetRbTr();
        StartBot(); // il faut instancier maxHealth pour instancier currentHealth
        StartHuman(); // voila pourquoi j'ai mis StartBot avant StartHuman

        listCordonnées = CrossManager.Instance.GetListPosition(); // je dois l'intancier avant d'utiliser 'FindNewDestination'

        FindNewDestination(); // va changer l'etat du bot
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) // Seul le masterClient contrôle les bots
            return;
        
        UpdateBot(); // quoi que ce soit son état, il fait ça

        if (EtatCheminTournerAttendre == 2)// s'il est en train d'attendre
        {
            moveAmount = Vector3.zero;
            return;
        }

        if (Time.time - lastCalculRotation > ecartTime) // il recalcule sa position tous les 'ecartTime'
        {
            FindAmountRotation();
        }
            
        if (EtatCheminTournerAttendre == 0) // est en chemin
        {
            if (Calcul.Distance(Tr.position, coordDestination) < ecartDistance) // arrivé
            {
                FindNewDestination();
                anim.enabled = false;
            }
            else // avancer
                Avancer();
        }
        else // se troune
        {
            moveAmount = Vector3.zero; // Le bot rectiligne n'avançera jamais lorqu'il tournera
            Tourner();
        }
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        moveEntity();
    }

    // Pour trouver une nouvelle destination, ce bot envoie des body Chercheur en direction de toutes les coordonnées de la 'listCoordonnées'.
    // Ainsi, il doit attendre la réponse de ceux-ci, pendant ce temps, ils vont rester immobile
    private void FindNewDestination()
    {
        for (int i = 0; i < listCordonnées.Length; i++)
        {
            if (Calcul.Distance(Tr.position, listCordonnées[i]) > 2) // on ne veut pas lancer un body chercheur la où on se situe
            {
                coordDestination = listCordonnées[i];
                FindAmountRotation(); // change amountRotation pour le bodyChercheur (c'est pas grave s'il change l'etat du joueur puisqu'on va le faire juste à la fin de cette fontion)
                
                BodyChercheur.InstancierStatic(this, listCordonnées[i], new Vector3(0, amountRotation, 0)); // rotate seulement sur y
            }
        }
        
        EtatCheminTournerAttendre = 2; // il doit attendre la réponse des bodyChercheur
        validDestinations = new List<Vector3>(); // instanicie la list où seront stockés toutes les positions valides
        nResultReceive = 0; // il n'a encore reçu aucun résultat
    }

    public void FoundResultDestination(bool valid, Vector3 coord) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
    {
        if (EtatCheminTournerAttendre != 2)
        {
            Debug.Log("PROBLEME");
            Debug.Log("Dans la class 'botRectilgne', dans la fonction 'FoundResultDestination'");
            Debug.Log("Un script a appelé cette fonction alors que ce que Sacha avait prévu que ça n'arriverai pas (Prévenez Sacha)");
            return;
        }
        
        nResultReceive += 1;

        if (valid)
        {
            validDestinations.Add(coord); // ajoute dans les potentielles destinations que le bot suivra
        }

        if (nResultReceive == listCordonnées.Length - 1) // a reçu tous les résultats (-1 parce qu'on n'envoie pas la coordonné où le bot est)
        {
            Debug.Log("Tous les body ont été reçu");
            
            if (validDestinations.Count == 0)
            {
                Debug.Log("Il y a une coordonnée qui ne possède aucune destination de valide");
            }

            coordDestination = validDestinations[Random.Range(0, validDestinations.Count)]; // il choisi aléatoirement une coord parmi toutes les deestinations valides
            FindAmountRotation(); // son état va changer (il ne va plus attendre)
        }
    }
    
    // Cette fonction trouve le degré nécessaire (entre ]-180, 180]) afin que le soit orienté face à sa destination
    public void FindAmountRotation() // Change aussi l'état du joueur
    {
        float diffX = coordDestination.x - Tr.position.x;
        float diffZ = coordDestination.z - Tr.position.z;

        if (diffX == 0)
        {
            amountRotation = 0;
        }
        else if (diffZ == 0)
        {
            if (diffX < 0)
                amountRotation = -90;
            else
                amountRotation = 90;
        }
        else
        {
            amountRotation = SimpleMath.ArcTan(diffX, diffZ); // amountRotation : Df = ]-90, 90[
            
            if (diffZ < 0) // Fait quatre schémas avec les différentes configurations pour comprendre
            {
                if (diffX < 0)
                    amountRotation -= 180; // amountRotation était positif
                else
                    amountRotation += 180; // amountRotation était négatif
            }
        }
        
        // On doit ajouter sa rotation initiale à la rotation qu'il devait faire s'il était à 0 degré
        amountRotation -= transform.eulerAngles.y; // eulerAngles pour récupérer l'angle en degré

        if (amountRotation > 180) // Le degré est déjè valide, seulement, il est préférable de tourner de -150° que de 210° (par exemple)
            amountRotation -= 360;
        else if (amountRotation < -180)
            amountRotation += 360;

        if (SimpleMath.Abs(amountRotation) < 5) // Si le dégré est négligeable, le bot continue sa course
        {
            EtatCheminTournerAttendre = 0; // va directement avancer
        }
        else
        {
            EtatCheminTournerAttendre = 1; // va tourner
        }

        lastCalculRotation = Time.time;
    }

    private void Avancer()
    {
        SetMoveAmount(new Vector3(0, 0, 1), 2);
        
        anim.enabled = true;
        anim.Play("Avant");
    }

    private void Tourner()
    {
        int sensRotation;
        if (amountRotation >= 0)
            sensRotation = 1;
        else
            sensRotation = -1;

        float yRot = sensRotation * rotationSpeed * Time.deltaTime;

        if (SimpleMath.Abs(amountRotation) < SimpleMath.Abs(yRot)) // Le cas où on a finis de tourner
        {
            Tr.Rotate(new Vector3(0, amountRotation, 0));
            EtatCheminTournerAttendre = 0; // il va avancer maintenant
        }
        else
        {
            Tr.Rotate(new Vector3(0, yRot, 0));
            amountRotation -= yRot;
        }
    }
}

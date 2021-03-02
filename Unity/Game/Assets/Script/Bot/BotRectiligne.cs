using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Script.Tools;
using UnityEngine;

public class BotRectiligne : BotClass
{
    //Etat
    private bool Enchemin;
    
    //Précédent point
    private int IndexPreviousPoint; //Va être instancier pour la première fois dans BotManager

    public void SetIndexPreviousPoint(int index)
    {
        IndexPreviousPoint = index;
    }
    
    //Destination
    private int IndexDestination;
    private Vector3 coordDestination;
    
    //Deplacement
    private float rotationSpeed = 100;
    private float amountRotation;
    
    //Ecart maximum entre le point et sa position pour qu'il soit considéré comme arrivé à destination
    private float ecartDistance = 0.5f;
    
    //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
    private float ecartTime = 1;
    private float lastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire


    void Awake()
    {
        AwakeEntity();
    }

    public void Start()
    {
        Vector3 coord = CrossManager.Instance.GetPosition(IndexPreviousPoint);
        Tr.position += coord;

        FindNewDestination();
        FindAmountRotation();
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (Time.time - lastCalculRotation > ecartTime)
        {
            FindAmountRotation();
        }
        
        if (Enchemin)
        {
            if (Calcul.Distance(Tr.position, coordDestination) < ecartDistance) // arrivé
            {
                IndexPreviousPoint = IndexDestination;
                FindNewDestination();
                FindAmountRotation();
                anim.enabled = false;
            }
            else
                Avancer();
        }
        else
        {
            moveAmount = Vector3.zero; //En effet, le bot rectiligne n'avançera jamais lorqu'il tournera
            Tourner();
        }
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        moveEntity();
    }

    private void FindNewDestination()
    {
        (coordDestination, IndexDestination) = CrossManager.Instance.GetRandomPosition(IndexPreviousPoint);
        
        //coordDestination = CrossManager.Instance.GetPosition(2);
    }

    public void FindAmountRotation()
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
        
        // On doit ajouter sa rotation initiale à la rotation qu'il devait faire s'il étatit à 0 degré
        amountRotation -= transform.eulerAngles.y; // eulerAngles pour récupérer l'angle en degré

        if (amountRotation > 180) // Le degré est déjè valide, seulement, il est préférable de tourner de -150° que de 210°
            amountRotation -= 360;
        else if (amountRotation < -180)
            amountRotation += 360;

        Enchemin = SimpleMath.Abs(amountRotation) < 5; // Si le dégré est négligeable, le bot continue sa course
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
            Enchemin = true;
        }
        else
        {
            Tr.Rotate(new Vector3(0, yRot, 0));
            amountRotation -= yRot;
        }

        lastCalculRotation = Time.time;
    }
}

using UnityEngine;
using Photon.Pun;

public class Entity : MonoBehaviourPunCallbacks
{
    protected Transform Tr;
    protected Rigidbody Rb;

    //Déplacement
    protected Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    private float smouthTime = 0.15f;
    
    protected void SetRbTr()
    {
        Rb = GetComponent<Rigidbody>();
        Tr = GetComponent<Transform>();
    }
    
    protected void SetMoveAmount(Vector3 moveDir, float speed) // moveDir doit être de la forme (1, 0, 0), (0, 0, -1), (1, 0, 1)... mais pas de 1 sur y (pour les humains du moins)
    {
        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDir * speed,
            ref smoothMoveVelocity, smouthTime);
    }
    
    protected void moveEntity()
    {
        //Déplace le corps du human grâce à moveAmount précédemment calculé
        Rb.MovePosition(Rb.position + Tr.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
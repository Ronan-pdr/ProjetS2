using UnityEngine;
using Photon.Pun;

namespace Script.EntityPlayer
{
    public class Entity : MonoBehaviourPunCallbacks
    {
        protected Transform Tr;
        protected Rigidbody Rb;

        //Déplacement
        protected Vector3 MoveAmount;
        private Vector3 smoothMoveVelocity;
        private float smouthTime = 0.15f;
    
        protected void SetRbTr()
        {
            Rb = GetComponent<Rigidbody>();
            Tr = GetComponent<Transform>();
        }
    
        protected void SetMoveAmount(Vector3 moveDir, float speed) // moveDir doit être de la forme (1, 0, 0), (0, 0, -1), (1, 0, 1)... mais pas de 1 sur y (pour les humains du moins)
        {
            MoveAmount = Vector3.SmoothDamp(MoveAmount,
                moveDir * speed,
                ref smoothMoveVelocity, smouthTime);
        }
    
        protected void MoveEntity()
        {
            //Déplace le corps du human grâce à moveAmount précédemment calculé
            Rb.MovePosition(Rb.position + Tr.TransformDirection(MoveAmount) * Time.fixedDeltaTime);
        }
    }
}
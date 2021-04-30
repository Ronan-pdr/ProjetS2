using System;
using Script.Manager;
using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanHeart : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        private Humanoide _mySelf;
        private float _timeLastHit;
        private int _degatHit;

        // ------------ Constructeur ------------
        private void Start()
        {
            _mySelf = GetComponentInParent<Humanoide>();

            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Game)
            {
                _degatHit = 13;
            }
            else
            {
                _degatHit = 33;
            }
        }

        // ------------ Events ------------
        private void OnTriggerEnter(Collider other)
        {
            Hit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            Hit(other);
        }

        private void Hit(Collider other)
        {
            // Si c'est pas à toi, tu ne fais
            if (!_mySelf.GetPv().IsMine)
                return;
            
            // Si c'est une Entity, on s'en fout
            if (other.GetComponent<Entity>())
                return;

            // On n'enlève des points de vie seulement tous les certains temps
            if (Time.time - _timeLastHit < 1)
                return;

            Debug.Log($"Le coeur de {_mySelf} est rentré en collision avec {other.name}");

            _mySelf.TakeDamage(_degatHit);
            _timeLastHit = Time.time;
        }
    }
}

using System;
using Script.Manager;
using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanHeart : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        private Humanoide _human;
        private float _timeLastHit;
        private int _degatHit;

        // ------------ Constructeur ------------
        private void Start()
        {
            _human = GetComponentInParent<Humanoide>();

            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Game)
            {
                _degatHit = 1;
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
            // Si c'est ton propre corps c'est pas grave
            if (other.GetComponent<Humanoide>() == _human)
                return;

            // On n'enlève des points de vie seulement tous les certains temps
            if (Time.time - _timeLastHit < 1)
                return;

            _human.TakeDamage(_degatHit);
            _timeLastHit = Time.time;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Script.Tools;
using UnityEngine;

namespace Script.Animation
{
    public abstract class HumanAnim : MonoBehaviour
    {
        // ------------ SerializeField ------------
    
        [SerializeField] private Animator animator;
    
        // ------------ Attributs ------------
        public enum Type
        {
            Idle,
            Forward,
            Backward,
            Right,
            Left,
            DiagR,
            DiagL,
            Run,
            Sit,
            Squat,
            Jump,
            Hit,
            Aiming,
            Shoot
        }

        private Dictionary<Type, int> _dict;
        private Type precAnim;
        
        // pour les triggers
        private (float time, Type anim) trigger;

        // ------------ Getter ------------

        protected abstract Dictionary<Type, int> GetDict();
        
        // ------------ Setter ------------
        
        public void Set(Type newAnim, bool etat = true)
        {
            // potentiel erreur
            CheckError(newAnim);

            StopPrevious();

            if (IsState(newAnim)) // state
            {
                // rien de plus
            }
            else if (IsTrigger(newAnim, out float timeAnim)) // trigger
            {
                // enlever la précédente
                Stop(trigger.anim);
                
                trigger.time = Time.time + timeAnim;
                trigger.anim = newAnim;
            }
            else // continue
            {
                precAnim = newAnim;
            }

            animator.SetBool(_dict[newAnim], etat);
        }

        public void Set(HumanAnim humanAnim)
        {
            if (humanAnim is null)
                return;

            foreach (KeyValuePair<Type, int> e in humanAnim._dict)
            {
                if (_dict.ContainsKey(e.Key) && animator.GetBool(e.Value))
                {
                    animator.SetBool(_dict[e.Key], true);

                    Debug.Log($"Le met en mode '{e.Key}'");
                }
            }
        }

        // ------------ Constructeur ------------

        private void Awake()
        {
            precAnim = Type.Idle;
            _dict = GetDict();
        }
        
        // ------------Update ------------

        private void Update()
        {
            // gérer les triggers
            if (SimpleMath.IsEncadré(Time.time, trigger.time, 0.1f))
            {
                Stop(trigger.anim);
                trigger.anim = Type.Idle;
            }
        }

        // ------------ Méthodes ------------

        public void Stop(Type animToStop)
        {
            // potentiel erreur
            CheckError(animToStop);
            
            if (animToStop != Type.Idle)
            {
                animator.SetBool(_dict[animToStop], false);
            }

            precAnim = Type.Idle;
        }

        public void StopPrevious()
        {
            Stop(precAnim);
        }

        // private
        private bool IsState(Type type)
        {
            return type == Type.Squat ||
                   type == Type.Sit ||
                   type == Type.Shoot;
        }

        private bool IsTrigger(Type type, out float time)
        {
            switch (type)
            {
                case Type.Hit:
                    time = 0.25f;
                    return true;
                default:
                    time = 0;
                    return false;
            }
        }

        private void CheckError(Type type)
        {
            if (type != Type.Idle && !_dict.ContainsKey(type))
            {
                throw new Exception($"La class '{this}' ne contient pas l'animation {type}");
            }
        }
    }
}
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

        protected Dictionary<Type, int> Dict;
        private Type[] _animContinue;
        
        // pour les triggers
        private (float time, Type anim) _trigger;
        
        // ------------ Setter ------------
        
        protected abstract void AddAnim();
        
        public void Set(Type newAnim)
        {
            // potentiel erreur
            CheckError(newAnim);

            StopContinue();
            
            if (IsState(newAnim)) // state
            {
                // rien de plus
            }
            else if (IsTrigger(newAnim, out float timeAnim)) // trigger
            {
                // enlever la précédente
                Stop(_trigger.anim);
                
                _trigger.time = Time.time + timeAnim;
                _trigger.anim = newAnim;
            }
            else // continue
            {
                // elle sont stocké dans une liste
            }

            animator.SetBool(Dict[newAnim], true);
        }

        public void Set(HumanAnim humanAnim)
        {
            if (humanAnim is null)
                return;

            foreach (KeyValuePair<Type, int> e in humanAnim.Dict)
            {
                if (Dict.ContainsKey(e.Key) && animator.GetBool(e.Value))
                {
                    animator.SetBool(Dict[e.Key], true);

                    Debug.Log($"Le met en mode '{e.Key}'");
                }
            }
        }

        // ------------ Constructeur ------------

        private void Awake()
        {
            Dict = new Dictionary<Type, int>();
            
            // deplacement (anim continue)
            _animContinue = new []
            {
                Type.Forward, Type.Backward, Type.Right, Type.Left,
                Type.DiagR, Type.DiagL, Type.Run
            };

            Dict.Add(Type.Forward, Animator.StringToHash("isWalking"));
            Dict.Add(Type.Backward, Animator.StringToHash("isWalkingBack"));
            Dict.Add(Type.Right, Animator.StringToHash("isSideWalkingR"));
            Dict.Add(Type.Left, Animator.StringToHash("isSideWalkingL"));
            Dict.Add(Type.DiagR, Animator.StringToHash("isRF"));
            Dict.Add(Type.DiagL, Animator.StringToHash("isLF"));
            Dict.Add(Type.Run, Animator.StringToHash("isRunning"));
            
            // one touch
            Dict.Add(Type.Squat, Animator.StringToHash("isSquatting"));
            Dict.Add(Type.Jump, Animator.StringToHash("isJumping"));
            
            // ajouter des anims spécifique
            AddAnim();
        }
        
        // ------------Update ------------

        private void Update()
        {
            // gérer les triggers
            if (SimpleMath.IsEncadré(Time.time, _trigger.time, 0.1f))
            {
                Stop(_trigger.anim);
                _trigger.anim = Type.Idle;
            }
        }

        // ------------ Méthodes ------------

        public void Stop(Type animToStop)
        {
            // potentiel erreur
            CheckError(animToStop);
            
            if (animToStop != Type.Idle)
            {
                animator.SetBool(Dict[animToStop], false);
            }
        }

        public void StopContinue()
        {
            foreach (Type anim in _animContinue)
            {
                Stop(anim);
            }
        }

        // private
        private bool IsState(Type type)
        {
            return type == Type.Squat ||
                   type == Type.Sit ||
                   type == Type.Aiming ||
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
            if (type != Type.Idle && !Dict.ContainsKey(type))
            {
                throw new Exception($"La class '{this}' ne contient pas l'animation {type}");
            }
        }
    }
}
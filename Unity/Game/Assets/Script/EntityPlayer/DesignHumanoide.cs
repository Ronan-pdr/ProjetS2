using System;
using Script.Animation;
using UnityEngine;


namespace Script.EntityPlayer
{
    public class DesignHumanoide
    {
        // ------------ Attributs ------------

        private HumanAnim _anim;
        private GameObject[] _designs;
        private int _indexDesign;
        
        // ------------ Getter ------------

        public int Index => _indexDesign;

        public int Length => _designs.Length;

        // ------------ Setter ------------

        public void Set(int index)
        {
            if (index >= _designs.Length)
            {
                throw new Exception($"Il n'y a que {_designs.Length} designs ; index = {index}");
            }

            _designs[_indexDesign].SetActive(false);
            _indexDesign = index;
            _designs[_indexDesign].SetActive(true);

            SetAnim();
        }

        private void SetAnim()
        {
            _anim.SetAnimator(_designs[_indexDesign].GetComponent<Animator>());
        }
        
        // ------------ Constructeur ------------

        public DesignHumanoide(HumanAnim anim, GameObject[] designs)
        {
            _anim = anim;
            _designs = designs;
            _indexDesign = 0;
            
            for (int i = designs.Length - 1; i >= 0; i--)
            {
                designs[i].SetActive(i == _indexDesign);
            }
            
            SetAnim();
        }
    }
}
﻿using System;

namespace Script.Tools
{
    public class NodeFile<T>
    {
        private NodeFile<T> _after;
        private T _key;

        // getters et setters
        public T Key
        {
            get => _key;
        }
        public NodeFile<T> After
        {
            // attribut
            get => _after;
            set
            {
                if (value == this)
                    throw new Exception("You try to link a node with himself");

                if (!(_after is null))
                    throw new Exception("Impossible to change \'After\' of a node in a file");
                
                _after = value;
            }
        }

        // constructeur
        public NodeFile(T key)
        {
            _key = key;
            _after = null;
        }
    }
    
    public class MyFile<T>
    {
        // attribut
        private NodeFile<T> _tete;
        private NodeFile<T> _queue;
        
        // constructeur
        public MyFile()
        {
            _tete = null;
            _queue = null;
        }
        
        // méthodes
        public bool IsEmpty() => _tete is null;

        public void Enfiler(T key)
        {
            NodeFile<T> node = new NodeFile<T>(key);
            
            if (_tete is null)
            {
                _tete = node;
                _queue = node;
            }
            else
            {
                _queue.After = node;
                _queue = node;
            }
        }

        public T Defiler()
        {
            if (IsEmpty())
            {
                throw new Exception("You try to défiler an empty file");
            }

            T res = _tete.Key;

            _tete = _tete.After;

            if (_tete is null)
            {
                _queue = null;
            }

            return res;
        }
    }
}
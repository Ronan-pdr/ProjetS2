using System;

namespace Script.Tools
{
    public class MyFile<T>
    {
        // attribut
        private Node<T> _tete;
        private Node<T> _queue;
        
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
            Node<T> node = new Node<T>(key);
            
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
        
        private class Node<TN>
        {
            private Node<TN> _after;
            private TN _key;

            // getters et setters
            public TN Key
            {
                get => _key;
            }
            public Node<TN> After
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
            public Node(TN key)
            {
                _key = key;
                _after = null;
            }
        }
    }
}
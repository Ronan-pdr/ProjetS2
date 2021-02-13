using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform _transform;
    public Rigidbody _rigidbody;
    public static Chassé PlayerChassé;
        
    void Start()
    {
        PlayerChassé = new Chassé(_transform, _rigidbody);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerChassé.Upd();
    }
}

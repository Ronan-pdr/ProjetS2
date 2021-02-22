using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform tr;
    public Rigidbody rb;
    public Transform cam;
    public static Chassé PlayerChassé;
        
    void Start()
    {
        PlayerChassé = new Chassé(tr, rb, cam);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerChassé.Upd();
    }
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            Debug.Log("I hit the ground !");
            PlayerChassé.Ground = true;
        }
    }
}

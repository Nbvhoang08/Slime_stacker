using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
   [SerializeField] private GameManager gameManager;
   [SerializeField] private Animator anim;



    void Update()
    {
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        anim.SetBool("checking", gameManager.isCountingDown);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("slime"))
        {
            Destroy(other);
        }   
    }



}

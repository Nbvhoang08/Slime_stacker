using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] private TextMeshPro timer;
 
    void OnEnable() 
    {
        if(gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }    
    }
    void Update()
    {
        if(gameManager == null){
            gameManager = FindObjectOfType<GameManager>();
        }
        timer.text = Convert.ToInt32(gameManager.timer).ToString();

    }



}

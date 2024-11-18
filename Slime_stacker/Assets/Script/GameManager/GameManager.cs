using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool hasWon;
    [SerializeField] private QueueSystem queueSystem;
    [SerializeField] private List<GameObject> slime;

    void Start()
    {
        if (queueSystem != null)
        {
            queueSystem.InitializePendingObjects(slime);
        }
    }
}

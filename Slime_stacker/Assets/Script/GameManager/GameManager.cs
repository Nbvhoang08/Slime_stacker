using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool hasWon;
    public bool gameOver;
    [SerializeField] private QueueSystem queueSystem;
    [SerializeField] private List<GameObject> slimePrefab;

    public List<GameObject> slime;
    public int countDown = 5;
    public float timer;
    
    // Thêm padding để tạo buffer zone


   public bool isCountingDown = false;

    void Start()
    {
        if (queueSystem != null)
        {
            queueSystem.InitializePendingObjects(slimePrefab);
        }
    }

    void Update()
    {
        CheckLose();
        if(queueSystem.objectQueue.Count == 1 && !queueSystem.isDragging)
        {
            CheckWin();
        }
    }    
    void CheckLose()
    {
        float distanceThreshold = 10.0f; // Khoảng cách tối đa từ góc tọa độ (0,0)
        foreach (GameObject obj in slime)
        {
            if (obj != null && !IsObjectWithinDistance(obj, distanceThreshold))
            {
                GameOver();
                break;
            }
        }
    }

    public void CheckWin()
    {
        if (!isCountingDown)
        {
            foreach (GameObject obj in slime)
            {
                if (obj != null && IsObjectWithinDistance(obj, 10.0f))
                {
                    StartCoroutine(CountDown());
                    break;
                }
            }
        }
    }

    IEnumerator CountDown()
    {
        isCountingDown = true;
        timer = countDown;
        while (timer > 0)
        {
            yield return new WaitForSeconds(1);
            timer--;
        }

        bool allObjectsVisible = true;
        foreach (GameObject obj in slimePrefab)
        {
            if (obj == null || !IsObjectWithinDistance(obj, 10.0f))
            {
                allObjectsVisible = false;
                GameOver();
                break;
            }
        }

        if (allObjectsVisible && !gameOver && !hasWon)
        {
            hasWon = true;
            UIManager.Instance.OpenUI<WinCanvas>();
            LevelManager.Instance.SaveGame();
            Time.timeScale = 0;
            Debug.Log("win");
        }

        //isCountingDown = false;
    }

    bool IsObjectWithinDistance(GameObject obj, float distanceThreshold)
    {
        float distance = Vector3.Distance(obj.gameObject.transform.position, Vector3.zero);
        
        return distance <= distanceThreshold;
    }
    void GameOver()
    {
        if(!gameOver)
        {
            UIManager.Instance.OpenUI<LoseCanvas>();
            Debug.Log("lose");
            gameOver = true;
        }else
        {
            return;
        }
        
    }
}

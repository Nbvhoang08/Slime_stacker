using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    void Start()
    {
        UIManager.Instance.OpenUI<HomeCanvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
            Debug.Log("?");
        }
    }
    void Update()
    {
        if ((canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.ScreenSpaceOverlay) && canvas.worldCamera == null)
        {
            Camera mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            if (mainCamera != null)
            {
                canvas.worldCamera = mainCamera;
            }
        }   
    }
}

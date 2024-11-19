using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoseCanvas : UICanvas
{
    // Start is called before the first frame update
    [SerializeField] Canvas canvas;
    void OnEnable()
    {
        if(canvas!=null) canvas.sortingOrder  = 100;
    }
    void OnDisable()
    {
        if(canvas!=null)canvas.sortingOrder = -100;
    }
    public void RetryBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UIManager.Instance.CloseUI<LoseCanvas>(0.1f);
            
    }

    public void HomeBtn()
    {
        UIManager.Instance.CloseAll();
        Time.timeScale = 1; 
        SceneManager.LoadScene("Home");     
        UIManager.Instance.OpenUI<HomeCanvas>();
            
    }
}

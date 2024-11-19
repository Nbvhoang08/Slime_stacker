using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinCanvas : UICanvas
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


    public void NextBtn()
    {
        Time.timeScale = 1;
        UIManager.Instance.CloseUI<WinCanvas>(0.2f);
        LoadNextScene();
        SoundManager.Instance.PlayVFXSound(2);
    }
    

    public void HomeBtn()
    {
        UIManager.Instance.CloseAll();
        Time.timeScale = 1; 
        SceneManager.LoadScene("Home");
        SoundManager.Instance.PlayVFXSound(2);    
        UIManager.Instance.OpenUI<HomeCanvas>();
            
    }
    public void SoundBtn()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        SoundManager.Instance.PlayVFXSound(2);   
    }

   
    
    public void LoadNextScene() 
    { 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        int nextSceneIndex = currentSceneIndex + 1; 
        // Kiểm tra xem scene tiếp theo có tồn tại không
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("Home");
            UIManager.Instance.OpenUI<HomeCanvas>();
            UIManager.Instance.CloseUIDirectly<GamePlayCanvas>();
        } 
    }

    IEnumerator NextSence()
    {
        yield return new WaitForSeconds(0.3f);
        LoadNextScene();
    }
}

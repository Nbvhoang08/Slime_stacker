using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GamePlayCanvas : UICanvas
{
   
        [SerializeField] private Text _levelText;
        [SerializeField] private GameManager _gameManager;
         [SerializeField] private Image buttonImage;
        public Sprite OnVolume;
        public Sprite OffVolume;
        
        


        private void Awake()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }
        }

    
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log(scene.name);
           
        }


        

        private void Update()
        {
            UpdateLevelText();
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }
           
            UpdateButtonImage();
        }

        public void RetryButton()
        {
            StartCoroutine(ReLoad());
            SoundManager.Instance.PlayVFXSound(2);
        }
         IEnumerator ReLoad()
        {
            yield return new WaitForSeconds(0.3f);
            ReloadCurrentScene();
        }
    
        public void ReloadCurrentScene()
        {
            // Lấy tên của scene hiện tại 
            string currentSceneName = SceneManager.GetActiveScene().name;
            //Tải lại scene hiện tại
            SceneManager.LoadScene(currentSceneName);
        }




        private void UpdateLevelText()
        {
            if (_levelText != null)
            {
                _levelText.text = SceneManager.GetActiveScene().name;
            }
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
            UpdateButtonImage();
           
        }

        private void UpdateButtonImage()
        {
            if (SoundManager.Instance.TurnOn)
            {
                buttonImage.sprite = OnVolume;
            }
            else
            {
                buttonImage.sprite = OffVolume;
            }
        }
  
        


}

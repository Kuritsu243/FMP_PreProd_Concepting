using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UI
{
    // src https://www.youtube.com/watch?v=CE9VOZivb3I
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private GameObject crossFadeObj;
        [SerializeField] private TextMeshProUGUI loadingText;
        

        private static readonly int Start1 = Animator.StringToHash("Start");
        private Animator crossFadeAnim;

        private bool _hasFaded;
        private void Start()
        {
            loadingText.enabled = false;
            startBtn.onClick.AddListener(StartGame);
            closeBtn.onClick.AddListener(CloseGame);

            crossFadeAnim = crossFadeObj.GetComponent<Animator>();
        }
        
        private void StartGame()
        {
            LoadNextScene();
        }

        private static void CloseGame()
        {
            switch (Application.platform)
            {
#if UNITY_EDITOR
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxEditor:
                EditorApplication.ExitPlaymode();
                break;
#endif
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:
                Application.Quit();
                break;
            }
        }

        private void LoadNextScene()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

        private IEnumerator LoadLevel(int levelIndex)
        {
            loadingText.enabled = true;
            crossFadeAnim.SetTrigger(Start1);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(levelIndex);
        }



    }
}

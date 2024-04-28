using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private Button retryBtn;
        [SerializeField] private Button quitBtn;

        private void Start()
        {
            retryBtn.onClick.AddListener(RetryGame);
            quitBtn.onClick.AddListener(CloseGame);
        }

        private static void CloseGame()
        {
            switch (Application.platform)
            {
#if UNITY_EDITOR
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                    EditorApplication.ExitPlaymode();
                    break;
#endif
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.WindowsPlayer:
                    Application.Quit();
                    break;
            }
        }

        private static void RetryGame()
        {
            SceneManager.LoadScene("startScene");
        }
    }
    
}

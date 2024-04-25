using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    // src https://www.youtube.com/watch?v=CE9VOZivb3I
    public class StartScreen : MonoBehaviour
    {

        [SerializeField] private GameObject crossFadeObj;
        [SerializeField] private TextMeshProUGUI loadingText;
        [Header("Buttons")]
        [SerializeField] private Button startBtn;
        [SerializeField] private Button aboutBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button settingsBtn;
        [SerializeField] private Button applyBtn;
        [SerializeField] private Button aboutCloseBtn;
        [SerializeField] private Button settingsCloseBtn;
        
        [Header("Settings Menu")] 
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float currentVolume;
        
        [Header("UI Panel Objects")]
        [SerializeField] private GameObject aboutMenu;
        [SerializeField] private GameObject settingsMenu;
        
        private List<Resolution> _screenResolutions;
        private static readonly int Start1 = Animator.StringToHash("Start");
        private Animator _crossFadeAnim;
        private int _currentResIndex;
        private bool _hasFaded;
        private void Start()
        {
            loadingText.enabled = false;
            startBtn.onClick.AddListener(StartGame);
            closeBtn.onClick.AddListener(CloseGame);
            aboutBtn.onClick.AddListener(ShowAboutMenu);
            aboutCloseBtn.onClick.AddListener(CloseAboutMenu);
            settingsBtn.onClick.AddListener(ShowSettingsMenu);
            settingsCloseBtn.onClick.AddListener(CloseSettingsMenu);
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
            volumeSlider.onValueChanged.AddListener(SetVolume);
            applyBtn.onClick.AddListener(SaveSettings);
            if (!crossFadeObj.activeSelf) crossFadeObj.SetActive(true);
            if (aboutMenu.activeSelf) aboutMenu.SetActive(false);
            if (settingsMenu.activeSelf) settingsMenu.SetActive(false);
            InitializeResolutions();
            LoadSettings(_currentResIndex);
            _crossFadeAnim = crossFadeObj.GetComponent<Animator>();
        }
        
        private void StartGame()
        {
            LoadNextScene();
        }



        private void ShowAboutMenu()
        {
            ToggleButtonInteractivity(false);
            if (!CheckButtonInteractivity()) return;
            CloseAllMenus();
            aboutMenu.SetActive(true);
        }

        private void CloseAboutMenu()
        {
            ToggleButtonInteractivity(true);
            if (!aboutMenu.activeSelf) return;
            aboutMenu.SetActive(false);
        }

        private void ShowSettingsMenu()
        {
            ToggleButtonInteractivity(true);
            if (!CheckButtonInteractivity()) return;
            CloseAllMenus();
            settingsMenu.SetActive(true);
        }

        private void CloseSettingsMenu()
        {
            ToggleButtonInteractivity(true);
            if (!settingsMenu.activeSelf) return;
            settingsMenu.SetActive(false);
        }

        private bool CheckButtonInteractivity()
        {
            return !(settingsMenu.activeSelf || aboutMenu.activeSelf);
        }

        private void ToggleButtonInteractivity(bool yn)
        {
            startBtn.interactable = yn;
            aboutBtn.interactable = yn;
            closeBtn.interactable = yn;
            settingsBtn.interactable = yn;
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


        private void CloseAllMenus()
        {
            settingsMenu.SetActive(false);
            aboutMenu.SetActive(false);
            ToggleButtonInteractivity(true);
        }
        
        private void ToggleFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        
        private void SetResolution(int resolutionIndex)
        {
            var resolution = _screenResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        
        private void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
            currentVolume = volume;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt("FullScreenPreference", Convert.ToInt32(Screen.fullScreen));
            PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
            PlayerPrefs.SetFloat("VolumePreference", currentVolume);
        }

        private void LoadSettings(int resolutionIndex)
        {
            Screen.fullScreen = PlayerPrefs.HasKey("FullScreenPreference") &&
                                Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenPreference"));

            resolutionDropdown.value = PlayerPrefs.HasKey("ResolutionPreference")
                ? PlayerPrefs.GetInt("ResolutionPreference")
                : resolutionIndex;

            volumeSlider.value = PlayerPrefs.HasKey("VolumePreference") ? PlayerPrefs.GetFloat("VolumePreference") : 1f;
        }

        private void InitializeResolutions()
        {
            resolutionDropdown.ClearOptions();
            var options = new List<string>();
            _screenResolutions = Screen.resolutions.ToList();
            foreach (var t in _screenResolutions)
            {
                var option = t.width + " x " + t.height;
                options.Add(option);
                if (t.width == Screen.currentResolution.width &
                    t.height == Screen.currentResolution.height)
                {
                    _currentResIndex = _screenResolutions.IndexOf(t);
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
        }

        private IEnumerator LoadLevel(int levelIndex)
        {
            loadingText.enabled = true;
            _crossFadeAnim.SetTrigger(Start1);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(levelIndex);
        }



    }
}

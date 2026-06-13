using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    private const string MusicVolumePref = "musicVolume";
    private const string SfxVolumePref = "sfxVolume";

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backFromSettingsButton;
    [SerializeField] private Button backFromCreditsButton;

    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("Title Animation")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainPanel();

        startButton?.onClick.AddListener(StartGame);
        settingsButton?.onClick.AddListener(ShowSettings);
        creditsButton?.onClick.AddListener(ShowCredits);
        quitButton?.onClick.AddListener(QuitGame);
        backFromSettingsButton?.onClick.AddListener(ShowMainPanel);
        backFromCreditsButton?.onClick.AddListener(ShowMainPanel);

        musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);

        InitializeAudioSettingsUI();
        SetupQualityDropdown();
    }

    private void InitializeAudioSettingsUI()
    {
        float musicValue = PlayerPrefs.GetFloat(MusicVolumePref, 0.3f);
        float sfxValue = PlayerPrefs.GetFloat(SfxVolumePref, 1f);

        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(musicValue);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(sfxValue);

        AudioManager.Instance?.SetMusicVolume(musicValue);
        AudioManager.Instance?.SetSFXVolume(sfxValue);
    }

    private void ShowMainPanel()
    {
        mainPanel?.SetActive(true);
        settingsPanel?.SetActive(false);
        creditsPanel?.SetActive(false);
    }

    private void ShowSettings()
    {
        mainPanel?.SetActive(false);
        settingsPanel?.SetActive(true);
    }

    private void ShowCredits()
    {
        mainPanel?.SetActive(false);
        creditsPanel?.SetActive(true);
    }

    private void StartGame()
    {
        SceneLoader.Instance?.LoadScene("GameScene");
    }

    private void QuitGame()
    {
        SceneLoader.Instance?.QuitGame();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.onValueChanged.AddListener(level => QualitySettings.SetQualityLevel(level));
    }
}

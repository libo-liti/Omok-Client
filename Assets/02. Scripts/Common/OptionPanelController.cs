using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [SerializeField]
    private Slider bgmSlider;     //BGM
    [SerializeField]
    private Slider seSlider;   // SE

    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Toggle windowScreenToggle;

    private void Start()
    {
        // 저장된 값 불러오기
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        seSlider.value = PlayerPrefs.GetFloat("SEVolume", 0.5f);

        // 슬라이더 이벤트
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        seSlider.onValueChanged.AddListener(OnSEVolumeChanged);

        fullScreenToggle.isOn = Screen.fullScreen;
        windowScreenToggle.isOn = !Screen.fullScreen;
        fullScreenToggle.onValueChanged.AddListener((ischeck) =>
        {
            fullScreenToggle.isOn = ischeck;
            windowScreenToggle.isOn = !ischeck;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        });
        windowScreenToggle.onValueChanged.AddListener((ischeck) =>
        {
            fullScreenToggle.isOn = !ischeck;
            windowScreenToggle.isOn = ischeck;
            Screen.SetResolution(1280, 720, false);
        });
    }

    private void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
    }

     private void OnSEVolumeChanged(float value)
     {
         AudioManager.Instance.SetSEVolume(value);
     }
}
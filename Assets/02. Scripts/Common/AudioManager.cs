using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource bgmSource; // BGM
    [SerializeField] private AudioClip bgmClip;  
    [SerializeField] private AudioSource seSource;  // SE 
    [SerializeField] private AudioClip placeStoneClip;
    
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            Debug.Log("1M");
            // 메인씬 BGM ON
            if (bgmSource != null && bgmClip != null)
            {
                if (!bgmSource.isPlaying)
                {
                    bgmSource.clip = bgmClip;
                    bgmSource.loop = true;
                    bgmSource.Play();
                }
            }
        }
        else if (scene.name == "Game")
        {
            Debug.Log("1G");
            // 게임씬 BGM OFF
            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoad;
        
        /*PlayerPrefs.DeleteAll();*/ //임시로 초기화 할떄
        if (!PlayerPrefs.HasKey("BGMVolume"))
        {
            PlayerPrefs.SetFloat("BGMVolume", 0.5f);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        // 기본 볼륨 불러오기
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        SetBGMVolume(bgmVolume);
        
         float seVolume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
         SetSEVolume(seVolume);
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

     public void PlaySE(AudioClip clip)
     {
         if (seSource != null)
         {
             seSource.PlayOneShot(clip);
         }
     }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

     public void SetSEVolume(float volume)
     {
         if (seSource != null) seSource.volume = volume;
         PlayerPrefs.SetFloat("SEVolume", volume);
     }
     
     public void PlayStoneSE()
     {
         PlaySE(placeStoneClip);
     }

}
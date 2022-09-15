using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource[] SFX, backgroundMusic;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)) {
            PlayBackgroundMusic(6);
        }
    }

    public void PlaySFX(int soundToPlay) {
        if (soundToPlay < SFX.Length) {
            SFX[soundToPlay].Play();
        }
    }

    public void PlayBackgroundMusic(int musicToPlay) {
        StopMusic();
        
        if (musicToPlay < backgroundMusic.Length) {
            backgroundMusic[musicToPlay].Play();
        }
    }

    public void StopMusic() {
        foreach (AudioSource music in backgroundMusic) {
            music.Stop();
        }
    }
}

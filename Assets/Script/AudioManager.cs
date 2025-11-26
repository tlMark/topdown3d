using UnityEngine;
using UnityEngine.PlayerLoop;

public class AudioManger : MonoBehaviour
{
    public static AudioManger instance;

    [Header("#BGM Audio Sources")]
    public AudioClip bgmClips;

    public float bgmVolume;

    AudioSource bgmAudioSource;
    AudioHighPassFilter bgmHighPass;

    [Header("#SFX Audio Sources")]
    public AudioClip[] sfxClips;

    public float sfxVolume;
    public int sfxChannels;
    int sfxChannelIndex;

    AudioSource[] sfxAudioSource;

    public enum SFX
    {
        Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win
    }

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        //배경음 초기화
        GameObject bgmObj = new GameObject("BGM Audio Source");
        bgmObj.transform.parent = transform;

        bgmAudioSource = bgmObj.AddComponent<AudioSource>();
        bgmHighPass = Camera.main.GetComponent<AudioHighPassFilter>();

        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.clip = bgmClips;

        //효과음 초기화
        GameObject sfxObj = new GameObject("SFX Audio Source");
        sfxObj.transform.parent = transform;

        sfxAudioSource = new AudioSource[sfxChannels];

        for (int i = 0; i < sfxAudioSource.Length; i++)
        {
            sfxAudioSource[i] = sfxObj.AddComponent<AudioSource>();
            sfxAudioSource[i].playOnAwake = false;
            sfxAudioSource[i].bypassListenerEffects = true;
            sfxAudioSource[i].volume = sfxVolume;
        }
    }
    
    public void PlayBgm(bool play)
    {
        if (play)
        {
            bgmAudioSource.Play();
        }
        else
        {
            bgmAudioSource.Stop();
        }
    }

    public void HighPassBgm(bool play)
    {
        bgmHighPass.enabled = play;
    }
    
    public void PlaySfx(SFX sfx)
    {
        for (int i = 0; i < sfxAudioSource.Length; i++)
        {
            int loopIndex = (i + sfxChannelIndex) % sfxAudioSource.Length;

            if (sfxAudioSource[loopIndex].isPlaying)
            {
                continue;
            }

            int randomIndex = 0;
            if (sfx == SFX.Hit || sfx == SFX.Melee)
            {
                randomIndex = Random.Range(0, 2);
            }

            sfxChannelIndex = loopIndex;
            sfxAudioSource[0].clip = sfxClips[(int)sfx + randomIndex];
            sfxAudioSource[0].Play();

            break;
        }
    }
}

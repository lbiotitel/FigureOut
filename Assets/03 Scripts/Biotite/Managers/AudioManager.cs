using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;



public class AudioManager : MonoBehaviour, IManager
{
    [SerializeField] private int sfxSourceCount = 20;

    private AudioSource bgmSource;
    private List<AudioSource> sfxSources;

    private Dictionary<string, AudioClip> audios;

    public void Init()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();

        audios = new Dictionary<string, AudioClip>();
        sfxSources = new List<AudioSource>();
        for (int i = 0; i < sfxSourceCount; i++)
        {
            sfxSources.Add(gameObject.AddComponent<AudioSource>());
        }

        Managers.Event.AddEvent<PlayerSpawnEvent>((player) => InitDatas());
    }

    private void InitDatas()
    {
        var data = Managers.Data.GetDatas<AudioClip>();

        foreach (var d in data)
        {
            audios.Add(d.name, d);
        }

        Managers.Event.RemoveEvent<PlayerSpawnEvent>((player) => InitDatas());
    }



    public void PlaySfx(string audioName)
    {
        AudioClip audioClip;
        if (!GetAudioClip(audioName, out audioClip)) return;
        foreach (var source in sfxSources)
        {
            if (source.isPlaying) continue;
            source.PlayOneShot(audioClip);
            break;
        }

        sfxSources[0].PlayOneShot(audioClip);
    }

    public void PlayBgm(string audioName, bool isLoop = true)
    {
        AudioClip audioClip;
        if (!GetAudioClip(audioName, out audioClip)) return;
        
        bgmSource.clip = audioClip;
        bgmSource.loop = isLoop;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();
    public void ResumeBgm() => bgmSource.Play();





    private bool GetAudioClip(string audioName, out AudioClip audioClip)
    {
        audioClip = null;

        if (audios.TryGetValue(audioName, out AudioClip targetClip))
        { 
            audioClip = targetClip;
            return true;
        }

        Logger.LogWarning($"{audioName}은 등록되지않은 오디오 입니다.");

        return false;
    }



    public void Release()
    {
        
    }
}

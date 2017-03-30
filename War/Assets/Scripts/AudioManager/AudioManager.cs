/// <summary>
/// 音效管理
/// author: fanzhengyong
/// date: 2017-03-30
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager
{
    private static AudioManager s_instance = null;

    private Dictionary<string, AudioClip> m_audioClips = 
        new Dictionary<string, AudioClip>();

    public void PlayAudio(string path, Vector3 position)
    {
        bool retCode = false;
        AudioClip audioClip = null;
        retCode = m_audioClips.TryGetValue(path, out audioClip);
        if (!retCode)
        {
            audioClip = ResourcesManagerMediator.
                GetNoGameObjectFromResourcesManager<AudioClip>(path);
            m_audioClips.Add(path, audioClip);
        }

        if (audioClip == null)
        {
            LogMediator.LogError("没有找到声音资源 " + path);
            return;
        }

        
        AudioSource.PlayClipAtPoint(audioClip, position);
    }

    public static AudioManager Instance 
    {
        get 
        {
            if (s_instance == null)
            {
                s_instance = new AudioManager();
            }
            return s_instance;
        }
    }

    public void Release()
    {
        s_instance   = null;

        m_audioClips.Clear();
        m_audioClips = null;
    }
}

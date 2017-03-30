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

    private Queue<string> m_audioNames = new Queue<string>();
    private static readonly int MAX_CACHE = 8;
    private int m_count = 0;
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
        s_instance = null;

        m_audioClips.Clear();
        m_audioClips = null;
    }
    public void PlayAudio(string path, Vector3 position)
    {
        bool retCode = false;
        AudioClip audioClip = null;
        retCode = m_audioClips.TryGetValue(path, out audioClip);
        if (!retCode)
        {
            audioClip = ResourcesManagerMediator.
                GetNoGameObjectFromResourcesManager<AudioClip>(path);
            if (audioClip == null)
            {
                LogMediator.LogError("没有找到声音资源 " + path);
                return;
            }
            m_audioClips.Add(path, audioClip);
            m_count++;
        }

        if (audioClip == null)
        {
            LogMediator.LogError("声音资源已经失效 " + path);
            return;
        }

        
        AudioSource.PlayClipAtPoint(audioClip, position);
    }
}

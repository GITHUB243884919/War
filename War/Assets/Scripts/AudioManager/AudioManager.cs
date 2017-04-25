/// <summary>
/// 音效管理
/// author: fanzhengyong
/// date: 2017-04-21
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UF_FrameWork
{
public class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance = null;

    private Dictionary<string, AudioClip> m_audioClips =
        new Dictionary<string, AudioClip>();

    private Queue<string> m_audioPaths = new Queue<string>();
    private static readonly int MAX_SOUND_COUNT = 8;

    private AudioSource m_audioSource = null;

    public Transform CameraTrs { get; set; }

    private readonly static float MAX_DISTANCE = 5f;
    public static AudioManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                Debug.Log("没有持有AudioManager的对象");
            }
            return s_instance;
        }
    }

    
    public void PlaySound(string path, Vector3 position)
    {
        bool retCode        = false;
        AudioClip audioClip = null;

        retCode = m_audioClips.TryGetValue(path, out audioClip);
        if (!retCode)
        {
            audioClip = ResourcesManagerMediator.
                GetNoGameObjectFromResourcesManager<AudioClip>(path);
            if (audioClip == null)
            {
                LogMediator.LogError("音效资源加载失败 " + path);
                return;
            }

            AddAudioClip(path, audioClip);
        }

        if (audioClip == null)
        {
            LogMediator.LogError("音效资源已经失效 " + path);
            return;
        }

        if (CameraTrs == null)
        {
            m_audioSource.PlayOneShot(audioClip);
        }
        else
        {
            float distance = (position - CameraTrs.position).magnitude;
            if (distance < MAX_DISTANCE)
            {
                AudioSource.PlayClipAtPoint(audioClip, position);
                Debug.Log((CameraTrs.position - position).magnitude + "" + position);
            }
            else
            {
                Vector3 _position = MAX_DISTANCE *
                    (position - CameraTrs.position).normalized;
                AudioSource.PlayClipAtPoint(audioClip, _position);
            }
        }
    }

    public void PlayMusic(string path, bool loop)
    {
        AudioClip audioClip = ResourcesManagerMediator.
            GetNoGameObjectFromResourcesManager<AudioClip>(path);
        if (audioClip == null)
        {
            LogMediator.LogError("音效资源加载失败 " + path);
            return;
        }
        
        m_audioSource.clip = audioClip;
        m_audioSource.loop = loop;
        m_audioSource.Play();
    }

    public void StopMusic()
    {
        m_audioSource.Stop();
    }

    private void AddAudioClip(string path, AudioClip audioClip)
    {
        if (m_audioClips.Count == MAX_SOUND_COUNT)
        {
            m_audioClips.Remove(m_audioPaths.Dequeue());
        }

        m_audioClips.Add(path, audioClip);
        m_audioPaths.Enqueue(path);
    }

    private void Release()
    {
        s_instance    = null;
        m_audioSource = null;

        m_audioClips.Clear();
        m_audioClips = null;
        m_audioPaths.Clear();
        m_audioPaths = null;
    }

    #region Unity API
    void Awake()
    {
        s_instance = this;

        m_audioSource = gameObject.GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnDestroy()
    {
        Release();
    }
    #endregion
}
}


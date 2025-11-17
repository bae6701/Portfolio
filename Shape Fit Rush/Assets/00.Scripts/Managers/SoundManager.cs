using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [1주차 v2.6] (v2.5 아키텍처 호환)
/// (MonoBehaviour - Managers.cs가 생성)
/// </summary>
public class SoundManager : MonoBehaviour // [수정]
{
    AudioSource[] _audioSources;
    Dictionary<Define.SoundID, AudioClip> _audioClips = new Dictionary<Define.SoundID, AudioClip>();

    public void Init()
    {
        _audioSources = new AudioSource[(int)Define.Sound.MaxCount];

        GameObject root = new GameObject { name = "@Sound" };
        root.transform.SetParent(Managers.Instance.transform);

        string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
        for (int i = 0; i < soundNames.Length - 1; i++) 
        {
            GameObject go = new GameObject { name = soundNames[i] };
            _audioSources[i] = go.AddComponent<AudioSource>();
            go.transform.parent = root.transform;
        }
        _audioSources[(int)Define.Sound.Bgm].loop = true;
        
        // 2. [핵심] SoundDatabase에서 모든 클립을 딕셔너리로 '캐싱'
        _audioClips.Clear();
        if (Managers.Instance.SoundDB != null)
        {
            foreach (SoundEntry entry in Managers.Instance.SoundDB.sounds)
            {
                if (entry.clip == null) continue;
                
                if (_audioClips.ContainsKey(entry.soundID))
                {
                    Debug.LogError($"SoundID 중복: {entry.soundID}");
                    continue;
                }
                _audioClips.Add(entry.soundID, entry.clip);
            }
        }
        
        // 3. DataManager에서 음소거 설정 로드 (v2.8)
        if (Managers.Instance.Data != null && Managers.Instance.Data.GameData != null)
        {
            MuteBGM(!Managers.Instance.Data.GameData.isBgmOn);
            MuteSFX(!Managers.Instance.Data.GameData.isSfxOn);
        }
    }
    public void MuteBGM(bool mute)
    {
        if (_audioSources == null) return;
        _audioSources[(int)Define.Sound.Bgm].mute = mute;
    }

    public void MuteSFX(bool mute)
    {
        if (_audioSources == null) return;
        _audioSources[(int)Define.Sound.Effect].mute = mute;
    }

    public void Play(Define.SoundID soundID, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetAudioClip(soundID); // [수정]
        Play(audioClip, type, pitch);
    }

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
	{
        if (audioClip == null) return; // (못찾으면 재생 안함)
        
		if (type == Define.Sound.Bgm) {
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
			if (audioSource.isPlaying) audioSource.Stop();
			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
			audioSource.Play();
		} else {
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(audioClip);
		}
	}

	AudioClip GetAudioClip(Define.SoundID soundID)
    {
        if (_audioClips.TryGetValue(soundID, out AudioClip audioClip))
        {
            return audioClip;
        }
        
        Debug.LogWarning($"AudioClip Missing ! (SoundID: {soundID})");
		return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
	// Start is called before the first frame update
	public static AudioMgr _inst;
	[SerializeField] List<AudioSource> audioSources = new List<AudioSource>();
	[SerializeField] AudioClip[] audoClips;
	public bool isSoundOn = true;
	public static AudioMgr inst
	{
		get
		{
			if (_inst == null)
				_inst = FindObjectOfType<AudioMgr>();
			return _inst;
		}
	}
	public void Start()
    {
        _inst = FindObjectOfType<AudioMgr>();
        SetSoundOn(isSoundOn);
    }

	// Update is called once per frame
	public void SetSoundOn(bool b)
	{
        if (audioSources.Count < 0) return;
        if (b)
        {
            if (!audioSources[0].isPlaying)
            {
				audioSources[0].Play();
            }

		} else
        {
			foreach (var audio in audioSources)
			{
				audio.Stop();
			}
		}
	}
	public void PlayMusic()
    {
		audioSources[0].loop = true;
		audioSources[0].Play();
    }
	public void PlaySound(AudioClip clip, float volume, float pitch, float playDelay = 0)
	{
		if (!isSoundOn)
			return;
		if (audioSources.Count < 1) return;
		AudioSource audio = this.audioSources[1];

		audio.playOnAwake = false;
		audio.loop = false;
		audio.volume = volume;
		audio.pitch = pitch;
		audio.clip = clip;

		if (playDelay == 0)
			audio.Play();
		else
			audio.PlayDelayed(playDelay);
	}
	public AudioSource GetAudioSourceOnStandBy()
	{
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (!audioSources[i].isPlaying)
                return audioSources[i];
        }

        AudioSource audio = _inst.gameObject.AddComponent<AudioSource>();
		audioSources.Add(audio);
        return audio;
	}
	public void PlayButton()
    {
		if (!isSoundOn) return;
		this.PlaySound(this.audoClips[0], 1, 1);
    }
	public void PlayGameOver()
	{
		if (!isSoundOn) return;
		this.PlaySound(this.audoClips[4], 1, 1);
	}
	public void PlayGotItem()
	{
		if (!isSoundOn) return;
		this.PlaySound(this.audoClips[2], 1, 1);
	}
	public void PlayBump()
	{
		if (!isSoundOn) return;
		this.PlaySound(this.audoClips[3], 1, 1);
	}
	public void PlayCollect()
	{
		if (!isSoundOn) return;
		this.PlaySound(this.audoClips[1], 1, 1);
	}
}

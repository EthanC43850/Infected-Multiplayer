using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomAudioClip : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip[] audioClips;

	void OnEnable()
	{
		int randomAudioClip = Random.Range(0, audioClips.Length);

		audioSource.clip = audioClips[randomAudioClip];

		audioSource.Play();
	}

}

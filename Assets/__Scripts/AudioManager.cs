using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class Sound {
    public string name;
    public AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 1.0f;
    [Range(0f, 1.5f)] public float pitch = 1.0f;
    public Vector2 randomVolumeRange = new Vector2(1.0f, 1.0f);
    public Vector2 randomPitchRange = new Vector2(1.0f, 1.0f);

    private AudioSource audioSource;

    public void SetSource(AudioSource source) {
        audioSource = source;
        int randomClip = Random.Range(0, clips.Length - 1);
        audioSource.clip = clips[randomClip];
    }

    public void Play() {
        if(clips.Length > 1) {
            int randomClip = Random.Range(0, clips.Length - 1);
            audioSource.clip = clips[randomClip];
        }
        audioSource.volume = volume * Random.Range(randomVolumeRange.x, randomVolumeRange.y);
        audioSource.pitch = pitch * Random.Range(randomPitchRange.x, randomPitchRange.y);
        audioSource.Play();
    }
}

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    [SerializeField] Sound[] sounds;

    private void Awake() {
        if(instance != null) {
            Debug.LogError("More than one AudioManger in scene");
        }
        else {
            instance = this;
        }
    }

    private void Start() {
        for(int i = 0; i < sounds.Length; i++) {
            GameObject go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            go.transform.SetParent(transform);
            sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string name) {
        for(int i = 0; i < sounds.Length; i++) {
            if(sounds[i].name == name) {
                sounds[i].Play();
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound name not found in list: " + name);
    }

    public AudioClip GetClip(string name) {
        for(int i = 0; i < sounds.Length; i++) {
            if(sounds[i].name == name) {
                return sounds[i].clips[0];
            }
        }
        
        Debug.LogWarning("AudioManager: Sound name not found in list: " + name);
        return null;
    }
}

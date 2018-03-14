using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    SCORED_SELF,
    SCORED_OTHER,
    SCORED_UNRELATED
}

public class SFX : MonoBehaviour {

    const int NUM_SOURCES = 8;

    static SFX instance;

    //should match SFXTypes
    public AudioClip[] clips;

    Transform t;
    AudioSource[] sources;
    int currentSource = 0;

    void Awake() {
        instance = this;
        t = transform;

        sources = new AudioSource[NUM_SOURCES];
        for (int i = 0; i < NUM_SOURCES; ++i ) {
            GameObject g = new GameObject("source_" + i);
            g.transform.parent = t;
            g.transform.localPosition = Vector3.zero;
            sources[i] = g.AddComponent<AudioSource>();
        }
    }

    public static void Play( SFXType sound ) {
        Debug.Assert(instance);
        instance.PlaySound(sound);
    }

    void PlaySound(SFXType sound) {
        AudioSource source = sources[currentSource++];

        source.clip = clips[(int)sound];
        source.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class MoveAnimation : MonoBehaviour
{
    public Animation component { get; private set; }

    [SerializeField] private AnimationClip clip;
    [HideInInspector] public float value;

    void Awake()
    {
        component = GetComponent<Animation>();

        component.AddClip(clip, clip.name);
        component.playAutomatically = false;
    }

    public void Play()
    {
        component.Play(clip.name);
    }

    public void Reset()
    {
        component.Stop();
        value = 0;
    }

    // Returns if the animation clip is currently playing
    public bool IsPlaying()
    {
        return component.IsPlaying(clip.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class AudioClipsSO : ScriptableObject
{
    public AudioClip[] countDownClips; 
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip jumpClip;
    public AudioClip scoreComputeClip;
    public AudioClip fireworkClip;
    public AudioClip clickButtonClip;
}
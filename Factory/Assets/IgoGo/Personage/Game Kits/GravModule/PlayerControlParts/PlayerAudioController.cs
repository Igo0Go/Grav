using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : PlayerControllerBlueprint
{
    [Tooltip("Источник всех звуков игрока")]
    public AudioSource source;
    [Tooltip("0 - земля, 1 - металл, 2 - трава")] public List<AudioClip> stepPack;
    [Tooltip("0 - земля, 1 - металл, 2 - трава")] public List<AudioClip> fallPack;
    [Tooltip("Звук падения в воду")] public AudioClip waterClip;

    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerGravMoveController.JumpSoundEvent += PlayJumpSoundByMaterial;
        playerState.playerGravMoveController.FloorSoundEvent += PlayStepSoundByMaterial;
        playerState.playerReactionsController.FallInWaterEvent += OnFallInWater;
    }

    public void PlayStepSoundByMaterial(PhysicMaterial floorMaterial)
    {
        switch (floorMaterial.name)
        {
            case "Ground (Instance)":
                PlaySound(stepPack[0]);
                break;
            case "Metal (Instance)":
                PlaySound(stepPack[1]);
                break;
            case "Grass (Instance)":
                PlaySound(stepPack[2]);
                break;
            default:
                PlaySound(stepPack[0]);
                break;
        }
    }
    public void PlayJumpSoundByMaterial(PhysicMaterial floorMaterial)
    {
        switch (floorMaterial.name)
        {
            case "Ground (Instance)":
                PlaySound(fallPack[0]);
                break;
            case "Metal (Instance)":
                PlaySound(fallPack[1]);
                break;
            case "Grass (Instance)":
                PlaySound(fallPack[2]);
                break;
            default:
                PlaySound(fallPack[0]);
                break;
        }
    }
    private void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound, UnityEngine.Random.Range(0.4f, 1));
    }
    private void OnFallInWater()
    {
        source.PlayOneShot(waterClip);
    }
}

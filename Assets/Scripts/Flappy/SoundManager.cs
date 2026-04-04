using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip jumping;
    public AudioClip starSound1;
    public AudioClip starSound2;
    public AudioClip fishpickup;
    public AudioClip firstsplash;
    public AudioClip secondsplash;

    public AudioClip gameover;
    public AudioClip explosion;
    public AudioSource sfx;
    public AudioSource music;

    

    public void StarPickup1() {
        sfx.PlayOneShot(starSound1);
    }

    public void StarPickup2() {
        sfx.PlayOneShot(starSound2);
    }

    public void StopMusic() {
        music.Pause();
    }

    public void PlayMusic() {
        music.Stop();
        music.Play();
    }
    public void ResumeMusic() {
        music.UnPause();
    }

    public void Jump() {
        sfx.PlayOneShot(jumping);
    }

    public void GameOver() {
        sfx.PlayOneShot(gameover);
    }

    public void PlayExplosion() {
        sfx.PlayOneShot(explosion);
    }
    public void FishPickUp() {
        sfx.PlayOneShot(fishpickup);
    }
    public void FirstSplash() {
        sfx.PlayOneShot(firstsplash);
    }
    public void SecondSplash() {
        sfx.PlayOneShot(secondsplash);
    }
}

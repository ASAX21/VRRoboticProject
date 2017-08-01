using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip beep;

    public int position = 0;
    public int samplerate = 44100;
    public float frequency = 440;

    // Use this for initialization
    void Start () {
        beep = AudioClip.Create("MySinusoid", samplerate/2, 1, samplerate, false, OnAudioRead);
		audioSource = gameObject.AddComponent<AudioSource> ();
        audioSource.volume = 0.01f;
    }
	
    public void PlayBeep()
    { 
        audioSource.PlayOneShot(beep);
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate));
            position++;
            count++;
        }
    }

    public void PlayClip(AudioClip clip)
    {
        return;
    }
}

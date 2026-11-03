using UnityEngine;

public class ReproducirSonido : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField, Range(0f, 1f)] float volumen = 0.8f;
    [SerializeField] float espera = 0f;
    [SerializeField] bool soloUnaVez = false;

    bool yaSono;

    public void Reproducir()
    {
        if (soloUnaVez && yaSono) return;
        yaSono = true;
        Invoke(nameof(Sonar), espera);
    }

    void Sonar()
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volumen);
    }
}

using UnityEngine;

public class Sonido : MonoBehaviour
{
    static Sonido instancia;

    [SerializeField] AudioSource musica;
    [SerializeField] AudioSource efectos;
    [SerializeField] AudioClip salto, destello, revivir, muerte, boton, texto;

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
        DontDestroyOnLoad(gameObject);
        if (!musica.isPlaying) musica.Play();
    }

    static void Tocar(AudioClip clip)
    {
        if (instancia != null && clip != null) instancia.efectos.PlayOneShot(clip);
    }

    public static void Salto() { if (instancia) Tocar(instancia.salto); }
    public static void Destello() { if (instancia) Tocar(instancia.destello); }
    public static void Revivir() { if (instancia) Tocar(instancia.revivir); }
    public static void Muerte() { if (instancia) Tocar(instancia.muerte); }
    public static void Boton() { if (instancia) Tocar(instancia.boton); }
    public static void Texto() { if (instancia) Tocar(instancia.texto); }
}

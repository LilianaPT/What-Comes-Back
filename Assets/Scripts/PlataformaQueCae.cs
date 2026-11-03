using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaQueCae : MonoBehaviour
{
    [Tooltip("Segundos que tiembla antes de caer")]
    [SerializeField] float tiempoAviso = 0.5f;
    [SerializeField] float gravedad = 4f;
    [Tooltip("Si esta marcado, reinicia el nivel si cae sobre el protagonista")]
    [SerializeField] bool aplastaProtagonista = true;
    [SerializeField] UnityEvent alAplastarHermana;

    Rigidbody2D rb;
    Vector3 posicionOriginal;
    float contadorAviso = -1f;
    bool cayendo;
    bool aterrizo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        //Solo cae recto, sin girar ni moverse de lado
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        posicionOriginal = transform.position;
    }

    //Lo llama el boton
    public void Soltar()
    {
        if (cayendo || contadorAviso >= 0f) return;
        contadorAviso = tiempoAviso;
    }

    void Update()
    {
        if (cayendo || contadorAviso < 0f) return;

        //Tiembla mientras cuenta
        contadorAviso -= Time.deltaTime;
        transform.position = posicionOriginal + (Vector3)(Random.insideUnitCircle * 0.05f);

        if(contadorAviso < 0f)
        {
            transform.position = posicionOriginal;
            cayendo = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravedad;
        }
    }

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (!cayendo || aterrizo) return;

        Hermana hermana = colision.collider.GetComponent<Hermana>();
        if(hermana != null)
        {
            hermana.Morir();
            alAplastarHermana.Invoke();
            return;             //sigue cayendo hasta el suelo
        }

        if(aplastaProtagonista && colision.collider.GetComponent<Protagonista>() != null)
        {
            EstadoJuego.ReiniciarNivel();
            return;
        }

        //Solo se queda fijo si toco algo por debajo (no el techo del que cuelga)
        if (colision.GetContact(0).normal.y > 0.5f)
        {
            aterrizo = true;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}

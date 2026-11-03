using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlataformaMovil : MonoBehaviour
{
    [Tooltip("Hacia donde se mueve desde su posicion inicial")]
    [SerializeField] Vector2 desplazamiento = new Vector2(4f, 0f);
    [SerializeField] float velocidad = 2f;
    [Tooltip("Segundos que espera en cada extremo")]
    [SerializeField] float pausaEnExtremos = 0.5f;
    [Tooltip("Si esta marcado, se mueve desde el inicio sin necesitar in boton")]
    [SerializeField] bool activaAlInicio = false;
    [Tooltip("Lo que pasa la primera vez que el protagonista se sube")]
    [SerializeField] UnityEvent alSubirProtagonista;
    bool protagonistaSubio;

    Rigidbody2D rb;
    Vector2 puntoA;
    Vector2 puntoB;
    Vector2 destino;
    bool activa;
    float contadorPausa;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        puntoA = rb.position;
        puntoB = puntoA + desplazamiento;
        destino = puntoB;
        activa = activaAlInicio;
    }

    //Lo llama el boton
    public void Activar()
    {
        activa = true;
    }

    void FixedUpdate()
    {
        if(!activa || contadorPausa > 0f)
        {
            contadorPausa -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 hacia = destino - rb.position;
        float paso = velocidad * Time.fixedDeltaTime;

        if(hacia.magnitude <= paso)
        {
            //Llego a un extremo: se acomoda, espera y cambia de direccion
            rb.MovePosition(destino);
            rb.linearVelocity = Vector2.zero;
            destino = destino == puntoB ? puntoA : puntoB;
            contadorPausa = pausaEnExtremos;
        }
        else
        {
            rb.linearVelocity = hacia.normalized * velocidad;
        }
    }

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (protagonistaSubio) return;
        if (colision.collider.GetComponent<Protagonista>() != null)
        {
            protagonistaSubio = true;
            alSubirProtagonista.Invoke();
        }
    }

    //Dibuja el recorrido en la vista Scene
    void OnDrawGizmoSelected()
    {
        Vector3 inicio = Application.isPlaying ? (Vector3)puntoA : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(inicio, inicio + (Vector3)desplazamiento);
    }
}

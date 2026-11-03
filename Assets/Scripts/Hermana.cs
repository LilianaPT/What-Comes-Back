using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Hermana : MonoBehaviour
{
    [Header("Seguir")]
    [SerializeField] Transform protagonista;
    [SerializeField] float velocidad = 5f;
    [SerializeField] float distanciaSeguir = 1.2f;

    [Header("Salto (desde la resurreccion 1")]
    [SerializeField] float fuerzaSalto = 14f;
    [Tooltip("Que tan adelante revisa si se acaba el suelo")]
    [SerializeField] float distanciaBorde = 0.3f;
    [Tooltip("Si hay suelo a menos de esta distancia abajo, baja caminando en lugar de saltar")]
    [SerializeField] float profundidadMaxima = 1.5f;

    [Header("Deteccion de suelo")]
    [SerializeField] LayerMask capaSuelo;
    [SerializeField] float distanciaSuelo = 0.05f;

    [Header("Reaccion")]
    [Tooltip("Segundos que tarda reaccionar en cada resurreccion (0, 1, 2...)")]
    [SerializeField] float[] retrasoPorResurreccion = { 0f, 0.4f, 0.7f, 1f, 1.3f, 1.6f };

    [Header("Opciones de este nivel")]
    [Tooltip("Marcalo en niveles donde ella no debe saltar")]
    [SerializeField] bool saltoDesactivado = false;

    [Tooltip("Si esta marcadom empieza quieta y no sigue al protagonista")]
    [SerializeField] bool quietaAlInicio = false;

    float contadorReaccion;
    bool esperandoSalto;
    float contadorSalto;
    float direccionSalto;
    float contadorEspera;
    bool esperandoIndefinido;

    Rigidbody2D rb;
    float direccion;
    Transform objetivo;

    public bool EstaMuerta { get; private set; }
    bool PuedeSaltar => EstadoJuego.resurrecciones >= 1 && !saltoDesactivado;
    public bool InmuneAPicos => EstadoJuego.resurrecciones >= 2;
    public bool AtraviesaRejas => EstadoJuego.resurrecciones >= 3;

    float Retraso
    {
        get
        {
            if (retrasoPorResurreccion.Length == 0) return 0f;
            int i = Mathf.Clamp(EstadoJuego.resurrecciones, 0, retrasoPorResurreccion.Length - 1);
            return retrasoPorResurreccion[i];
        }
    }
    SpriteRenderer sr;
    Camera camara;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        camara = Camera.main;
        objetivo = protagonista;
    }

    void Start()
    {
        if (quietaAlInicio) Esperar(-1f);
    }

    void Update()
    {
        if (EstaMuerta) return;

        //Salio por la parte de abajo de la camara?
        Vector3 posPantalla = camara.WorldToViewportPoint(transform.position);
        if(posPantalla.y < -0.1f)
        {
            Morir();
            return;
        }

        bool enSuelo = EnSuelo() && rb.linearVelocity.y <= 0.01f;

        if (enSuelo)
        {
            //Si le pidieron esperar, se queda quieta
            if (esperandoIndefinido || contadorEspera > 0f)
            {
                contadorEspera -= Time.deltaTime;
                direccion = 0f;
                return;
            }
            //Distancia horizontal entre ella y el protagonista
            float dx = objetivo.position.x - transform.position.x;

            //Si sigue al protagonista guarda distancia; si va a un destino, llega hasta el
            float distanciaParada = objetivo == protagonista ? distanciaSeguir : 0.1f;
            float deseada = Mathf.Abs(dx) > distanciaParada ? Mathf.Sign(dx) : 0f;

            //Esta dudando en la orilla antes de saltar
            if (esperandoSalto)
            {
                //Si el protagonista regreso hacia ella, ya no salta
                if(deseada != direccionSalto)
                {
                    esperandoSalto = false;
                }
                else
                {
                    direccion = 0f;
                    contadorSalto += Time.deltaTime;
                    if(contadorSalto >= Retraso)
                    {
                        esperandoSalto = false;
                        direccion = direccionSalto;
                        rb.linearVelocity = new Vector2(direccion * velocidad, fuerzaSalto);
                    }
                    return;
                }
            }

            //Si ya esta cerca, se detiene de inmediato
            if(deseada == 0f)
            {
                direccion = 0f;
                contadorReaccion = 0f;
            }
            //Para empezar a moverse o cambiar de direccion, tarda en reaccionar
            else if(deseada != direccion)
            {
                contadorReaccion += Time.deltaTime;
                if(contadorReaccion >= Retraso)
                {
                    direccion = deseada;
                    contadorReaccion = 0f;
                }
            }
            else
            {
                contadorReaccion = 0f;
            }

            //Llega a la orilla: se detiene y empieza a dudar
            if(PuedeSaltar && direccion != 0f && SeAcabaElSuelo())
            {
                esperandoSalto = true;
                contadorSalto = 0f;
                direccionSalto = direccion;
                direccion = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (EstaMuerta) return;
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
    }

    bool EnSuelo()
    {
        Bounds b = col.bounds;
        Vector2 centro = new Vector2(b.center.x, b.min.y);
        Vector2 tamano = new Vector2(b.size.x * 0.9f, distanciaSuelo * 2f);
        return Physics2D.OverlapBox(centro, tamano, 0f, capaSuelo) != null;
    }

    bool SeAcabaElSuelo()
    {
        Bounds b = col.bounds;
        Vector2 origen = new Vector2(b.center.x + direccion * (b.extents.x + distanciaBorde), b.min.y + 0.1f);
        RaycastHit2D golpe = Physics2D.Raycast(origen, Vector2.down, profundidadMaxima, capaSuelo);
        return golpe.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        float dir = direccion == 0f ? 1f : direccion;
        Vector3 origen = new Vector3(b.center.x + dir * (b.extents.x + distanciaBorde), b.min.y + 0.1f, 0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origen, origen + Vector3.down * profundidadMaxima);
    }

    public void Morir()
    {
        if (EstaMuerta) return;
        EstaMuerta = true;
        direccion = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;           //deja de caer y de chocar
        sr.enabled = false;             //ya no se ve
    }

    public void Revivir(Vector3 posicion)
    {
        rb.position = posicion;
        transform.position = posicion;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;
        sr.enabled = true;
        EstaMuerta = false;
        direccion = 0f;
        contadorReaccion = 0f;
        esperandoSalto = false;
        esperandoIndefinido = false;
        contadorEspera = 0f;
        objetivo = protagonista;
    }

    //segundos > 0: espera ese tiemp. segundos < 0: espera hasta que alguien la libere
    public void Esperar(float segundos)
    {
        if (segundos < 0f) esperandoIndefinido = true;
        else contadorEspera = segundos;

        direccion = 0f;
        esperandoSalto = false;
    }

    //Hace que camine hacia otro punto en lugar de seguir al protagonista
    public void SeguirA(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
    }

    public void DejarDeEsperar()
    {
        esperandoIndefinido = false;
        contadorEspera = 0f;
    }

    //Cambia una espera indefinida por una con tiempo: se libera despues de X segundos
    public void LiberarDespuesDe(float segundos)
    {
        esperandoIndefinido = false;
        contadorEspera = segundos;
    }

    public void Teletransportar(Transform destino)
    {
        rb.position = destino.position;
        transform.position = destino.position;
        rb.linearVelocity = Vector2.zero;
    }
}

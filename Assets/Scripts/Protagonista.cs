using UnityEngine;
using UnityEngine.InputSystem;

//Movimiento del protagonista: caminar y saltar
//Va en el GameObject "Protagonista" (necesita Rigidbody 2D y Box Collider 2D).
[RequireComponent(typeof(Rigidbody2D))]
public class Protagonista : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float velocidad = 6f;

    [Header("Salto")]
    [SerializeField] float fuerzaSalto = 12f;
    [Tooltip("Tiempo extra para saltar justo despues de salir de una orilla")]
    [SerializeField] float tiempoCoyote = 0.1f;
    [Tooltip("Si presionas saltar un poco antes de tocar el suelo, igual salta")]
    [SerializeField] float bufferSalto = 0.1f;

    [Header("Deteccion de suelo")]
    [SerializeField] LayerMask capaSuelo;
    [SerializeField] float distanciaSuelo = 0.05f;

    //Otros scripts (como el de la hermana) podran leer estos valores
    public bool EnSuelo { get; private set; }
    public float Direccion { get; private set; } = 1f;

    Rigidbody2D rb;
    Collider2D col;
    float entradaX;
    float contadorCoyote;
    float contadorBuffer;
    Camera camara;
    Collider2D sueloActual;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        camara = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // Cayo por debajo de la camara?
        Vector3 posPantalla = camara.WorldToViewportPoint(transform.position);
        if(posPantalla.y < -0.1f)
        {
            EstadoJuego.ReiniciarNivel();
            return;
        }
        Keyboard kbR = Keyboard.current;
        Gamepad gpR = Gamepad.current;
        if ((kbR != null && kbR.rKey.wasPressedThisFrame) ||
            (gpR != null && gpR.selectButton.wasPressedThisFrame))
        {
            EstadoJuego.ReiniciarNivel();
            return;
        }
        LeerEntrada(out bool presionarSalto, out bool soltarSalto);
        if (entradaX != 0) Direccion = Mathf.Sign(entradaX);

        EnSuelo = RevisarSuelo();

        //Coyote time: cuenta hacia atras desde que dejas el suelo
        contadorCoyote = EnSuelo ? tiempoCoyote : contadorCoyote - Time.deltaTime;
        //Buffer: recuerda que presionaste saltar por un momento
        contadorBuffer = presionarSalto ? bufferSalto : contadorBuffer - Time.deltaTime;

        if(contadorBuffer > 0f && contadorCoyote > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            contadorBuffer = 0f;
            contadorCoyote = 0f;
        }

        //Si sueltas el boton mientras subes, el salto es mas bajo
        if(soltarSalto && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    void FixedUpdate()
    {
        //Si esta parado sobre algo que se mueve, se mueve con eso
        Vector2 velocidadSuelo = Vector2.zero;
        if (sueloActual != null && sueloActual.attachedRigidbody != null)
        {
            velocidadSuelo = sueloActual.attachedRigidbody.linearVelocity;
        }

        rb.linearVelocity = new Vector2(entradaX * velocidad + velocidadSuelo.x, rb.linearVelocity.y);
    }

    void LeerEntrada(out bool presionarSalto, out bool soltarSalto)
    {
        entradaX = 0f;
        presionarSalto = false;
        soltarSalto = false;

        Keyboard kb = Keyboard.current;
        if(kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) entradaX -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) entradaX += 1f;

            presionarSalto = kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame;
            soltarSalto = kb.spaceKey.wasReleasedThisFrame || kb.wKey.wasReleasedThisFrame || kb.upArrowKey.wasReleasedThisFrame;
        }

        //Control (opcional)
        Gamepad gp = Gamepad.current;
        if (gp != null)
        {
            float x = gp.leftStick.x.ReadValue();
            if (Mathf.Abs(x) > 0.2f) entradaX = Mathf.Sign(x);
            presionarSalto |= gp.buttonSouth.wasPressedThisFrame;
            soltarSalto |= gp.buttonSouth.wasReleasedThisFrame;
        }
    }

    bool RevisarSuelo()
    {
        Bounds b = col.bounds;
        Vector2 centro = new Vector2(b.center.x, b.min.y);
        Vector2 tamano = new Vector2(b.size.x * 0.9f, distanciaSuelo * 2f);
        sueloActual = Physics2D.OverlapBox(centro, tamano, 0f, capaSuelo);
        return sueloActual != null;
    }

    //Dibuja la zona de deteccion de suelo en la vista Scene
    void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = EnSuelo ? Color.green : Color.red;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y, 0f), new Vector3(b.size.x * 0.9f, distanciaSuelo * 2f, 0f));
    }
}

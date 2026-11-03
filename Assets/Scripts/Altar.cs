using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Altar : MonoBehaviour
{
    [Header("Aparicion")]
    [Tooltip("Si esta marcado, el altar esta oculto hasta que ella muere")]
    [SerializeField] bool aparecerAlMorir = false;
    [SerializeField] Hermana hermana;
    [SerializeField] Protagonista protagonista;
    [SerializeField] GameObject panelResurreccion;
    [Tooltip("Que tan cerca debe estar el protagonista para usar el altar")]
    [SerializeField] float distanciaUso = 3f;
    [Tooltip("A que distancia detras del protagonista aparece ella")]
    [SerializeField] float separacion = 1f;
    [SerializeField] GameObject avisoTecla;
    [Tooltip("Donde aparece ella al revivir. Si esta vacio, aparece detras del protagonista")]
    [SerializeField] Transform puntoAparacion;
    [SerializeField] UnityEvent alRevivir;

    SpriteRenderer sr;
    bool aparecido;

    void Awake()
    {
        panelResurreccion.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        if (aparecerAlMorir) sr.enabled = false;
    }

    void Update()
    {
        //Si este altar aparece al morir ella, espera oculto hasta entonces
        if(aparecerAlMorir && !aparecido)
        {
            if (!hermana.EstaMuerta)
            {
                avisoTecla.SetActive(false);
                return;
            }
            aparecido = true;
            sr.enabled = true;
        }
        //Si el panel esta abierto, solo esperamos a que acepte
        if (panelResurreccion.activeSelf)
        {
            avisoTecla.SetActive(false);
            if (PresionoAceptar()) Aceptar();
            return;
        }

        float distancia = Vector2.Distance(protagonista.transform.position, transform.position);
        bool puedeUsar = hermana.EstaMuerta && distancia <= distanciaUso;

        //El aviso solo se ve cuando se puede usar el altar
        avisoTecla.SetActive(puedeUsar);

        if (puedeUsar && PresionoInteractuar())
        {
            panelResurreccion.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    bool PresionoInteractuar()
    {
        Keyboard kb = Keyboard.current;
        Gamepad gp = Gamepad.current;
        return (kb != null && kb.eKey.wasPressedThisFrame) || (gp != null && gp.buttonNorth.wasPressedThisFrame);
    }

    bool PresionoAceptar()
    {
        Keyboard kb = Keyboard.current;
        Gamepad gp = Gamepad.current;
        return (kb != null && (kb.enterKey.wasPressedThisFrame)) || (gp != null && gp.buttonWest.wasPressedThisFrame);
    }

    public void Aceptar()
    {
        if (!panelResurreccion.activeSelf) return;                      //evita que cuente doble
        EstadoJuego.resurrecciones++;
        Vector3 posicion;
        if(puntoAparacion != null)
        {
            posicion = puntoAparacion.position;
        }
        else
        {
            posicion = protagonista.transform.position - new Vector3(protagonista.Direccion * separacion, 0f, 0f);
        }
        hermana.Revivir(posicion);

        CerrarPanel();
        alRevivir.Invoke();
    }

    public void Cancelar()
    {
        CerrarPanel();
    }

    void CerrarPanel()
    {
        panelResurreccion.SetActive(false);
        Time.timeScale = 1f;
    }
}

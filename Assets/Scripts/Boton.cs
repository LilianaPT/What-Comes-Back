using UnityEngine;
using UnityEngine.Events;

public class Boton : MonoBehaviour
{
    [Tooltip("Si esta marcado, solo la hermana puede presionarlo")]
    [SerializeField] bool soloHermana = true;
    [SerializeField] Color colorPresionado = Color.green;
    [Tooltip("Que tan cerca del centro debe estar ella para activarlo")]
    [SerializeField] float tolerancia = 0.1f;

    [Tooltip("Lo que pasa al presionarlo")]
    [SerializeField] UnityEvent alPresionar;

    [Tooltip("0 = no la detiene. Mayor que 0 = la detiene esos segundos/ -1 = hasta que alguien la libere")]
    [SerializeField] float detenerHermana = 0f;
    [SerializeField] Sprite spritePresionado;
    [SerializeField] AudioClip sonidoPresionar;

    bool presionado;
    SpriteRenderer sr;
    Hermana hermanaDentro;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (presionado || hermanaDentro == null) return;

        //Espera a que el centro de ella llegue al centro del boton
        float distancia = Mathf.Abs(hermanaDentro.transform.position.x - transform.position.x);
        if(distancia <= tolerancia)
        {
            Presionar(hermanaDentro);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (presionado) return;

        Hermana hermana = otro.GetComponent<Hermana>();
        if(hermana != null)
        {
            hermanaDentro = hermana;                //todavia no se activa, solo la recuerda
            return;
        }

        if(!soloHermana && otro.GetComponent<Protagonista>() != null)
        {
            Presionar(null);
        }
    }

    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.GetComponent<Hermana>() != null) hermanaDentro = null;
    }

    void Presionar(Hermana hermana)
    {
        presionado = true;
        if (sr != null) sr.color = colorPresionado;
        if (spritePresionado != null) GetComponent<SpriteRenderer>().sprite = spritePresionado;
        if (sonidoPresionar != null)
            AudioSource.PlayClipAtPoint(sonidoPresionar, Camera.main.transform.position, 0.7f);
        alPresionar.Invoke();

        if(hermana != null && detenerHermana != 0f)
        {
            hermana.Esperar(detenerHermana);
        }
    }
}

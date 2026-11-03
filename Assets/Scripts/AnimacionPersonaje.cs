using UnityEngine;

public class AnimacionPersonaje : MonoBehaviour
{
    [System.Serializable]
    public class Etapa
    {
        public Sprite quieto;
        public Sprite[] caminar;
        public Sprite saltar;
    }

    [SerializeField] Etapa[] etapas;
    [SerializeField] bool usarResurrecciones = false;
    [SerializeField] float cuadrosPorSegundo = 8f;
    [SerializeField] float velocidadMinima = 0.1f;
    [SerializeField] bool voltearSprite = true;

    SpriteRenderer sr;
    Rigidbody2D rb;
    float tiempo;
    bool mirandoDerecha = true;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    int EtapaActual()
    {
        if (!usarResurrecciones) return 0;
        return Mathf.Clamp(EstadoJuego.resurrecciones, 0, etapas.Length - 1);
    }

    void Update()
    {
        if (etapas.Length == 0) return;
        Etapa e = etapas[EtapaActual()];
        Vector2 v = rb.linearVelocity;

        if (Mathf.Abs(v.y) > 0.5f && e.saltar != null)
        {
            sr.sprite = e.saltar;
        }
        else if (Mathf.Abs(v.x) > velocidadMinima && e.caminar.Length > 0)
        {
            tiempo += Time.deltaTime * cuadrosPorSegundo;
            sr.sprite = e.caminar[(int)tiempo % e.caminar.Length];
        }
        else
        {
            tiempo = 0f;
            sr.sprite = e.quieto;
        }

        if (v.x > velocidadMinima) mirandoDerecha = true;
        else if (v.x < -velocidadMinima) mirandoDerecha = false;
        if (voltearSprite) sr.flipX = !mirandoDerecha;
    }
}

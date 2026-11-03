using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LogoAnimado : MonoBehaviour
{
    [SerializeField] float duracionAparicion = 2f;
    [SerializeField] float alturaFlotar = 8f;
    [SerializeField] float velocidadFlotar = 0.5f;
    [SerializeField] float intensidadTemblor = 2f;
    [SerializeField] Vector2 tiempoEntreParpadeos = new Vector2(3f, 7f);
    [Tooltip("Imagen alterna que aparece por un instante al parpadear (opcional)")]
    [SerializeField] GameObject logoAlterno;
    [Tooltip("Segundos que se queda la version alterna en pantalla")]
    [SerializeField] float duracionAlterno = 1.2f;

    Image imagenPrincipal;
    CanvasGroup grupo;
    RectTransform rt;
    Vector2 posicionInicial;
    float alfaAparicion;
    float alfaParpadeo = 1f;

    void Awake()
    {
        grupo = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
        posicionInicial = rt.anchoredPosition;
        if (logoAlterno != null) logoAlterno.SetActive(false);
        imagenPrincipal = GetComponent<Image>();
    }

    void Start()
    {
        StartCoroutine(Aparecer());
        StartCoroutine(Parpadeos());
    }

    IEnumerator Aparecer()
    {
        float t = 0f;
        while (t < duracionAparicion)
        {
            t += Time.deltaTime;
            alfaAparicion = t / duracionAparicion;
            yield return null;
        }
        alfaAparicion = 1f;
    }

    IEnumerator Parpadeos()
    {
        yield return new WaitForSeconds(duracionAparicion);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(tiempoEntreParpadeos.x, tiempoEntreParpadeos.y));

            // Parpadeo rapido, como una luz que falla
            yield return Parpadear();

            // Cambia al titulo alterno: oculta el normal para que no se encimen
            imagenPrincipal.enabled = false;
            if (logoAlterno != null) logoAlterno.SetActive(true);

            yield return new WaitForSeconds(duracionAlterno);

            // Otro parpadeo y regresa al titulo normal
            yield return Parpadear();
            if (logoAlterno != null) logoAlterno.SetActive(false);
            imagenPrincipal.enabled = true;
        }
    }

    IEnumerator Parpadear()
    {
        alfaParpadeo = 0.2f;
        yield return new WaitForSeconds(0.05f);
        alfaParpadeo = 1f;
        yield return new WaitForSeconds(0.05f);
        alfaParpadeo = 0f;
        yield return new WaitForSeconds(0.08f);
        alfaParpadeo = 1f;
    }

    void Update()
    {
        // Flota suavemente
        float y = Mathf.Sin(Time.time * velocidadFlotar * Mathf.PI * 2f) * alturaFlotar;

        // Temblor suave de todo el logo
        float tiempo = Time.time * 4f;
        Vector2 temblor = new Vector2(
            Mathf.PerlinNoise(tiempo, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, tiempo) - 0.5f) * 2f * intensidadTemblor;

        rt.anchoredPosition = posicionInicial + new Vector2(0f, y) + temblor;
        grupo.alpha = alfaAparicion * alfaParpadeo;
    }
}

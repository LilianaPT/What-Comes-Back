using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Destello : MonoBehaviour
{
    [SerializeField] CanvasGroup pantallaNegra;
    [Tooltip("Segundos que la pantalla se queda en negro")]
    [SerializeField] float duracionNegro = 0.25f;
    [Tooltip("Segundos que tarda en volver a verse")]
    [SerializeField] float duracionDesvanecer = 0.15f;

    [Tooltip("Lo que pasa mientras la pantalla esta negra")]
    [SerializeField] UnityEvent duranteLaOscuridad;

    public void Ejecutar()
    {
        StartCoroutine(Secuencia());
    }

    IEnumerator Secuencia()
    {
        pantallaNegra.alpha = 1f;
        duranteLaOscuridad.Invoke();

        yield return new WaitForSecondsRealtime(duracionNegro);

        float t = 0f;
        while(t < duracionDesvanecer)
        {
            t += Time.unscaledDeltaTime;
            pantallaNegra.alpha = 1f - t / duracionDesvanecer;
            yield return null;
        }
        pantallaNegra.alpha = 0f;
    }
}

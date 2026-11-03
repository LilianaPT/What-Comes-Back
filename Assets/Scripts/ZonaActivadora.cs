using UnityEngine;
using UnityEngine.Events;

public class ZonaActivadora : MonoBehaviour
{
    public enum QuienActiva {  Hermana, Protagonista, Cualquiera }

    [SerializeField] QuienActiva quienActiva = QuienActiva.Hermana;
    [SerializeField] UnityEvent alEntrar;

    bool usada;

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (usada) return;

        bool esHermana = otro.GetComponent<Hermana>() != null;
        bool esProtagonista = otro.GetComponent<Protagonista>() != null;

        bool valido =
            (quienActiva == QuienActiva.Hermana && esHermana) ||
            (quienActiva == QuienActiva.Protagonista && esProtagonista) ||
            (quienActiva == QuienActiva.Cualquiera && (esHermana || esProtagonista));

        if (valido)
        {
            usada = true;
            alEntrar.Invoke();
        }
    }
}

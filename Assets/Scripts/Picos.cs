using UnityEngine;
using UnityEngine.Events;

public class Picos : MonoBehaviour
{
    [SerializeField] UnityEvent alMatarHermana;
    void OnTriggerEnter2D(Collider2D otro)
    {
        Hermana hermana = otro.GetComponent<Hermana>();
        if (hermana != null)
        {
            if (!hermana.InmuneAPicos)
            {
                hermana.Morir();
                alMatarHermana.Invoke();
            }
            return;
        }

        if(otro.GetComponent<Protagonista>() != null)
        {
            EstadoJuego.ReiniciarNivel();
        }
    }
}

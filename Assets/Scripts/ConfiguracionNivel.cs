using UnityEngine;

public class ConfiguracionNivel : MonoBehaviour
{
    [Tooltip("Resurreciones con las que empieza este nivel como minimo")]
    [SerializeField] int resurreccionesMinimas = 0;

    void Awake()
    {
        if(EstadoJuego.resurrecciones < resurreccionesMinimas)
        {
            EstadoJuego.resurrecciones = resurreccionesMinimas;
        }
    }
}

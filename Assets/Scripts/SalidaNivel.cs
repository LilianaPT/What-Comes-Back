using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaNivel : MonoBehaviour
{
    [SerializeField] Hermana hermana;
    [Tooltip("Ella tiene que estar viva y cerca para poder salir? ")]
    [SerializeField] bool requiereHermana = true;
    [SerializeField] float distanciaHermana = 3f;

    bool protagonistaDentro;
    bool usada;

    void Update()
    {
        if (usada || !protagonistaDentro) return;

        if (requiereHermana)
        {
            if (!hermana.gameObject.activeInHierarchy) return;
            if (hermana.EstaMuerta) return;
            float distancia = Vector2.Distance(hermana.transform.position, transform.position);
            if (distancia > distanciaHermana) return;
        }

        usada = true;           //evita que se cargue dos veces
        CargarSiguienteNivel();
    }

    void CargarSiguienteNivel()
    {
        int siguiente = SceneManager.GetActiveScene().buildIndex + 1;

        if(siguiente < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguiente);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.GetComponent<Protagonista>() != null) protagonistaDentro = true;
    }
    void OnTriggerExit2D(Collider2D otro)
    {
        if (otro.GetComponent<Protagonista>() != null) protagonistaDentro = false;
    }
}

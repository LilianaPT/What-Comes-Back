using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LimitesPantalla : MonoBehaviour
{
    [SerializeField] float grosor = 1f;
    [Tooltip("Que tan altas son las paredes, para que no se puedan saltar")]
    [SerializeField] float altoExtra = 20f;

    Camera cam;
    BoxCollider2D paredIzquierda;
    BoxCollider2D paredDerecha;

    void Awake()
    {
        cam = GetComponent<Camera>();
        paredIzquierda = CrearPared("ParedIzquierda");
        paredDerecha = CrearPared("ParedDerecha");
    }

    BoxCollider2D CrearPared(string nombre)
    {
        GameObject pared = new GameObject(nombre);
        pared.transform.SetParent(transform);
        return pared.AddComponent<BoxCollider2D>();
    }

    void LateUpdate()
    {
        //Esquinas de la pantalla en coordenadas del mundo
        Vector3 abajoIzq = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 arribaDer = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        float alto = arribaDer.y - abajoIzq.y + altoExtra;
        float centroY = (abajoIzq.y + arribaDer.y) / 2f;

        paredIzquierda.transform.position = new Vector3(abajoIzq.x - grosor / 2f, centroY, 0f);
        paredIzquierda.size = new Vector2(grosor, alto);

        paredDerecha.transform.position = new Vector3(arribaDer.x + grosor / 2f, centroY, 0f);
        paredDerecha.size = new Vector2(grosor, alto);
    }
}

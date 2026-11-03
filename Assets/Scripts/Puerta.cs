using UnityEngine;

public class Puerta : MonoBehaviour
{
    [Tooltip("Hacia donde se mueve al abrirse")]
    [SerializeField] Vector3 desplazamiento = new Vector3(0f, 3f, 0f);
    [SerializeField] float velocidad = 3f;

    Vector3 posicionAbierta;
    bool abriendo;

    void Awake()
    {
        posicionAbierta = transform.position + desplazamiento;
    }

    //Lo llama el boton
    public void Abrir()
    {
        abriendo = true;
    }

    void Update()
    {
        if (!abriendo) return;
        transform.position = Vector3.MoveTowards(transform.position, posicionAbierta, velocidad * Time.deltaTime);
    }
}

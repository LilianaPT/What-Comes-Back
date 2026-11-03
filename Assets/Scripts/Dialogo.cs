using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class Dialogo : MonoBehaviour
{
    [System.Serializable]
    public class Linea
    {
        public Transform quienHabla;
        [TextArea] public string texto;
    }

    [SerializeField] Linea[] lineas;
    [SerializeField] GameObject burbuja;
    [SerializeField] SpriteRenderer fondo;
    [SerializeField] TMP_Text texto;
    [SerializeField] Vector2 desplazamiento = new Vector2(0f, 1.4f);
    [SerializeField] Vector2 relleno = new Vector2(0.25f, 0.15f);
    [SerializeField] bool iniciarAlEmpezar = false;
    [SerializeField] float esperaInicial = 0.5f;
    [SerializeField] bool pausarJuego = true;
    [SerializeField] UnityEvent alTerminar;
    [SerializeField] bool soloUnaVez = true;
    [SerializeField] float avanceAutomatico = 0f;
    bool usado = false;

    int indice = -1;
    Transform actual;
    float tiempoMostrado;

    void Start()
    {
        burbuja.SetActive(false);
        if (iniciarAlEmpezar) StartCoroutine(EmpezarConEspera());
    }

    public void IniciarConEspera()
    {
        StartCoroutine(EmpezarConEspera());
    }

    IEnumerator EmpezarConEspera()
    {
        yield return new WaitForSecondsRealtime(esperaInicial);
        Iniciar();
    }

    public void Iniciar()
    {
        if (soloUnaVez && usado) return;
        usado = true;
        if (lineas.Length == 0) return;
        if (pausarJuego) Time.timeScale = 0f;
        indice = -1;
        Siguiente();
    }

    void Siguiente()
    {
        indice++;
        while (indice < lineas.Length && !lineas[indice].quienHabla.gameObject.activeInHierarchy)
        {
            indice++;
        }
        if (indice >= lineas.Length)
        {
            Terminar();
            return;
        }

        actual = lineas[indice].quienHabla;
        texto.text = lineas[indice].texto;

        Vector2 tam = texto.GetPreferredValues(lineas[indice].texto);
        texto.rectTransform.sizeDelta = tam;

        Vector2 tamFondo = tam + relleno * 2f;
        tamFondo.x = Mathf.Ceil(tamFondo.x * 32f) / 32f;
        tamFondo.y = Mathf.Ceil(tamFondo.y * 32f) / 32f;
        fondo.size = tamFondo;

        texto.rectTransform.localPosition = new Vector3(0f, tamFondo.y / 2f, 0f);

        burbuja.SetActive(true);
        tiempoMostrado = Time.unscaledTime;
        Colocar();
    }

    void Terminar()
    {
        burbuja.SetActive(false);
        indice = -1;
        if (pausarJuego) Time.timeScale = 1f;
        alTerminar.Invoke();
    }

    void Update()
    {
        if (indice < 0) return;
        if (!actual.gameObject.activeInHierarchy)
        {
            Siguiente();
            return;
        }
        if (Time.unscaledTime - tiempoMostrado < 0.25f) return;
        if(avanceAutomatico > 0f && Time.unscaledTime - tiempoMostrado >= avanceAutomatico)
        {
            Siguiente();
            return;
        }

        var mouse = Mouse.current;
        var kb = Keyboard.current;
        var gp = Gamepad.current;

        bool avanzar = (mouse != null && mouse.leftButton.wasPressedThisFrame)
                    || (kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
                    || (gp != null && gp.buttonSouth.wasPressedThisFrame);

        if (avanzar) Siguiente();
    }

    void LateUpdate()
    {
        if (indice >= 0) Colocar();
    }

    void Colocar()
    {
        Vector2 pos = (Vector2)actual.position + desplazamiento;
        burbuja.transform.position = new Vector3(pos.x, pos.y, 0f);
    }

}

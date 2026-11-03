using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuInicio : MonoBehaviour
{
    [Header("Titulo")]
    [SerializeField] RectTransform contenedorTitulo;
    [Tooltip("Cuantos pixeles sube el titulo")]
    [SerializeField] float subirTitulo = 150f;
    [SerializeField] float duracionSubir = 0.8f;

    [Header("Aviso 'Press any key'")]
    [SerializeField] CanvasGroup avisoPresiona;
    [SerializeField] float velocidadParpadeo = 1f;

    [Header("Boton")]
    [SerializeField] CanvasGroup grupoBoton;
    [SerializeField] GameObject botonEmpezar;

    [Header("Inicio del juego")]
    [SerializeField] CanvasGroup pantallaNegra;
    [SerializeField] float duracionFundido = 1f;
    [SerializeField] string primeraEscena = "Nivel1";

    bool mostrado;
    bool botonListo;
    bool cargando;

    void Start()
    {
        Time.timeScale = 1f;

        // El boton empieza invisible y sin poder usarse
        grupoBoton.alpha = 0f;
        grupoBoton.interactable = false;
        grupoBoton.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update()
    {
        if (!mostrado)
        {
            // El aviso parpadea suavemente
            if (avisoPresiona != null)
            {
                avisoPresiona.alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * velocidadParpadeo * Mathf.PI * 2f);
            }

            if (PresionoCualquierCosa())
            {
                mostrado = true;
                StartCoroutine(MostrarBoton());
            }
            return;
        }

        if (!botonListo || cargando) return;

        EventSystem es = EventSystem.current;

        if (UsoNavegacion())
        {
            // Con control o flechas: selecciona el boton (se pone naranja)
            if (es.currentSelectedGameObject == null) es.SetSelectedGameObject(botonEmpezar);
        }
        else if (MovioMouse() && es.currentSelectedGameObject != null)
        {
            // Si vuelve a usar el mouse, quita la seleccion y manda el hover
            es.SetSelectedGameObject(null);
        }

        // Enter o el boton del control sin nada seleccionado: empieza directo
        if (es.currentSelectedGameObject == null && PresionoConfirmar())
        {
            Empezar();
        }
    }

    bool PresionoCualquierCosa()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        Gamepad gp = Gamepad.current;

        if (kb != null && kb.anyKey.wasPressedThisFrame) return true;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;
        if (gp != null && (gp.buttonSouth.wasPressedThisFrame || gp.buttonEast.wasPressedThisFrame ||
                           gp.buttonWest.wasPressedThisFrame || gp.buttonNorth.wasPressedThisFrame ||
                           gp.startButton.wasPressedThisFrame)) return true;
        return false;
    }

    IEnumerator MostrarBoton()
    {
        Vector2 inicio = contenedorTitulo.anchoredPosition;
        Vector2 fin = inicio + new Vector2(0f, subirTitulo);
        float alfaAviso = avisoPresiona != null ? avisoPresiona.alpha : 0f;

        float t = 0f;
        while (t < duracionSubir)
        {
            t += Time.deltaTime;
            // SmoothStep hace que empiece y termine suave, no de golpe
            float p = Mathf.SmoothStep(0f, 1f, t / duracionSubir);

            contenedorTitulo.anchoredPosition = Vector2.Lerp(inicio, fin, p);
            grupoBoton.alpha = p;
            if (avisoPresiona != null) avisoPresiona.alpha = alfaAviso * (1f - p);

            yield return null;
        }

        contenedorTitulo.anchoredPosition = fin;
        grupoBoton.alpha = 1f;
        if (avisoPresiona != null) avisoPresiona.gameObject.SetActive(false);

        // Pequena pausa para que la misma tecla no presione el boton de una vez
        yield return new WaitForSeconds(0.2f);

        grupoBoton.interactable = true;
        grupoBoton.blocksRaycasts = true;
        botonListo = true;
    }

    // Lo llama el boton
    public void Empezar()
    {
        if (cargando) return;
        cargando = true;
        StartCoroutine(FundirYCargar());
    }

    IEnumerator FundirYCargar()
    {
        pantallaNegra.blocksRaycasts = true;

        float t = 0f;
        while (t < duracionFundido)
        {
            t += Time.deltaTime;
            pantallaNegra.alpha = t / duracionFundido;
            yield return null;
        }

        EstadoJuego.NuevoJuego();
        SceneManager.LoadScene(primeraEscena);
    }

    bool UsoNavegacion()
    {
        Keyboard kb = Keyboard.current;
        Gamepad gp = Gamepad.current;

        if (gp != null)
        {
            if (gp.leftStick.ReadValue().magnitude > 0.5f) return true;
            if (gp.dpad.up.wasPressedThisFrame || gp.dpad.down.wasPressedThisFrame ||
                gp.dpad.left.wasPressedThisFrame || gp.dpad.right.wasPressedThisFrame) return true;
        }
        if (kb != null)
        {
            if (kb.upArrowKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame ||
                kb.leftArrowKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame ||
                kb.wKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame ||
                kb.aKey.wasPressedThisFrame || kb.dKey.wasPressedThisFrame) return true;
        }
        return false;
    }

    bool MovioMouse()
    {
        Mouse mouse = Mouse.current;
        return mouse != null && mouse.delta.ReadValue().sqrMagnitude > 1f;
    }

    bool PresionoConfirmar()
    {
        Keyboard kb = Keyboard.current;
        Gamepad gp = Gamepad.current;
        return (kb != null && (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)) ||
               (gp != null && gp.buttonSouth.wasPressedThisFrame);
    }
}

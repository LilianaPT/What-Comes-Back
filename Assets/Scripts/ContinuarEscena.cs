using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ContinuarEscena : MonoBehaviour
{
    [SerializeField] string siguienteEscena = "Panteon";
    [SerializeField] float esperaMinima = 1f;

    float inicio;

    void Start()
    {
        Time.timeScale = 1f;
        inicio = Time.unscaledTime;
    }

    void Update()
    {
        if (Time.unscaledTime - inicio < esperaMinima) return;

        var kb = Keyboard.current;
        var gp = Gamepad.current;

        bool continuar = (kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
                      || (gp != null && gp.buttonSouth.wasPressedThisFrame);

        if (continuar) Continuar();
    }

    public void Continuar()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene(siguienteEscena);
    }
}
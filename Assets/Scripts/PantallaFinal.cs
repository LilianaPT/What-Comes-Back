using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PantallaFinal : MonoBehaviour
{
    [SerializeField] string escenaMenu = "Menu";
    [SerializeField] GameObject botones;
    [SerializeField] GameObject panelGracias;
    [SerializeField] float esperaMinima = 2f;

    void Start()
    {
        Time.timeScale = 1f;
        if (botones != null) botones.SetActive(false);
        if (panelGracias != null) panelGracias.SetActive(false);
        Invoke(nameof(MostrarBotones), esperaMinima);
    }

    void MostrarBotones()
    {
        if (botones != null) botones.SetActive(true);
    }

    public void IrAlMenu()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        EstadoJuego.NuevoJuego();
        SceneManager.LoadScene(escenaMenu);
    }

    public void Salir()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (botones != null) botones.SetActive(false);
        if (panelGracias != null) panelGracias.SetActive(true);
#else
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
    }
}

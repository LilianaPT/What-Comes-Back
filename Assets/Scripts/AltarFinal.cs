using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AltarFinal : MonoBehaviour
{
    [SerializeField] Protagonista protagonista;
    [SerializeField] GameObject panelDecision;
    [SerializeField] GameObject avisoTecla;
    [SerializeField] float distanciaUso = 1.5f;

    [Header("Finales")]
    [SerializeField] string escenaTraerla = "FinalJuntos";
    [SerializeField] string escenaDejarla = "FinalSolo";

    SpriteRenderer sr;
    bool visible;
    bool decidido;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        panelDecision.SetActive(false);
        avisoTecla.SetActive(false);
    }

    public void Aparecer()
    {
        visible = true;
        sr.enabled = true;
    }

    void Update()
    {
        if (!visible || decidido) return;

        Keyboard kb = Keyboard.current;
        Gamepad gp = Gamepad.current;

        if (panelDecision.activeSelf)
        {
            if ((kb != null && kb.rKey.wasPressedThisFrame) || (gp != null && gp.buttonWest.wasPressedThisFrame))
            {
                TraerlaDeVuelta();
            } else if ((kb != null && kb.tKey.wasPressedThisFrame) || (gp != null && gp.buttonEast.wasPressedThisFrame))
            {
                DejarlaIr();
            }
            return;
        }

        float distancia = Vector2.Distance(protagonista.transform.position, transform.position);
        bool cerca = distancia <= distanciaUso;
        avisoTecla.SetActive(cerca);

        bool interactuar = (kb != null && kb.eKey.wasPressedThisFrame) || (gp != null && gp.buttonNorth.wasPressedThisFrame);

        if(cerca && interactuar)
        {
            panelDecision.SetActive(true);
            avisoTecla.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    public void TraerlaDeVuelta() { Elegir(escenaTraerla); }
    public void DejarlaIr() { Elegir(escenaDejarla); }

    void Elegir (string escena)
    {
        if (decidido) return;
        decidido = true;
        Time.timeScale = 1f;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene(escena);
    }
}

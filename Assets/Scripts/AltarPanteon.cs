using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AltarPanteon : MonoBehaviour
{
    [SerializeField] Transform protagonista;
    [SerializeField] GameObject panelInscripcion;
    [SerializeField] GameObject avisoTecla;
    [SerializeField] float distanciaUso = 1.5f;
    [SerializeField] UnityEvent alLeer;

    SpriteRenderer sprite;
    bool visible = false;
    bool leido = false;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        panelInscripcion.SetActive(false);
        if (avisoTecla != null) avisoTecla.SetActive(false);
    }

    public void Aparecer()
    {
        visible = true;
        sprite.enabled = true;
    }

    void Update()
    {
        if (!visible || leido) return;

        var kb = Keyboard.current;
        var gp = Gamepad.current;

        if (panelInscripcion.activeSelf)
        {
            bool leer = (kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
                     || (gp != null && gp.buttonWest.wasPressedThisFrame);
            if (leer) Leer();
            return;
        }

        bool cerca = Vector2.Distance(protagonista.position, transform.position) <= distanciaUso;
        if (avisoTecla != null) avisoTecla.SetActive(cerca);

        bool interactuar = (kb != null && kb.eKey.wasPressedThisFrame)
                        || (gp != null && gp.buttonNorth.wasPressedThisFrame);
        if (cerca && interactuar)
        {
            panelInscripcion.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Leer()
    {
        if (!panelInscripcion.activeSelf) return;

        leido = true;
        panelInscripcion.SetActive(false);
        if (avisoTecla != null) avisoTecla.SetActive(false);
        Time.timeScale = 1f;
        alLeer.Invoke();
    }
}

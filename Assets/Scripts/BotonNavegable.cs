using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class BotonNavegable : MonoBehaviour
{
    Selectable boton;

    void Awake()
    {
        boton = GetComponent<Selectable>();
    }

    void OnDisable()
    {
        // Al cerrarse el panel, que no se quede seleccionado
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void Update()
    {
        if (!boton.interactable) return;
        EventSystem es = EventSystem.current;
        if (es == null) return;

        if (UsoNavegacion())
        {
            if (es.currentSelectedGameObject == null) es.SetSelectedGameObject(gameObject);
        }
        else if (MovioMouse() && es.currentSelectedGameObject == gameObject)
        {
            es.SetSelectedGameObject(null);
        }
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
                kb.leftArrowKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) return true;
        }
        return false;
    }

    bool MovioMouse()
    {
        Mouse mouse = Mouse.current;
        return mouse != null && mouse.delta.ReadValue().sqrMagnitude > 1f;
    }
}

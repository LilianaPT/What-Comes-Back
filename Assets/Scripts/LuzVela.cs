using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuzVela : MonoBehaviour
{
    [SerializeField] float intensidadBase = 1.2f;
    [SerializeField] float parpadeo = 0.25f;
    [SerializeField] float velocidad = 3f;

    Light2D luz;
    SpriteRenderer altar;

    void Awake()
    {
        luz = GetComponent<Light2D>();
        altar = transform.parent != null ? transform.parent.GetComponent<SpriteRenderer>() : null;
    }

    void Update()
    {
        luz.enabled = altar == null || altar.enabled;
        float n = Mathf.PerlinNoise(Time.time * velocidad, 0f);
        luz.intensity = intensidadBase + (n - 0.5f) * 2f * parpadeo;
    }
}
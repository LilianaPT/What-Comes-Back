using UnityEngine;

public class Reja : MonoBehaviour
{
    [SerializeField] Hermana hermana;

    void Start()
    {
        //Si ella ya puede, la reja deja de chocar con ella (pero sigue bloqueando al protagonista)
        if (hermana.AtraviesaRejas)
        {
            Collider2D colReja = GetComponent<Collider2D>();
            Collider2D colHermana = hermana.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(colReja, colHermana, true);
        }
    }
}

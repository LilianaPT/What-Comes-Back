using UnityEngine;
using UnityEngine.SceneManagement;

public static class EstadoJuego
{
    public static int resurrecciones = 0;
    static int resurreccionesInicioNivel = 0;

    //Unity llama esto automaticamente cada vez que empieza el juego
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        resurrecciones = 0;
        resurreccionesInicioNivel = 0;

        //Cada vez que se carga una escena, guardamos el valor inicial
        SceneManager.sceneLoaded -= AlCargarEscena;                             //evita registrarlo dos veces
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        resurreccionesInicioNivel = resurrecciones;
    }

    //Lo que llama el protagonista al caer
    public static void ReiniciarNivel()
    {
        resurrecciones = resurreccionesInicioNivel;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void NuevoJuego()
    {
        resurrecciones = 0;
        resurreccionesInicioNivel = 0;
    }
}

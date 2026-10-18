using UnityEngine;

public class GestorPenalizacion : MonoBehaviour
{
    // VARIABLES
    // Usamos corchetes [] para indicar que esto es un "Array" (una lista de varios objetos, no solo uno).

    [Header("Vidas del Módulo de Servicio")]
    public GameObject[] estrellas; // Aquí arrastraremos las 3 estrellas
    private int erroresServicio = 0; // Esta variable interna cuenta cuántas veces se ha equivocado el jugador

    [Header("Vidas del Módulo de Phishing")]
    public GameObject[] candados; // Aquí arrastraremos los 3 candados
    private int erroresPhishing = 0;

    [Header("Pantallas de Fin de Juego")]
    public GameObject panelGameOver; // Aquí arrastraremos el panel de derrota
    //public GameObject panelNivelServicio;         ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
    //public GameObject panelNivelPhishing;

    void Start()
    {
        // Por seguridad, nos aseguramos de que la pantalla de Game Over esté apagada al iniciar
        panelGameOver.SetActive(false);
    }

    // ==========================================
    // MÉTODOS PARA RESTAR VIDAS
    // ==========================================

    // Esta función se conectará a los botones de "Respuesta Incorrecta" en Servicio al Cliente
    public void RestarEstrella()
    {
        // Verificamos si el jugador aún tiene estrellas (es decir, menos de 3 errores)
        if (erroresServicio < 3)
        {
            // Los Arrays en programación empiezan a contar desde el 0.
            // Si erroresServicio es 0, apagará la estrella en la posición 0.
            estrellas[erroresServicio].SetActive(false);

            // Sumamos 1 al contador de errores
            erroresServicio++;

            // Si después de sumar, llegamos a 3 errores, perdimos.
            if (erroresServicio >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // Esta función se conectará a los botones de "Respuesta Incorrecta" (Aprobar un correo falso)
    public void RestarCandado()
    {
        if (erroresPhishing < 3)
        {
            candados[erroresPhishing].SetActive(false);
            erroresPhishing++;

            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // Método interno para mostrar la derrota
    private void MostrarGameOver()
    {
        //panelNivelServicio.SetActive(false);           ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
        //panelNivelPhishing.SetActive(false);

        panelGameOver.SetActive(true);
    }
}
using UnityEngine;
using UnityEngine.UI; // Librería OBLIGATORIA para poder controlar componentes 'Image' de la interfaz (UI) de Unity.

// [System.Serializable] permite que esta "caja de datos" sea visible en el Inspector de Unity.
[System.Serializable]
public class CasoPhishing
{
    [Tooltip("Solo sirve para que te guíes en el Inspector (ej. 'Nivel 1: SMS Falso' o 'Nivel 2: Correo RRHH')")]
    public string nombreDelCaso; // Un texto opcional de guía para organizarte en el editor.

    [Tooltip("Arrastra aquí la imagen del correo, SMS o pantalla creada por el diseñador")]
    public Sprite imagenDelMensaje; // 'Sprite' es el archivo de imagen 2D que vamos a mostrar.

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing; // Guarda Verdadero (true) si es una trampa, o Falso (false) si es real.
}

public class GestorPhishing : MonoBehaviour
{
    // VARIABLES DE LA INTERFAZ (UI)
    [Header("Elementos en Pantalla")]
    // 'Image' es el componente en el Canvas (como un portarretratos vacío) donde pondremos nuestros 'Sprites' (las fotos).
    public Image contenedorImagenPantalla;

    [Header("Conexión con otros sistemas")]
    public GestorPenalizacion scriptPenalizacion; // Referencia a tu script que apaga los 3 candados.

    // DATOS DE LOS 5 NIVELES
    [Header("Lista de los 5 Casos (Correos, SMS, etc.)")]
    public CasoPhishing[] nivelesPhishing; // Array (lista) que guardará las 5 imágenes y sus respuestas.
    private int nivelActual = 0;           // Lleva la cuenta de en qué imagen vamos (empieza en 0).

    //AUDIO
    public GestorAudio scriptAudio;

    // MÉTODO START: Se ejecuta una sola vez al iniciar el juego.
    void Start()
    {
        // Cargamos la primera imagen de nuestra lista (posición 0).
        CargarImagenNivel(0);
    }

    // MÉTODO PARA CAMBIAR LA IMAGEN
    // Esta función busca la imagen en nuestra lista y la coloca en el contenedor de la pantalla.
    public void CargarImagenNivel(int indice)
    {
        // Actualizamos nuestro contador con el nivel actual.
        nivelActual = indice;

        // '.sprite' es la propiedad del componente Image que nos permite cambiar el dibujo que se ve en pantalla.
        contenedorImagenPantalla.sprite = nivelesPhishing[nivelActual].imagenDelMensaje;
    }

    // MÉTODOS PARA LOS BOTONES (VERDE Y ROJO)
    // 1. Esta función se conectará al BOTÓN VERDE ("Seguro")
    public void BotonVerdeSeguro()
    {
        // Si el jugador presiona "Seguro", revisamos si en este nivel 'esPhishing' es Falso (false).
        if (nivelesPhishing[nivelActual].esPhishing == false)
        {
            // ¡Acertó! El mensaje o correo era legítimo de NexoBank.
            Debug.Log("¡Correcto! El mensaje era seguro. Pasando al siguiente caso...");

            scriptAudio.ReproducirAcierto();

            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Era un ataque (correo falso, SMS con virus, etc.) y el jugador confió en él.
            Debug.Log("¡Error! Era una trampa de Phishing. Pierdes un candado de seguridad.");
            scriptPenalizacion.RestarCandado();
        }
    }

    // 2. Esta función se conectará al BOTÓN ROJO ("Peligroso / Reportar")
    public void BotonRojoPeligroso()
    {
        // Si el jugador presiona "Peligroso", revisamos si en este nivel 'esPhishing' es Verdadero (true).
        if (nivelesPhishing[nivelActual].esPhishing == true)
        {
            // ¡Acertó! Detectó las pistas sospechosas en la imagen (mala ortografía, link falso, etc.).
            Debug.Log("¡Excelente! Detectaste el intento de Phishing. Pasando al siguiente caso...");

            scriptAudio.ReproducirAcierto();

            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Reportó un mensaje o correo real de la empresa por equivocación.
            Debug.Log("¡Error! Reportaste un mensaje oficial de NexoBank. Pierdes un candado.");
            scriptPenalizacion.RestarCandado();
        }
    }

    // MÉTODO INTERNO PARA AVANZAR DE NIVEL
    private void AvanzarSiguienteCaso()
    {
        // Sumamos 1 para pasar a la siguiente imagen de la lista.
        nivelActual++;

        // Verificamos si todavía quedan imágenes por revisar en la lista (menos de 5).
        if (nivelActual < nivelesPhishing.Length)
        {
            // Mostramos la nueva imagen en pantalla.
            CargarImagenNivel(nivelActual);
        }
        else
        {
            // Si ya superó las 5 imágenes, encendemos la pantalla de victoria.
            Debug.Log("¡Felicidades! Superaste los 5 niveles del Detector de Phishing.");

            // Llamamos al GestorPenalizacion para detener el reloj y mostrar estadísticas de candados
            scriptPenalizacion.VictoriaPhishing();
        }
    }
}
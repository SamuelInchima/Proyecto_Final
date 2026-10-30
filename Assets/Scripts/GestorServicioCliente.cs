using UnityEngine;
using UnityEngine.UI; // Librería obligatoria para controlar el componente 'Image' donde se verá el NPC.
using TMPro; // Librería para modificar los textos TextMeshPro de la interfaz.

// [System.Serializable] permite que esta "plantilla" se pueda ver y rellenar en el Inspector de Unity.
// Representa la estructura de un nivel individual de atención al cliente.
// Plantilla de datos para cada uno de los 5 niveles en el Inspector
[System.Serializable]
public class CasoCliente
{
    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente de este caso")]
    public Sprite fotoCliente; // Guarda la ilustración del personaje (NPC) para este nivel.

    [TextArea(2, 4)] // Hace que la caja de texto en el Inspector sea más grande y cómoda de escribir.
    public string mensajeCliente; // La queja o pregunta que hace el cliente.

    public string textoBoton1;    // Texto para la primera opción de respuesta (A).
    public string textoBoton2;    // Texto para la segunda opción de respuesta (B).
    public string textoBoton3;    // Texto para la tercera opción de respuesta (C).

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta correcta")]
    public int respuestaCorrecta; // Guardará el número (1, 2 o 3) del botón correcto en este nivel.
}

public class GestorServicioCliente : MonoBehaviour
{
    // VARIABLES DE INTERFAZ (TEXTOS E IMAGEN DEL NPC)
    [Header("Elementos de Texto en Pantalla")]
    public Image contenedorFotoCliente;          // El recuadro UI Image a la izquierda donde aparecerá el NPC.
    public TextMeshProUGUI textoPantallaCliente; // El texto donde se lee el mensaje del cliente.
    public TextMeshProUGUI textoEnBoton1;        // El texto dentro del Botón 1 (A).
    public TextMeshProUGUI textoEnBoton2;        // El texto dentro del Botón 2 (B).
    public TextMeshProUGUI textoEnBoton3;        // El texto dentro del Botón 3 (C).

    [Header("Conexión con otros sistemas")]
    public GestorPenalizacion scriptPenalizacion; // Conexión con el script que resta estrellas y maneja la victoria.

    // DATOS DE LOS 5 NIVELES
    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles; // Lista (Array) de los 5 casos configurables desde el Inspector.
    private int nivelActual = 0;  // Lleva la cuenta de en qué nivel vamos (empieza en 0).

    // AUIDIO
    public GestorAudio scriptAudio;

    void Start()
    {
        // Cargamos el primer nivel (posición 0 de la lista) al iniciar.
        CargarNivel(0);
    }

    // MÉTODOS DE LÓGICA DEL JUEGO
    // Esta función actualiza tanto al personaje (NPC) como los textos en pantalla según el nivel en el que estemos.
    // Es 'public' por si deseas llamarla también desde un menú selector de niveles (1 al 5).
    public void CargarNivel(int indiceNivel)
    {
        // Guardamos el número de nivel actual
        nivelActual = indiceNivel;

        // Si asignaste una imagen de cliente en el Inspector, la colocamos en el recuadro de la pantalla.
        if (contenedorFotoCliente != null && niveles[nivelActual].fotoCliente != null)
        {
            contenedorFotoCliente.sprite = niveles[nivelActual].fotoCliente;
        }

        // Actualizamos el problema del cliente y las 3 opciones de respuesta (A, B y C).
        textoPantallaCliente.text = niveles[nivelActual].mensajeCliente;
        textoEnBoton1.text = niveles[nivelActual].textoBoton1;
        textoEnBoton2.text = niveles[nivelActual].textoBoton2;
        textoEnBoton3.text = niveles[nivelActual].textoBoton3;
    }

    // Esta función se ejecuta al presionar cualquiera de los 3 botones de opción (enviando 1, 2 o 3).
    // Recibe un número (1, 2 o 3) dependiendo de cuál botón presionó el jugador.
    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Comparamos si el botón que presionó el jugador es igual a la respuesta correcta de este nivel
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            Debug.Log("¡Respuesta profesional! Pasando al siguiente caso...");

            scriptAudio.ReproducirAcierto();

            AvanzarSiguienteNivel();

        }
        else
        {
            Debug.Log("Respuesta poco profesional. Pierdes una estrella.");
            // Llamamos a la función RestarEstrella() de tu otro script
            scriptPenalizacion.RestarEstrella();
        }
    }

    // Método interno para pasar al siguiente caso o ganar el módulo
    private void AvanzarSiguienteNivel()
    {
        // Sumamos 1 para pasar al siguiente nivel
        nivelActual++;

        // Si aún quedan clientes en la lista, cargamos el siguiente (cambiando de texto y de NPC automáticamente).
        if (nivelActual < niveles.Length)
        {
            // Cargamos el nuevo nivel en pantalla
            CargarNivel(nivelActual);
        }
        else
        {
            // Si ya no hay más niveles en la lista, significa que superó los 5 casos con éxito
            Debug.Log("¡Módulo de Atención al Cliente completado!");

            // Llamamos al GestorPenalizacion para detener el reloj y mostrar estadísticas de estrellas
            // Si ya atendió a los 5 clientes, llamamos a la pantalla de victoria y estadísticas.
            scriptPenalizacion.VictoriaServicio();
        }
    }
}
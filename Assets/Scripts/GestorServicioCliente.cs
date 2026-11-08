// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos la librería de Interfaz de Usuario para controlar el componente 'Image' del NPC.
using UnityEngine.UI;
// Importamos TextMeshPro para modificar los textos de la interfaz y de la ventana emergente.
using TMPro;

// =========================================================================
// PLANTILLA DE DATOS SERIALIZABLE: CasoCliente
// =========================================================================
[System.Serializable]
public class CasoCliente
{
    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente de este caso")]
    public Sprite fotoCliente; // Ilustración del personaje (NPC) para este nivel.

    [TextArea(2, 4)]
    public string mensajeCliente; // El diálogo directo que plantea el cliente o compañero.

    public string textoBoton1;    // Texto para la Opción A.
    public string textoBoton2;    // Texto para la Opción B.
    public string textoBoton3;    // Texto para la Opción C.

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta correcta")]
    public int respuestaCorrecta; // Número (1, 2 o 3) del botón correcto en este nivel.

    // Caja de texto en el Inspector para escribir la explicación normativa que saldrá en el Pop-up.
    [TextArea(2, 4)]
    [Tooltip("Explica por qué la respuesta es correcta y qué política del banco se aplica en este caso")]
    public string retroalimentacionCaso;
}

// =========================================================================
// CLASE PRINCIPAL DEL MÓDULO: GestorServicioCliente
// =========================================================================
public class GestorServicioCliente : MonoBehaviour
{
    [Header("Elementos de Texto en Pantalla")]
    public Image contenedorFotoCliente;          // Recuadro UI Image donde aparece el NPC.
    public TextMeshProUGUI textoPantallaCliente; // Texto donde se lee el mensaje del cliente.
    public TextMeshProUGUI textoEnBoton1;        // Texto dentro del Botón 1 (A).
    public TextMeshProUGUI textoEnBoton2;        // Texto dentro del Botón 2 (B).
    public TextMeshProUGUI textoEnBoton3;        // Texto dentro del Botón 3 (C).

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;               // El objeto Panel de la ventana emergente.
    public TextMeshProUGUI textoTituloFeedback;         // Título del Pop-up (ej. "¡DECISIÓN CORRECTA!").
    public TextMeshProUGUI textoExplicacionFeedback;    // Texto donde se mostrará 'retroalimentacionCaso'.

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;      // Conexión con el script que resta estrellas y maneja el reloj.
    public GestorAudio scriptAudio;              // Conexión con el script de sonidos.

    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles;                // Lista (Array) de los 5 casos en el Inspector.
    private int nivelActual = 0;                 // Lleva la cuenta de en qué nivel vamos (empieza en 0).

    // Interruptor de seguridad para saber si este módulo abrió el Pop-up.
    private bool popUpActivoEnServicio = false;

    void Start()
    {
        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }

        CargarNivel(0);
    }

    // =========================================================================
    // MÉTODOS DE LÓGICA DEL JUEGO
    // =========================================================================

    public void CargarNivel(int indiceNivel)
    {
        nivelActual = indiceNivel;

        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }
        popUpActivoEnServicio = false;

        if (contenedorFotoCliente != null && niveles[nivelActual].fotoCliente != null)
        {
            contenedorFotoCliente.sprite = niveles[nivelActual].fotoCliente;
        }

        textoPantallaCliente.text = niveles[nivelActual].mensajeCliente;
        textoEnBoton1.text = niveles[nivelActual].textoBoton1;
        textoEnBoton2.text = niveles[nivelActual].textoBoton2;
        textoEnBoton3.text = niveles[nivelActual].textoBoton3;
    }

    // Se ejecuta al presionar cualquiera de los 3 botones de respuesta (1, 2 o 3).
    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Evaluamos si el botón presionado coincide con la respuesta correcta.
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            // ¡ACERTÓ! Pausamos el reloj, sonamos el acierto y abrimos el Pop-up de retroalimentación.
            scriptMecanicas.PausarReloj();
            scriptAudio.ReproducirAcierto();
            MostrarPopUp("¡DECISIÓN CORRECTA!", niveles[nivelActual].retroalimentacionCaso);
        }
        else
        {
            // ¡SE EQUIVOCÓ! Solo restamos una estrella (y suena el error).
            // NO abrimos el Pop-up ni avanzamos de caso, permitiendo que el jugador vuelva a intentarlo.
            Debug.Log("Respuesta incorrecta. Pierdes una estrella, intenta con otra opción.");
            scriptMecanicas.RestarEstrella();
        }
    }

    // Método interno que enciende la ventana emergente y rellena sus textos.
    private void MostrarPopUp(string titulo, string explicacion)
    {
        popUpActivoEnServicio = true;
        panelPopUpFeedback.SetActive(true);
        textoTituloFeedback.text = titulo;
        textoExplicacionFeedback.text = explicacion;
    }

    // Este método se conecta al botón "Continuar / Entendido" dentro de la ventana emergente.
    public void BotonCerrarPopUpServicio()
    {
        if (popUpActivoEnServicio == false)
        {
            return;
        }

        panelPopUpFeedback.SetActive(false);
        popUpActivoEnServicio = false;

        // Reanudamos el reloj y pasamos al siguiente caso tras haber acertado.
        scriptMecanicas.ReanudarReloj();
        AvanzarSiguienteNivel();
    }

    private void AvanzarSiguienteNivel()
    {
        nivelActual++;

        if (nivelActual < niveles.Length)
        {
            CargarNivel(nivelActual);
        }
        else
        {
            Debug.Log("¡Módulo de Atención al Cliente completado!");
            scriptMecanicas.VictoriaServicio();
        }
    }
}
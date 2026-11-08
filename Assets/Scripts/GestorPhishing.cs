// Importamos las herramientas base de Unity.
using UnityEngine;
// Librería obligatoria para controlar componentes 'Image' del Canvas.
using UnityEngine.UI;
// Librería obligatoria para controlar los textos TextMeshPro de la ventana emergente.
using TMPro;

// =========================================================================
// PLANTILLA DE DATOS SERIALIZABLE: CasoPhishing
// =========================================================================
[System.Serializable]
public class CasoPhishing
{
    [Tooltip("Solo sirve para que te guíes en el Inspector (ej. 'Nivel 1: SMS Falso')")]
    public string nombreDelCaso;

    [Tooltip("Arrastra aquí la imagen del correo, SMS o pantalla creada por el diseñador")]
    public Sprite imagenDelMensaje;

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing;

    // Caja de texto en el Inspector para explicar la pista clave de este mensaje en el Pop-up.
    [TextArea(2, 4)]
    [Tooltip("Explica qué pista delata el ataque de Phishing o por qué el mensaje es legítimo")]
    public string retroalimentacionPhishing;
}

// =========================================================================
// CLASE PRINCIPAL DEL MÓDULO: GestorPhishing
// =========================================================================
public class GestorPhishing : MonoBehaviour
{
    [Header("Elementos en Pantalla")]
    public Image contenedorImagenPantalla;

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;               // El objeto Panel de la ventana emergente.
    public TextMeshProUGUI textoTituloFeedback;         // Título del Pop-up (ej. "¡AMENAZA BLOQUEADA!").
    public TextMeshProUGUI textoExplicacionFeedback;    // Texto donde se mostrará 'retroalimentacionPhishing'.

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;             // Referencia al script que apaga los 3 candados y controla el reloj.
    public GestorAudio scriptAudio;                     // Referencia al script de audio.

    [Header("Lista de los 5 Casos (Correos, SMS, etc.)")]
    public CasoPhishing[] nivelesPhishing;              // Array que guarda las 5 imágenes y sus respuestas.
    private int nivelActual = 0;                        // Lleva la cuenta de en qué imagen vamos (empieza en 0).

    // Interruptor de seguridad para saber si Phishing abrió el Pop-up.
    private bool popUpActivoEnPhishing = false;

    void Start()
    {
        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }

        CargarImagenNivel(0);
    }

    public void CargarImagenNivel(int indice)
    {
        nivelActual = indice;

        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }
        popUpActivoEnPhishing = false;

        contenedorImagenPantalla.sprite = nivelesPhishing[nivelActual].imagenDelMensaje;
    }

    // =========================================================================
    // MÉTODOS PARA LOS BOTONES DE DECISIÓN (VERDE Y ROJO)
    // =========================================================================

    // 1. Conectado al BOTÓN VERDE ("Seguro")
    public void BotonVerdeSeguro()
    {
        if (nivelesPhishing[nivelActual].esPhishing == false)
        {
            // ¡ACERTÓ! Pausamos el reloj y abrimos la ventana emergente.
            scriptMecanicas.PausarReloj();
            scriptAudio.ReproducirAcierto();
            MostrarPopUp("¡ANÁLISIS CORRECTO!", nivelesPhishing[nivelActual].retroalimentacionPhishing);
        }
        else
        {
            // ¡SE EQUIVOCÓ! Solo restamos un candado sin abrir el Pop-up ni cambiar de caso.
            Debug.Log("¡Error! Era una trampa de Phishing. Pierdes un candado de seguridad.");
            scriptMecanicas.RestarCandado();
        }
    }

    // 2. Conectado al BOTÓN ROJO ("Peligroso / Reportar")
    public void BotonRojoPeligroso()
    {
        if (nivelesPhishing[nivelActual].esPhishing == true)
        {
            // ¡ACERTÓ! Pausamos el reloj y abrimos la ventana emergente.
            scriptMecanicas.PausarReloj();
            scriptAudio.ReproducirAcierto();
            MostrarPopUp("¡AMENAZA BLOQUEADA!", nivelesPhishing[nivelActual].retroalimentacionPhishing);
        }
        else
        {
            // ¡SE EQUIVOCÓ! Solo restamos un candado sin abrir el Pop-up ni cambiar de caso.
            Debug.Log("¡Error! Reportaste un mensaje oficial de NexoBank. Pierdes un candado.");
            scriptMecanicas.RestarCandado();
        }
    }

    // Método interno para encender el Pop-up con los textos del caso actual.
    private void MostrarPopUp(string titulo, string explicacion)
    {
        popUpActivoEnPhishing = true;
        panelPopUpFeedback.SetActive(true);
        textoTituloFeedback.text = titulo;
        textoExplicacionFeedback.text = explicacion;
    }

    // Se conecta al botón "Continuar / Entendido" dentro de la ventana emergente.
    public void BotonCerrarPopUpPhishing()
    {
        if (popUpActivoEnPhishing == false)
        {
            return;
        }

        panelPopUpFeedback.SetActive(false);
        popUpActivoEnPhishing = false;

        scriptMecanicas.ReanudarReloj();
        AvanzarSiguienteCaso();
    }

    private void AvanzarSiguienteCaso()
    {
        nivelActual++;

        if (nivelActual < nivelesPhishing.Length)
        {
            CargarImagenNivel(nivelActual);
        }
        else
        {
            Debug.Log("¡Felicidades! Superaste los 5 niveles del Detector de Phishing.");
            scriptMecanicas.VictoriaPhishing();
        }
    }
}
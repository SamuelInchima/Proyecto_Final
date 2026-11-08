// Importamos la librería principal de Unity para manejar objetos de la escena (GameObjects).
using UnityEngine;
// Importamos 'System.Collections' para usar Corrutinas (IEnumerator) en las animaciones suaves de paneles.
using System.Collections;

public class GestorMenuPrincipal : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: VARIABLES DE LOS PANELES DE LA INTERFAZ
    // =========================================================================
    [Header("Paneles de la Interfaz")]
    public GameObject panelMenuPrincipal;    // Panel del menú inicial de NexoBank.
    public GameObject panelSelectorModulos;  // Panel donde se elige Servicio al Cliente o Phishing.
    public GameObject panelInstrucciones;    // Panel con las reglas generales del simulador.

    [Header("Paneles de los Módulos y Tutorial")]
    public GameObject panelNivelServicio;    // Panel del juego de Servicio al Cliente.
    public GameObject panelTutorialPhishing; // NUEVO: Panel educativo previo que explica qué es el Phishing.
    public GameObject panelNivelPhishing;    // Panel del juego de Detector de Phishing.

    // =========================================================================
    // SECCIÓN 2: CONFIGURACIÓN DE LA ANIMACIÓN
    // =========================================================================
    [Header("Ajustes de Animación")]
    [Tooltip("Velocidad con la que aparecen los paneles (4 = un cuarto de segundo)")]
    public float velocidadAnimacion = 4f;

    // =========================================================================
    // SECCIÓN 3: CONEXIÓN CON LOS OTROS SCRIPTS (PATRÓN MEDIADOR)
    // =========================================================================
    [Header("Conexión con los otros Scripts")]
    public GestorMecanicas scriptMecanicas;         // Maneja vidas, reloj y pantallas de victoria/derrota.
    public GestorServicioCliente scriptServicio;    // Carga los casos de Servicio al Cliente.
    public GestorPhishing scriptPhishing;           // Carga las imágenes de Phishing.

    [Header("Audio")]
    public GestorAudio scriptAudio;                 // Cambia la música de fondo según la pantalla.

    // =========================================================================
    // MÉTODO START: CONFIGURACIÓN DEL PRIMER FOTOGRAMA
    // =========================================================================
    void Start()
    {
        // Apagamos todos los paneles secundarios al iniciar el juego.
        panelSelectorModulos.SetActive(false);
        panelInstrucciones.SetActive(false);
        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);

        // NUEVO: Nos aseguramos de que el tutorial de Phishing también inicie apagado.
        if (panelTutorialPhishing != null)
        {
            panelTutorialPhishing.SetActive(false);
        }

        // Mostramos el Menú Principal con animación suave.
        AbrirPanelAnimado(panelMenuPrincipal);
    }

    // =========================================================================
    // SECCIÓN 4: MÉTODOS PARA LOS BOTONES DEL MENÚ PRINCIPAL
    // =========================================================================

    public void BotonJugar()
    {
        panelMenuPrincipal.SetActive(false);
        AbrirPanelAnimado(panelSelectorModulos);
    }

    public void BotonInstrucciones()
    {
        panelMenuPrincipal.SetActive(false);
        AbrirPanelAnimado(panelInstrucciones);
    }

    public void BotonVolver()
    {
        panelInstrucciones.SetActive(false);
        panelSelectorModulos.SetActive(false);
        AbrirPanelAnimado(panelMenuPrincipal);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo del simulador de NexoBank");
        Application.Quit();
    }

    // =========================================================================
    // SECCIÓN 5: MÉTODOS DEL SELECTOR DE MÓDULOS Y TUTORIAL DE PHISHING
    // =========================================================================

    // 1. Se ejecuta al presionar el botón "Servicio al Cliente" en el Selector.
    public void BotonModuloServicio()
    {
        panelSelectorModulos.SetActive(false);
        AbrirPanelAnimado(panelNivelServicio);

        scriptServicio.CargarNivel(0);
        scriptMecanicas.PrepararModuloServicio();
        scriptAudio.CambiarMusica(scriptAudio.musicaServicio);
    }

    // 2. MODIFICADO: Al presionar "Detector de Phishing" en el Selector, primero abrimos el Tutorial.
    // Todavía NO arrancamos el reloj para que el jugador lea la explicación sin perder tiempo.
    public void BotonModuloPhishing()
    {
        panelSelectorModulos.SetActive(false);
        AbrirPanelAnimado(panelTutorialPhishing);
        scriptAudio.CambiarMusica(scriptAudio.musicaPhishing);
    }

    // 3. NUEVO: Se conecta al botón "Comenzar Evaluación" dentro del panelTutorialPhishing.
    // Cierra el tutorial, abre el módulo de Phishing y arranca el cronómetro.
    public void BotonIniciarPhishingDesdeTutorial()
    {
        panelTutorialPhishing.SetActive(false);
        AbrirPanelAnimado(panelNivelPhishing);

        scriptPhishing.CargarImagenNivel(0);
        scriptMecanicas.PrepararModuloPhishing();
    }

    // 4. Se ejecuta al presionar "Volver al Selector" desde cualquier módulo o pantalla final.
    public void BotonVolverASelector()
    {
        scriptMecanicas.DetenerReloj();

        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);
        if (panelTutorialPhishing != null)
        {
            panelTutorialPhishing.SetActive(false);
        }
        scriptMecanicas.panelGameOver.SetActive(false);
        scriptMecanicas.panelVictoria.SetActive(false);

        AbrirPanelAnimado(panelSelectorModulos);
        scriptAudio.CambiarMusica(scriptAudio.musicaMenu);
    }

    // =========================================================================
    // SECCIÓN 6: SISTEMA DE ANIMACIÓN DE PANELES
    // =========================================================================

    public void AbrirPanelAnimado(GameObject panel)
    {
        StopAllCoroutines();
        StartCoroutine(CorrutinaAparecerPanel(panel));
    }

    private IEnumerator CorrutinaAparecerPanel(GameObject panel)
    {
        panel.SetActive(true);

        CanvasGroup grupo = panel.GetComponent<CanvasGroup>();
        if (grupo == null)
        {
            grupo = panel.AddComponent<CanvasGroup>();
        }

        grupo.alpha = 0f;
        panel.transform.localScale = new Vector3(0.95f, 0.95f, 1f);

        while (grupo.alpha < 1f)
        {
            grupo.alpha += Time.deltaTime * velocidadAnimacion;
            float escalaActual = Mathf.Lerp(0.95f, 1f, grupo.alpha);
            panel.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        grupo.alpha = 1f;
        panel.transform.localScale = Vector3.one;
    }
}
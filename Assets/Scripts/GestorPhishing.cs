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
    [Tooltip("Ejemplo: 'Caso 1: Smishing por WhatsApp' o 'Caso 3: Vishing Telefónico'")]
    public string nombreDelCaso;

    [Tooltip("Arrastra aquí la imagen del correo, SMS, llamada o QR creada por el diseñador")]
    public Sprite imagenDelMensaje;

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing;

    [TextArea(2, 4)]
    [Tooltip("Explica qué pista delata el ataque de Phishing o por qué el mensaje es legítimo")]
    public string retroalimentacionPhishing;
}

// =========================================================================
// CLASE PRINCIPAL DEL MÓDULO 2: GestorPhishing
// =========================================================================
public class GestorPhishing : MonoBehaviour
{
    [Header("Elementos en Pantalla")]
    public Image contenedorImagenPantalla;

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;
    public TextMeshProUGUI textoTituloFeedback;
    public TextMeshProUGUI textoExplicacionFeedback;

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;
    public GestorAudio scriptAudio;
    public GestorSupervisor scriptSupervisor; // NUEVO (PUNTO 6): Conexión con el historial del Supervisor.

    [Header("Lista de los 5 Casos (Correos, SMS, QR, etc.)")]
    public CasoPhishing[] nivelesPhishing;
    private int nivelActual = 0;
    private bool popUpActivoEnPhishing = false;

    // =========================================================================
    // NUEVO (PUNTO 6): VARIABLES PARA EL PROMEDIO Y DIAGNÓSTICO DE PHISHING
    // =========================================================================
    private int erroresEnCasoActual = 0; // Cuenta si se equivocó en la imagen actual antes de acertar.
    private int casosSinErrores = 0;     // Cuenta cuántos de los 5 casos detectó al primer intento.

    [HideInInspector] public int ultimoPromedioPhishing = 0;
    [HideInInspector] public string ultimoDiagnosticoPhishing = "";
    [HideInInspector] public string ultimoReportePhishing = "";

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

        // Si empieza desde el primer caso (índice 0), limpiamos las estadísticas de la partida anterior.
        if (indice == 0)
        {
            casosSinErrores = 0;
            ultimoPromedioPhishing = 0;
            ultimoDiagnosticoPhishing = "";
            ultimoReportePhishing = "";
        }

        erroresEnCasoActual = 0;

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
            // ¡ACERTÓ!
            RegistrarAciertoCaso("¡ANÁLISIS CORRECTO!");
        }
        else
        {
            // ¡SE EQUIVOCÓ! Aprobó una trampa de Phishing.
            RegistrarFalloCaso();
        }
    }

    // 2. Conectado al BOTÓN ROJO ("Peligroso / Reportar")
    public void BotonRojoPeligroso()
    {
        if (nivelesPhishing[nivelActual].esPhishing == true)
        {
            // ¡ACERTÓ!
            RegistrarAciertoCaso("¡AMENAZA BLOQUEADA!");
        }
        else
        {
            // ¡SE EQUIVOCÓ! Reportó un mensaje oficial legítimo.
            RegistrarFalloCaso();
        }
    }

    // Método auxiliar cuando el jugador acierta (con botón verde o rojo).
    private void RegistrarAciertoCaso(string tituloPopUp)
    {
        scriptMecanicas.PausarReloj();
        scriptAudio.ReproducirAcierto();

        string nombreCaso = string.IsNullOrEmpty(nivelesPhishing[nivelActual].nombreDelCaso)
            ? "Caso " + (nivelActual + 1)
            : nivelesPhishing[nivelActual].nombreDelCaso;

        if (erroresEnCasoActual == 0)
        {
            casosSinErrores++;
            ultimoReportePhishing += "• " + nombreCaso + ": DETECTADO AL 1ER INTENTO\n";
        }
        else
        {
            ultimoReportePhishing += "• " + nombreCaso + ": SUPERADO CON FALLO PREVIO\n";
        }

        MostrarPopUp(tituloPopUp, nivelesPhishing[nivelActual].retroalimentacionPhishing);
    }

    // Método auxiliar cuando el jugador falla y pierde un candado.
    private void RegistrarFalloCaso()
    {
        erroresEnCasoActual++;
        scriptMecanicas.RestarCandado();

        // Si al restar el candado perdió las 3 vidas (se encendió Game Over), guardamos el reporte de derrota.
        if (scriptMecanicas.panelGameOver.activeSelf == true)
        {
            string nombreCaso = string.IsNullOrEmpty(nivelesPhishing[nivelActual].nombreDelCaso)
                ? "Caso " + (nivelActual + 1)
                : nivelesPhishing[nivelActual].nombreDelCaso;

            ultimoReportePhishing += "• " + nombreCaso + ": REPROBADO (Vulneración de Seguridad)\n";
            CalcularYGuardarDiagnosticoPhishing(false);
        }
    }

    private void MostrarPopUp(string titulo, string explicacion)
    {
        popUpActivoEnPhishing = true;
        panelPopUpFeedback.SetActive(true);
        textoTituloFeedback.text = titulo;
        textoExplicacionFeedback.text = explicacion;
    }

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
            // ¡Completó los 5 casos de Phishing! Calculamos y guardamos su diagnóstico en el Supervisor.
            CalcularYGuardarDiagnosticoPhishing(true);
            scriptMecanicas.VictoriaPhishing();
        }
    }

    // =========================================================================
    // NUEVO (PUNTO 6): CÁLCULO Y GUARDADO DEL DIAGNÓSTICO DE CIBERSEGURIDAD
    // =========================================================================
    public void CalcularYGuardarDiagnosticoPhishing(bool superoTodosLosCasos)
    {
        ultimoPromedioPhishing = (casosSinErrores * 100) / nivelesPhishing.Length;

        if (superoTodosLosCasos == false)
        {
            ultimoDiagnosticoPhishing = "NO APTO EN CIBERSEGURIDAD - RIESGO CRÍTICO DE HACKEO (Módulo Reprobado)";
        }
        else if (ultimoPromedioPhishing == 100)
        {
            ultimoDiagnosticoPhishing = "APTO SOBRESALIENTE EN CIBERSEGURIDAD\nIdentifica sin errores ataques de Phishing, Smishing, Vishing y Quishing.";
        }
        else if (ultimoPromedioPhishing >= 80)
        {
            ultimoDiagnosticoPhishing = "APTO CON OBSERVACIÓN EN CIBERSEGURIDAD\nBuen criterio general, pero presentó un descuido ante ingeniería social.";
        }
        else
        {
            ultimoDiagnosticoPhishing = "VULNERABILIDAD MODERADA - REQUIERE REFUERZO EN SEGURIDAD TI\nSuperó el módulo al límite; alto riesgo de caer en enlaces o llamadas fraudulentas.";
        }

        // Enviamos el reporte al archivo del Supervisor identificándolo como el MÓDULO 2.
        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 2: DETECTOR DE PHISHING", ultimoPromedioPhishing, ultimoDiagnosticoPhishing, ultimoReportePhishing);
        }
    }
}
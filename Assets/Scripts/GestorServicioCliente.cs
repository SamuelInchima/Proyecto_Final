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
    // NUEVO: Nombre corto de la norma evaluada para mostrarlo en la tabla de resultados finales.
    [Tooltip("Ejemplo: 'Caso 1: Línea Ética y Cero Tolerancia'")]
    public string nombrePolitica;

    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente de este caso")]
    public Sprite fotoCliente;

    [TextArea(2, 4)]
    public string mensajeCliente;

    public string textoBoton1;
    public string textoBoton2;
    public string textoBoton3;

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta correcta")]
    public int respuestaCorrecta;

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
    public Image contenedorFotoCliente;
    public TextMeshProUGUI textoPantallaCliente;
    public TextMeshProUGUI textoEnBoton1;
    public TextMeshProUGUI textoEnBoton2;
    public TextMeshProUGUI textoEnBoton3;

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;
    public TextMeshProUGUI textoTituloFeedback;
    public TextMeshProUGUI textoExplicacionFeedback;

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;
    public GestorAudio scriptAudio;
    public GestorSupervisor scriptSupervisor; // NUEVO: Conexión para guardar los datos al terminar el test.

    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles;
    private int nivelActual = 0;
    private bool popUpActivoEnServicio = false;

    // =========================================================================
    // NUEVO: VARIABLES PARA CALCULAR EL PROMEDIO Y EL DIAGNÓSTICO DETALLADO
    // =========================================================================
    private int erroresEnCasoActual = 0; // Cuenta cuántas veces falló el jugador en la pregunta actual.
    private int casosSinErrores = 0;     // Cuenta cuántos de los 5 casos respondió bien al primer intento.

    // Variables públicas que guardan el resultado final (nos servirán también en el Paso 7 para el Supervisor).
    [HideInInspector] public int ultimoPromedioServicio = 0;
    [HideInInspector] public string ultimoDiagnosticoServicio = "";
    [HideInInspector] public string ultimoReporteDetallado = "";

    void Start()
    {
        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }

        CargarNivel(0);
    }

    // =========================================================================
    // MÉTODOS DE LÓGICA DEL JUEGO Y REGISTRO DE DESEMPEÑO
    // =========================================================================

    public void CargarNivel(int indiceNivel)
    {
        nivelActual = indiceNivel;

        // NUEVO: Si estamos empezando desde el primer caso (nivel 0), limpiamos las estadísticas anteriores.
        if (indiceNivel == 0)
        {
            casosSinErrores = 0;
            ultimoPromedioServicio = 0;
            ultimoDiagnosticoServicio = "";
            ultimoReporteDetallado = "";
        }

        // Reiniciamos a 0 los errores de esta pregunta específica al cargarla en pantalla.
        erroresEnCasoActual = 0;

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

    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Obtenemos el nombre de la norma de este caso (o ponemos un nombre por defecto si se dejó vacío en el Inspector).
        string nombreDeLaNorma = string.IsNullOrEmpty(niveles[nivelActual].nombrePolitica)
            ? "Caso " + (nivelActual + 1)
            : niveles[nivelActual].nombrePolitica;

        // Evaluamos si el jugador presionó el botón correcto.
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            scriptMecanicas.PausarReloj();
            scriptAudio.ReproducirAcierto();

            // NUEVO: Verificamos si acertó al primer intento (0 errores en este caso) o si había fallado antes.
            if (erroresEnCasoActual == 0)
            {
                casosSinErrores++; // Suma 1 al contador de casos limpios para el promedio.
                ultimoReporteDetallado += "• " + nombreDeLaNorma + ": APROBADO (1er intento)\n";
            }
            else
            {
                ultimoReporteDetallado += "• " + nombreDeLaNorma + ": SUPERADO CON FALLO (" + erroresEnCasoActual + " error/es)\n";
            }

            MostrarPopUp("¡DECISIÓN CORRECTA!", niveles[nivelActual].retroalimentacionCaso);
        }
        else
        {
            // ¡SE EQUIVOCÓ! Sumamos 1 error en este caso y restamos una estrella.
            erroresEnCasoActual++;
            scriptMecanicas.RestarEstrella();

            // NUEVO: Si al restar la estrella se activó el Game Over (perdió las 3 vidas),
            // registramos que reprobó en este caso y calculamos su diagnóstico de derrota.
            if (scriptMecanicas.panelGameOver.activeSelf == true)
            {
                ultimoReporteDetallado += "• " + nombreDeLaNorma + ": REPROBADO (Agotó vidas)\n";
                CalcularDiagnosticoFinal(false);
            }
        }
    }

    private void MostrarPopUp(string titulo, string explicacion)
    {
        popUpActivoEnServicio = true;
        panelPopUpFeedback.SetActive(true);
        textoTituloFeedback.text = titulo;
        textoExplicacionFeedback.text = explicacion;
    }

    public void BotonCerrarPopUpServicio()
    {
        if (popUpActivoEnServicio == false)
        {
            return;
        }

        panelPopUpFeedback.SetActive(false);
        popUpActivoEnServicio = false;

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
            // ¡Completó los 5 casos! Calculamos el promedio y el diagnóstico antes de abrir la pantalla final.
            CalcularDiagnosticoFinal(true);

            // Le enviamos a GestorMecanicas el promedio, el diagnóstico y el desglose de los 5 casos.
            scriptMecanicas.VictoriaServicioDetallada(ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }

    // =========================================================================
    // NUEVO: MÉTODO QUE CALCULA EL PROMEDIO (%) Y EL DIAGNÓSTICO PSICOLÓGICO
    // =========================================================================
    public void CalcularDiagnosticoFinal(bool superoTodosLosCasos)
    {
        // Calculamos el porcentaje de casos resueltos sin errores (ej. 5 de 5 = 100%, 4 de 5 = 80%, 3 de 5 = 60%).
        ultimoPromedioServicio = (casosSinErrores * 100) / niveles.Length;

        if (superoTodosLosCasos == false)
        {
            ultimoDiagnosticoServicio = "NO APTO - RIESGO NORMATIVO CRÍTICO (Módulo Reprobado)";
            return;
        }

        // Asignamos el diagnóstico según el promedio obtenido.
        if (ultimoPromedioServicio == 100)
        {
            ultimoDiagnosticoServicio = "APTO SOBRESALIENTE (Cumplimiento Normativo Total)\nDominas el Código de Ética, Reserva Bancaria y SARLAFT sin ceder a la presión.";
        }
        else if (ultimoPromedioServicio >= 80)
        {
            ultimoDiagnosticoServicio = "APTO CON OBSERVACIÓN (Riesgo Operativo Bajo)\nConoces las normas de NexoBank, pero dudaste en un caso. Repasa la política señalada.";
        }
        else
        {
            ultimoDiagnosticoServicio = "REQUIERE CAPACITACIÓN (Riesgo Operativo Moderado)\nPasaste al límite. Cedes ante trampas de empatía o urgencia que exponen al banco.";
        }

        // NUEVO: Apenas termina de calcular el diagnóstico (gane o pierda), lo guarda en el archivo del Supervisor.
        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador(ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }
}
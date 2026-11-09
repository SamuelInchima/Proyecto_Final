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
    [Tooltip("Ejemplo: 'Caso 1: Línea Ética y Cero Tolerancia'")]
    public string nombrePolitica;

    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente con expresión normal")]
    public Sprite fotoCliente;

    // NUEVO (PUNTO 3C): Segunda imagen del mismo NPC con expresión de enojado o preocupado si el jugador responde mal.
    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del mismo cliente con cara de enojado o preocupado")]
    public Sprite fotoClienteMolesto;

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
    [Header("Elementos de Texto e Imagen en Pantalla")]
    public Image contenedorFotoCliente;
    public TextMeshProUGUI textoPantallaCliente;
    public TextMeshProUGUI textoEnBoton1;
    public TextMeshProUGUI textoEnBoton2;
    public TextMeshProUGUI textoEnBoton3;

    // NUEVO (PUNTO 3C): Ajustes para regular el movimiento de respiración del NPC desde el Inspector.
    [Header("Animación de Respiración del NPC")]
    [Tooltip("Qué tantos píxeles se mueve suavemente hacia los lados (ej. 4 píxeles)")]
    public float amplitudMovimientoLados = 4f;
    [Tooltip("Qué tan lento se mueve (1.2 = movimiento muy pausado y natural)")]
    public float velocidadRespiracion = 1.2f;
    private Vector3 posicionInicialNPC;
    private bool posicionNPCGuardada = false;

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;
    public TextMeshProUGUI textoTituloFeedback;
    public TextMeshProUGUI textoExplicacionFeedback;

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;
    public GestorAudio scriptAudio;
    public GestorSupervisor scriptSupervisor;

    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles;
    private int nivelActual = 0;
    private bool popUpActivoEnServicio = false;

    private int erroresEnCasoActual = 0;
    private int casosSinErrores = 0;

    [HideInInspector] public int ultimoPromedioServicio = 0;
    [HideInInspector] public string ultimoDiagnosticoServicio = "";
    [HideInInspector] public string ultimoReporteDetallado = "";

    void Start()
    {
        // Guardamos la posición original donde colocaste el retrato del NPC en el Canvas.
        if (contenedorFotoCliente != null)
        {
            posicionInicialNPC = contenedorFotoCliente.transform.localPosition;
            posicionNPCGuardada = true;
        }

        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }

        CargarNivel(0);
    }

    // =========================================================================
    // NUEVO (PUNTO 3C): MÉTODO UPDATE PARA QUE EL NPC RESPIRE Y SE MUEVA SUAVE
    // =========================================================================
    void Update()
    {
        if (contenedorFotoCliente != null && posicionNPCGuardada)
        {
            // 'Mathf.Sin' genera una onda suave que va de -1 a 1 continuamente con el paso del tiempo (Time.time).
            // 1. Movimiento horizontal lento y corto de un lado a otro:
            float desplazamientoX = Mathf.Sin(Time.time * velocidadRespiracion) * amplitudMovimientoLados;
            contenedorFotoCliente.transform.localPosition = posicionInicialNPC + new Vector3(desplazamientoX, 0f, 0f);

            // 2. Efecto sutil de respiración en el pecho/hombros (sube y baja su altura apenas un 1.5%):
            float escalaRespiracionY = 1f + Mathf.Sin(Time.time * (velocidadRespiracion * 1.8f)) * 0.015f;
            contenedorFotoCliente.transform.localScale = new Vector3(1f, escalaRespiracionY, 1f);
        }
    }

    // =========================================================================
    // MÉTODOS DE LÓGICA DEL JUEGO Y CAMBIO DE EXPRESIÓN DEL NPC
    // =========================================================================
    public void CargarNivel(int indiceNivel)
    {
        nivelActual = indiceNivel;

        if (indiceNivel == 0)
        {
            casosSinErrores = 0;
            ultimoPromedioServicio = 0;
            ultimoDiagnosticoServicio = "";
            ultimoReporteDetallado = "";
        }

        erroresEnCasoActual = 0;

        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }
        popUpActivoEnServicio = false;

        // Al iniciar cada caso, colocamos la imagen del cliente con su expresión normal/tranquila.
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
        string nombreDeLaNorma = string.IsNullOrEmpty(niveles[nivelActual].nombrePolitica)
            ? "Caso " + (nivelActual + 1)
            : niveles[nivelActual].nombrePolitica;

        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            // Si había puesto cara de molesto por un error previo y ahora acertamos, vuelve a su cara normal.
            if (contenedorFotoCliente != null && niveles[nivelActual].fotoCliente != null)
            {
                contenedorFotoCliente.sprite = niveles[nivelActual].fotoCliente;
            }

            scriptMecanicas.PausarReloj();
            scriptAudio.ReproducirAcierto();

            if (erroresEnCasoActual == 0)
            {
                casosSinErrores++;
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
            // ¡SE EQUIVOCÓ!
            erroresEnCasoActual++;

            // NUEVO (PUNTO 3C): Cambiamos el dibujo del NPC por su expresión de enojado o preocupado.
            if (contenedorFotoCliente != null && niveles[nivelActual].fotoClienteMolesto != null)
            {
                contenedorFotoCliente.sprite = niveles[nivelActual].fotoClienteMolesto;
            }

            scriptMecanicas.RestarEstrella();

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
            CalcularDiagnosticoFinal(true);
            scriptMecanicas.VictoriaServicioDetallada(ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }

    public void CalcularDiagnosticoFinal(bool superoTodosLosCasos)
    {
        ultimoPromedioServicio = (casosSinErrores * 100) / niveles.Length;

        if (superoTodosLosCasos == false)
        {
            ultimoDiagnosticoServicio = "NO APTO - RIESGO NORMATIVO CRÍTICO (Módulo Reprobado)";
        }
        else if (ultimoPromedioServicio == 100)
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

        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 1: SERVICIO AL CLIENTE", ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // NUEVO: Necesario para usar List<int> en el orden aleatorio.

[System.Serializable]
public class CasoCliente
{
    [Tooltip("Ejemplo: 'Caso 1: Línea Ética y Cero Tolerancia'")]
    public string nombrePolitica;

    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente con expresión normal")]
    public Sprite fotoCliente;

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
    public string retroalimentacionCaso;
}

public class GestorServicioCliente : MonoBehaviour
{
    [Header("Elementos de Texto e Imagen en Pantalla")]
    public Image contenedorFotoCliente;
    public TextMeshProUGUI textoPantallaCliente;
    public TextMeshProUGUI textoEnBoton1;
    public TextMeshProUGUI textoEnBoton2;
    public TextMeshProUGUI textoEnBoton3;

    [Header("Animación de Respiración del NPC")]
    public float amplitudMovimientoLados = 4f;
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

    [Header("Configuración de los Niveles")]
    public CasoCliente[] niveles;

    // NUEVO (PUNTO 2): Control del orden aleatorio
    private List<int> ordenAleatorio = new List<int>(); // Guarda el orden revuelto de esta partida.
    private int pasoActual = 0;                         // Indica en qué turno vamos (0, 1, 2, 3, 4...).

    private bool popUpActivoEnServicio = false;
    private int erroresEnCasoActual = 0;
    private int casosSinErrores = 0;

    [HideInInspector] public int ultimoPromedioServicio = 0;
    [HideInInspector] public string ultimoDiagnosticoServicio = "";
    [HideInInspector] public string ultimoReporteDetallado = "";

    void Start()
    {
        if (contenedorFotoCliente != null)
        {
            posicionInicialNPC = contenedorFotoCliente.transform.localPosition;
            posicionNPCGuardada = true;
        }

        if (panelPopUpFeedback != null)
        {
            panelPopUpFeedback.SetActive(false);
        }
    }

    void Update()
    {
        if (contenedorFotoCliente != null && posicionNPCGuardada)
        {
            float desplazamientoX = Mathf.Sin(Time.time * velocidadRespiracion) * amplitudMovimientoLados;
            contenedorFotoCliente.transform.localPosition = posicionInicialNPC + new Vector3(desplazamientoX, 0f, 0f);

            float escalaRespiracionY = 1f + Mathf.Sin(Time.time * (velocidadRespiracion * 1.8f)) * 0.015f;
            contenedorFotoCliente.transform.localScale = new Vector3(1f, escalaRespiracionY, 1f);
        }
    }

    // =========================================================================
    // NUEVO (PUNTO 2): ALGORITMO FISHER-YATES PARA BARAJAR LOS CASOS
    // =========================================================================
    private void BarajarCasosServicio()
    {
        ordenAleatorio.Clear();

        for (int i = 0; i < niveles.Length; i++)
        {
            ordenAleatorio.Add(i);
        }

        for (int i = ordenAleatorio.Count - 1; i > 0; i--)
        {
            int posicionAzar = Random.Range(0, i + 1);
            int temporal = ordenAleatorio[i];
            ordenAleatorio[i] = ordenAleatorio[posicionAzar];
            ordenAleatorio[posicionAzar] = temporal;
        }

        // Mensaje en Consola para que puedas verificar que cada partida sale un orden distinto.
        Debug.Log("[Servicio al Cliente] Nuevo orden aleatorio generado: " + string.Join(", ", ordenAleatorio));
    }

    // =========================================================================
    // MÉTODOS DE LÓGICA DEL JUEGO (USANDO 'indiceReal')
    // =========================================================================
    public void CargarNivel(int indicePaso)
    {
        if (niveles == null || niveles.Length == 0) return;

        pasoActual = indicePaso;

        // Si estamos empezando una partida nueva (turno 0), reiniciamos contadores y BARAJAMOS las preguntas.
        if (indicePaso == 0)
        {
            BarajarCasosServicio();
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

        // CLAVE DEL PUNTO 2: Obtenemos qué caso del Inspector corresponde a este turno.
        int indiceReal = ordenAleatorio[pasoActual];
        CasoCliente casoActual = niveles[indiceReal];

        if (contenedorFotoCliente != null && casoActual.fotoCliente != null)
        {
            contenedorFotoCliente.sprite = casoActual.fotoCliente;
        }

        textoPantallaCliente.text = casoActual.mensajeCliente;
        textoEnBoton1.text = casoActual.textoBoton1;
        textoEnBoton2.text = casoActual.textoBoton2;
        textoEnBoton3.text = casoActual.textoBoton3;
    }

    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Consultamos siempre el caso usando 'indiceReal' desde la lista barajada.
        int indiceReal = ordenAleatorio[pasoActual];
        CasoCliente casoActual = niveles[indiceReal];

        string nombreDeLaNorma = string.IsNullOrEmpty(casoActual.nombrePolitica)
            ? "Caso " + (indiceReal + 1)
            : casoActual.nombrePolitica;

        if (botonPresionado == casoActual.respuestaCorrecta)
        {
            if (contenedorFotoCliente != null && casoActual.fotoCliente != null)
            {
                contenedorFotoCliente.sprite = casoActual.fotoCliente;
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

            MostrarPopUp("¡DECISIÓN CORRECTA!", casoActual.retroalimentacionCaso);
        }
        else
        {
            erroresEnCasoActual++;

            if (contenedorFotoCliente != null && casoActual.fotoClienteMolesto != null)
            {
                contenedorFotoCliente.sprite = casoActual.fotoClienteMolesto;
            }

            scriptMecanicas.RestarEstrella();

            if (scriptMecanicas.panelGameOver.activeSelf == true)
            {
                ultimoReporteDetallado += "• " + nombreDeLaNorma + ": REQUIERE REFUERZO (Agotó vidas)\n";
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
        if (popUpActivoEnServicio == false) return;

        panelPopUpFeedback.SetActive(false);
        popUpActivoEnServicio = false;

        scriptMecanicas.ReanudarReloj();
        AvanzarSiguienteNivel();
    }

    private void AvanzarSiguienteNivel()
    {
        pasoActual++;

        // Comparamos contra ordenAleatorio.Count en lugar de niveles.Length.
        if (pasoActual < ordenAleatorio.Count)
        {
            CargarNivel(pasoActual);
        }
        else
        {
            CalcularDiagnosticoFinal(true);
            scriptMecanicas.VictoriaServicioDetallada(ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }

    public void CalcularDiagnosticoFinal(bool superoTodosLosCasos)
    {
        ultimoPromedioServicio = (casosSinErrores * 100) / ordenAleatorio.Count;

        if (superoTodosLosCasos == false)
        {
            ultimoDiagnosticoServicio = "NIVEL INICIAL - BRECHA NORMATIVA DETECTADA (Requiere refuerzo guiado)";
        }
        else if (ultimoPromedioServicio == 100)
        {
            ultimoDiagnosticoServicio = "NIVEL SOBRESALIENTE (Dominio Normativo Total)\nDominas el Código de Ética, Reserva Bancaria y SARLAFT sin ceder a la presión.";
        }
        else if (ultimoPromedioServicio >= 80)
        {
            ultimoDiagnosticoServicio = "NIVEL EN CONSOLIDACIÓN (1 área por reforzar)\nConoces las normas de NexoBank, pero dudaste en un caso. Repasa la política señalada.";
        }
        else
        {
            ultimoDiagnosticoServicio = "REQUIERE ACOMPAÑAMIENTO PRÁCTICO\nSuperaste la práctica al límite. Refuerza los protocolos ante dilemas de urgencia.";
        }

        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 1: SERVICIO AL CLIENTE", ultimoPromedioServicio, ultimoDiagnosticoServicio, ultimoReporteDetallado);
        }
    }
}
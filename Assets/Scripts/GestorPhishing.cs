using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // NUEVO: Necesario para List<int>

[System.Serializable]
public class CasoPhishing
{
    [Tooltip("Ejemplo: 'Caso 1: Smishing por WhatsApp' o 'Caso 3: Vishing Telefónico'")]
    public string nombreDelCaso;

    [Tooltip("Arrastra aquí la imagen del correo, WhatsApp, llamada o QR")]
    public Sprite imagenDelMensaje;

    [Tooltip("Opcional: Arrastra aquí el audio de la voz IA si este nivel es una llamada telefónica (Vishing)")]
    public AudioClip audioLlamadaCaso;

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing;

    [TextArea(2, 4)]
    public string retroalimentacionPhishing;
}

public class GestorPhishing : MonoBehaviour
{
    [Header("Elementos en Pantalla")]
    public Image contenedorImagenPantalla;

    [Tooltip("Opcional: Botón de 'Repetir Audio' que solo aparecerá en los niveles que tengan voz IA")]
    public GameObject botonRepetirLlamada;

    [Header("Ventana Emergente de Retroalimentación")]
    public GameObject panelPopUpFeedback;
    public TextMeshProUGUI textoTituloFeedback;
    public TextMeshProUGUI textoExplicacionFeedback;

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas;
    public GestorAudio scriptAudio;
    public GestorSupervisor scriptSupervisor;

    [Header("Lista de Casos")]
    public CasoPhishing[] nivelesPhishing;

    // NUEVO (PUNTO 2): Control del orden aleatorio
    private List<int> ordenAleatorio = new List<int>();
    private int pasoActual = 0;

    private bool popUpActivoEnPhishing = false;
    private int erroresEnCasoActual = 0;
    private int casosSinErrores = 0;

    [HideInInspector] public int ultimoPromedioPhishing = 0;
    [HideInInspector] public string ultimoDiagnosticoPhishing = "";
    [HideInInspector] public string ultimoReportePhishing = "";

    void Start()
    {
        if (panelPopUpFeedback != null) panelPopUpFeedback.SetActive(false);
        if (botonRepetirLlamada != null) botonRepetirLlamada.SetActive(false);
    }

    // =========================================================================
    // NUEVO (PUNTO 2): ALGORITMO FISHER-YATES PARA BARAJAR CASOS DE PHISHING
    // =========================================================================
    private void BarajarCasosPhishing()
    {
        ordenAleatorio.Clear();

        for (int i = 0; i < nivelesPhishing.Length; i++)
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

        Debug.Log("[Detector de Phishing] Nuevo orden aleatorio generado: " + string.Join(", ", ordenAleatorio));
    }

    public void CargarImagenNivel(int indicePaso)
    {
        if (nivelesPhishing == null || nivelesPhishing.Length == 0) return;

        pasoActual = indicePaso;

        // Si empieza desde el primer turno (0), limpiamos estadísticas y barajamos las imágenes.
        if (indicePaso == 0)
        {
            BarajarCasosPhishing();
            casosSinErrores = 0;
            ultimoPromedioPhishing = 0;
            ultimoDiagnosticoPhishing = "";
            ultimoReportePhishing = "";
        }

        erroresEnCasoActual = 0;

        if (panelPopUpFeedback != null) panelPopUpFeedback.SetActive(false);
        popUpActivoEnPhishing = false;

        // CLAVE DEL PUNTO 2: Leemos el caso real que tocó en este turno.
        int indiceReal = ordenAleatorio[pasoActual];
        CasoPhishing casoActual = nivelesPhishing[indiceReal];

        contenedorImagenPantalla.sprite = casoActual.imagenDelMensaje;
        contenedorImagenPantalla.preserveAspect = true;

        if (scriptAudio != null && scriptAudio.reproductorEfectos != null)
        {
            scriptAudio.reproductorEfectos.Stop();
        }

        if (casoActual.audioLlamadaCaso != null)
        {
            if (botonRepetirLlamada != null) botonRepetirLlamada.SetActive(true);
            ReproducirVozLlamadaActual();
        }
        else
        {
            if (botonRepetirLlamada != null) botonRepetirLlamada.SetActive(false);
        }
    }

    public void ReproducirVozLlamadaActual()
    {
        if (ordenAleatorio.Count == 0) return;

        int indiceReal = ordenAleatorio[pasoActual];
        AudioClip clipActual = nivelesPhishing[indiceReal].audioLlamadaCaso;

        if (scriptAudio != null && scriptAudio.reproductorEfectos != null && clipActual != null)
        {
            scriptAudio.reproductorEfectos.Stop();
            scriptAudio.reproductorEfectos.PlayOneShot(clipActual);
        }
    }

    public void BotonVerdeSeguro()
    {
        int indiceReal = ordenAleatorio[pasoActual];
        if (nivelesPhishing[indiceReal].esPhishing == false)
            RegistrarAciertoCaso("¡ANÁLISIS CORRECTO!");
        else
            RegistrarFalloCaso();
    }

    public void BotonRojoPeligroso()
    {
        int indiceReal = ordenAleatorio[pasoActual];
        if (nivelesPhishing[indiceReal].esPhishing == true)
            RegistrarAciertoCaso("¡AMENAZA BLOQUEADA!");
        else
            RegistrarFalloCaso();
    }

    private void RegistrarAciertoCaso(string tituloPopUp)
    {
        if (scriptAudio != null && scriptAudio.reproductorEfectos != null)
        {
            scriptAudio.reproductorEfectos.Stop();
        }

        scriptMecanicas.PausarReloj();
        scriptAudio.ReproducirAcierto();

        int indiceReal = ordenAleatorio[pasoActual];
        CasoPhishing casoActual = nivelesPhishing[indiceReal];

        string nombreCaso = string.IsNullOrEmpty(casoActual.nombreDelCaso)
            ? "Caso " + (indiceReal + 1)
            : casoActual.nombreDelCaso;

        if (erroresEnCasoActual == 0)
        {
            casosSinErrores++;
            ultimoReportePhishing += "• " + nombreCaso + ": DETECTADO AL 1ER INTENTO\n";
        }
        else
        {
            ultimoReportePhishing += "• " + nombreCaso + ": SUPERADO CON FALLO PREVIO\n";
        }

        MostrarPopUp(tituloPopUp, casoActual.retroalimentacionPhishing);
    }

    private void RegistrarFalloCaso()
    {
        erroresEnCasoActual++;
        scriptMecanicas.RestarCandado();

        if (scriptMecanicas.panelGameOver.activeSelf == true)
        {
            int indiceReal = ordenAleatorio[pasoActual];
            CasoPhishing casoActual = nivelesPhishing[indiceReal];

            string nombreCaso = string.IsNullOrEmpty(casoActual.nombreDelCaso)
                ? "Caso " + (indiceReal + 1)
                : casoActual.nombreDelCaso;

            ultimoReportePhishing += "• " + nombreCaso + ": REQUIERE REFUERZO (Agotó candados)\n";
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
        if (popUpActivoEnPhishing == false) return;

        panelPopUpFeedback.SetActive(false);
        popUpActivoEnPhishing = false;

        scriptMecanicas.ReanudarReloj();
        AvanzarSiguienteCaso();
    }

    private void AvanzarSiguienteCaso()
    {
        pasoActual++;

        if (pasoActual < ordenAleatorio.Count)
        {
            CargarImagenNivel(pasoActual);
        }
        else
        {
            CalcularYGuardarDiagnosticoPhishing(true);
            scriptMecanicas.VictoriaPhishing();
        }
    }

    public void CalcularYGuardarDiagnosticoPhishing(bool superoTodosLosCasos)
    {
        ultimoPromedioPhishing = (casosSinErrores * 100) / ordenAleatorio.Count;

        if (superoTodosLosCasos == false)
            ultimoDiagnosticoPhishing = "BRECHA CRÍTICA DE CIBERSEGURIDAD (Requiere taller práctico de ingeniería social)";
        else if (ultimoPromedioPhishing == 100)
            ultimoDiagnosticoPhishing = "DOMINIO ALTO EN CIBERDEFENSA\nIdentifica sin errores ataques de Phishing, Smishing, Vishing y Quishing.";
        else if (ultimoPromedioPhishing >= 80)
            ultimoDiagnosticoPhishing = "EN PROGRESO (Reforzar verificación de canales)\nBuen criterio general, con una alerta puntual por repasar.";
        else
            ultimoDiagnosticoPhishing = "VULNERABILIDAD MODERADA - REQUIERE REFUERZO EN SEGURIDAD TI\nSuperó la práctica al límite; reforzar señales de ingeniería social.";

        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 2: DETECTOR DE PHISHING", ultimoPromedioPhishing, ultimoDiagnosticoPhishing, ultimoReportePhishing);
        }
    }
}
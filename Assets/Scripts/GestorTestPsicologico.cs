using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // NUEVO: Necesario para List<int>

[System.Serializable]
public class PreguntaPsicologica
{
    public string temaEvaluado;

    [TextArea(2, 4)]
    public string enunciadoPregunta;

    public string textoBoton1;
    public string textoBoton2;
    public string textoBoton3;

    [Header("Puntaje Secreto de cada Botón (Escribe 2, 1 o 0)")]
    public int puntosBoton1 = 2;
    public int puntosBoton2 = 1;
    public int puntosBoton3 = 0;
}

public class GestorTestPsicologico : MonoBehaviour
{
    [Header("Elementos de Texto en Pantalla")]
    public TextMeshProUGUI textoProgresoPregunta;
    public TextMeshProUGUI textoEnunciadoPantalla;
    public TextMeshProUGUI textoEnBoton1;
    public TextMeshProUGUI textoEnBoton2;
    public TextMeshProUGUI textoEnBoton3;

    [Header("Efectos Visuales y Progreso")]
    public Slider barraProgresoAnimada;
    public TextMeshProUGUI textoEtapaMotivacional;
    public GameObject contenedorTarjetaPregunta;

    [Header("Pantalla Final (Diagnóstico General para el Colaborador)")]
    public GameObject panelAgradecimientoFinal;
    public TextMeshProUGUI textoDiagnosticoJugador;
    public GameObject simboloCompletadoPsicologico;

    [Header("Conexión con otros sistemas")]
    public GestorAudio scriptAudio;
    public GestorSupervisor scriptSupervisor;

    [Header("Lista de Preguntas")]
    public PreguntaPsicologica[] preguntas;

    // NUEVO (PUNTO 2): Control del orden aleatorio
    private List<int> ordenAleatorio = new List<int>();
    private int pasoActual = 0;

    private int puntosIntegridadYEtica = 0;
    private int puntosPresionYEmpatia = 0;
    private int puntosSkillsBancarias = 0;
    private string bitacoraRespuestas = "";
    private bool animandoTransicion = false;

    void Start()
    {
        if (panelAgradecimientoFinal != null) panelAgradecimientoFinal.SetActive(false);
        if (simboloCompletadoPsicologico != null) simboloCompletadoPsicologico.SetActive(false);
    }

    // =========================================================================
    // NUEVO (PUNTO 2): ALGORITMO FISHER-YATES PARA BARAJAR LAS 15 PREGUNTAS
    // =========================================================================
    private void BarajarPreguntasPsicologicas()
    {
        ordenAleatorio.Clear();

        for (int i = 0; i < preguntas.Length; i++)
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

        Debug.Log("[Test Perfil y Skills] Nuevo orden aleatorio generado: " + string.Join(", ", ordenAleatorio));
    }

    public void IniciarTestPsicologico()
    {
        if (preguntas == null || preguntas.Length == 0) return;

        // Barajamos las 15 preguntas cada vez que se inicia el módulo.
        BarajarPreguntasPsicologicas();

        pasoActual = 0;
        puntosIntegridadYEtica = 0;
        puntosPresionYEmpatia = 0;
        puntosSkillsBancarias = 0;
        bitacoraRespuestas = "";
        animandoTransicion = false;

        if (panelAgradecimientoFinal != null) panelAgradecimientoFinal.SetActive(false);
        if (barraProgresoAnimada != null)
        {
            barraProgresoAnimada.minValue = 0f;
            barraProgresoAnimada.maxValue = ordenAleatorio.Count;
            barraProgresoAnimada.value = 0f;
        }

        CargarPreguntaEnPantalla(0);
    }

    public void CargarPreguntaEnPantalla(int indicePaso)
    {
        pasoActual = indicePaso;

        // Para el texto de avance y la barra usamos 'pasoActual' (Turno 1 de 15, 2 de 15...).
        int numeroVisible = pasoActual + 1;
        int porcentajeAvance = (numeroVisible * 100) / ordenAleatorio.Count;

        if (textoProgresoPregunta != null)
        {
            textoProgresoPregunta.text = "Pregunta " + numeroVisible + " de " + ordenAleatorio.Count + " (" + porcentajeAvance + "%)";
        }

        if (textoEtapaMotivacional != null)
        {
            if (porcentajeAvance <= 34)
                textoEtapaMotivacional.text = "BLOQUE 1 DE 3: Explorando tu criterio laboral";
            else if (porcentajeAvance <= 67)
                textoEtapaMotivacional.text = "¡Excelente ritmo! BLOQUE 2 DE 3: Continuamos con tu perfil";
            else
                textoEtapaMotivacional.text = "¡Recta final! BLOQUE 3 DE 3: Últimas situaciones de diagnóstico";
        }

        // Para leer la pregunta y las opciones usamos 'indiceReal'.
        int indiceReal = ordenAleatorio[pasoActual];

        StopAllCoroutines();
        StartCoroutine(CorrutinaAnimarCambioPregunta(indiceReal, numeroVisible));
    }

    private IEnumerator CorrutinaAnimarCambioPregunta(int indiceReal, int numeroPasoVisible)
    {
        animandoTransicion = true;

        GameObject objetivoAnimar = (contenedorTarjetaPregunta != null) ? contenedorTarjetaPregunta : textoEnunciadoPantalla.gameObject;
        CanvasGroup grupo = objetivoAnimar.GetComponent<CanvasGroup>();
        if (grupo == null) grupo = objetivoAnimar.AddComponent<CanvasGroup>();

        grupo.alpha = 0f;
        objetivoAnimar.transform.localScale = new Vector3(0.96f, 0.96f, 1f);

        // Cargamos los textos usando 'indiceReal' (la pregunta que salió en la baraja).
        PreguntaPsicologica p = preguntas[indiceReal];
        textoEnunciadoPantalla.text = p.enunciadoPregunta;
        textoEnBoton1.text = p.textoBoton1;
        textoEnBoton2.text = p.textoBoton2;
        textoEnBoton3.text = p.textoBoton3;

        // La barra de progreso se llena según 'numeroPasoVisible' (1, 2, 3... hasta 15).
        float metaBarra = numeroPasoVisible;
        float inicioBarra = (barraProgresoAnimada != null) ? barraProgresoAnimada.value : 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4.5f;
            grupo.alpha = Mathf.Lerp(0f, 1f, t);

            float escala = Mathf.Lerp(0.96f, 1f, t);
            objetivoAnimar.transform.localScale = new Vector3(escala, escala, 1f);

            if (barraProgresoAnimada != null)
            {
                barraProgresoAnimada.value = Mathf.Lerp(inicioBarra, metaBarra, t);
            }

            yield return null;
        }

        grupo.alpha = 1f;
        objetivoAnimar.transform.localScale = Vector3.one;
        animandoTransicion = false;
    }

    public void SeleccionarOpcionPsicologica(int botonPresionado)
    {
        if (animandoTransicion) return;

        if (scriptAudio != null) scriptAudio.ReproducirClic();

        // Obtenemos la pregunta real que estaba viendo el jugador en este turno.
        int indiceReal = ordenAleatorio[pasoActual];
        PreguntaPsicologica p = preguntas[indiceReal];

        int puntosObtenidos = 0;
        string letraElegida = "";

        if (botonPresionado == 1) { puntosObtenidos = p.puntosBoton1; letraElegida = "A"; }
        else if (botonPresionado == 2) { puntosObtenidos = p.puntosBoton2; letraElegida = "B"; }
        else if (botonPresionado == 3) { puntosObtenidos = p.puntosBoton3; letraElegida = "C"; }

        // DETALLE CLAVE: Para sumar a la dimensión correcta, revisamos 'indiceReal' (su posición original en el Inspector)
        // y NO 'pasoActual', porque las preguntas salieron mezcladas.
        if (indiceReal < 5) puntosIntegridadYEtica += puntosObtenidos;
        else if (indiceReal < 10) puntosPresionYEmpatia += puntosObtenidos;
        else puntosSkillsBancarias += puntosObtenidos;

        string enfoque = (puntosObtenidos == 2) ? "Enfoque Normativo/Óptimo" : (puntosObtenidos == 1) ? "Enfoque Operativo/Rápido" : "Enfoque Flexible/Riesgo";
        bitacoraRespuestas += "Turno " + (pasoActual + 1) + " - " + p.temaEvaluado + ": Opción " + letraElegida + " [" + enfoque + "]\n";

        pasoActual++;

        if (pasoActual < ordenAleatorio.Count)
        {
            CargarPreguntaEnPantalla(pasoActual);
        }
        else
        {
            FinalizarTestYGuardarInforme();
        }
    }

    private void FinalizarTestYGuardarInforme()
    {
        int puntajeMaximoTotal = ordenAleatorio.Count * 2;
        int puntajeTotal = puntosIntegridadYEtica + puntosPresionYEmpatia + puntosSkillsBancarias;
        int promedioGeneral = (puntajeTotal * 100) / puntajeMaximoTotal;

        int porcIntegridad = puntosIntegridadYEtica * 10;
        int porcPresion = puntosPresionYEmpatia * 10;
        int porcSkills = puntosSkillsBancarias * 10;

        string resumenParaElJugador = "";
        string diagnosticoConfidencialEmpresa = "";

        if (promedioGeneral >= 85 && porcIntegridad >= 80)
        {
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Analítico, Normativo y Resiliente\n\n" +
                "Tus decisiones reflejan un sólido equilibrio entre calidad de servicio y apego al control interno bancario. " +
                "Destacas por priorizar la seguridad de la entidad y aplicar criterio técnico en operaciones complejas.\n\n" +
                "Gracias por completar tu diagnóstico inicial. El reporte detallado fue enviado a Formación y Desarrollo.";

            diagnosticoConfidencialEmpresa =
                "PERFIL SÓLIDO (Listo para autonomía operativa)\n" +
                "Dictamen Formativo: Alta adherencia normativa (SJT), autocontrol bajo presión y buen dominio técnico previo.";
        }
        else if (promedioGeneral >= 65)
        {
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Operativo y Orientado a la Agilidad\n\n" +
                "Demuestras alta proactividad para resolver situaciones rápidamente y vocación de servicio. " +
                "Como oportunidad de crecimiento, recuerda que en el sector financiero la trazabilidad y el protocolo formal " +
                "siempre deben prevalecer sobre la rapidez.\n\n" +
                "Gracias por completar tu diagnóstico inicial. El reporte detallado fue enviado a Formación y Desarrollo.";

            diagnosticoConfidencialEmpresa =
                "PERFIL EN DESARROLLO (Requiere acompañamiento en protocolos)\n" +
                "Dictamen Formativo: Prioriza la eficiencia o la empatía sobre el rigor procedimental en casos grises.";
        }
        else
        {
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Relacional en Etapa de Fortalecimiento\n\n" +
                "Tu estilo de trabajo prioriza la flexibilidad interpersonal por encima de los procedimientos rígidos. " +
                "El equipo de Formación te acompañará reforzando normativas de control y análisis financiero en tus primeras semanas.\n\n" +
                "Gracias por completar tu diagnóstico inicial. El reporte detallado fue enviado a Formación y Desarrollo.";

            diagnosticoConfidencialEmpresa =
                "PERFIL CON BRECHAS PRIORITARIAS (Plan de inducción intensivo)\n" +
                "Dictamen Formativo: Alta flexibilidad ante dilemas de excepción operativa y brechas en conocimientos técnicos.";
        }

        if (panelAgradecimientoFinal != null) panelAgradecimientoFinal.SetActive(true);
        if (textoDiagnosticoJugador != null) textoDiagnosticoJugador.text = resumenParaElJugador;
        if (simboloCompletadoPsicologico != null) simboloCompletadoPsicologico.SetActive(true);

        string reporteCompletoSupervisor =
            "MODELO APLICADO: Prueba de Juicio Situacional (SJT) — Orden Aleatorio\n" +
            "• Adherencia Normativa e Integridad (HEXACO/16PF): " + porcIntegridad + "%\n" +
            "• Gestión de Presión y Criterio Operativo (Moss/STAR): " + porcPresion + "%\n" +
            "• Competencias Técnicas y Financieras (Hard Skills): " + porcSkills + "%\n\n" +
            "HISTORIAL DE DECISIONES:\n" + bitacoraRespuestas;

        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 3: PERFIL LABORAL Y SKILLS (SJT)", promedioGeneral, diagnosticoConfidencialEmpresa, reporteCompletoSupervisor);
        }
    }
}
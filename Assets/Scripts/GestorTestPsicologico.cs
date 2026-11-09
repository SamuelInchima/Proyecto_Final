// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos TextMeshPro para actualizar el número de pregunta, el enunciado, los botones y el diagnóstico final.
using TMPro;

// =========================================================================
// PLANTILLA DE DATOS SERIALIZABLE: PreguntaPsicologica
// =========================================================================
[System.Serializable]
public class PreguntaPsicologica
{
    [Tooltip("Ejemplo: 'P1: Manejo de sobrante en caja'")]
    public string temaEvaluado; // Guía corta para el reporte confidencial del Supervisor.

    [TextArea(2, 4)]
    public string enunciadoPregunta; // La pregunta de entrevista o situación laboral.

    public string textoBoton1; // Texto de la Opción A.
    public string textoBoton2; // Texto de la Opción B.
    public string textoBoton3; // Texto de la Opción C.

    // PUNTAJES INTERNOS SECRETOS (Invisibles para el jugador):
    // 2 puntos = Perfil Bancario Ideal / Alta Competencia
    // 1 punto  = Perfil Operativo Estándar / Dudoso
    // 0 puntos = Perfil Impulsivo / Riesgo Normativo o Falta de Skill
    [Header("Puntaje Secreto de cada Botón (Escribe 2, 1 o 0)")]
    public int puntosBoton1 = 2;
    public int puntosBoton2 = 1;
    public int puntosBoton3 = 0;
}

// =========================================================================
// CLASE PRINCIPAL DEL MÓDULO 3: GestorTestPsicologico
// =========================================================================
public class GestorTestPsicologico : MonoBehaviour
{
    [Header("Elementos de Texto en Pantalla")]
    public TextMeshProUGUI textoProgresoPregunta; // Muestra "Pregunta 1 de 15", "Pregunta 2 de 15", etc.
    public TextMeshProUGUI textoEnunciadoPantalla;// Donde se lee la situación o pregunta de entrevista.
    public TextMeshProUGUI textoEnBoton1;         // Texto dentro del Botón 1 (A).
    public TextMeshProUGUI textoEnBoton2;         // Texto dentro del Botón 2 (B).
    public TextMeshProUGUI textoEnBoton3;         // Texto dentro del Botón 3 (C).

    // MODIFICADO: Ahora el panel final tiene un componente de texto dinámico para mostrar el diagnóstico general al jugador.
    [Header("Pantalla Final (Diagnóstico General para el Jugador)")]
    [Tooltip("Panel que se abre al responder la pregunta 15")]
    public GameObject panelAgradecimientoFinal;

    [Tooltip("NUEVO: Texto dentro de 'panelAgradecimientoFinal' donde el jugador leerá su diagnóstico general")]
    public TextMeshProUGUI textoDiagnosticoJugador;

    [Tooltip("Chulito o insignia de completado junto al botón del Test en el Selector")]
    public GameObject simboloCompletadoPsicologico;

    [Header("Conexión con otros sistemas")]
    public GestorAudio scriptAudio;           // Para reproducir sonidos al interactuar y finalizar.
    public GestorSupervisor scriptSupervisor; // Para enviar el informe detallado y confidencial al Administrador.

    [Header("Lista de las 15 Preguntas (Entrevista y Skills)")]
    public PreguntaPsicologica[] preguntas;   // Lista (Array) de tamaño 15 en el Inspector.
    private int indicePreguntaActual = 0;     // Cuenta en cuál de las 15 preguntas vamos (0 a 14).

    // =========================================================================
    // CONTADORES PSICOLÓGICOS INTERNOS (DETALLES EXCLUSIVOS DEL SUPERVISOR)
    // =========================================================================
    private int puntosIntegridadYEtica = 0;   // Preguntas 1 a 5 (Índices 0 a 4)
    private int puntosPresionYEmpatia = 0;    // Preguntas 6 a 10 (Índices 5 a 9)
    private int puntosSkillsBancarias = 0;    // Preguntas 11 a 15 (Índices 10 a 14)

    // Guarda el registro detallado de qué eligió el candidato en cada una de las 15 preguntas.
    private string bitacoraRespuestas = "";

    void Start()
    {
        if (panelAgradecimientoFinal != null) panelAgradecimientoFinal.SetActive(false);
        if (simboloCompletadoPsicologico != null) simboloCompletadoPsicologico.SetActive(false);
    }

    // =========================================================================
    // MÉTODOS DE FLUJO DEL TEST
    // =========================================================================

    public void IniciarTestPsicologico()
    {
        indicePreguntaActual = 0;
        puntosIntegridadYEtica = 0;
        puntosPresionYEmpatia = 0;
        puntosSkillsBancarias = 0;
        bitacoraRespuestas = "";

        if (panelAgradecimientoFinal != null)
        {
            panelAgradecimientoFinal.SetActive(false);
        }

        CargarPreguntaEnPantalla(0);
    }

    public void CargarPreguntaEnPantalla(int indice)
    {
        indicePreguntaActual = indice;

        if (textoProgresoPregunta != null)
        {
            textoProgresoPregunta.text = "Pregunta " + (indicePreguntaActual + 1) + " de " + preguntas.Length;
        }

        textoEnunciadoPantalla.text = preguntas[indicePreguntaActual].enunciadoPregunta;
        textoEnBoton1.text = preguntas[indicePreguntaActual].textoBoton1;
        textoEnBoton2.text = preguntas[indicePreguntaActual].textoBoton2;
        textoEnBoton3.text = preguntas[indicePreguntaActual].textoBoton3;
    }

    public void SeleccionarOpcionPsicologica(int botonPresionado)
    {
        if (scriptAudio != null)
        {
            scriptAudio.ReproducirClic();
        }

        int puntosObtenidos = 0;
        string letraElegida = "";

        if (botonPresionado == 1)
        {
            puntosObtenidos = preguntas[indicePreguntaActual].puntosBoton1;
            letraElegida = "A";
        }
        else if (botonPresionado == 2)
        {
            puntosObtenidos = preguntas[indicePreguntaActual].puntosBoton2;
            letraElegida = "B";
        }
        else if (botonPresionado == 3)
        {
            puntosObtenidos = preguntas[indicePreguntaActual].puntosBoton3;
            letraElegida = "C";
        }

        if (indicePreguntaActual < 5)
        {
            puntosIntegridadYEtica += puntosObtenidos;
        }
        else if (indicePreguntaActual < 10)
        {
            puntosPresionYEmpatia += puntosObtenidos;
        }
        else
        {
            puntosSkillsBancarias += puntosObtenidos;
        }

        string calificacionDecision = (puntosObtenidos == 2) ? "Óptima" : (puntosObtenidos == 1) ? "Moderada" : "Riesgosa";
        bitacoraRespuestas += "P" + (indicePreguntaActual + 1) + " (" + preguntas[indicePreguntaActual].temaEvaluado + "): Opción " + letraElegida + " [" + calificacionDecision + "]\n";

        indicePreguntaActual++;

        if (indicePreguntaActual < preguntas.Length)
        {
            CargarPreguntaEnPantalla(indicePreguntaActual);
        }
        else
        {
            FinalizarTestYGuardarInforme();
        }
    }

    // =========================================================================
    // MODIFICADO: GENERA EL DIAGNÓSTICO GENERAL (JUGADOR) Y EL DETALLADO (EMPRESA)
    // =========================================================================
    private void FinalizarTestYGuardarInforme()
    {
        // 1. Calculamos los puntajes internos (sobre 30 puntos totales y 10 por cada dimensión).
        int puntajeTotal = puntosIntegridadYEtica + puntosPresionYEmpatia + puntosSkillsBancarias;
        int promedioGeneral = (puntajeTotal * 100) / 30;

        int porcIntegridad = puntosIntegridadYEtica * 10;
        int porcPresion = puntosPresionYEmpatia * 10;
        int porcSkills = puntosSkillsBancarias * 10;

        // Variables separadas: una sencilla para el jugador y otra técnica/completa para el banco.
        string resumenParaElJugador = "";
        string diagnosticoConfidencialEmpresa = "";

        // 2. Evaluamos el perfil para redactar ambos diagnósticos:
        if (promedioGeneral >= 85 && porcIntegridad >= 80)
        {
            // LO QUE VE EL JUGADOR EN PANTALLA (General y orientativo):
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Analítico, Normativo y Resiliente\n\n" +
                "Tus respuestas reflejan una alta afinidad con los valores del sector bancario. " +
                "Destacas por priorizar el cumplimiento de las normas, mantener la calma bajo presión " +
                "y aplicar un criterio técnico acertado en operaciones de oficina.\n\n" +
                "Gracias por responder con total sinceridad. El informe detallado ha sido enviado al área de Selección.";

            // LO QUE VE SOLO EL SUPERVISOR / EMPRESA (Veredicto técnico):
            diagnosticoConfidencialEmpresa =
                "ALTAMENTE APTO - PERFIL EJECUTIVO CONFIABLE Y TÉCNICO\n" +
                "Dictamen para RRHH: Alta integridad ética, autocontrol emocional bajo presión y rigurosidad operativa. Recomendado para contratación.";
        }
        else if (promedioGeneral >= 65)
        {
            // LO QUE VE EL JUGADOR EN PANTALLA:
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Operativo y Adaptable\n\n" +
                "Cuentas con buena disposición para la atención al público y el trabajo en equipo. " +
                "Como oportunidad de mejora, te recomendamos priorizar siempre los protocolos de seguridad " +
                "por encima de la prisa operativa y repasar conceptos financieros básicos.\n\n" +
                "Gracias por responder con total sinceridad. El informe detallado ha sido enviado al área de Selección.";

            // LO QUE VE SOLO EL SUPERVISOR / EMPRESA:
            diagnosticoConfidencialEmpresa =
                "APTO CON ACOMPAÑAMIENTO - PERFIL OPERATIVO ESTÁNDAR\n" +
                "Dictamen para RRHH: Candidato bien intencionado, pero tiende a flexibilizar protocolos por rapidez o requiere nivelación técnica.";
        }
        else
        {
            // LO QUE VE EL JUGADOR EN PANTALLA:
            resumenParaElJugador =
                "DIAGNÓSTICO GENERAL DE TU PERFIL:\n" +
                "Perfil Práctico en Proceso de Fortalecimiento\n\n" +
                "Tus respuestas indican que prefieres soluciones rápidas e informales, lo cual en el entorno bancario " +
                "puede generar riesgos operativos. Te sugerimos fortalecer tu tolerancia a la presión, el apego estricto " +
                "a los reglamentos internos y el análisis numérico antes de asumir cargos de caja.\n\n" +
                "Gracias por responder con total sinceridad. El informe detallado ha sido enviado al área de Selección.";

            // LO QUE VE SOLO EL SUPERVISOR / EMPRESA:
            diagnosticoConfidencialEmpresa =
                "NO APTO PARA SECTOR BANCARIO - RIESGO CONDUCTUAL O TÉCNICO\n" +
                "Dictamen para RRHH: Baja tolerancia a la presión, tendencia a omitir controles de seguridad o vacíos críticos en conocimientos financieros.";
        }

        // 3. Mostramos el panel al jugador con su diagnóstico general (sin porcentajes ni bitácora).
        if (panelAgradecimientoFinal != null)
        {
            panelAgradecimientoFinal.SetActive(true);
        }
        if (textoDiagnosticoJugador != null)
        {
            textoDiagnosticoJugador.text = resumenParaElJugador;
        }
        if (simboloCompletadoPsicologico != null)
        {
            simboloCompletadoPsicologico.SetActive(true);
        }

        // 4. Armamos el desglose técnico exclusivo que se guardará para el Supervisor / Administrador:
        string reporteCompletoSupervisor =
            "RESUMEN POR DIMENSIONES EVALUADAS (EXCLUSIVO EMPRESA):\n" +
            "• Integridad, Ética y Apego a Normas (Reid/16PF): " + porcIntegridad + "%\n" +
            "• Tolerancia a la Presión y Adaptabilidad (Moss/DISC): " + porcPresion + "%\n" +
            "• Skills Bancarias y Agilidad Financiera: " + porcSkills + "%\n\n" +
            "HISTORIAL DETALLADO DE LAS 15 RESPUESTAS:\n" + bitacoraRespuestas;

        // 5. Guardamos el informe completo en el archivo cifrado y en la nube del Supervisor.
        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 3: TEST PSICOLÓGICO Y SKILLS", promedioGeneral, diagnosticoConfidencialEmpresa, reporteCompletoSupervisor);
        }
    }
}
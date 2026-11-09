// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos TextMeshPro para actualizar el número de pregunta, el enunciado y los 3 botones en pantalla.
using TMPro;

// =========================================================================
// PLANTILLA DE DATOS SERIALIZABLE: PreguntaPsicologica
// =========================================================================
[System.Serializable]
public class PreguntaPsicologica
{
    [Tooltip("Ejemplo: 'P1: Manejo de sobrante en caja'")]
    public string temaEvaluado; // Guía corta para el reporte del Supervisor.

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

    [Header("Pantalla Final (Sin Estadísticas para el Jugador)")]
    [Tooltip("Panel que solo da las gracias por responder con sinceridad")]
    public GameObject panelAgradecimientoFinal;

    [Tooltip("Opcional: Chulito o insignia de completado junto al botón del Test en el Selector")]
    public GameObject simboloCompletadoPsicologico;

    [Header("Conexión con otros sistemas")]
    public GestorAudio scriptAudio;           // Para reproducir un clic suave al pasar de pregunta.
    public GestorSupervisor scriptSupervisor; // Para enviar el diagnóstico secreto al Administrador.

    [Header("Lista de las 15 Preguntas (Entrevista y Skills)")]
    public PreguntaPsicologica[] preguntas;   // Lista (Array) de tamaño 15 en el Inspector.
    private int indicePreguntaActual = 0;     // Cuenta en cuál de las 15 preguntas vamos (0 a 14).

    // =========================================================================
    // CONTADORES PSICOLÓGICOS INTERNOS (SOLO LOS VERÁ EL SUPERVISOR)
    // =========================================================================
    // Dividimos las 15 preguntas en 3 dimensiones de 5 preguntas cada una (máximo 10 puntos por dimensión):
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

    // Se llama desde GestorMenuPrincipal cuando el jugador presiona "Comenzar Evaluación" en el tutorial.
    public void IniciarTestPsicologico()
    {
        // Reiniciamos todos los contadores a cero para evaluar limpiamente al jugador actual.
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

    // Actualiza los textos de la interfaz con la pregunta actual.
    public void CargarPreguntaEnPantalla(int indice)
    {
        indicePreguntaActual = indice;

        // Mostramos el contador arriba (ej. "Pregunta 1 de 15").
        if (textoProgresoPregunta != null)
        {
            textoProgresoPregunta.text = "Pregunta " + (indicePreguntaActual + 1) + " de " + preguntas.Length;
        }

        // Copiamos el enunciado y las 3 opciones desde el Inspector hacia la pantalla.
        textoEnunciadoPantalla.text = preguntas[indicePreguntaActual].enunciadoPregunta;
        textoEnBoton1.text = preguntas[indicePreguntaActual].textoBoton1;
        textoEnBoton2.text = preguntas[indicePreguntaActual].textoBoton2;
        textoEnBoton3.text = preguntas[indicePreguntaActual].textoBoton3;
    }

    // Se conecta a los 3 botones de respuesta enviando 1 (Botón 1), 2 (Botón 2) o 3 (Botón 3).
    public void SeleccionarOpcionPsicologica(int botonPresionado)
    {
        // Reproducimos un sonido de clic neutro (no usamos sonido de error ni de acierto para no condicionar al jugador).
        if (scriptAudio != null)
        {
            scriptAudio.ReproducirClic();
        }

        // 1. Miramos cuántos puntos secretos vale el botón que acaba de elegir (2, 1 o 0).
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

        // 2. Sumamos los puntos a la dimensión correspondiente según el número de pregunta:
        if (indicePreguntaActual < 5)
        {
            // Preguntas 1 a 5: Evalúan Integridad y Apego a Normas.
            puntosIntegridadYEtica += puntosObtenidos;
        }
        else if (indicePreguntaActual < 10)
        {
            // Preguntas 6 a 10: Evalúan Tolerancia a la Presión y Trabajo en Equipo.
            puntosPresionYEmpatia += puntosObtenidos;
        }
        else
        {
            // Preguntas 11 a 15: Evalúan Conocimientos Técnicos y Agilidad Numérica (Skills).
            puntosSkillsBancarias += puntosObtenidos;
        }

        // 3. Anotamos su elección en la bitácora secreta para el Supervisor.
        string calificacionDecision = (puntosObtenidos == 2) ? "Óptima" : (puntosObtenidos == 1) ? "Moderada" : "Riesgosa";
        bitacoraRespuestas += "P" + (indicePreguntaActual + 1) + " (" + preguntas[indicePreguntaActual].temaEvaluado + "): Opción " + letraElegida + " [" + calificacionDecision + "]\n";

        // 4. Avanzamos inmediatamente a la siguiente pregunta.
        indicePreguntaActual++;

        if (indicePreguntaActual < preguntas.Length)
        {
            CargarPreguntaEnPantalla(indicePreguntaActual);
        }
        else
        {
            // Si ya respondió las 15 preguntas, cerramos el test y generamos el informe secreto.
            FinalizarTestYGuardarInforme();
        }
    }

    // =========================================================================
    // CÁLCULO DEL DIAGNÓSTICO PSICOLÓGICO SECRETO PARA EL SUPERVISOR
    // =========================================================================
    private void FinalizarTestYGuardarInforme()
    {
        // 1. Al jugador únicamente le mostramos la ventana de agradecimiento y encendemos su insignia.
        if (panelAgradecimientoFinal != null)
        {
            panelAgradecimientoFinal.SetActive(true);
        }
        if (simboloCompletadoPsicologico != null)
        {
            simboloCompletadoPsicologico.SetActive(true);
        }

        // 2. Calculamos los porcentajes internos para el Supervisor:
        // El puntaje máximo posible en las 15 preguntas es 30 puntos (15 preguntas x 2 puntos).
        int puntajeTotal = puntosIntegridadYEtica + puntosPresionYEmpatia + puntosSkillsBancarias;
        int promedioGeneral = (puntajeTotal * 100) / 30;

        // Cada dimensión tiene 5 preguntas (máximo 10 puntos -> multiplicamos por 10 para tener porcentaje de 0% a 100%).
        int porcIntegridad = puntosIntegridadYEtica * 10;
        int porcPresion = puntosPresionYEmpatia * 10;
        int porcSkills = puntosSkillsBancarias * 10;

        // 3. Determinamos el Perfil Psicológico Laboral y de Aptitud según el resultado:
        string diagnosticoPsicologico = "";

        if (promedioGeneral >= 85 && porcIntegridad >= 80)
        {
            diagnosticoPsicologico =
                "ALTAMENTE APTO - PERFIL EJECUTIVO CONFIABLE Y TÉCNICO\n" +
                "Perfil Psicológico: Alta integridad ética, autocontrol emocional bajo presión y rigurosidad operativa. Recomendado para contratación inmediata.";
        }
        else if (promedioGeneral >= 65)
        {
            diagnosticoPsicologico =
                "APTO CON ACOMPAÑAMIENTO - PERFIL OPERATIVO ESTÁNDAR\n" +
                "Perfil Psicológico: Candidato adaptable y bien intencionado, pero tiende a flexibilizar protocolos por rapidez o requiere nivelación técnica.";
        }
        else
        {
            diagnosticoPsicologico =
                "NO APTO PARA SECTOR BANCARIO - RIESGO CONDUCTUAL O TÉCNICO\n" +
                "Perfil Psicológico: Baja tolerancia a la presión, tendencia a omitir controles de seguridad o vacíos críticos en conocimientos financieros.";
        }

        // 4. Armamos el reporte detallado con las 3 dimensiones y las 15 respuestas:
        string reporteCompletoSupervisor =
            "RESUMEN POR DIMENSIONES EVALUADAS:\n" +
            "• Integridad, Ética y Apego a Normas (Reid/16PF): " + porcIntegridad + "%\n" +
            "• Tolerancia a la Presión y Adaptabilidad (Moss/DISC): " + porcPresion + "%\n" +
            "• Skills Bancarias y Agilidad Financiera: " + porcSkills + "%\n\n" +
            "HISTORIAL DE LAS 15 RESPUESTAS:\n" + bitacoraRespuestas;

        if (scriptSupervisor != null)
        {
            scriptSupervisor.GuardarEvaluacionJugador("MÓDULO 3: TEST PSICOLÓGICO Y SKILLS", promedioGeneral, diagnosticoPsicologico, reporteCompletoSupervisor);
        }
    }
}
// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos TextMeshPro para leer las cajas de texto interactivas (TMP_InputField) y mostrar el historial.
using TMPro;
// 'System.IO' permite crear, escribir y leer archivos en el disco duro del computador.
using System.IO;

public class GestorSupervisor : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: REGISTRO DEL JUGADOR ACTUAL (EN EL SELECTOR DE MÓDULOS)
    // =========================================================================
    [Header("Registro del Aspirante / Empleado")]
    [Tooltip("Caja de texto (InputField) en el Selector donde el jugador escribe su nombre antes de jugar")]
    public TMP_InputField inputNombreJugador;

    // =========================================================================
    // SECCIÓN 2: VENTANAS Y CLAVE DEL SUPERVISOR
    // =========================================================================
    [Header("Panel de Contraseña (Login Supervisor)")]
    public GameObject panelLoginSupervisor;
    public TMP_InputField inputClaveSupervisor;
    public TextMeshProUGUI textoErrorClave;

    [Tooltip("Contraseña que deberá escribir el supervisor para ver los resultados")]
    public string claveSecreta = "Nexo2026";

    [Header("Panel de Resultados del Supervisor")]
    public GameObject panelHistorialSupervisor;
    public TextMeshProUGUI textoPantallaHistorial;

    [Header("Conexión con Sonidos")]
    public GestorAudio scriptAudio;

    private string rutaArchivoHistorial;

    void Start()
    {
        rutaArchivoHistorial = Application.persistentDataPath + "/Historial_Evaluaciones_NexoBank.txt";

        if (panelLoginSupervisor != null) panelLoginSupervisor.SetActive(false);
        if (panelHistorialSupervisor != null) panelHistorialSupervisor.SetActive(false);
        if (textoErrorClave != null) textoErrorClave.text = "";
    }

    // =========================================================================
    // SECCIÓN 3: GUARDADO AUTOMÁTICO PARA CUALQUIERA DE LOS 3 MÓDULOS
    // =========================================================================
    // NUEVO (PUNTO 6): Ahora recibe 'string nombreModulo' para identificar si el reporte viene de
    // Servicio al Cliente, Detector de Phishing o Test Psicológico.
    public void GuardarEvaluacionJugador(string nombreModulo, int promedio, string diagnostico, string detalleCasos)
    {
        // 1. Leemos el nombre que el jugador escribió en el Selector de Módulos.
        string nombreEmpleado = "Aspirante Sin Nombre";
        if (inputNombreJugador != null && !string.IsNullOrEmpty(inputNombreJugador.text))
        {
            nombreEmpleado = inputNombreJugador.text;
        }

        // 2. Capturamos la fecha y hora exacta en que terminó este módulo.
        string fechaActual = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // 3. Armamos la ficha completa indicando el nombre del empleado y cuál de los 3 módulos completó.
        string bloqueRegistro =
            "========================================================\n" +
            "EMPLEADO / ASPIRANTE: " + nombreEmpleado.ToUpper() + "\n" +
            "FECHA: " + fechaActual + " | MÓDULO: " + nombreModulo + "\n" +
            "PROMEDIO / PUNTAJE: " + promedio + "%\n" +
            "DIAGNÓSTICO:\n" + diagnostico + "\n\n" +
            "DESGLOSE DETALLADO:\n" + detalleCasos +
            "========================================================\n\n";

        // 4. Guardamos inmediatamente en el archivo sin borrar los módulos anteriores.
        File.AppendAllText(rutaArchivoHistorial, bloqueRegistro);

        Debug.Log("Módulo [" + nombreModulo + "] guardado para: " + nombreEmpleado);
    }

    // =========================================================================
    // SECCIÓN 4: MÉTODOS DE ACCESO DEL SUPERVISOR Y EXPORTACIÓN A PDF
    // =========================================================================
    public void BotonAbrirLoginSupervisor()
    {
        inputClaveSupervisor.text = "";
        if (textoErrorClave != null) textoErrorClave.text = "";
        panelLoginSupervisor.SetActive(true);
    }

    public void BotonVerificarContraseña()
    {
        if (inputClaveSupervisor.text == claveSecreta)
        {
            if (scriptAudio != null) scriptAudio.ReproducirAcierto();

            panelLoginSupervisor.SetActive(false);
            panelHistorialSupervisor.SetActive(true);
            MostrarHistorialEnPantalla();
        }
        else
        {
            if (scriptAudio != null) scriptAudio.ReproducirError();
            if (textoErrorClave != null)
            {
                textoErrorClave.text = "Acceso denegado: Contraseña de supervisor incorrecta.";
            }
        }
    }

    private void MostrarHistorialEnPantalla()
    {
        if (File.Exists(rutaArchivoHistorial))
        {
            textoPantallaHistorial.text = File.ReadAllText(rutaArchivoHistorial);
        }
        else
        {
            textoPantallaHistorial.text = "Aún no hay evaluaciones registradas en el sistema de NexoBank.";
        }
    }

    public void BotonBorrarHistorial()
    {
        if (File.Exists(rutaArchivoHistorial))
        {
            File.Delete(rutaArchivoHistorial);
        }
        textoPantallaHistorial.text = "Historial eliminado correctamente.";
    }

    public void BotonCerrarPanelesSupervisor()
    {
        panelLoginSupervisor.SetActive(false);
        panelHistorialSupervisor.SetActive(false);
    }

    public void BotonExportarInformePDF()
    {
        if (!File.Exists(rutaArchivoHistorial)) return;

        string contenidoHistorial = File.ReadAllText(rutaArchivoHistorial);
        string rutaInformeHTML = Application.persistentDataPath + "/Informe_Oficial_NexoBank.html";

        string plantillaInforme =
            "<html><head><meta charset='UTF-8'><title>Informe de Resultados - NexoBank</title>" +
            "<style>" +
            "body { font-family: Arial, sans-serif; background-color: #F4F9FD; color: #0A2540; padding: 40px; }" +
            ".contenedor { background: white; padding: 30px; border-radius: 10px; border-top: 8px solid #0096D6; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }" +
            "h1 { color: #0A2540; margin-bottom: 5px; }" +
            "h3 { color: #0096D6; margin-top: 0; }" +
            "pre { background: #F4F9FD; padding: 20px; border-left: 4px solid #0096D6; font-size: 14px; white-space: pre-wrap; }" +
            "button { background: #0096D6; color: white; border: none; padding: 12px 24px; font-size: 16px; border-radius: 5px; cursor: pointer; margin-bottom: 20px; }" +
            "@media print { button { display: none; } body { background: white; padding: 0; } }" +
            "</style></head><body>" +
            "<div class='contenedor'>" +
            "<button onclick='window.print()'>Descargar / Guardar como PDF</button>" +
            "<h1>NEXOBANK - DEPARTAMENTO DE AUDITORÍA Y SELECCIÓN</h1>" +
            "<h3>Informe Consolidado de Evaluaciones (Servicio, Phishing y Psicotécnico)</h3>" +
            "<p>Fecha de generación del reporte: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>" +
            "<pre>" + contenidoHistorial + "</pre>" +
            "</div></body></html>";

        File.WriteAllText(rutaInformeHTML, plantillaInforme);
        Application.OpenURL(rutaInformeHTML);
    }
}
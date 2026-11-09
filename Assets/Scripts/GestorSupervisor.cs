// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos TextMeshPro para leer las cajas de texto interactivas (TMP_InputField) y mostrar el historial.
using TMPro;
// 'System.IO' permite crear, escribir y leer el archivo de respaldo local en el disco duro.
using System.IO;
// 'System' y 'System.Text' permiten cifrar el archivo local para que el jugador no pueda abrirlo en Windows.
using System;
using System.Text;
// 'System.Collections' nos permite usar Corrutinas (IEnumerator) para esperar las respuestas de Internet sin congelar el juego.
using System.Collections;
// NUEVO (PUNTO 8): 'UnityEngine.Networking' es la librería oficial de Unity para conectarse a Internet
// y enviar o descargar datos desde un servidor en la nube (UnityWebRequest).
using UnityEngine.Networking;

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

    // =========================================================================
    // NUEVO (PUNTO 8): CONEXIÓN CON LA NUBE PARA ACCEDER DESDE OTRO COMPUTADOR
    // =========================================================================
    [Header("Servidor en la Nube (Acceso desde otro PC)")]
    [Tooltip("Pega aquí el enlace (URL) de tu Google Apps Script para guardar y leer los datos por Internet")]
    public string urlServidorNube = "";

    [Tooltip("Opcional: Si los PCs están en la misma red local o usan OneDrive/Google Drive Desktop, puedes poner aquí la ruta de esa carpeta compartida")]
    public string carpetaRedCompartidaOpcional = "";

    [Header("Conexión con Sonidos")]
    public GestorAudio scriptAudio;

    private string rutaArchivoHistorial;
    private readonly string llaveSeguridadInterna = "ClaveCifrado_NexoBank_SENA_2026";

    void Start()
    {
        // Si el administrador escribió una carpeta compartida en red/OneDrive y existe en este PC, la usa.
        // De lo contrario, usa la carpeta segura por defecto del sistema operativo.
        if (!string.IsNullOrEmpty(carpetaRedCompartidaOpcional) && Directory.Exists(carpetaRedCompartidaOpcional))
        {
            rutaArchivoHistorial = Path.Combine(carpetaRedCompartidaOpcional, "Historial_Protegido_NexoBank.dat");
        }
        else
        {
            rutaArchivoHistorial = Application.persistentDataPath + "/Historial_Protegido_NexoBank.dat";
        }

        if (panelLoginSupervisor != null) panelLoginSupervisor.SetActive(false);
        if (panelHistorialSupervisor != null) panelHistorialSupervisor.SetActive(false);
        if (textoErrorClave != null) textoErrorClave.text = "";
    }

    // =========================================================================
    // SECCIÓN 3: GUARDADO AUTOMÁTICO (DISCO CIFRADO + ENVÍO A LA NUBE)
    // =========================================================================
    public void GuardarEvaluacionJugador(string nombreModulo, int promedio, string diagnostico, string detalleCasos)
    {
        string nombreEmpleado = "Aspirante Sin Nombre";
        if (inputNombreJugador != null && !string.IsNullOrEmpty(inputNombreJugador.text))
        {
            nombreEmpleado = inputNombreJugador.text;
        }

        string fechaActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        string bloqueRegistro =
            "--------------------------------------------------------\n" +
            "EMPLEADO / ASPIRANTE: " + nombreEmpleado.ToUpper() + "\n" +
            "FECHA: " + fechaActual + " | MÓDULO: " + nombreModulo + "\n" +
            "PROMEDIO / PUNTAJE: " + promedio + "%\n" +
            "DIAGNÓSTICO:\n" + diagnostico + "\n\n" +
            "DESGLOSE DETALLADO:\n" + detalleCasos +
            "--------------------------------------------------------\n\n";

        // 1. GUARDADO LOCAL CIFRADO (Punto 7):
        string historialAnteriorLegible = LeerYDescifrarArchivoLocal();
        string historialActualizado = historialAnteriorLegible + bloqueRegistro;
        string archivoBlindado = CifrarTexto(historialActualizado);
        File.WriteAllText(rutaArchivoHistorial, archivoBlindado);

        Debug.Log("Respaldo local cifrado guardado en: " + rutaArchivoHistorial);

        // 2. ENVÍO A LA NUBE POR INTERNET (Punto 8):
        // Si pegaste el enlace de tu servidor en el Inspector, enviamos el reporte por Internet
        // para que el Supervisor pueda verlo desde cualquier otro computador.
        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaEnviarReporteNube(fechaActual, nombreEmpleado, nombreModulo, promedio + "%", diagnostico, bloqueRegistro));
        }
    }

    // =========================================================================
    // NUEVO (PUNTO 8): CORRUTINAS DE INTERNET (SUBIR Y DESCARGAR DE LA NUBE)
    // =========================================================================

    // Sube los resultados del jugador actual al servidor en la nube (Google Sheets).
    private IEnumerator CorrutinaEnviarReporteNube(string fecha, string empleado, string modulo, string puntaje, string diagnostico, string bloqueCompleto)
    {
        // 'WWWForm' empaqueta los datos como un formulario web seguro para enviarlos por Internet.
        WWWForm paqueteDatos = new WWWForm();
        paqueteDatos.AddField("accion", "guardar");
        paqueteDatos.AddField("fecha", fecha);
        paqueteDatos.AddField("empleado", empleado);
        paqueteDatos.AddField("modulo", modulo);
        paqueteDatos.AddField("puntaje", puntaje);
        paqueteDatos.AddField("diagnostico", diagnostico);
        paqueteDatos.AddField("bloqueCompleto", bloqueCompleto);

        // 'UnityWebRequest.Post' envía el paquete a la dirección URL de la nube.
        using (UnityWebRequest peticionWeb = UnityWebRequest.Post(urlServidorNube, paqueteDatos))
        {
            // Esperamos unos segundos mientras viaja por Internet sin congelar la pantalla del juego.
            yield return peticionWeb.SendWebRequest();

            if (peticionWeb.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("¡Reporte sincronizado exitosamente en la Nube para consulta en otros PCs!");
            }
            else
            {
                Debug.LogWarning("No se pudo conectar a la nube (quedó guardado en el respaldo local): " + peticionWeb.error);
            }
        }
    }

    // Descarga todos los historiales desde la nube cuando el Supervisor abre el panel en cualquier computador.
    private IEnumerator CorrutinaDescargarHistorialNube()
    {
        textoPantallaHistorial.text = "Conectando con el servidor central de NexoBank...\nDescargando evaluaciones de todos los equipos...";

        using (UnityWebRequest peticionWeb = UnityWebRequest.Get(urlServidorNube))
        {
            yield return peticionWeb.SendWebRequest();

            // Si hay internet y el servidor responde correctamente, mostramos los datos de todos los computadores:
            if (peticionWeb.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(peticionWeb.downloadHandler.text))
            {
                textoPantallaHistorial.text = peticionWeb.downloadHandler.text;
            }
            else
            {
                // Si en ese momento no hay internet, cargamos automáticamente el respaldo cifrado de este computador.
                MostrarHistorialLocalEnPantalla();
            }
        }
    }

    // =========================================================================
    // ALGORITMO DE CIFRADO Y DESCIFRADO LOCAL (PUNTO 7)
    // =========================================================================
    private string CifrarTexto(string textoNormal)
    {
        byte[] bytesTexto = Encoding.UTF8.GetBytes(textoNormal);
        byte[] bytesLlave = Encoding.UTF8.GetBytes(llaveSeguridadInterna);

        for (int i = 0; i < bytesTexto.Length; i++)
        {
            bytesTexto[i] = (byte)(bytesTexto[i] ^ bytesLlave[i % bytesLlave.Length]);
        }
        return Convert.ToBase64String(bytesTexto);
    }

    private string LeerYDescifrarArchivoLocal()
    {
        if (!File.Exists(rutaArchivoHistorial)) return "";

        try
        {
            string textoCifradoEnDisco = File.ReadAllText(rutaArchivoHistorial);
            byte[] bytesCifrados = Convert.FromBase64String(textoCifradoEnDisco);
            byte[] bytesLlave = Encoding.UTF8.GetBytes(llaveSeguridadInterna);

            for (int i = 0; i < bytesCifrados.Length; i++)
            {
                bytesCifrados[i] = (byte)(bytesCifrados[i] ^ bytesLlave[i % bytesLlave.Length]);
            }
            return Encoding.UTF8.GetString(bytesCifrados);
        }
        catch
        {
            return "ALERTA DE AUDITORÍA: El archivo local fue alterado externamente.\n\n";
        }
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

            // NUEVO (PUNTO 8): Si tenemos configurada la URL de la nube, descargamos los resultados de todos los PCs.
            // Si la casilla está vacía, mostramos los resultados guardados en este computador.
            if (!string.IsNullOrEmpty(urlServidorNube))
            {
                StartCoroutine(CorrutinaDescargarHistorialNube());
            }
            else
            {
                MostrarHistorialLocalEnPantalla();
            }
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

    private void MostrarHistorialLocalEnPantalla()
    {
        string contenidoLegible = LeerYDescifrarArchivoLocal();
        if (!string.IsNullOrEmpty(contenidoLegible))
        {
            textoPantallaHistorial.text = contenidoLegible;
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

        // Si está conectado a la nube, también envía la orden de limpiar la hoja remota.
        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            WWWForm formBorrar = new WWWForm();
            formBorrar.AddField("accion", "borrar");
            UnityWebRequest.Post(urlServidorNube, formBorrar).SendWebRequest();
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
        // Tomamos el texto que esté cargado actualmente en la pantalla del Supervisor (incluyendo lo descargado de otros PCs).
        string contenidoHistorial = textoPantallaHistorial.text;
        if (string.IsNullOrEmpty(contenidoHistorial)) return;

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
            "<h3>Informe Consolidado Multi-Equipo (Servicio, Phishing y Psicotécnico)</h3>" +
            "<p>Fecha de generación del reporte: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>" +
            "<pre>" + contenidoHistorial + "</pre>" +
            "</div></body></html>";

        File.WriteAllText(rutaInformeHTML, plantillaInforme);
        Application.OpenURL(rutaInformeHTML);
    }
}
using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Text;
using System.Collections;
using UnityEngine.Networking;

public class GestorSupervisor : MonoBehaviour
{
    [Header("Registro del Colaborador")]
    public TMP_InputField inputNombreJugador;

    [Header("Panel de Contraseña (Login Supervisor)")]
    public GameObject panelLoginSupervisor;
    public TMP_InputField inputClaveSupervisor;
    public TextMeshProUGUI textoErrorClave;
    public string claveSecreta = "Nexo2026";

    [Header("Cambiar Contraseña Global")]
    public TMP_InputField inputNuevaClaveGlobal;
    public TextMeshProUGUI textoConfirmacionClave;

    [Header("Panel de Resultados del Supervisor")]
    public GameObject panelHistorialSupervisor;
    public TextMeshProUGUI textoPantallaHistorial;

    [Header("Servidor en la Nube y Excel")]
    public string urlServidorNube = "";
    public string urlDescargaExcelNube = "";
    public string carpetaRedCompartidaOpcional = "";

    [Header("Conexión con Sonidos")]
    public GestorAudio scriptAudio;

    private string rutaArchivoHistorial;
    private string rutaArchivoCSV;
    private readonly string llaveSeguridadInterna = "ClaveCifrado_NexoBank_SENA_2026";

    void Start()
    {
#if !UNITY_WEBGL
        if (!string.IsNullOrEmpty(carpetaRedCompartidaOpcional) && Directory.Exists(carpetaRedCompartidaOpcional))
        {
            rutaArchivoHistorial = Path.Combine(carpetaRedCompartidaOpcional, "Historial_Protegido_NexoBank.dat");
            rutaArchivoCSV = Path.Combine(carpetaRedCompartidaOpcional, "Resultados_NexoBank.csv");
        }
        else
        {
            rutaArchivoHistorial = Application.persistentDataPath + "/Historial_Protegido_NexoBank.dat";
            rutaArchivoCSV = Application.persistentDataPath + "/Resultados_NexoBank.csv";
        }
#endif

        if (panelLoginSupervisor != null) panelLoginSupervisor.SetActive(false);
        if (panelHistorialSupervisor != null) panelHistorialSupervisor.SetActive(false);
        if (textoErrorClave != null) textoErrorClave.text = "";
        if (textoConfirmacionClave != null) textoConfirmacionClave.text = "";

        if (PlayerPrefs.HasKey("ClaveSupervisorCifrada"))
        {
            claveSecreta = DescifrarTexto(PlayerPrefs.GetString("ClaveSupervisorCifrada"));
        }

        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaSincronizarClaveGlobal());
        }
    }

    private IEnumerator CorrutinaSincronizarClaveGlobal()
    {
        using (UnityWebRequest peticion = UnityWebRequest.Get(urlServidorNube + "?accion=obtenerClave"))
        {
            yield return peticion.SendWebRequest();

            if (peticion.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(peticion.downloadHandler.text))
            {
                string claveDescargada = peticion.downloadHandler.text.Trim();
                if (claveDescargada.Length >= 4)
                {
                    claveSecreta = claveDescargada;
                    PlayerPrefs.SetString("ClaveSupervisorCifrada", CifrarTexto(claveSecreta));
                    PlayerPrefs.Save();
                }
            }
        }
    }

    public void BotonCambiarContraseñaGlobal()
    {
        if (inputNuevaClaveGlobal == null || string.IsNullOrEmpty(inputNuevaClaveGlobal.text))
        {
            if (textoConfirmacionClave != null)
                textoConfirmacionClave.text = "Escribe una contraseña válida.";
            return;
        }

        claveSecreta = inputNuevaClaveGlobal.text.Trim();
        PlayerPrefs.SetString("ClaveSupervisorCifrada", CifrarTexto(claveSecreta));
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaSubirNuevaClaveGlobal(claveSecreta));
        }
        else if (textoConfirmacionClave != null)
        {
            textoConfirmacionClave.text = "¡Contraseña actualizada localmente en este equipo!";
        }

        inputNuevaClaveGlobal.text = "";
    }

    private IEnumerator CorrutinaSubirNuevaClaveGlobal(string nuevaClave)
    {
        if (textoConfirmacionClave != null)
            textoConfirmacionClave.text = "Actualizando contraseña en todos los equipos...";

        WWWForm paquete = new WWWForm();
        paquete.AddField("accion", "cambiarClave");
        paquete.AddField("nuevaClave", nuevaClave);

        using (UnityWebRequest peticion = UnityWebRequest.Post(urlServidorNube, paquete))
        {
            yield return peticion.SendWebRequest();

            if (peticion.result == UnityWebRequest.Result.Success)
            {
                if (scriptAudio != null) scriptAudio.ReproducirAcierto();
                if (textoConfirmacionClave != null)
                    textoConfirmacionClave.text = "¡Contraseña global actualizada en todos los equipos!";
            }
            else if (textoConfirmacionClave != null)
            {
                textoConfirmacionClave.text = "Actualizada en este equipo (sin conexión a la nube).";
            }
        }
    }

    public void GuardarEvaluacionJugador(string nombreModulo, int promedio, string diagnostico, string detalleCasos)
    {
        string nombreEmpleado = "Colaborador Sin Registro";
        if (inputNombreJugador != null && !string.IsNullOrEmpty(inputNombreJugador.text))
        {
            nombreEmpleado = inputNombreJugador.text.Trim();
        }

        string fechaActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        string bloqueRegistro =
            " ========================================================\n" +
            "COLABORADOR: " + nombreEmpleado.ToUpper() + "\n" +
            "FECHA: " + fechaActual + " | EVALUACIÓN: " + nombreModulo + "\n" +
            "EFECTIVIDAD: " + promedio + "%\n" +
            "DIAGNÓSTICO:\n" + diagnostico + "\n\n" +
            "DESGLOSE POR CASO:\n" + detalleCasos +
            " ========================================================\n\n";

#if !UNITY_WEBGL
        string historialAnteriorLegible = LeerYDescifrarArchivoLocal();
        string historialActualizado = historialAnteriorLegible + bloqueRegistro;
        File.WriteAllText(rutaArchivoHistorial, CifrarTexto(historialActualizado));

        UTF8Encoding utf8ConTildes = new UTF8Encoding(true);
        if (!File.Exists(rutaArchivoCSV))
        {
            File.WriteAllText(rutaArchivoCSV, "Fecha;Colaborador;Evaluacion;Efectividad;Diagnostico\n", utf8ConTildes);
        }

        string empleadoLimpio = nombreEmpleado.Replace(";", ",");
        string diagnosticoLimpio = diagnostico.Replace(";", ",").Replace("\n", " ");
        string filaExcel = fechaActual + ";" + empleadoLimpio + ";" + nombreModulo + ";" + promedio + "%;" + diagnosticoLimpio + "\n";
        File.AppendAllText(rutaArchivoCSV, filaExcel, utf8ConTildes);
#endif

        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaEnviarReporteNube(fechaActual, nombreEmpleado, nombreModulo, promedio + "%", diagnostico, bloqueRegistro));
        }
    }

    private IEnumerator CorrutinaEnviarReporteNube(string fecha, string empleado, string modulo, string puntaje, string diagnostico, string bloqueCompleto)
    {
        WWWForm paqueteDatos = new WWWForm();
        paqueteDatos.AddField("accion", "guardar");
        paqueteDatos.AddField("fecha", fecha);
        paqueteDatos.AddField("empleado", empleado);
        paqueteDatos.AddField("modulo", modulo);
        paqueteDatos.AddField("puntaje", puntaje);
        paqueteDatos.AddField("diagnostico", diagnostico);
        paqueteDatos.AddField("bloqueCompleto", bloqueCompleto);

        using (UnityWebRequest peticionWeb = UnityWebRequest.Post(urlServidorNube, paqueteDatos))
        {
            yield return peticionWeb.SendWebRequest();
        }
    }

    private IEnumerator CorrutinaDescargarHistorialNube()
    {
        textoPantallaHistorial.text = "Conectando con el servidor de NexoBank...\nDescargando evaluaciones...";

        using (UnityWebRequest peticionWeb = UnityWebRequest.Get(urlServidorNube + "?accion=historial"))
        {
            yield return peticionWeb.SendWebRequest();

            if (peticionWeb.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(peticionWeb.downloadHandler.text))
            {
                textoPantallaHistorial.text = peticionWeb.downloadHandler.text;
            }
            else
            {
                MostrarHistorialLocalEnPantalla();
            }
        }
    }

    private IEnumerator CorrutinaBorrarHistorialNube()
    {
        WWWForm formBorrar = new WWWForm();
        formBorrar.AddField("accion", "borrar");

        using (UnityWebRequest peticionWeb = UnityWebRequest.Post(urlServidorNube, formBorrar))
        {
            yield return peticionWeb.SendWebRequest();
        }
    }

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

    private string DescifrarTexto(string textoEnBase64)
    {
        try
        {
            byte[] bytesCifrados = Convert.FromBase64String(textoEnBase64);
            byte[] bytesLlave = Encoding.UTF8.GetBytes(llaveSeguridadInterna);

            for (int i = 0; i < bytesCifrados.Length; i++)
            {
                bytesCifrados[i] = (byte)(bytesCifrados[i] ^ bytesLlave[i % bytesLlave.Length]);
            }
            return Encoding.UTF8.GetString(bytesCifrados);
        }
        catch
        {
            return "Nexo2026";
        }
    }

    private string LeerYDescifrarArchivoLocal()
    {
#if !UNITY_WEBGL
        if (!File.Exists(rutaArchivoHistorial)) return "";
        return DescifrarTexto(File.ReadAllText(rutaArchivoHistorial));
#else
        return "";
#endif
    }

    public void BotonAbrirLoginSupervisor()
    {
        inputClaveSupervisor.text = "";
        if (textoErrorClave != null) textoErrorClave.text = "";
        panelLoginSupervisor.SetActive(true);

        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaSincronizarClaveGlobal());
        }
    }

    public void BotonVerificarContraseña()
    {
        if (inputClaveSupervisor.text == claveSecreta)
        {
            if (scriptAudio != null) scriptAudio.ReproducirAcierto();

            inputClaveSupervisor.text = "";
            panelLoginSupervisor.SetActive(false);
            panelHistorialSupervisor.SetActive(true);

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
        textoPantallaHistorial.text = !string.IsNullOrEmpty(contenidoLegible)
            ? contenidoLegible
            : "Aún no hay evaluaciones registradas en el sistema de NexoBank.";
    }

    public void BotonBorrarHistorial()
    {
#if !UNITY_WEBGL
        if (File.Exists(rutaArchivoHistorial)) File.Delete(rutaArchivoHistorial);
        if (File.Exists(rutaArchivoCSV)) File.Delete(rutaArchivoCSV);
#endif

        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaBorrarHistorialNube());
        }
        textoPantallaHistorial.text = "Historial eliminado correctamente.";
    }

    public void BotonCerrarPanelesSupervisor()
    {
        inputClaveSupervisor.text = "";
        panelLoginSupervisor.SetActive(false);
        panelHistorialSupervisor.SetActive(false);
    }

    public void BotonDescargarExcelCSV()
    {
#if UNITY_WEBGL
        if (!string.IsNullOrEmpty(urlDescargaExcelNube))
        {
            Application.OpenURL(urlDescargaExcelNube);
        }
#else
        if (File.Exists(rutaArchivoCSV))
        {
            Application.OpenURL(rutaArchivoCSV);
        }
        else if (!string.IsNullOrEmpty(urlDescargaExcelNube))
        {
            Application.OpenURL(urlDescargaExcelNube);
        }
#endif
    }

    public void BotonExportarInformePDF()
    {
#if !UNITY_WEBGL
        string contenidoHistorial = textoPantallaHistorial.text;
        if (string.IsNullOrEmpty(contenidoHistorial)) return;

        string rutaInformeHTML = Application.persistentDataPath + "/Informe_Oficial_NexoBank.html";
        string plantillaInforme =
            "<html><head><meta charset='UTF-8'><title>Informe de Resultados - NexoBank</title>" +
            "<style>body { font-family: Arial, sans-serif; background-color: #F4F9FD; color: #0A2540; padding: 40px; }" +
            ".contenedor { background: white; padding: 30px; border-radius: 10px; border-top: 8px solid #0096D6; }" +
            "pre { background: #F4F9FD; padding: 20px; border-left: 4px solid #0096D6; font-size: 14px; white-space: pre-wrap; }" +
            "button { background: #0096D6; color: white; border: none; padding: 12px 24px; font-size: 16px; border-radius: 5px; cursor: pointer; }" +
            "@media print { button { display: none; } }</style></head><body><div class='contenedor'>" +
            "<button onclick='window.print()'>Descargar / Guardar como PDF</button>" +
            "<h1>NEXOBANK - REPORTE DE INDUCCIÓN Y CUMPLIMIENTO</h1>" +
            "<h3>Informe Consolidado del Simulador Interactivo</h3>" +
            "<p>Fecha de generación del reporte: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>" +
            "<pre>" + contenidoHistorial + "</pre></div></body></html>";

        File.WriteAllText(rutaInformeHTML, plantillaInforme);
        Application.OpenURL(rutaInformeHTML);
#endif
    }
}
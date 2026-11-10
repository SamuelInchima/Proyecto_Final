using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Text;
using System.Collections;
using UnityEngine.Networking;

public class GestorSupervisor : MonoBehaviour
{
    [Header("Registro del Aspirante / Empleado")]
    public TMP_InputField inputNombreJugador;

    [Header("Panel de Contraseña (Login Supervisor)")]
    public GameObject panelLoginSupervisor;
    public TMP_InputField inputClaveSupervisor;
    public TextMeshProUGUI textoErrorClave;

    [Tooltip("Contraseña inicial por defecto si el PC no tiene internet la primera vez")]
    public string claveSecreta = "12345";

    // NUEVO: Casillas dentro del Panel del Supervisor para cambiar la clave en el .exe para todos los PCs.
    [Header("Cambiar Contraseña Global (Dentro del Panel Supervisor)")]
    [Tooltip("Caja de texto donde el Supervisor escribe la nueva contraseña que tendrán todos los PCs")]
    public TMP_InputField inputNuevaClaveGlobal;
    [Tooltip("Texto pequeño para confirmar que la contraseña se actualizó en todos los equipos")]
    public TextMeshProUGUI textoConfirmacionClave;

    [Header("Panel de Resultados del Supervisor")]
    public GameObject panelHistorialSupervisor;
    public TextMeshProUGUI textoPantallaHistorial;

    [Header("Servidor en la Nube (Acceso y Clave Global Multi-PC)")]
    public string urlServidorNube = "";
    public string carpetaRedCompartidaOpcional = "";

    [Header("Conexión con Sonidos")]
    public GestorAudio scriptAudio;

    // Variable interna para controlar el temporizador del texto y reiniciarlo si haces clic varias veces.
    private Coroutine corrutinaTextoConfirmacion;
    private string rutaArchivoHistorial;
    private readonly string llaveSeguridadInterna = "ClaveCifrado_NexoBank_SENA_2026";

    void Start()
    {
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
        if (textoConfirmacionClave != null) textoConfirmacionClave.text = "";

        // 1. Cargamos primero la última clave guardada y cifrada en este PC (por si hoy no hay internet).
        if (PlayerPrefs.HasKey("ClaveSupervisorCifrada"))
        {
            claveSecreta = DescifrarTexto(PlayerPrefs.GetString("ClaveSupervisorCifrada"));
        }

        // 2. Si hay conexión a la nube, descargamos silenciosamente la contraseña global vigente para todos los PCs.
        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaSincronizarClaveGlobal());
        }
    }

    // =========================================================================
    // NUEVO: SISTEMA PARA SINCRONIZAR Y CAMBIAR LA CONTRASEÑA EN TODOS LOS PC
    // =========================================================================

    // Se ejecuta sola al abrir el juego (.exe) en cualquier computador para traer la contraseña actual de la nube.
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
                    // Guardamos una copia cifrada en este PC por si mañana lo abren sin internet.
                    PlayerPrefs.SetString("ClaveSupervisorCifrada", CifrarTexto(claveSecreta));
                    PlayerPrefs.Save();
                }
            }
        }
    }

    // Se conecta al botón "Cambiar Contraseña" dentro del Panel del Supervisor en el .exe.
    public void BotonCambiarContraseñaGlobal()
    {
        if (inputNuevaClaveGlobal == null || string.IsNullOrEmpty(inputNuevaClaveGlobal.text))
        {
            // Mostramos el aviso y hacemos que desaparezca solo a los 3 segundos.
            MostrarConfirmacionTemporal("Escribe una contraseña válida.");
            return;
        }

        // 1. Actualizamos la contraseña inmediatamente en este computador (cifrada).
        claveSecreta = inputNuevaClaveGlobal.text.Trim();
        PlayerPrefs.SetString("ClaveSupervisorCifrada", CifrarTexto(claveSecreta));
        PlayerPrefs.Save();

        // 2. Si tenemos la nube conectada, enviamos la orden para que cambie en TODOS los demás computadores.
        if (!string.IsNullOrEmpty(urlServidorNube))
        {
            StartCoroutine(CorrutinaSubirNuevaClaveGlobal(claveSecreta));
        }
        else
        {
            MostrarConfirmacionTemporal("¡Contraseña actualizada localmente en este equipo!");
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
                // Mostramos la confirmación final y se borrará sola tras 3 segundos.
                MostrarConfirmacionTemporal("¡Contraseña global actualizada en todos los PC!");
            }
            else
            {
                MostrarConfirmacionTemporal("Actualizada en este PC (sin conexión a la nube).");
            }
        }
    }

    // =========================================================================
    // NUEVO: MÉTODO Y CORRUTINA PARA BORRAR EL TEXTO DESPUÉS DE 2 SEGUNDOS
    // =========================================================================
    private void MostrarConfirmacionTemporal(string mensaje)
    {
        if (textoConfirmacionClave == null) return;

        // Si ya había un conteo de 3 segundos corriendo, lo detenemos para empezar uno nuevo.
        if (corrutinaTextoConfirmacion != null)
        {
            StopCoroutine(corrutinaTextoConfirmacion);
        }

        corrutinaTextoConfirmacion = StartCoroutine(CorrutinaLimpiarConfirmacion(mensaje));
    }

    private IEnumerator CorrutinaLimpiarConfirmacion(string mensaje)
    {
        // 1. Ponemos el mensaje en pantalla.
        textoConfirmacionClave.text = mensaje;

        // 2. Esperamos 2 segundos reales.
        yield return new WaitForSeconds(2f);

        // 3. Vaciamos el texto para que desaparezca de la pantalla.
        textoConfirmacionClave.text = "";
    }

    // =========================================================================
    // GUARDADO AUTOMÁTICO DE EVALUACIONES (DISCO CIFRADO + NUBE)
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
            " ========================================================\n" +
            "EMPLEADO / ASPIRANTE: " + nombreEmpleado.ToUpper() + "\n" +
            "FECHA: " + fechaActual + " | MÓDULO: " + nombreModulo + "\n" +
            "PROMEDIO / PUNTAJE: " + promedio + "%\n" +
            "DIAGNÓSTICO:\n" + diagnostico + "\n\n" +
            "DESGLOSE DETALLADO:\n" + detalleCasos +
            " ========================================================\n\n";

        string historialAnteriorLegible = LeerYDescifrarArchivoLocal();
        string historialActualizado = historialAnteriorLegible + bloqueRegistro;
        File.WriteAllText(rutaArchivoHistorial, CifrarTexto(historialActualizado));

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
        textoPantallaHistorial.text = "Conectando con el servidor central de NexoBank...\nDescargando evaluaciones...";

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

    // =========================================================================
    // ALGORITMO DE CIFRADO Y DESCIFRADO (PARA ARCHIVO Y CONTRASEÑA)
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
            return "12345";
        }
    }

    private string LeerYDescifrarArchivoLocal()
    {
        if (!File.Exists(rutaArchivoHistorial)) return "";
        return DescifrarTexto(File.ReadAllText(rutaArchivoHistorial));
    }

    // =========================================================================
    // MÉTODOS DE LOGIN Y BOTONES DEL SUPERVISOR
    // =========================================================================
    public void BotonAbrirLoginSupervisor()
    {
        inputClaveSupervisor.text = "";
        if (textoErrorClave != null) textoErrorClave.text = "";
        panelLoginSupervisor.SetActive(true);

        // Cada vez que alguien abre la ventana de Login, verificamos si el administrador cambió la clave en la nube.
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

            inputClaveSupervisor.text = ""; // Limpiamos la clave por seguridad.
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
        if (File.Exists(rutaArchivoHistorial)) File.Delete(rutaArchivoHistorial);

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
        inputClaveSupervisor.text = "";
        panelLoginSupervisor.SetActive(false);
        panelHistorialSupervisor.SetActive(false);
        if (textoConfirmacionClave != null) textoConfirmacionClave.text = "";
    }

    public void BotonExportarInformePDF()
    {
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
            "<h1>NEXOBANK - DEPARTAMENTO DE AUDITORÍA Y SELECCIÓN</h1>" +
            "<h3>Informe Consolidado Multi-Equipo (Servicio, Phishing y Psicotécnico)</h3>" +
            "<p>Fecha de generación del reporte: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>" +
            "<pre>" + contenidoHistorial + "</pre></div></body></html>";

        File.WriteAllText(rutaInformeHTML, plantillaInforme);
        Application.OpenURL(rutaInformeHTML);
    }
}
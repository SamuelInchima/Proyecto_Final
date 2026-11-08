// Importamos las herramientas básicas de Unity.
using UnityEngine;
// Importamos TextMeshPro para leer las cajas de texto interactivas (TMP_InputField) y mostrar el historial.
using TMPro;
// NUEVO PARA TU SUSTENTACIÓN: 'System.IO' (Input/Output) es la librería oficial de C# que permite
// crear, escribir y leer archivos reales en el disco duro del computador.
using System.IO;

public class GestorSupervisor : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: REGISTRO DEL JUGADOR ACTUAL (EN EL SELECTOR DE MÓDULOS)
    // =========================================================================
    [Header("Registro del Aspirante / Empleado")]
    [Tooltip("Caja de texto (InputField) en el Selector donde el jugador escribe su nombre antes de jugar")]
    public TMP_InputField inputNombreJugador; // Guarda lo que el empleado escriba en su caja de nombre.

    // =========================================================================
    // SECCIÓN 2: VENTANAS Y CLAVE DEL SUPERVISOR
    // =========================================================================
    [Header("Panel de Contraseña (Login Supervisor)")]
    public GameObject panelLoginSupervisor;      // Ventana que pide la contraseña de administrador.
    public TMP_InputField inputClaveSupervisor;  // Caja donde el supervisor escribe la contraseña.
    public TextMeshProUGUI textoErrorClave;      // Texto rojo que avisa si la contraseña es incorrecta.

    [Tooltip("Contraseña que deberá escribir el supervisor para ver los resultados")]
    public string claveSecreta = "Nexo2026";     // Puedes cambiar esta clave desde el Inspector de Unity.

    [Header("Panel de Resultados del Supervisor")]
    public GameObject panelHistorialSupervisor;       // Ventana exclusiva donde se ven todos los resultados.
    public TextMeshProUGUI textoPantallaHistorial;    // Texto grande donde se cargan todas las evaluaciones.

    [Header("Conexión con Sonidos")]
    public GestorAudio scriptAudio;                   // Para sonar clic, acierto o error al validar la clave.

    // Variable privada que guardará la dirección exacta en el disco duro donde vivirá el archivo de texto.
    private string rutaArchivoHistorial;

    // =========================================================================
    // MÉTODO START: PREPARACIÓN DE LA RUTA EN EL DISCO DURO
    // =========================================================================
    void Start()
    {
        // 'Application.persistentDataPath' es una carpeta segura que Unity crea automáticamente en cualquier
        // computador (Windows, Mac, etc.) y que NUNCA se borra aunque cierres el juego o reinicies el PC.
        rutaArchivoHistorial = Application.persistentDataPath + "/Historial_Evaluaciones_NexoBank.txt";

        // Nos aseguramos de que las ventanas del supervisor inicien apagadas al abrir el juego.
        if (panelLoginSupervisor != null) panelLoginSupervisor.SetActive(false);
        if (panelHistorialSupervisor != null) panelHistorialSupervisor.SetActive(false);
        if (textoErrorClave != null) textoErrorClave.text = "";
    }

    // =========================================================================
    // SECCIÓN 3: MÉTODO QUE GUARDA AUTOMÁTICAMENTE A CADA JUGADOR
    // =========================================================================
    // Esta función es llamada automáticamente por 'GestorServicioCliente' apenas un jugador termina (gane o pierda).
    public void GuardarEvaluacionJugador(int promedio, string diagnostico, string detalleCasos)
    {
        // 1. Verificamos qué nombre escribió el jugador. Si dejó la casilla vacía, le ponemos "Aspirante Sin Nombre".
        string nombreEmpleado = "Aspirante Sin Nombre";
        if (inputNombreJugador != null && !string.IsNullOrEmpty(inputNombreJugador.text))
        {
            nombreEmpleado = inputNombreJugador.text;
        }

        // 2. Obtenemos la fecha y hora exacta del computador en este momento (ej. "2026-11-08 14:30").
        string fechaActual = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // 3. Armamos el bloque de texto ordenado con todos los datos de este jugador.
        string bloqueRegistro =
            "========================================================\n" +
            "FECHA: " + fechaActual + " | EMPLEADO: " + nombreEmpleado + "\n" +
            "PROMEDIO OBTENIDO: " + promedio + "%\n" +
            "DIAGNÓSTICO: " + diagnostico + "\n" +
            "DETALLE POR CASO:\n" + detalleCasos +
            "========================================================\n\n";

        // 4. 'File.AppendAllText' busca el archivo en 'rutaArchivoHistorial' (si no existe, lo crea solo)
        // y pega este nuevo bloque al final sin borrar a los jugadores que hicieron el test antes.
        File.AppendAllText(rutaArchivoHistorial, bloqueRegistro);

        Debug.Log("Evaluación guardada exitosamente en: " + rutaArchivoHistorial);
    }

    // =========================================================================
    // SECCIÓN 4: MÉTODOS PARA LOS BOTONES DE ACCESO DEL SUPERVISOR
    // =========================================================================

    // 1. Se conecta al botón "Acceso Supervisor" en el Menú Principal.
    public void BotonAbrirLoginSupervisor()
    {
        inputClaveSupervisor.text = ""; // Limpiamos la caja de contraseña.
        if (textoErrorClave != null) textoErrorClave.text = "";
        panelLoginSupervisor.SetActive(true);
    }

    // 2. Se conecta al botón "Ingresar / Verificar" dentro del panelLoginSupervisor.
    public void BotonVerificarContraseña()
    {
        // Comparamos si lo que escribió en la caja es exactamente igual a nuestra 'claveSecreta'.
        if (inputClaveSupervisor.text == claveSecreta)
        {
            if (scriptAudio != null) scriptAudio.ReproducirAcierto();

            // Cerramos el Login, abrimos la ventana del Historial y cargamos los datos del archivo.
            panelLoginSupervisor.SetActive(false);
            panelHistorialSupervisor.SetActive(true);
            MostrarHistorialEnPantalla();
        }
        else
        {
            // Si se equivocó de contraseña, suena alerta de error y mostramos aviso.
            if (scriptAudio != null) scriptAudio.ReproducirError();
            if (textoErrorClave != null)
            {
                textoErrorClave.text = "Acceso denegado: Contraseña de supervisor incorrecta.";
            }
        }
    }

    // 3. Lee el archivo del disco duro y lo escribe en la pantalla del Supervisor.
    private void MostrarHistorialEnPantalla()
    {
        // 'File.Exists' verifica si ya se creó el archivo (es decir, si al menos 1 jugador ya hizo el test).
        if (File.Exists(rutaArchivoHistorial))
        {
            // 'File.ReadAllText' saca todo el texto guardado en el archivo y lo pone en la pantalla.
            textoPantallaHistorial.text = File.ReadAllText(rutaArchivoHistorial);
        }
        else
        {
            textoPantallaHistorial.text = "Aún no hay evaluaciones registradas en el sistema de NexoBank.";
        }
    }

    // 4. Opcional: Se conecta a un botón "Borrar Historial" dentro del panel del Supervisor.
    public void BotonBorrarHistorial()
    {
        if (File.Exists(rutaArchivoHistorial))
        {
            File.Delete(rutaArchivoHistorial); // Elimina el archivo para empezar desde cero con otro grupo.
        }
        textoPantallaHistorial.text = "Historial eliminado correctamente.";
    }

    // 5. Se conecta al botón "Cerrar / Volver" tanto en el Login como en el Historial del Supervisor.
    public void BotonCerrarPanelesSupervisor()
    {
        panelLoginSupervisor.SetActive(false);
        panelHistorialSupervisor.SetActive(false);
    }

    // =========================================================================
    // SECCIÓN 5 (OPCIONAL): EXPORTAR INFORME CORPORATIVO PARA GUARDAR EN PDF
    // =========================================================================
    // Se conecta a un botón "Exportar Informe / PDF" dentro del panel del Supervisor.
    public void BotonExportarInformePDF()
    {
        // 1. Verificamos primero si ya existen datos guardados.
        if (!File.Exists(rutaArchivoHistorial))
        {
            Debug.Log("No hay datos todavía para exportar el informe.");
            return;
        }

        // 2. Leemos todos los resultados guardados hasta hoy.
        string contenidoHistorial = File.ReadAllText(rutaArchivoHistorial);

        // 3. Definimos la ruta donde se creará el documento de informe oficial.
        string rutaInformeHTML = Application.persistentDataPath + "/Informe_Oficial_NexoBank.html";

        // 4. Armamos una plantilla limpia con los colores corporativos de NexoBank (#0A2540 y #0096D6)
        // y un botón integrado que abre directamente la ventana de "Guardar como PDF".
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
            "<h1>NEXOBANK - DEPARTAMENTO DE CUMPLIMIENTO Y AUDITORÍA</h1>" +
            "<h3>Informe Oficial de Evaluaciones de Servicio al Cliente y Normativa</h3>" +
            "<p>Fecha de generación del reporte: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>" +
            "<pre>" + contenidoHistorial + "</pre>" +
            "</div></body></html>";

        // 5. Guardamos el informe en el computador.
        File.WriteAllText(rutaInformeHTML, plantillaInforme);

        // 6. 'Application.OpenURL' abre el informe automáticamente frente al supervisor en su navegador
        // listo para presionar el botón "Descargar / Guardar como PDF".
        Application.OpenURL(rutaInformeHTML);
    }
}
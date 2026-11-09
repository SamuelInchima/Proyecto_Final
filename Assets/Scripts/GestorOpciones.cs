// Importamos las herramientas básicas de Unity (incluye el control de pantalla Screen.fullScreen).
using UnityEngine;
// Importamos 'UnityEngine.UI' porque es OBLIGATORIO para controlar Sliders (barras deslizables),
// Toggles (casillas de verificación Sí/No) e Images (el filtro de brillo).
using UnityEngine.UI;

public class GestorOpciones : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: VENTANA DE OPCIONES Y CONEXIÓN CON MENÚ / AUDIO
    // =========================================================================
    [Header("Panel de Opciones")]
    [Tooltip("Arrastra aquí el Panel de Opciones que se abrirá desde el Menú Inicial")]
    public GameObject panelOpciones;

    [Header("Conexión con otros Scripts")]
    public GestorAudio scriptAudio;               // Conexión para controlar el volumen y silencio de los parlantes.
    public GestorMenuPrincipal scriptMenu;        // Opcional: Para abrir el panel con la animación suave del Paso 1.

    // =========================================================================
    // SECCIÓN 2: CONTROLES VISUALES (BRILLO Y MODO DE PANTALLA)
    // =========================================================================
    [Header("Configuración de Brillo")]
    [Tooltip("Imagen negra que cubre toda la pantalla (debe tener Raycast Target DESMARCADO)")]
    public Image imagenFiltroBrillo;              // El velo negro que oscurece o aclara el juego.

    [Tooltip("Barra deslizable (Slider) para subir o bajar el brillo")]
    public Slider sliderBrillo;                   // Cuando vale 1 el juego brilla al 100%; cuando baja, oscurece.

    [Header("Configuración de Pantalla (Completa / Ventana)")]
    [Tooltip("Casilla (Toggle) para marcar Pantalla Completa o desmarcar para Modo Ventana")]
    public Toggle togglePantallaCompleta;

    // =========================================================================
    // SECCIÓN 3: CONTROLES DE AUDIO (MÚSICA SÍ/NO Y VOLÚMENES)
    // =========================================================================
    [Header("Configuración de Música y Sonidos")]
    [Tooltip("Casilla (Toggle) para activar (Sí) o silenciar (No) la música de fondo")]
    public Toggle toggleMusicaSiNo;

    [Tooltip("Barra deslizable (Slider) para el nivel de volumen de la Música de fondo")]
    public Slider sliderVolumenMusica;

    [Tooltip("Barra deslizable (Slider) para el nivel de volumen de los Efectos de Sonido")]
    public Slider sliderVolumenSonidos;

    // =========================================================================
    // MÉTODO START: CONFIGURACIÓN INICIAL AL ARRANCAR EL JUEGO
    // =========================================================================
    void Start()
    {
        // 1. Nos aseguramos de que el panel de Opciones inicie apagado al abrir el juego.
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
        }

        // 2. Iniciamos el brillo al máximo (1.0 = sin oscuridad) para que el juego se vea nítido.
        if (sliderBrillo != null)
        {
            sliderBrillo.value = 1f;
            CambiarBrillo();
        }

        // 3. Sincronizamos la casilla de Pantalla Completa con el estado actual del monitor.
        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.isOn = Screen.fullScreen;
        }

        // 4. Sincronizamos las barras de volumen y la casilla de música con los parlantes de GestorAudio.
        if (scriptAudio != null)
        {
            if (scriptAudio.reproductorMusica != null)
            {
                if (sliderVolumenMusica != null)
                    sliderVolumenMusica.value = scriptAudio.reproductorMusica.volume;

                if (toggleMusicaSiNo != null)
                    toggleMusicaSiNo.isOn = !scriptAudio.reproductorMusica.mute;
            }

            if (scriptAudio.reproductorEfectos != null && sliderVolumenSonidos != null)
            {
                sliderVolumenSonidos.value = scriptAudio.reproductorEfectos.volume;
            }
        }
    }

    // =========================================================================
    // SECCIÓN 4: MÉTODOS PARA ABRIR Y CERRAR EL PANEL DE OPCIONES
    // =========================================================================

    // Se conecta al botón "Opciones" en el Menú Principal.
    public void BotonAbrirOpciones()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();

        // Si conectaste GestorMenuPrincipal, usa la animación suave; si no, lo enciende directamente.
        if (scriptMenu != null)
        {
            scriptMenu.AbrirPanelAnimado(panelOpciones);
        }
        else
        {
            panelOpciones.SetActive(true);
        }
    }

    // Se conecta al botón "Volver / Cerrar" dentro del Panel de Opciones.
    public void BotonCerrarOpciones()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelOpciones.SetActive(false);
    }

    // =========================================================================
    // SECCIÓN 5: MÉTODOS QUE EJECUTAN CADA OPCIÓN EN TIEMPO REAL
    // =========================================================================

    // 1. BRILLO: Se conecta al evento 'On Value Changed' del Slider de Brillo.
    public void CambiarBrillo()
    {
        if (imagenFiltroBrillo != null && sliderBrillo != null)
        {
            // Calculamos cuánta oscuridad aplicar:
            // Si el Slider está en 1 (brillo máximo), la oscuridad es 0 (totalmente transparente).
            // Limitamos la oscuridad máxima a 0.85f para que la pantalla nunca quede 100% negra por accidente.
            float nivelOscuridad = Mathf.Clamp(1f - sliderBrillo.value, 0f, 0.85f);

            // Aplicamos color negro (0, 0, 0) con la transparencia (alpha) calculada en 'nivelOscuridad'.
            imagenFiltroBrillo.color = new Color(0f, 0f, 0f, nivelOscuridad);

            // Seguro adicional: desactivamos 'raycastTarget' por código para que el filtro jamás bloquee los clics.
            imagenFiltroBrillo.raycastTarget = false;
        }
    }

    // 2. PANTALLA COMPLETA O EN VENTANA: Se conecta al evento 'On Value Changed' del Toggle de Pantalla.
    public void CambiarModoPantalla()
    {
        if (togglePantallaCompleta != null)
        {
            // Si la casilla tiene el chulito (true), pasa a Pantalla Completa.
            // Si se le quita el chulito (false), pasa a Modo Ventana.
            Screen.fullScreen = togglePantallaCompleta.isOn;
            Debug.Log("Modo Pantalla Completa cambiado a: " + togglePantallaCompleta.isOn);
        }
    }

    // (Opcional: Si en vez de un Toggle prefieres poner 2 botones separados para "Pantalla Completa" y "Ventana", usa estos dos):
    public void BotonActivarPantallaCompleta() { Screen.fullScreen = true; }
    public void BotonActivarModoVentana() { Screen.fullScreen = false; }

    // 3. MÚSICA SÍ O NO: Se conecta al evento 'On Value Changed' del Toggle de Música.
    public void CambiarMusicaSiNo()
    {
        if (scriptAudio != null && scriptAudio.reproductorMusica != null && toggleMusicaSiNo != null)
        {
            // '.mute' significa silenciar.
            // Si el jugador marca la casilla "Música: Sí" (isOn == true), entonces 'mute' debe ser Falso (!true = false).
            // Si desmarca la casilla (isOn == false), entonces 'mute' pasa a Verdadero (se silencia la música).
            scriptAudio.reproductorMusica.mute = !toggleMusicaSiNo.isOn;
        }
    }

    // 4. NIVEL DE MÚSICA: Se conecta al evento 'On Value Changed' del Slider de Volumen de Música.
    public void CambiarVolumenMusica()
    {
        if (scriptAudio != null && scriptAudio.reproductorMusica != null && sliderVolumenMusica != null)
        {
            // '.volume' va de 0.0 (silencio) a 1.0 (volumen máximo) igual que el Slider.
            scriptAudio.reproductorMusica.volume = sliderVolumenMusica.value;
        }
    }

    // 5. NIVEL DE SONIDO (EFECTOS): Se conecta al evento 'On Value Changed' del Slider de Volumen de Sonidos.
    public void CambiarVolumenSonidos()
    {
        if (scriptAudio != null && scriptAudio.reproductorEfectos != null && sliderVolumenSonidos != null)
        {
            // Ajusta el volumen de los clics, aciertos, errores y tonadas de fin de juego.
            scriptAudio.reproductorEfectos.volume = sliderVolumenSonidos.value;
        }
    }
}
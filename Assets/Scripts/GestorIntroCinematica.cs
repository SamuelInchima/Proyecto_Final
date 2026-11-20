using UnityEngine;
using System.Collections;
using TMPro;

// Compatibilidad automática con el Nuevo Input System y el teclado clásico de Unity.
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GestorIntroCinematica : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: ELEMENTOS VISUALES (UI)
    // =========================================================================
    [Header("Elementos de la Interfaz")]
    [Tooltip("Panel negro a pantalla completa que tapa el menú al iniciar el juego")]
    public GameObject panelIntroCinematica;

    [Tooltip("Texto central donde aparecerá el mensaje de bienvenida")]
    public TextMeshProUGUI textoNarrativo;

    [Tooltip("Texto pequeño en la esquina: 'Presiona [ESPACIO] para saltar'")]
    public TextMeshProUGUI textoAvisoSaltar;

    // =========================================================================
    // SECCIÓN 2: TEXTO Y TIEMPOS (EDITABLES EN EL INSPECTOR)
    // =========================================================================
    [Header("Texto de la Intro")]
    [TextArea(5, 8)]
    public string mensajeIntro =
        "Bienvenido a NexoBank.\n\n" +
        "Simularás ser un nuevo asesor en periodo de prueba dentro de nuestra entidad financiera. " +
        "Tu misión será atender clientes bajo estrictas normativas bancarias, detectar intentos de phishing " +
        "y demostrar tu criterio profesional bajo presión.\n\n" +
        "Tu turno está por comenzar...";

    [Header("Tiempos (en segundos)")]
    [Tooltip("Segundos que tarda el texto en aparecer y desaparecer")]
    public float duracionFundidoTexto = 1.2f;

    [Tooltip("Segundos que el texto permanece visible para leerlo")]
    public float tiempoLectura = 6.0f;

    [Tooltip("Segundos que tarda el fondo negro en desvanecerse para mostrar el Menú Principal")]
    public float duracionFundidoAlMenu = 1.2f;

    // Variables privadas de control interno
    private CanvasGroup grupoPanelNegro;
    private CanvasGroup grupoTextoNarrativo;
    private bool introActiva = false;

    // =========================================================================
    // START: ARRANCA LA PANTALLA EN NEGRO AL ABRIR EL JUEGO
    // =========================================================================
    void Start()
    {
        if (panelIntroCinematica == null || textoNarrativo == null) return;

        // 1. Encendemos el panel negro para que cubra el menú desde el primer fotograma.
        panelIntroCinematica.SetActive(true);

        // 2. Obtenemos (o añadimos automáticamente) el CanvasGroup del panel y del texto.
        grupoPanelNegro = panelIntroCinematica.GetComponent<CanvasGroup>();
        if (grupoPanelNegro == null) grupoPanelNegro = panelIntroCinematica.AddComponent<CanvasGroup>();

        grupoTextoNarrativo = textoNarrativo.GetComponent<CanvasGroup>();
        if (grupoTextoNarrativo == null) grupoTextoNarrativo = textoNarrativo.gameObject.AddComponent<CanvasGroup>();

        // 3. Asignamos el texto y configuramos el inicio: fondo opaco (1) y texto invisible (0).
        textoNarrativo.text = mensajeIntro;
        grupoPanelNegro.alpha = 1f;
        grupoPanelNegro.blocksRaycasts = true; // Evita clics accidentales sobre el menú de atrás.
        grupoTextoNarrativo.alpha = 0f;

        if (textoAvisoSaltar != null) textoAvisoSaltar.gameObject.SetActive(true);

        // 4. Iniciamos la secuencia de fundido único.
        introActiva = true;
        StartCoroutine(ReproducirIntroUnica());
    }

    // =========================================================================
    // UPDATE: ESCUCHA LA TECLA ESPACIO Y ANIMA EL AVISO DE SALTAR
    // =========================================================================
    void Update()
    {
        if (!introActiva) return;

        // Parpadeo sutil del aviso inferior.
        if (textoAvisoSaltar != null)
        {
            textoAvisoSaltar.alpha = 0.35f + Mathf.PingPong(Time.time * 0.7f, 0.65f);
        }

        // Si el jugador presiona Espacio, saltamos suavemente al Menú Principal.
        if (SePresionoEspacio())
        {
            SaltarIntro();
        }
    }

    private bool SePresionoEspacio()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    // =========================================================================
    // CORRUTINAS DE FUNDIDO EN NEGRO
    // =========================================================================
    private IEnumerator ReproducirIntroUnica()
    {
        // Breve pausa en negro puro al abrir el juego.
        yield return new WaitForSeconds(0.4f);

        // 1. Aparece suavemente el texto (0 -> 1).
        yield return StartCoroutine(AnimarAlpha(grupoTextoNarrativo, 0f, 1f, duracionFundidoTexto));

        // 2. Espera el tiempo de lectura en pantalla.
        yield return new WaitForSeconds(tiempoLectura);

        // 3. Desaparece el texto a negro (1 -> 0).
        yield return StartCoroutine(AnimarAlpha(grupoTextoNarrativo, 1f, 0f, duracionFundidoTexto));

        // 4. Desvanece el fondo negro para revelar el Menú Principal.
        introActiva = false;
        yield return StartCoroutine(RevelarMenuPrincipal(duracionFundidoAlMenu));
    }

    public void SaltarIntro()
    {
        introActiva = false;
        StopAllCoroutines();
        StartCoroutine(RevelarMenuPrincipal(0.4f));
    }

    private IEnumerator RevelarMenuPrincipal(float duracion)
    {
        if (textoAvisoSaltar != null) textoAvisoSaltar.gameObject.SetActive(false);

        grupoPanelNegro.blocksRaycasts = false;
        yield return StartCoroutine(AnimarAlpha(grupoPanelNegro, grupoPanelNegro.alpha, 0f, duracion));

        panelIntroCinematica.SetActive(false);
    }

    private IEnumerator AnimarAlpha(CanvasGroup grupo, float inicio, float fin, float duracion)
    {
        float t = 0f;
        grupo.alpha = inicio;

        while (t < duracion)
        {
            t += Time.deltaTime;
            grupo.alpha = Mathf.Lerp(inicio, fin, t / duracion);
            yield return null;
        }

        grupo.alpha = fin;
    }
}
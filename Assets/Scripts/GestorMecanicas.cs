// Importamos TextMeshPro para modificar los textos de la interfaz (reloj, aviso de fallo y mensajes finales).
using TMPro;
// Importamos las funciones principales de Unity.
using UnityEngine;
// Importamos UnityEngine.UI para crear los pequeños cuadritos (Image) de la explosión.
using UnityEngine.UI;
// Importamos System.Collections para usar Corrutinas (IEnumerator) tanto en la explosión como en el aviso temporal.
using System.Collections;

public class GestorMecanicas : MonoBehaviour
{
    // =========================================================================
    // SECCIÓN 1: VARIABLES DE VIDAS (ESTRELLAS Y CANDADOS)
    // =========================================================================
    [Header("Vidas del Módulo de Servicio")]
    public GameObject[] estrellas;
    private int erroresServicio = 0;

    [Header("Vidas del Módulo de Phishing")]
    public GameObject[] candados;
    private int erroresPhishing = 0;

    // =========================================================================
    // SECCIÓN 2: VARIABLES DEL CONTADOR DE TIEMPO (CRONÓMETRO)
    // =========================================================================
    [Header("Configuración del Cronómetro")]
    public float tiempoPorModulo = 60f;
    private float tiempoRestante;
    private bool relojActivo = false;
    public TextMeshProUGUI textoReloj;
    public GameObject contenedorReloj;

    // =========================================================================
    // NUEVO (PUNTO 4): VENTANA EMERGENTE RÁPIDA AL FALLAR
    // =========================================================================
    [Header("Aviso Emergente Temporal al Fallar")]
    [Tooltip("Arrastra aquí el pequeño panel o recuadro que avisa que la respuesta es incorrecta")]
    public GameObject panelAvisoFallo;

    [Tooltip("Arrastra aquí el texto (TMP) que está dentro del panelAvisoFallo")]
    public TextMeshProUGUI textoAvisoFallo;

    [Tooltip("Cuántos segundos durará visible el mensaje antes de desaparecer solo")]
    public float segundosDuracionAviso = 2.5f;

    // Variable interna para guardar únicamente la corrutina del aviso y poder reiniciarla si falla dos veces rápido,
    // sin detener por error la corrutina de los cuadritos que explotan.
    private Coroutine corrutinaAvisoActual;

    // =========================================================================
    // SECCIÓN 3: VARIABLES DE FIN DE JUEGO, ESTADÍSTICAS E INSIGNIAS
    // =========================================================================
    [Header("Pantallas de Fin de Juego")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    [Header("Elementos de Estadísticas (Dentro del Panel Victoria)")]
    public TextMeshProUGUI textoMensajeVictoria;
    public GameObject[] iconosVictoria;

    [Header("Insignias en el Selector de Módulos")]
    public GameObject simboloCompletadoServicio;
    public GameObject simboloCompletadoPhishing;

    [Header("Conexión con el Audio")]
    public GestorAudio scriptAudio;

    void Start()
    {
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // NUEVO: Nos aseguramos de que el pequeño aviso de fallo inicie apagado al arrancar el juego.
        if (panelAvisoFallo != null)
        {
            panelAvisoFallo.SetActive(false);
        }

        simboloCompletadoServicio.SetActive(false);
        simboloCompletadoPhishing.SetActive(false);

        contenedorReloj.SetActive(false);
    }

    void Update()
    {
        if (relojActivo == true)
        {
            tiempoRestante = tiempoRestante - Time.deltaTime;
            int segundosEnteros = Mathf.CeilToInt(tiempoRestante);
            textoReloj.text = segundosEnteros.ToString() + "s";

            if (tiempoRestante <= 0)
            {
                textoReloj.text = "Tiempo: 0s";
                MostrarGameOver();
            }
        }
    }

    // =========================================================================
    // SECCIÓN 4: MÉTODOS PARA INICIAR MÓDULOS Y CONTROLAR EL RELOJ
    // =========================================================================
    public void PrepararModuloServicio()
    {
        erroresServicio = 0;
        estrellas[0].SetActive(true);
        estrellas[1].SetActive(true);
        estrellas[2].SetActive(true);

        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);
        if (panelAvisoFallo != null) panelAvisoFallo.SetActive(false);

        contenedorReloj.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    public void PrepararModuloPhishing()
    {
        erroresPhishing = 0;
        candados[0].SetActive(true);
        candados[1].SetActive(true);
        candados[2].SetActive(true);

        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);
        if (panelAvisoFallo != null) panelAvisoFallo.SetActive(false);

        contenedorReloj.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    public void DetenerReloj()
    {
        relojActivo = false;
        contenedorReloj.SetActive(false);
        if (panelAvisoFallo != null) panelAvisoFallo.SetActive(false);
    }

    public void PausarReloj()
    {
        relojActivo = false;
        // Si el jugador acierta mientras el aviso de un error anterior seguía en pantalla, lo ocultamos de inmediato.
        if (panelAvisoFallo != null) panelAvisoFallo.SetActive(false);
    }

    public void ReanudarReloj()
    {
        relojActivo = true;
    }

    // =========================================================================
    // SECCIÓN 5: MÉTODOS PARA RESTAR VIDAS, EXPLOSIÓN Y AVISO DE FALLO
    // =========================================================================
    public void RestarEstrella()
    {
        if (erroresServicio < 3)
        {
            // Lanzamos los cuadritos de explosión desde la estrella que se apaga.
            StartCoroutine(EfectoExplosionCuadritos(estrellas[erroresServicio].transform));

            estrellas[erroresServicio].SetActive(false);
            erroresServicio++;

            scriptAudio.ReproducirError();

            // Verificamos si ya perdió las 3 vidas o si todavía puede seguir intentando.
            if (erroresServicio >= 3)
            {
                MostrarGameOver();
            }
            else
            {
                // NUEVO (PUNTO 4): Si aún le quedan vidas, mostramos el mensaje emergente que desaparece solo.
                MostrarAvisoTemporal("¡Respuesta incorrecta! Incumples la normativa bancaria (-1 Estrella). Intenta de nuevo.");
            }
        }
    }

    public void RestarCandado()
    {
        if (erroresPhishing < 3)
        {
            // Lanzamos los cuadritos de explosión desde el candado que se apaga.
            StartCoroutine(EfectoExplosionCuadritos(candados[erroresPhishing].transform));

            candados[erroresPhishing].SetActive(false);
            erroresPhishing++;

            scriptAudio.ReproducirError();

            // Verificamos si ya perdió los 3 candados o si aún le quedan intentos.
            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
            else
            {
                // NUEVO (PUNTO 4): Si aún le quedan vidas, mostramos el mensaje emergente que desaparece solo.
                MostrarAvisoTemporal("¡Fallo de análisis! Clasificación equivocada (-1 Candado). Revisa bien las pistas.");
            }
        }
    }

    // =========================================================================
    // NUEVO (PUNTO 4): SISTEMA DE MENSAJE EMERGENTE AUTOMÁTICO
    // =========================================================================
    public void MostrarAvisoTemporal(string mensajeError)
    {
        // Verificamos que hayas conectado el panel y el texto en el Inspector para evitar errores.
        if (panelAvisoFallo == null || textoAvisoFallo == null)
        {
            return;
        }

        // Si ya había un aviso mostrándose en pantalla porque el jugador falló hace 1 segundo,
        // detenemos ese conteo anterior para reiniciar los 2.5 segundos completos con el nuevo error.
        if (corrutinaAvisoActual != null)
        {
            StopCoroutine(corrutinaAvisoActual);
        }

        // Iniciamos la corrutina que mostrará el mensaje y lo apagará solo.
        corrutinaAvisoActual = StartCoroutine(CorrutinaMostrarYDesaparecer(mensajeError));
    }

    private IEnumerator CorrutinaMostrarYDesaparecer(string mensaje)
    {
        // 1. Escribimos el texto del fallo y encendemos el pequeño panel emergente.
        textoAvisoFallo.text = mensaje;
        panelAvisoFallo.SetActive(true);

        // 2. 'WaitForSeconds' detiene esta función durante la cantidad de segundos indicada en 'segundosDuracionAviso' (ej. 2.5s)
        // mientras el resto del juego y el cronómetro siguen funcionando normalmente.
        yield return new WaitForSeconds(segundosDuracionAviso);

        // 3. Pasados los 2.5 segundos, apagamos el panel automáticamente.
        panelAvisoFallo.SetActive(false);
    }

    // =========================================================================
    // CORRUTINA DE EXPLOSIÓN SUTIL DE CUADRITOS (PUNTO 3)
    // =========================================================================
    private IEnumerator EfectoExplosionCuadritos(Transform iconoVida)
    {
        int cantidadCuadritos = 8;
        GameObject[] cuadritos = new GameObject[cantidadCuadritos];
        Vector2[] direcciones = new Vector2[cantidadCuadritos];

        for (int i = 0; i < cantidadCuadritos; i++)
        {
            GameObject c = new GameObject("CuadritoExplosion");
            c.transform.SetParent(iconoVida.parent, false);
            c.transform.position = iconoVida.position;

            Image img = c.AddComponent<Image>();
            img.color = new Color(0f, 0.66f, 0.9f, 1f);
            img.raycastTarget = false;
            c.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 10f);

            float angulo = i * (360f / cantidadCuadritos) * Mathf.Deg2Rad;
            direcciones[i] = new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo));
            cuadritos[i] = c;
        }

        float tiempo = 0f;
        float duracion = 0.45f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;

            for (int i = 0; i < cantidadCuadritos; i++)
            {
                if (cuadritos[i] != null)
                {
                    cuadritos[i].transform.localPosition += (Vector3)(direcciones[i] * 85f * Time.deltaTime);
                    cuadritos[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progreso);
                    Image img = cuadritos[i].GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1f - progreso);
                }
            }
            yield return null;
        }

        for (int i = 0; i < cantidadCuadritos; i++)
        {
            if (cuadritos[i] != null)
            {
                Destroy(cuadritos[i]);
            }
        }
    }

    // =========================================================================
    // SECCIÓN 6: MÉTODOS DE VICTORIA, ESTADÍSTICAS Y DERROTA
    // =========================================================================
    public void VictoriaServicio()
    {
        DetenerReloj();
        relojActivo = false;
        panelVictoria.SetActive(true);
        simboloCompletadoServicio.SetActive(true);

        scriptAudio.DetenerMusica();
        scriptAudio.ReproducirVictoria();

        int estrellasFinales = 3 - erroresServicio;
        ActualizarIconosVictoria(estrellasFinales);

        if (estrellasFinales == 3)
        {
            textoMensajeVictoria.text = "¡Desempeño Perfecto (3/3 Estrellas)!\nAtendiste a todos los clientes con total profesionalidad.";
        }
        else if (estrellasFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Estrellas)!\nSuperaste el turno, pero ten más paciencia con los clientes difíciles.";
        }
        else if (estrellasFinales == 1)
        {
            textoMensajeVictoria.text = "¡Al límite (1/3 Estrellas)!\nEstuviste a punto de ser despedido.";
        }
    }

    public void VictoriaServicioDetallada(int promedio, string diagnostico, string detalleCasos)
    {
        DetenerReloj();
        relojActivo = false;
        panelVictoria.SetActive(true);
        simboloCompletadoServicio.SetActive(true);

        scriptAudio.DetenerMusica();
        scriptAudio.ReproducirVictoria();

        int estrellasFinales = 3 - erroresServicio;
        ActualizarIconosVictoria(estrellasFinales);

        textoMensajeVictoria.text =
            "RESULTADOS DE EVALUACIÓN BANCARIA\n" +
            "Promedio de Cumplimiento: " + promedio + "% (" + estrellasFinales + "/3 Estrellas)\n\n" +
            "DIAGNÓSTICO DEL PERFIL:\n" + diagnostico + "\n\n" +
            "DESGLOSE POR NORMATIVA:\n" + detalleCasos;
    }

    public void VictoriaPhishing()
    {
        DetenerReloj();
        relojActivo = false;
        panelVictoria.SetActive(true);
        simboloCompletadoPhishing.SetActive(true);

        scriptAudio.DetenerMusica();
        scriptAudio.ReproducirVictoria();

        int candadosFinales = 3 - erroresPhishing;
        ActualizarIconosVictoria(candadosFinales);

        if (candadosFinales == 3)
        {
            textoMensajeVictoria.text = "¡Seguridad Impecable (3/3 Candados)!\nDescartaste todas las amenazas sin fallar.";
        }
        else if (candadosFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Candados)!\nProtegiste la seguridad de NexoBank, pero tuviste un descuido. Revisa bien los remitentes y detalles.";
        }
        else if (candadosFinales == 1)
        {
            textoMensajeVictoria.text = "¡Alerta de Riesgo (1/3 Candados)!\nSobreviviste por poco. Un error más y NexoBank habría sido hackeado.";
        }
    }

    private void ActualizarIconosVictoria(int cantidadVidas)
    {
        iconosVictoria[0].SetActive(false);
        iconosVictoria[1].SetActive(false);
        iconosVictoria[2].SetActive(false);

        if (cantidadVidas >= 1) iconosVictoria[0].SetActive(true);
        if (cantidadVidas >= 2) iconosVictoria[1].SetActive(true);
        if (cantidadVidas == 3) iconosVictoria[2].SetActive(true);
    }

    private void MostrarGameOver()
    {
        DetenerReloj();
        relojActivo = false;
        if (panelAvisoFallo != null) panelAvisoFallo.SetActive(false);
        panelGameOver.SetActive(true);

        scriptAudio.DetenerMusica();
        scriptAudio.ReproducirGameOver();
    }
}
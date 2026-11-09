// Importamos TextMeshPro para modificar los textos de la interfaz (reloj y mensajes finales).
using TMPro;
// Importamos las funciones principales de Unity.
using UnityEngine;
// NUEVO: Importamos UnityEngine.UI para poder crear los pequeños cuadritos (Image) de la explosión.
using UnityEngine.UI;
// NUEVO: Importamos System.Collections para animar los cuadritos con una Corrutina (IEnumerator).
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

        contenedorReloj.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    public void DetenerReloj()
    {
        relojActivo = false;
        contenedorReloj.SetActive(false);
    }

    public void PausarReloj()
    {
        relojActivo = false;
    }

    public void ReanudarReloj()
    {
        relojActivo = true;
    }

    // =========================================================================
    // SECCIÓN 5: MÉTODOS PARA RESTAR VIDAS Y LANZAR EXPLOSIÓN DE CUADRITOS
    // =========================================================================
    public void RestarEstrella()
    {
        if (erroresServicio < 3)
        {
            // NUEVO: Antes de apagar la estrella, lanzamos los cuadritos desde la posición de esa estrella.
            StartCoroutine(EfectoExplosionCuadritos(estrellas[erroresServicio].transform));

            estrellas[erroresServicio].SetActive(false);
            erroresServicio++;

            scriptAudio.ReproducirError();

            if (erroresServicio >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    public void RestarCandado()
    {
        if (erroresPhishing < 3)
        {
            // NUEVO: Antes de apagar el candado, lanzamos los cuadritos desde la posición de ese candado.
            StartCoroutine(EfectoExplosionCuadritos(candados[erroresPhishing].transform));

            candados[erroresPhishing].SetActive(false);
            erroresPhishing++;

            scriptAudio.ReproducirError();

            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // =========================================================================
    // NUEVA CORRUTINA: EXPLOSIÓN SUTIL DE CUADRITOS DESDE EL CENTRO DEL ICONO
    // =========================================================================
    private IEnumerator EfectoExplosionCuadritos(Transform iconoVida)
    {
        int cantidadCuadritos = 8; // Cantidad de fragmentos que saldrán disparados en círculo.
        GameObject[] cuadritos = new GameObject[cantidadCuadritos];
        Vector2[] direcciones = new Vector2[cantidadCuadritos];

        // 1. Creamos los 8 cuadritos en el centro exacto de la estrella o candado que se acaba de romper.
        for (int i = 0; i < cantidadCuadritos; i++)
        {
            GameObject c = new GameObject("CuadritoExplosion");
            // Lo ponemos dentro del mismo contenedor padre de las vidas para que se vea en el Canvas.
            c.transform.SetParent(iconoVida.parent, false);
            c.transform.position = iconoVida.position;

            // Le agregamos un componente 'Image' de 10x10 píxeles con color cian brillante.
            Image img = c.AddComponent<Image>();
            img.color = new Color(0f, 0.66f, 0.9f, 1f);
            img.raycastTarget = false; // Desactivamos Raycast para que los cuadritos no estorben a los clics.
            c.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 10f);

            // Calculamos el ángulo para que cada uno de los 8 cuadritos salga hacia un lado distinto (360° / 8 = cada 45°).
            float angulo = i * (360f / cantidadCuadritos) * Mathf.Deg2Rad;
            direcciones[i] = new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo));
            cuadritos[i] = c;
        }

        // 2. Durante 0.45 segundos, movemos los cuadritos hacia afuera y los vamos volviendo transparentes.
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
                    // Se alejan del centro suavemente a una velocidad de 85 píxeles por segundo.
                    cuadritos[i].transform.localPosition += (Vector3)(direcciones[i] * 85f * Time.deltaTime);

                    // Se van haciendo más pequeñitos y transparentes a medida que se alejan.
                    cuadritos[i].transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progreso);
                    Image img = cuadritos[i].GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1f - progreso);
                }
            }
            yield return null;
        }

        // 3. Al terminar la animación, borramos los 8 cuadritos para dejar la memoria limpia.
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
        panelGameOver.SetActive(true);

        scriptAudio.DetenerMusica();
        scriptAudio.ReproducirGameOver();
    }
}
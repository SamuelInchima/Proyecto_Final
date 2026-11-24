using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DatosCasoTurno
{
    public string tituloCaso;
    public GameObject tarjetaDocumento;
    public int pistasNecesarias = 2;
    public bool debeAprobar;
    public AudioClip audioOpcional;
    [TextArea] public string feedbackAcierto;
    [TextArea] public string feedbackError;
}

public class GestorTurnoEscritorio : MonoBehaviour
{
    [Header("Casos del Turno (6)")]
    public DatosCasoTurno[] casos;

    [Header("Barra estilo Duolingo y HUD")]
    public Slider barraProgreso;
    public TextMeshProUGUI textoPistas;
    public TextMeshProUGUI textoRacha;
    public GameObject[] vidasVisuales;

    [Header("Sellos y Manual")]
    public GameObject selloAprobadoVisual;
    public GameObject selloReportadoVisual;
    public GameObject panelManual;

    [Header("Franja de Feedback")]
    public GameObject franjaFeedback;
    public Image fondoFranjaFeedback;
    public TextMeshProUGUI textoFeedback;
    public Color colorAcierto = new Color(0.06f, 0.72f, 0.50f);
    public Color colorError = new Color(0.93f, 0.26f, 0.26f);

    [Header("Pantalla Final")]
    public GameObject panelResumenFinal;
    public TextMeshProUGUI textoResumenFinal;

    [Header("Audio")]
    public AudioSource reproductorEfectos;
    public AudioSource reproductorVoz;
    public AudioClip sonidoPista;
    public AudioClip sonidoSello;
    public AudioClip sonidoAcierto;
    public AudioClip sonidoError;

    private int indiceActual = 0;
    private int pistasEncontradas = 0;
    private int aciertos = 0;
    private int racha = 0;
    private int vidas = 3;
    private bool bloqueado = false;

    public void IniciarTurno()
    {
        indiceActual = 0;
        aciertos = 0;
        racha = 0;
        vidas = vidasVisuales.Length;

        for (int i = 0; i < vidasVisuales.Length; i++)
            vidasVisuales[i].SetActive(true);

        if (panelManual != null) panelManual.SetActive(false);
        if (panelResumenFinal != null) panelResumenFinal.SetActive(false);

        barraProgreso.maxValue = casos.Length;
        barraProgreso.value = 0;
        ActualizarTextoRacha();
        CargarCasoActual();
    }

    void CargarCasoActual()
    {
        bloqueado = false;
        pistasEncontradas = 0;

        selloAprobadoVisual.SetActive(false);
        selloReportadoVisual.SetActive(false);
        franjaFeedback.SetActive(false);

        for (int i = 0; i < casos.Length; i++)
            casos[i].tarjetaDocumento.SetActive(false);

        DatosCasoTurno actual = casos[indiceActual];
        actual.tarjetaDocumento.SetActive(true);

        RectTransform rect = actual.tarjetaDocumento.GetComponent<RectTransform>();
        if (rect != null) rect.anchoredPosition = Vector2.zero;

        PistaClicable[] pistas = actual.tarjetaDocumento.GetComponentsInChildren<PistaClicable>(true);
        foreach (PistaClicable p in pistas)
            p.ReiniciarPista();

        textoPistas.text = "Pistas revisadas: 0 / " + actual.pistasNecesarias;

        if (reproductorVoz != null)
        {
            reproductorVoz.Stop();
            if (actual.audioOpcional != null)
            {
                reproductorVoz.clip = actual.audioOpcional;
                reproductorVoz.Play();
            }
        }
    }

    public void RegistrarPistaEncontrada()
    {
        pistasEncontradas++;
        DatosCasoTurno actual = casos[indiceActual];
        textoPistas.text = "Pistas revisadas: " + pistasEncontradas + " / " + actual.pistasNecesarias;

        if (reproductorEfectos != null && sonidoPista != null)
            reproductorEfectos.PlayOneShot(sonidoPista);
    }

    public void BotonSelloAprobar()
    {
        IntentarSellar(true);
    }

    public void BotonSelloReportar()
    {
        IntentarSellar(false);
    }

    void IntentarSellar(bool eligioAprobar)
    {
        if (bloqueado) return;

        DatosCasoTurno actual = casos[indiceActual];
        if (pistasEncontradas < actual.pistasNecesarias)
        {
            textoPistas.text = "Revisa las " + actual.pistasNecesarias + " pistas en el documento antes de sellar.";
            return;
        }

        bloqueado = true;
        if (reproductorVoz != null) reproductorVoz.Stop();

        selloAprobadoVisual.SetActive(eligioAprobar);
        selloReportadoVisual.SetActive(!eligioAprobar);

        if (reproductorEfectos != null && sonidoSello != null)
            reproductorEfectos.PlayOneShot(sonidoSello);

        StartCoroutine(MostrarResultadoTrasSello(eligioAprobar));
    }

    IEnumerator MostrarResultadoTrasSello(bool eligioAprobar)
    {
        yield return new WaitForSeconds(0.6f);

        DatosCasoTurno actual = casos[indiceActual];
        bool acerto = (eligioAprobar == actual.debeAprobar);

        franjaFeedback.SetActive(true);

        if (acerto)
        {
            aciertos++;
            racha++;
            fondoFranjaFeedback.color = colorAcierto;
            textoFeedback.text = actual.feedbackAcierto;
            if (reproductorEfectos != null && sonidoAcierto != null)
                reproductorEfectos.PlayOneShot(sonidoAcierto);
        }
        else
        {
            racha = 0;
            vidas--;
            if (vidas >= 0 && vidas < vidasVisuales.Length)
                vidasVisuales[vidas].SetActive(false);

            fondoFranjaFeedback.color = colorError;
            textoFeedback.text = actual.feedbackError;
            if (reproductorEfectos != null && sonidoError != null)
                reproductorEfectos.PlayOneShot(sonidoError);
        }

        barraProgreso.value = indiceActual + 1;
        ActualizarTextoRacha();
    }

    public void BotonSiguienteCaso()
    {
        if (vidas <= 0 || indiceActual + 1 >= casos.Length)
        {
            MostrarResumenFinal();
        }
        else
        {
            indiceActual++;
            CargarCasoActual();
        }
    }

    void MostrarResumenFinal()
    {
        franjaFeedback.SetActive(false);
        panelResumenFinal.SetActive(true);

        int porcentaje = (aciertos * 100) / casos.Length;
        string estado = (porcentaje >= 80) ? "CERTIFICADO - LISTO PARA OPERAR" :
                        (porcentaje >= 60) ? "EN PROGRESO - REFORZAR MANUAL" :
                                             "RIESGO OPERATIVO - REQUIERE REPASO";

        textoResumenFinal.text = "TURNO FINALIZADO\n\n" +
                                 "Casos resueltos correctamente: " + aciertos + " / " + casos.Length + "\n" +
                                 "Efectividad: " + porcentaje + "%\n" +
                                 "Diagnóstico: " + estado;
    }

    public void BotonAlternarManual()
    {
        panelManual.SetActive(!panelManual.activeSelf);
    }

    void ActualizarTextoRacha()
    {
        textoRacha.text = (racha >= 2) ? "Racha x" + racha : "";
    }
}
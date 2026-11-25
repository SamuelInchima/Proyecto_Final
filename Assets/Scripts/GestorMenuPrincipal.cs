using UnityEngine;

public class GestorMenuPrincipal : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject panelMenuPrincipal;
    public GameObject panelIngresoTurno;
    public GameObject panelEscritorioTurno;
    public GameObject panelInstrucciones;
    public GameObject panelOpciones;

    [Header("Conexión con otros Scripts")]
    public GestorTurnoEscritorio scriptTurno;
    public GestorAudio scriptAudio;

    void Start()
    {
        panelMenuPrincipal.SetActive(true);
        panelIngresoTurno.SetActive(false);
        panelEscritorioTurno.SetActive(false);
        panelInstrucciones.SetActive(false);
        panelOpciones.SetActive(false);
    }

    public void BotonJugar()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelMenuPrincipal.SetActive(false);
        panelIngresoTurno.SetActive(true);
    }

    public void BotonIniciarTurno()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelIngresoTurno.SetActive(false);
        panelEscritorioTurno.SetActive(true);
        scriptTurno.IniciarTurno();
    }

    public void BotonInstrucciones()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelMenuPrincipal.SetActive(false);
        panelInstrucciones.SetActive(true);
    }

    public void BotonOpciones()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelMenuPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BotonVolverMenu()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelIngresoTurno.SetActive(false);
        panelEscritorioTurno.SetActive(false);
        panelInstrucciones.SetActive(false);
        panelOpciones.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo de NexoBank...");
        Application.Quit();
    }
}
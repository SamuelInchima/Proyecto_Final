using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GestorEditorPreguntasUI : MonoBehaviour
{
    [Header("Panel y Conexión con el Banco")]
    public GameObject panelEditorPreguntas;
    public GestorBancoPreguntas scriptBanco;
    public GestorAudio scriptAudio;

    [Header("Textos de Encabezado y Estado")]
    public TextMeshProUGUI textoModuloActivo;
    public TextMeshProUGUI textoCantidadPartida;
    public TextMeshProUGUI textoIndicePregunta;
    public TextMeshProUGUI textoEstadoEditor;

    [Header("Campos Comunes")]
    public TMP_InputField inputTitulo;
    public TMP_InputField inputEnunciado;
    public TMP_InputField inputRetroalimentacion;

    [Header("Grupo 1: Opciones de Respuesta (Servicio y Test)")]
    public GameObject grupoOpcionesTexto;
    public TMP_InputField inputOpcion1;
    public TMP_InputField inputOpcion2;
    public TMP_InputField inputOpcion3;

    [Header("Grupo 2: Controles Solo de Servicio al Cliente")]
    public GameObject grupoSoloServicio;
    public TMP_Dropdown dropdownRespuestaCorrecta;
    public TMP_Dropdown dropdownNpcNormal;
    public TMP_Dropdown dropdownNpcMolesto;

    [Header("Grupo 3: Controles Solo de Detector de Phishing")]
    public GameObject grupoSoloPhishing;
    public Toggle toggleEsPhishing;
    public TMP_Dropdown dropdownImagenPhishing;
    public TMP_Dropdown dropdownAudioPhishing;
    public Image vistaPreviaPhishing;

    [Header("Grupo 4: Controles Solo de Test Psicológico / Perfil")]
    public GameObject grupoSoloPsicologico;
    public TMP_Dropdown dropdownPuntos1;
    public TMP_Dropdown dropdownPuntos2;
    public TMP_Dropdown dropdownPuntos3;

    // 0 = Servicio al Cliente | 1 = Detector de Phishing | 2 = Perfil y Skills
    private int moduloSeleccionado = 0;
    private int indiceActual = 0;
    private Coroutine corrutinaAviso;

    void Start()
    {
        if (panelEditorPreguntas != null) panelEditorPreguntas.SetActive(false);
        if (textoEstadoEditor != null) textoEstadoEditor.text = "";
    }

    // =========================================================================
    // ABRIR Y CERRAR EL EDITOR (CONECTAR AL BOTÓN DENTRO DEL PANEL SUPERVISOR)
    // =========================================================================
    public void BotonAbrirEditor()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        InicializarDropdowns();
        panelEditorPreguntas.SetActive(true);
        SeleccionarModulo(0); // Arranca mostrando Servicio al Cliente
    }

    public void BotonCerrarEditor()
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();
        panelEditorPreguntas.SetActive(false);
    }

    // Llena automáticamente los menús desplegables con los nombres de tus Sprites, Audios y Puntajes.
    private void InicializarDropdowns()
    {
        // Respuesta correcta Servicio
        LlenarDropdown(dropdownRespuestaCorrecta, new List<string> { "Correcta: Opción 1", "Correcta: Opción 2", "Correcta: Opción 3" });

        // Puntajes Test Psicológico
        List<string> opcionesPuntos = new List<string> { "0 pts (Flexible)", "1 pt (Operativo)", "2 pts (Óptimo)" };
        LlenarDropdown(dropdownPuntos1, opcionesPuntos);
        LlenarDropdown(dropdownPuntos2, opcionesPuntos);
        LlenarDropdown(dropdownPuntos3, opcionesPuntos);

        // Nombres de NPCs
        List<string> nombresNpc = new List<string>();
        foreach (Sprite s in scriptBanco.catalogoSpritesNPC)
            nombresNpc.Add(s != null ? s.name : "Sin imagen");
        if (nombresNpc.Count == 0) nombresNpc.Add("Sin NPCs");
        LlenarDropdown(dropdownNpcNormal, nombresNpc);
        LlenarDropdown(dropdownNpcMolesto, nombresNpc);

        // Nombres de imágenes de Phishing
        List<string> nombresPhishing = new List<string>();
        foreach (Sprite s in scriptBanco.catalogoImagenesPhishing)
            nombresPhishing.Add(s != null ? s.name : "Sin imagen");
        if (nombresPhishing.Count == 0) nombresPhishing.Add("Sin imágenes");
        LlenarDropdown(dropdownImagenPhishing, nombresPhishing);

        // Nombres de audios de Phishing (índice 0 siempre es "Sin audio")
        List<string> nombresAudio = new List<string> { "Sin audio (No es llamada)" };
        foreach (AudioClip a in scriptBanco.catalogoAudiosPhishing)
            nombresAudio.Add(a != null ? a.name : "Audio vacío");
        LlenarDropdown(dropdownAudioPhishing, nombresAudio);
    }

    private void LlenarDropdown(TMP_Dropdown dd, List<string> opciones)
    {
        if (dd == null) return;
        dd.ClearOptions();
        dd.AddOptions(opciones);
    }

    // =========================================================================
    // PESTAÑAS DE MÓDULO (CONECTAR EN ON CLICK ENVIANDO 0, 1 o 2)
    // =========================================================================
    public void SeleccionarModulo(int numeroModulo)
    {
        moduloSeleccionado = numeroModulo;
        indiceActual = 0;

        // Encendemos solo los campos que pertenecen al módulo activo
        if (inputEnunciado != null) inputEnunciado.gameObject.SetActive(moduloSeleccionado != 1);
        if (inputRetroalimentacion != null) inputRetroalimentacion.gameObject.SetActive(moduloSeleccionado != 2);
        if (grupoOpcionesTexto != null) grupoOpcionesTexto.SetActive(moduloSeleccionado != 1);
        if (grupoSoloServicio != null) grupoSoloServicio.SetActive(moduloSeleccionado == 0);
        if (grupoSoloPhishing != null) grupoSoloPhishing.SetActive(moduloSeleccionado == 1);
        if (grupoSoloPsicologico != null) grupoSoloPsicologico.SetActive(moduloSeleccionado == 2);

        if (textoModuloActivo != null)
        {
            if (moduloSeleccionado == 0) textoModuloActivo.text = "EDITANDO: MÓDULO 1 - SERVICIO AL CLIENTE";
            else if (moduloSeleccionado == 1) textoModuloActivo.text = "EDITANDO: MÓDULO 2 - DETECTOR DE PHISHING";
            else textoModuloActivo.text = "EDITANDO: MÓDULO 3 - PERFIL LABORAL Y SKILLS";
        }

        MostrarFichaActual();
    }

    // =========================================================================
    // CONTROL DE CANTIDAD POR PARTIDA (BOTONES "-" Y "+")
    // =========================================================================
    public void BotonCambiarCantidadPorPartida(int delta) // Enviar -1 en botón "-" y +1 en botón "+"
    {
        if (scriptAudio != null) scriptAudio.ReproducirClic();

        if (moduloSeleccionado == 0)
            scriptBanco.datosActuales.cantidadServicioPorPartida += delta;
        else if (moduloSeleccionado == 1)
            scriptBanco.datosActuales.cantidadPhishingPorPartida += delta;
        else
            scriptBanco.datosActuales.cantidadPsicologicoPorPartida += delta;

        // Aplica los límites seguros (1 hasta min(20, total disponible)) y guarda de inmediato
        scriptBanco.GuardarEnDisco();
        ActualizarTextoCantidad();
        MostrarAviso("Cantidad por partida actualizada.");
    }

    private void ActualizarTextoCantidad()
    {
        int totalDisponibles = ObtenerTotalModuloActual();
        int topePermitido = Mathf.Min(GestorBancoPreguntas.MAX_PREGUNTAS_PARTIDA, totalDisponibles);
        int elegida = (moduloSeleccionado == 0) ? scriptBanco.datosActuales.cantidadServicioPorPartida
                    : (moduloSeleccionado == 1) ? scriptBanco.datosActuales.cantidadPhishingPorPartida
                    : scriptBanco.datosActuales.cantidadPsicologicoPorPartida;

        if (textoCantidadPartida != null)
        {
            textoCantidadPartida.text = "Por partida: " + elegida + " (Máx: " + topePermitido + ")";
        }
    }

    // =========================================================================
    // NAVEGACIÓN ENTRE FICHAS (< ANTERIOR, SIGUIENTE >, + NUEVA, ELIMINAR)
    // =========================================================================
    public void BotonAnteriorPregunta()
    {
        GuardarCambiosDeFichaEnMemoria();
        if (indiceActual > 0) indiceActual--;
        else indiceActual = ObtenerTotalModuloActual() - 1;
        MostrarFichaActual();
    }

    public void BotonSiguientePregunta()
    {
        GuardarCambiosDeFichaEnMemoria();
        if (indiceActual < ObtenerTotalModuloActual() - 1) indiceActual++;
        else indiceActual = 0;
        MostrarFichaActual();
    }

    public void BotonAgregarNuevaPregunta()
    {
        GuardarCambiosDeFichaEnMemoria();

        // LÍMITE SEGURO: Máximo 50 preguntas por módulo
        if (ObtenerTotalModuloActual() >= GestorBancoPreguntas.MAX_PREGUNTAS_BANCO)
        {
            MostrarAviso("Límite seguro alcanzado: máximo " + GestorBancoPreguntas.MAX_PREGUNTAS_BANCO + " preguntas por módulo.");
            return;
        }

        if (moduloSeleccionado == 0)
        {
            CasoClienteJSON nueva = new CasoClienteJSON
            {
                nombrePolitica = "Nuevo Caso Normativo",
                mensajeCliente = "Escribe aquí el diálogo del cliente...",
                textoBoton1 = "Opción A",
                textoBoton2 = "Opción B",
                textoBoton3 = "Opción C",
                respuestaCorrecta = 1,
                retroalimentacionCaso = "Escribe aquí la explicación de la norma."
            };
            scriptBanco.datosActuales.bancoServicio.Add(nueva);
            indiceActual = scriptBanco.datosActuales.bancoServicio.Count - 1;
        }
        else if (moduloSeleccionado == 1)
        {
            CasoPhishingJSON nueva = new CasoPhishingJSON
            {
                nombreDelCaso = "Nuevo Caso de Phishing",
                indiceImagenCatalogo = 0,
                indiceAudioCatalogo = -1,
                esPhishing = true,
                retroalimentacionPhishing = "Explica aquí las pistas de seguridad."
            };
            scriptBanco.datosActuales.bancoPhishing.Add(nueva);
            indiceActual = scriptBanco.datosActuales.bancoPhishing.Count - 1;
        }
        else
        {
            PreguntaPsicologica nueva = new PreguntaPsicologica
            {
                temaEvaluado = "Nuevo Tema SJT",
                enunciadoPregunta = "Escribe aquí la situación laboral...",
                textoBoton1 = "Opción A",
                textoBoton2 = "Opción B",
                textoBoton3 = "Opción C",
                puntosBoton1 = 2,
                puntosBoton2 = 1,
                puntosBoton3 = 0
            };
            scriptBanco.datosActuales.bancoPsicologico.Add(nueva);
            indiceActual = scriptBanco.datosActuales.bancoPsicologico.Count - 1;
        }

        scriptBanco.GuardarEnDisco();
        MostrarFichaActual();
        MostrarAviso("Nueva pregunta creada. Edítala y presiona Guardar.");
    }

    public void BotonEliminarPreguntaActual()
    {
        // LÍMITE SEGURO: Jamás permitir dejar un módulo con 0 preguntas
        if (ObtenerTotalModuloActual() <= GestorBancoPreguntas.MIN_PREGUNTAS)
        {
            MostrarAviso("No se puede eliminar: cada módulo debe tener mínimo 1 pregunta.");
            return;
        }

        if (moduloSeleccionado == 0) scriptBanco.datosActuales.bancoServicio.RemoveAt(indiceActual);
        else if (moduloSeleccionado == 1) scriptBanco.datosActuales.bancoPhishing.RemoveAt(indiceActual);
        else scriptBanco.datosActuales.bancoPsicologico.RemoveAt(indiceActual);

        indiceActual = Mathf.Clamp(indiceActual, 0, ObtenerTotalModuloActual() - 1);
        scriptBanco.GuardarEnDisco();
        MostrarFichaActual();
        MostrarAviso("Pregunta eliminada correctamente.");
    }

    // =========================================================================
    // MOSTRAR Y GUARDAR LA FICHA ACTUAL
    // =========================================================================
    private void MostrarFichaActual()
    {
        int total = ObtenerTotalModuloActual();
        if (total == 0) return;

        indiceActual = Mathf.Clamp(indiceActual, 0, total - 1);

        if (textoIndicePregunta != null)
            textoIndicePregunta.text = "Pregunta " + (indiceActual + 1) + " de " + total + " (Máx banco: " + GestorBancoPreguntas.MAX_PREGUNTAS_BANCO + ")";

        ActualizarTextoCantidad();

        if (moduloSeleccionado == 0)
        {
            CasoClienteJSON c = scriptBanco.datosActuales.bancoServicio[indiceActual];
            inputTitulo.text = c.nombrePolitica;
            inputEnunciado.text = c.mensajeCliente;
            inputRetroalimentacion.text = c.retroalimentacionCaso;
            inputOpcion1.text = c.textoBoton1;
            inputOpcion2.text = c.textoBoton2;
            inputOpcion3.text = c.textoBoton3;
            dropdownRespuestaCorrecta.value = Mathf.Clamp(c.respuestaCorrecta - 1, 0, 2);
            dropdownNpcNormal.value = Mathf.Clamp(c.indiceNpcNormal, 0, Mathf.Max(0, dropdownNpcNormal.options.Count - 1));
            dropdownNpcMolesto.value = Mathf.Clamp(c.indiceNpcMolesto, 0, Mathf.Max(0, dropdownNpcMolesto.options.Count - 1));
        }
        else if (moduloSeleccionado == 1)
        {
            CasoPhishingJSON p = scriptBanco.datosActuales.bancoPhishing[indiceActual];
            inputTitulo.text = p.nombreDelCaso;
            inputRetroalimentacion.text = p.retroalimentacionPhishing;
            toggleEsPhishing.isOn = p.esPhishing;
            dropdownImagenPhishing.value = Mathf.Clamp(p.indiceImagenCatalogo, 0, Mathf.Max(0, dropdownImagenPhishing.options.Count - 1));
            dropdownAudioPhishing.value = (p.indiceAudioCatalogo < 0) ? 0 : Mathf.Clamp(p.indiceAudioCatalogo + 1, 0, dropdownAudioPhishing.options.Count - 1);
            ActualizarVistaPreviaPhishing();
        }
        else
        {
            PreguntaPsicologica t = scriptBanco.datosActuales.bancoPsicologico[indiceActual];
            inputTitulo.text = t.temaEvaluado;
            inputEnunciado.text = t.enunciadoPregunta;
            inputOpcion1.text = t.textoBoton1;
            inputOpcion2.text = t.textoBoton2;
            inputOpcion3.text = t.textoBoton3;
            dropdownPuntos1.value = Mathf.Clamp(t.puntosBoton1, 0, 2);
            dropdownPuntos2.value = Mathf.Clamp(t.puntosBoton2, 0, 2);
            dropdownPuntos3.value = Mathf.Clamp(t.puntosBoton3, 0, 2);
        }
    }

    // Conectar al evento 'On Value Changed' de dropdownImagenPhishing para ver la miniatura al instante.
    public void ActualizarVistaPreviaPhishing()
    {
        if (vistaPreviaPhishing == null || scriptBanco.catalogoImagenesPhishing.Count == 0) return;
        int idx = Mathf.Clamp(dropdownImagenPhishing.value, 0, scriptBanco.catalogoImagenesPhishing.Count - 1);
        vistaPreviaPhishing.sprite = scriptBanco.catalogoImagenesPhishing[idx];
        vistaPreviaPhishing.preserveAspect = true;
    }

    private void GuardarCambiosDeFichaEnMemoria()
    {
        if (ObtenerTotalModuloActual() == 0) return;

        if (moduloSeleccionado == 0)
        {
            CasoClienteJSON c = scriptBanco.datosActuales.bancoServicio[indiceActual];
            c.nombrePolitica = inputTitulo.text;
            c.mensajeCliente = inputEnunciado.text;
            c.retroalimentacionCaso = inputRetroalimentacion.text;
            c.textoBoton1 = inputOpcion1.text;
            c.textoBoton2 = inputOpcion2.text;
            c.textoBoton3 = inputOpcion3.text;
            c.respuestaCorrecta = dropdownRespuestaCorrecta.value + 1;
            c.indiceNpcNormal = dropdownNpcNormal.value;
            c.indiceNpcMolesto = dropdownNpcMolesto.value;
        }
        else if (moduloSeleccionado == 1)
        {
            CasoPhishingJSON p = scriptBanco.datosActuales.bancoPhishing[indiceActual];
            p.nombreDelCaso = inputTitulo.text;
            p.retroalimentacionPhishing = inputRetroalimentacion.text;
            p.esPhishing = toggleEsPhishing.isOn;
            p.indiceImagenCatalogo = dropdownImagenPhishing.value;
            p.indiceAudioCatalogo = dropdownAudioPhishing.value - 1; // 0 en dropdown equivale a -1 (sin audio)
        }
        else
        {
            PreguntaPsicologica t = scriptBanco.datosActuales.bancoPsicologico[indiceActual];
            t.temaEvaluado = inputTitulo.text;
            t.enunciadoPregunta = inputEnunciado.text;
            t.textoBoton1 = inputOpcion1.text;
            t.textoBoton2 = inputOpcion2.text;
            t.textoBoton3 = inputOpcion3.text;
            t.puntosBoton1 = dropdownPuntos1.value;
            t.puntosBoton2 = dropdownPuntos2.value;
            t.puntosBoton3 = dropdownPuntos3.value;
        }
    }

    public void BotonGuardarTodoEnDisco()
    {
        GuardarCambiosDeFichaEnMemoria();
        scriptBanco.GuardarEnDisco();
        if (scriptAudio != null) scriptAudio.ReproducirAcierto();
        MostrarAviso("¡Cambios guardados y aplicados al simulador!");
    }

    public void BotonRestaurarPreguntasOriginales()
    {
        scriptBanco.RestaurarValoresDeFabrica();
        indiceActual = 0;
        MostrarFichaActual();
        MostrarAviso("Preguntas originales restauradas correctamente.");
    }

    private int ObtenerTotalModuloActual()
    {
        if (moduloSeleccionado == 0) return scriptBanco.datosActuales.bancoServicio.Count;
        if (moduloSeleccionado == 1) return scriptBanco.datosActuales.bancoPhishing.Count;
        return scriptBanco.datosActuales.bancoPsicologico.Count;
    }

    private void MostrarAviso(string mensaje)
    {
        if (textoEstadoEditor == null) return;
        if (corrutinaAviso != null) StopCoroutine(corrutinaAviso);
        corrutinaAviso = StartCoroutine(CorrutinaLimpiarAviso(mensaje));
    }

    private IEnumerator CorrutinaLimpiarAviso(string mensaje)
    {
        textoEstadoEditor.text = mensaje;
        yield return new WaitForSeconds(3f);
        textoEstadoEditor.text = "";
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.IO;

// =========================================================================
// PLANTILLAS DE DATOS QUE SÍ PUEDEN GUARDARSE EN UN ARCHIVO JSON
// =========================================================================
[System.Serializable]
public class CasoClienteJSON
{
    public string nombrePolitica;
    public int indiceNpcNormal;
    public int indiceNpcMolesto;
    public string mensajeCliente;
    public string textoBoton1;
    public string textoBoton2;
    public string textoBoton3;
    public int respuestaCorrecta = 1;
    public string retroalimentacionCaso;
}

[System.Serializable]
public class CasoPhishingJSON
{
    public string nombreDelCaso;
    public int indiceImagenCatalogo;
    public int indiceAudioCatalogo = -1; // -1 significa "Sin audio de llamada"
    public bool esPhishing;
    public string retroalimentacionPhishing;
}

[System.Serializable]
public class DatosBancoCompleto
{
    // Cantidades que elige el administrador para cada partida
    public int cantidadServicioPorPartida = 5;
    public int cantidadPhishingPorPartida = 5;
    public int cantidadPsicologicoPorPartida = 15;

    // Listas editables de los 3 módulos
    public List<CasoClienteJSON> bancoServicio = new List<CasoClienteJSON>();
    public List<CasoPhishingJSON> bancoPhishing = new List<CasoPhishingJSON>();
    public List<PreguntaPsicologica> bancoPsicologico = new List<PreguntaPsicologica>();
}

// =========================================================================
// CLASE PRINCIPAL: GestorBancoPreguntas
// =========================================================================
public class GestorBancoPreguntas : MonoBehaviour
{
    // LÍMITES SEGUROS DEL SISTEMA (Constantes que nadie puede romper)
    public const int MIN_PREGUNTAS = 1;
    public const int MAX_PREGUNTAS_PARTIDA = 20;
    public const int MAX_PREGUNTAS_BANCO = 50;

    [Header("Conexión con los 3 Módulos")]
    public GestorServicioCliente scriptServicio;
    public GestorPhishing scriptPhishing;
    public GestorTestPsicologico scriptPsicologico;

    [Header("Catálogos Extra (Opcional: agrega aquí más fotos o audios para el editor)")]
    public List<Sprite> catalogoSpritesNPC = new List<Sprite>();
    public List<Sprite> catalogoImagenesPhishing = new List<Sprite>();
    public List<AudioClip> catalogoAudiosPhishing = new List<AudioClip>();

    [HideInInspector] public DatosBancoCompleto datosActuales = new DatosBancoCompleto();

    // Respaldo interno en memoria de tus preguntas originales del Inspector (por si el admin quiere restaurar fábrica)
    private string jsonFabricaOriginal;
    private string rutaArchivoBanco;

    void Awake()
    {
        rutaArchivoBanco = Path.Combine(Application.persistentDataPath, "BancoPreguntas_NexoBank.json");

        // 1. Leemos primero lo que ya tienes armado en el Inspector y lo guardamos como respaldo de fábrica.
        DatosBancoCompleto respaldoFabrica = ImportarDesdeInspectorActual();
        jsonFabricaOriginal = JsonUtility.ToJson(respaldoFabrica, true);

        // 2. Si el administrador ya había guardado cambios en este PC, cargamos su archivo JSON.
        // Si es la primera vez que abre el juego, creamos el archivo JSON con tus preguntas del Inspector.
        if (File.Exists(rutaArchivoBanco))
        {
            CargarDesdeDisco();
        }
        else
        {
            datosActuales = respaldoFabrica;
            GuardarEnDisco();
        }

        // 3. Pasamos las preguntas y las cantidades activas a los 3 módulos del juego.
        AplicarBancoALosModulos();
    }

    // =========================================================================
    // IMPORTACIÓN AUTOMÁTICA DESDE EL INSPECTOR (NO PIERDES TU TRABAJO ACTUAL)
    // =========================================================================
    private DatosBancoCompleto ImportarDesdeInspectorActual()
    {
        DatosBancoCompleto datos = new DatosBancoCompleto();

        // --- MÓDULO 1: SERVICIO AL CLIENTE ---
        if (scriptServicio != null && scriptServicio.niveles != null)
        {
            foreach (CasoCliente c in scriptServicio.niveles)
            {
                CasoClienteJSON nuevo = new CasoClienteJSON();
                nuevo.nombrePolitica = c.nombrePolitica;
                nuevo.indiceNpcNormal = RegistrarYObtenerIndiceSprite(catalogoSpritesNPC, c.fotoCliente);
                nuevo.indiceNpcMolesto = RegistrarYObtenerIndiceSprite(catalogoSpritesNPC, c.fotoClienteMolesto);
                nuevo.mensajeCliente = c.mensajeCliente;
                nuevo.textoBoton1 = c.textoBoton1;
                nuevo.textoBoton2 = c.textoBoton2;
                nuevo.textoBoton3 = c.textoBoton3;
                nuevo.respuestaCorrecta = Mathf.Clamp(c.respuestaCorrecta, 1, 3);
                nuevo.retroalimentacionCaso = c.retroalimentacionCaso;
                datos.bancoServicio.Add(nuevo);
            }
            datos.cantidadServicioPorPartida = Mathf.Clamp(datos.bancoServicio.Count, MIN_PREGUNTAS, MAX_PREGUNTAS_PARTIDA);
        }

        // --- MÓDULO 2: PHISHING ---
        if (scriptPhishing != null && scriptPhishing.nivelesPhishing != null)
        {
            foreach (CasoPhishing p in scriptPhishing.nivelesPhishing)
            {
                CasoPhishingJSON nuevo = new CasoPhishingJSON();
                nuevo.nombreDelCaso = p.nombreDelCaso;
                nuevo.indiceImagenCatalogo = RegistrarYObtenerIndiceSprite(catalogoImagenesPhishing, p.imagenDelMensaje);
                nuevo.indiceAudioCatalogo = RegistrarYObtenerIndiceAudio(catalogoAudiosPhishing, p.audioLlamadaCaso);
                nuevo.esPhishing = p.esPhishing;
                nuevo.retroalimentacionPhishing = p.retroalimentacionPhishing;
                datos.bancoPhishing.Add(nuevo);
            }
            datos.cantidadPhishingPorPartida = Mathf.Clamp(datos.bancoPhishing.Count, MIN_PREGUNTAS, MAX_PREGUNTAS_PARTIDA);
        }

        // --- MÓDULO 3: TEST PSICOLÓGICO ---
        if (scriptPsicologico != null && scriptPsicologico.preguntas != null)
        {
            foreach (PreguntaPsicologica t in scriptPsicologico.preguntas)
            {
                PreguntaPsicologica copia = new PreguntaPsicologica();
                copia.temaEvaluado = t.temaEvaluado;
                copia.enunciadoPregunta = t.enunciadoPregunta;
                copia.textoBoton1 = t.textoBoton1;
                copia.textoBoton2 = t.textoBoton2;
                copia.textoBoton3 = t.textoBoton3;
                copia.puntosBoton1 = Mathf.Clamp(t.puntosBoton1, 0, 2);
                copia.puntosBoton2 = Mathf.Clamp(t.puntosBoton2, 0, 2);
                copia.puntosBoton3 = Mathf.Clamp(t.puntosBoton3, 0, 2);
                datos.bancoPsicologico.Add(copia);
            }
            datos.cantidadPsicologicoPorPartida = Mathf.Clamp(datos.bancoPsicologico.Count, MIN_PREGUNTAS, MAX_PREGUNTAS_PARTIDA);
        }

        return datos;
    }

    // Busca si una imagen ya está en el catálogo; si no está, la agrega sola para que nunca falte nada.
    private int RegistrarYObtenerIndiceSprite(List<Sprite> catalogo, Sprite imagen)
    {
        if (imagen == null) return 0;
        int idx = catalogo.IndexOf(imagen);
        if (idx == -1)
        {
            catalogo.Add(imagen);
            idx = catalogo.Count - 1;
        }
        return idx;
    }

    private int RegistrarYObtenerIndiceAudio(List<AudioClip> catalogo, AudioClip audio)
    {
        if (audio == null) return -1; // -1 significa sin audio
        int idx = catalogo.IndexOf(audio);
        if (idx == -1)
        {
            catalogo.Add(audio);
            idx = catalogo.Count - 1;
        }
        return idx;
    }

    // =========================================================================
    // GUARDADO, CARGA, RESTAURACIÓN Y LÍMITES SEGUROS
    // =========================================================================
    public void AplicarLimitesSeguros()
    {
        int maxS = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(MIN_PREGUNTAS, datosActuales.bancoServicio.Count));
        int maxP = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(MIN_PREGUNTAS, datosActuales.bancoPhishing.Count));
        int maxT = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(MIN_PREGUNTAS, datosActuales.bancoPsicologico.Count));

        datosActuales.cantidadServicioPorPartida = Mathf.Clamp(datosActuales.cantidadServicioPorPartida, MIN_PREGUNTAS, maxS);
        datosActuales.cantidadPhishingPorPartida = Mathf.Clamp(datosActuales.cantidadPhishingPorPartida, MIN_PREGUNTAS, maxP);
        datosActuales.cantidadPsicologicoPorPartida = Mathf.Clamp(datosActuales.cantidadPsicologicoPorPartida, MIN_PREGUNTAS, maxT);
    }

    public void GuardarEnDisco()
    {
        AplicarLimitesSeguros();
        string json = JsonUtility.ToJson(datosActuales, true);
        File.WriteAllText(rutaArchivoBanco, json);
        AplicarBancoALosModulos();
        Debug.Log("[Banco de Preguntas] Cambios guardados en: " + rutaArchivoBanco);
    }

    public void CargarDesdeDisco()
    {
        try
        {
            string json = File.ReadAllText(rutaArchivoBanco);
            datosActuales = JsonUtility.FromJson<DatosBancoCompleto>(json);
            AplicarLimitesSeguros();
        }
        catch
        {
            // Si el archivo se corrompe, restauramos el respaldo de fábrica sin bloquear el juego.
            RestaurarValoresDeFabrica();
        }
    }

    public void RestaurarValoresDeFabrica()
    {
        datosActuales = JsonUtility.FromJson<DatosBancoCompleto>(jsonFabricaOriginal);
        GuardarEnDisco();
    }

    // =========================================================================
    // ENVÍA LOS DATOS EDITADOS A LOS 3 SCRIPTS DE JUEGO
    // =========================================================================
    public void AplicarBancoALosModulos()
    {
        AplicarLimitesSeguros();

        // 1. Actualizar Servicio al Cliente
        if (scriptServicio != null && datosActuales.bancoServicio.Count > 0)
        {
            CasoCliente[] nuevosCasos = new CasoCliente[datosActuales.bancoServicio.Count];
            for (int i = 0; i < datosActuales.bancoServicio.Count; i++)
            {
                CasoClienteJSON origen = datosActuales.bancoServicio[i];
                CasoCliente c = new CasoCliente();
                c.nombrePolitica = origen.nombrePolitica;
                c.fotoCliente = ObtenerSpriteSeguro(catalogoSpritesNPC, origen.indiceNpcNormal);
                c.fotoClienteMolesto = ObtenerSpriteSeguro(catalogoSpritesNPC, origen.indiceNpcMolesto);
                c.mensajeCliente = origen.mensajeCliente;
                c.textoBoton1 = origen.textoBoton1;
                c.textoBoton2 = origen.textoBoton2;
                c.textoBoton3 = origen.textoBoton3;
                c.respuestaCorrecta = Mathf.Clamp(origen.respuestaCorrecta, 1, 3);
                c.retroalimentacionCaso = origen.retroalimentacionCaso;
                nuevosCasos[i] = c;
            }
            scriptServicio.niveles = nuevosCasos;
            scriptServicio.cantidadPreguntasPorPartida = datosActuales.cantidadServicioPorPartida;
        }

        // 2. Actualizar Detector de Phishing
        if (scriptPhishing != null && datosActuales.bancoPhishing.Count > 0)
        {
            CasoPhishing[] nuevosPhishing = new CasoPhishing[datosActuales.bancoPhishing.Count];
            for (int i = 0; i < datosActuales.bancoPhishing.Count; i++)
            {
                CasoPhishingJSON origen = datosActuales.bancoPhishing[i];
                CasoPhishing p = new CasoPhishing();
                p.nombreDelCaso = origen.nombreDelCaso;
                p.imagenDelMensaje = ObtenerSpriteSeguro(catalogoImagenesPhishing, origen.indiceImagenCatalogo);
                p.audioLlamadaCaso = ObtenerAudioSeguro(catalogoAudiosPhishing, origen.indiceAudioCatalogo);
                p.esPhishing = origen.esPhishing;
                p.retroalimentacionPhishing = origen.retroalimentacionPhishing;
                nuevosPhishing[i] = p;
            }
            scriptPhishing.nivelesPhishing = nuevosPhishing;
            scriptPhishing.cantidadPreguntasPorPartida = datosActuales.cantidadPhishingPorPartida;
        }

        // 3. Actualizar Test Psicológico / Perfil y Skills
        if (scriptPsicologico != null && datosActuales.bancoPsicologico.Count > 0)
        {
            scriptPsicologico.preguntas = datosActuales.bancoPsicologico.ToArray();
            scriptPsicologico.cantidadPreguntasPorPartida = datosActuales.cantidadPsicologicoPorPartida;
        }
    }

    private Sprite ObtenerSpriteSeguro(List<Sprite> catalogo, int indice)
    {
        if (catalogo == null || catalogo.Count == 0) return null;
        int idx = Mathf.Clamp(indice, 0, catalogo.Count - 1);
        return catalogo[idx];
    }

    private AudioClip ObtenerAudioSeguro(List<AudioClip> catalogo, int indice)
    {
        if (indice < 0 || catalogo == null || indice >= catalogo.Count) return null;
        return catalogo[indice];
    }
}
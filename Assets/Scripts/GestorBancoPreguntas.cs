using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class CasoPhishingEditable
{
    public string nombreDelCaso;
    public int indiceImagenCatalogo; // Apunta a una imagen del catálogo pre-cargado
    public int indiceAudioCatalogo = -1; // -1 significa sin audio de llamada
    public bool esPhishing;
    public string retroalimentacionPhishing;
}

[System.Serializable]
public class DatosBancoCompleto
{
    // Cantidades elegidas por el administrador para cada partida
    public int cantidadServicioPorPartida = 5;
    public int cantidadPhishingPorPartida = 5;
    public int cantidadPsicologicoPorPartida = 15;

    // Bancos editables (usa tus clases [System.Serializable] existentes)
    public List<CasoClienteEditable> bancoServicio = new List<CasoClienteEditable>();
    public List<CasoPhishingEditable> bancoPhishing = new List<CasoPhishingEditable>();
    public List<PreguntaPsicologica> bancoPsicologico = new List<PreguntaPsicologica>();
}

[System.Serializable]
public class CasoClienteEditable
{
    public string nombrePolitica;
    public int indiceNpcNormal;
    public int indiceNpcMolesto;
    public string mensajeCliente;
    public string textoBoton1;
    public string textoBoton2;
    public string textoBoton3;
    public int respuestaCorrecta;
    public string retroalimentacionCaso;
}

public class GestorBancoPreguntas : MonoBehaviour
{
    // Límites duros de seguridad para evitar que el administrador rompa el rendimiento
    public const int MAX_PREGUNTAS_BANCO = 50;
    public const int MAX_PREGUNTAS_PARTIDA = 20;

    [Header("Catálogos Visuales y de Audio (Para referenciar por índice en el JSON)")]
    public Sprite[] catalogoSpritesNPC;
    public Sprite[] catalogoImagenesPhishing;
    public AudioClip[] catalogoAudiosPhishing;

    [Header("Datos Activos en Memoria")]
    public DatosBancoCompleto datosActuales = new DatosBancoCompleto();

    private string rutaArchivoBanco;

    void Awake()
    {
        rutaArchivoBanco = Path.Combine(Application.persistentDataPath, "BancoPreguntas_NexoBank.json");
        CargarBancoDesdeDisco();
    }

    public void CargarBancoDesdeDisco()
    {
        if (File.Exists(rutaArchivoBanco))
        {
            string json = File.ReadAllText(rutaArchivoBanco);
            datosActuales = JsonUtility.FromJson<DatosBancoCompleto>(json);
            AplicarLimitesSeguros();
        }
        else
        {
            // Primera ejecución: guarda las preguntas iniciales configuradas en el Inspector
            AplicarLimitesSeguros();
            GuardarBancoEnDisco();
        }
    }

    public void GuardarBancoEnDisco()
    {
        AplicarLimitesSeguros();
        string json = JsonUtility.ToJson(datosActuales, true);
        File.WriteAllText(rutaArchivoBanco, json);
    }

    // Garantiza que nunca haya cantidades inválidas, cero preguntas o excesos que cuelguen la app
    public void AplicarLimitesSeguros()
    {
        int maxS = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(1, datosActuales.bancoServicio.Count));
        int maxP = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(1, datosActuales.bancoPhishing.Count));
        int maxT = Mathf.Min(MAX_PREGUNTAS_PARTIDA, Mathf.Max(1, datosActuales.bancoPsicologico.Count));

        datosActuales.cantidadServicioPorPartida = Mathf.Clamp(datosActuales.cantidadServicioPorPartida, 1, maxS);
        datosActuales.cantidadPhishingPorPartida = Mathf.Clamp(datosActuales.cantidadPhishingPorPartida, 1, maxP);
        datosActuales.cantidadPsicologicoPorPartida = Mathf.Clamp(datosActuales.cantidadPsicologicoPorPartida, 1, maxT);
    }

    // Ejemplo de método seguro para añadir una pregunta desde el Panel de Administrador
    public bool IntentarAgregarPreguntaPsicologica(PreguntaPsicologica nueva)
    {
        if (datosActuales.bancoPsicologico.Count >= MAX_PREGUNTAS_BANCO)
        {
            Debug.LogWarning("Límite máximo de 50 preguntas alcanzado en este módulo.");
            return false;
        }
        datosActuales.bancoPsicologico.Add(nueva);
        GuardarBancoEnDisco();
        return true;
    }

    public bool IntentarEliminarPreguntaPsicologica(int indice)
    {
        // Nunca permitimos dejar un módulo con 0 preguntas
        if (datosActuales.bancoPsicologico.Count <= 1 || indice < 0 || indice >= datosActuales.bancoPsicologico.Count)
            return false;

        datosActuales.bancoPsicologico.RemoveAt(indice);
        GuardarBancoEnDisco();
        return true;
    }
}
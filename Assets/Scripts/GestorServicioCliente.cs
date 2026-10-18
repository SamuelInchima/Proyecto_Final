using UnityEngine;
using TMPro; // Librería necesaria para modificar textos TextMeshPro en Unity 2022.

// [System.Serializable] permite que esta "plantilla" se pueda ver y rellenar en el Inspector de Unity.
// Representa la estructura de un nivel individual de atención al cliente.
[System.Serializable]
public class CasoCliente
{
    // AGREGAR QUE DESAPAREZCA EL PANEL DE NIVEL AL COMPLETAR EL MODULO

    [TextArea(2, 4)] // Hace que la cajita de texto en el Inspector sea más grande y cómoda de escribir.
    public string mensajeCliente; // La queja o pregunta que hace el cliente de NexoBank.

    public string textoBoton1;    // Texto para la primera opción de respuesta.
    public string textoBoton2;    // Texto para la segunda opción de respuesta.
    public string textoBoton3;    // Texto para la tercera opción de respuesta.

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta empática/correcta")]
    public int respuestaCorrecta; // Guardará el número (1, 2 o 3) del botón correcto en este nivel.
}

public class GestorServicioCliente : MonoBehaviour
{
    // ==========================================
    // VARIABLES DE INTERFAZ (TEXTOS Y PANELES)
    // ==========================================
    [Header("Elementos de Texto en Pantalla")]
    public TextMeshProUGUI textoPantallaCliente; // El texto donde se lee el mensaje del cliente.
    public TextMeshProUGUI textoEnBoton1;        // El texto dentro del Botón 1.
    public TextMeshProUGUI textoEnBoton2;        // El texto dentro del Botón 2.
    public TextMeshProUGUI textoEnBoton3;        // El texto dentro del Botón 3.

    [Header("Conexión con otros sistemas")]
    public GestorPenalizacion scriptPenalizacion; // Referencia a tu script anterior para poder restar estrellas.
    public GameObject panelVictoria;              // Panel que se encenderá al superar los 5 niveles.

    // ==========================================
    // DATOS DE LOS 5 NIVELES
    // ==========================================
    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles; // Lista (Array) donde escribiremos los 5 casos desde el Inspector.
    private int nivelActual = 0;  // Lleva la cuenta de en qué nivel vamos (en programación se empieza en 0).

    void Start()
    {
        // Nos aseguramos de que el panel de victoria esté apagado al empezar.
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(false);
        }

        // Cargamos el primer nivel (posición 0 de la lista) al iniciar.
        CargarNivel(0);
    }

    // ==========================================
    // MÉTODOS DE LÓGICA DEL JUEGO
    // ==========================================

    // Esta función actualiza los textos de la pantalla según el nivel en el que estemos.
    // Es 'public' por si deseas llamarla también desde un menú selector de niveles (1 al 5).
    public void CargarNivel(int indiceNivel)
    {
        // Guardamos el número de nivel actual
        nivelActual = indiceNivel;

        // Copiamos los textos de nuestra lista y los ponemos en los textos reales de la pantalla
        textoPantallaCliente.text = niveles[nivelActual].mensajeCliente;
        textoEnBoton1.text = niveles[nivelActual].textoBoton1;
        textoEnBoton2.text = niveles[nivelActual].textoBoton2;
        textoEnBoton3.text = niveles[nivelActual].textoBoton3;
    }

    // Esta función se conectará a los 3 botones de respuesta.
    // Recibe un número (1, 2 o 3) dependiendo de cuál botón presionó el jugador.
    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Comparamos si el botón que presionó el jugador es igual a la respuesta correcta de este nivel
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            Debug.Log("¡Respuesta profesional! Pasando al siguiente caso...");
            AvanzarSiguienteNivel();
        }
        else
        {
            Debug.Log("Respuesta poco profesional. Pierdes una estrella.");
            // Llamamos a la función RestarEstrella() de tu otro script
            scriptPenalizacion.RestarEstrella();
        }
    }

    // Método interno para pasar al siguiente caso o ganar el módulo
    private void AvanzarSiguienteNivel()
    {
        // Sumamos 1 para pasar al siguiente nivel
        nivelActual++;

        // Verificamos si todavía quedan niveles en la lista (por ejemplo, si aún no llegamos al 5)
        if (nivelActual < niveles.Length)
        {
            // Cargamos el nuevo nivel en pantalla
            CargarNivel(nivelActual);
        }
        else
        {
            // Si ya no hay más niveles en la lista, significa que superó los 5 casos con éxito
            Debug.Log("¡Módulo de Atención al Cliente completado!");
            if (panelVictoria != null)
            {
                panelVictoria.SetActive(true);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI; // Importamos la librería de Interfaz de Usuario de Unity para controlar el componente 'Image' donde se dibuja el personaje (NPC) del cliente.
using TMPro; // Librería para modificar los textos TextMeshPro de la interfaz.

// PLANTILLA DE DATOS SERIALIZABLE: CasoCliente
// '[System.Serializable]' convierte esta clase en un formulario editable dentro del Inspector de Unity.
// Cada elemento de la lista representará a un cliente distinto con su foto, su queja, sus 3 opciones de respuesta y el número de la opción correcta.
[System.Serializable]
public class CasoCliente
{
    [Tooltip("Arrastra aquí el dibujo 2D (Sprite) del cliente de este caso")]
    public Sprite fotoCliente; // Guarda la ilustración del personaje (NPC) para este nivel.

    [TextArea(2, 4)] // Hace que la caja de texto en el Inspector sea más grande y cómoda de escribir.
    public string mensajeCliente; // La queja o pregunta que hace el cliente.

    public string textoBoton1;    // Texto para la primera opción de respuesta (A).
    public string textoBoton2;    // Texto para la segunda opción de respuesta (B).
    public string textoBoton3;    // Texto para la tercera opción de respuesta (C).

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta correcta")]
    // Variable numérica entera ('int') donde escribimos en el Inspector cuál de los 3 botones (1, 2 o 3) tiene la respuesta correcta.
    public int respuestaCorrecta; // Guardará el número (1, 2 o 3) del botón correcto en este nivel.
}

// CLASE PRINCIPAL DEL MÓDULO: GestorServicioCliente
public class GestorServicioCliente : MonoBehaviour
{
    // VARIABLES DE INTERFAZ (TEXTOS E IMAGEN DEL NPC)
    [Header("Elementos de Texto en Pantalla")]
    public Image contenedorFotoCliente;          // El recuadro UI Image a donde aparecerá el NPC.
    public TextMeshProUGUI textoPantallaCliente; // El texto donde se lee el mensaje del cliente.
    public TextMeshProUGUI textoEnBoton1;        // El texto dentro del Botón 1 (A).
    public TextMeshProUGUI textoEnBoton2;        // El texto dentro del Botón 2 (B).
    public TextMeshProUGUI textoEnBoton3;        // El texto dentro del Botón 3 (C).

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas; // Conexión con el script que resta estrellas y maneja la victoria.

    // DATOS DE LOS 5 NIVELES
    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles; // Lista (Array) de los 5 casos configurables desde el Inspector.
    private int nivelActual = 0;  // Lleva la cuenta de en qué nivel vamos (empieza en 0).

    // AUDIO
    public GestorAudio scriptAudio;

    void Start()
    {
        // Cargamos el primer nivel (posición 0 de la lista) al iniciar.
        CargarNivel(0);
    }

    // MÉTODOS DE LÓGICA DEL JUEGO
    // Función pública que actualiza en la pantalla el dibujo del cliente, su mensaje y las 3 opciones de respuesta según el número 'indiceNivel' recibido.
    public void CargarNivel(int indiceNivel)
    {
        // Actualizamos nuestro contador interno con el nivel que vamos a mostrar.
        nivelActual = indiceNivel;

        // Verificación de seguridad doble ('&&' significa Y ADEMÁS, '!=' significa NO ESTÁ VACÍO):
        // Si asignaste una imagen de cliente en el Inspector, la colocamos en el recuadro de la pantalla.
        if (contenedorFotoCliente != null && niveles[nivelActual].fotoCliente != null)
        {
            // Colocamos el dibujo del cliente de este nivel ('.fotoCliente') dentro del recuadro de la pantalla ('.sprite').
            contenedorFotoCliente.sprite = niveles[nivelActual].fotoCliente;
        }

        // Copiamos los textos guardados en el cajón 'nivelActual' de nuestra lista y los escribimos en la propiedad '.text' de la pantalla y de los 3 botones.
        textoPantallaCliente.text = niveles[nivelActual].mensajeCliente;
        textoEnBoton1.text = niveles[nivelActual].textoBoton1;
        textoEnBoton2.text = niveles[nivelActual].textoBoton2;
        textoEnBoton3.text = niveles[nivelActual].textoBoton3;
    }

    // En lugar de crear 3 funciones separadas para los 3 botones de opción, creamos UNA SOLA función inteligente que recibe un número entero ('int botonPresionado').
    // En el Inspector de Unity, configuramos que el Botón 1 envíe el número 1, el Botón 2 envíe el 2, y el Botón 3 envíe el 3.
    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Comparamos con '==' si el número del botón que presionó el jugador es exactamente igual al número de 'respuestaCorrecta' de este cliente.
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            // ¡Acertó!
            Debug.Log("¡Respuesta profesional! Pasando al siguiente caso...");

            // Hacemos sonar el efecto de audio de acierto.
            scriptAudio.ReproducirAcierto();

            // Llamamos a la función interna para pasar al siguiente cliente de la fila.
            AvanzarSiguienteNivel();

        }
        else
        {
            // DE LO CONTRARIO (eligió una de las dos respuestas incorrectas):
            Debug.Log("Respuesta poco profesional. Pierdes una estrella.");

            // Le ordenamos a 'GestorMecanicas' que apague una estrella y haga sonar la alerta de error.
            scriptMecanicas.RestarEstrella();
        }
    }

    // Método interno para avanzar al siguiente cliente o declarar la victoria si ya atendió a todos.
    private void AvanzarSiguienteNivel()
    {
        // Sumamos 1 para pasar al siguiente nivel
        nivelActual++;

        // Si aún quedan clientes en la lista, cargamos el siguiente (cambiando de texto y de NPC automáticamente).
        if (nivelActual < niveles.Length)
        {
            // Si aún quedan clientes por atender, actualizamos la pantalla con los datos y el NPC del nuevo nivel.
            CargarNivel(nivelActual);
        }
        else
        {
            // Si ya no quedan más clientes en la lista, significa que completó los 5 niveles con éxito.
            Debug.Log("¡Módulo de Atención al Cliente completado!");

            // Le ordenamos a 'Gestormecanicas' que detenga el cronómetro, encienda el panel de Victoria y muestre las estadisticas.
            scriptMecanicas.VictoriaServicio();
        }
    }
}
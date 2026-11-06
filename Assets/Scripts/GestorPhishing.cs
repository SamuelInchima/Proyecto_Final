using UnityEngine;
using UnityEngine.UI;

// PLANTILLA DE DATOS SERIALIZABLE: CasoPhishing
// Por defecto, Unity solo muestra en el Inspector scripts que tienen ': MonoBehaviour'.
// Al escribir '[System.Serializable]' encima de una clase normal, le ordenamos a Unity que convierta esta clase en un "formulario desplegable"
// dentro del Inspector para poder rellenar los 5 casos (su nombre, su imagen y si es trampa o no) sin tener que programar un script por cada nivel.
[System.Serializable]
public class CasoPhishing
{
    [Tooltip("Solo sirve para que te guíes en el Inspector (ej. 'Nivel 1: SMS Falso' o 'Nivel 2: Correo RRHH')")]
    public string nombreDelCaso; // Un texto opcional de guía para organizarte en el editor.

    [Tooltip("Arrastra aquí la imagen del correo, SMS o pantalla creada por el diseñador")]
    public Sprite imagenDelMensaje; // 'Sprite' es el archivo de imagen 2D que vamos a mostrar.

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing; // Guarda Verdadero (true) si es una trampa, o Falso (false) si es real.
}

// CLASE PRINCIPAL DEL MÓDULO: GestorPhishing
public class GestorPhishing : MonoBehaviour
{
    // VARIABLES DE LA INTERFAZ (UI)
    [Header("Elementos en Pantalla")]
    // - 'Image' (contenedorImagenPantalla) es el "portarretratos" fijo que está puesto en el Canvas de tu escena.
    // - 'Sprite' (imagenDelMensaje) es la "fotografía" que metemos y cambiamos dentro de ese portarretratos en cada nivel.
    public Image contenedorImagenPantalla;

    [Header("Conexión con otros sistemas")]
    public GestorMecanicas scriptMecanicas; // Referencia a tu script que apaga los 3 candados.

    // DATOS DE LOS 5 NIVELES
    [Header("Lista de los 5 Casos (Correos, SMS, etc.)")]
    // Creamos un arreglo/lista ('[]') de nuestra plantilla 'CasoPhishing'. En el Inspector le ponemos tamaño 5 para cargar las 5 imágenes del módulo.
    public CasoPhishing[] nivelesPhishing; // Array (lista) que guardará las 5 imágenes y sus respuestas.

    // Variable privada entera que recuerda en cuál caso de la lista estamos jugando actualmente (empieza en el caso 0).
    private int nivelActual = 0;           // Lleva la cuenta de en qué imagen vamos (empieza en 0).

    //AUDIO
    public GestorAudio scriptAudio;

    void Start()
    {
        // Cargamos la primera imagen de nuestra lista (posición 0).
        CargarImagenNivel(0);
    }

    // MÉTODO PARA ACTUALIZAR LA IMAGEN EN PANTALLA
    // Esta función recibe entre paréntesis el número de caso ('indice') que queremos mostrar en pantalla (0, 1, 2, 3 o 4).
    public void CargarImagenNivel(int indice)
    {
        // Guardamos en 'nivelActual' el número de nivel que nos pidieron cargar.
        nivelActual = indice;

        // Buscamos en la lista 'nivelesPhishing' el cajón 'nivelActual', sacamos su foto ('.imagenDelMensaje')
        // y la colocamos dentro de la propiedad '.sprite' de nuestro portarretratos ('contenedorImagenPantalla').
        contenedorImagenPantalla.sprite = nivelesPhishing[nivelActual].imagenDelMensaje;
    }

    // MÉTODOS PARA LOS BOTONES (VERDE Y ROJO)
    // 1. Esta función se conectará al BOTÓN VERDE ("Seguro")
    public void BotonVerdeSeguro()
    {
        // Si el jugador presiona "Seguro", revisamos si en este nivel 'esPhishing' es Falso (false).
        if (nivelesPhishing[nivelActual].esPhishing == false)
        {
            // ¡Acertó! El mensaje o correo era legítimo de NexoBank.
            Debug.Log("¡Correcto! El mensaje era seguro. Pasando al siguiente caso...");

            scriptAudio.ReproducirAcierto();

            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Era un ataque (correo falso, SMS con virus, etc.) y el jugador confió en él.
            Debug.Log("¡Error! Era una trampa de Phishing. Pierdes un candado de seguridad.");
            scriptMecanicas.RestarCandado();
        }
    }

    // 2. Esta función se conectará al BOTÓN ROJO ("Peligroso / Reportar")
    public void BotonRojoPeligroso()
    {
        // Si el jugador presiona "Peligroso", revisamos si en este nivel 'esPhishing' es Verdadero (true).
        if (nivelesPhishing[nivelActual].esPhishing == true)
        {
            // ¡Acertó! Detectó las pistas sospechosas en la imagen (mala ortografía, link falso, etc.).
            Debug.Log("¡Excelente! Detectaste el intento de Phishing. Pasando al siguiente caso...");

            scriptAudio.ReproducirAcierto();

            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Reportó un mensaje o correo real de la empresa por equivocación.
            Debug.Log("¡Error! Reportaste un mensaje oficial de NexoBank. Pierdes un candado.");
            scriptMecanicas.RestarCandado();
        }
    }

    // MÉTODO INTERNO PARA AVANZAR DE NIVEL
    private void AvanzarSiguienteCaso()
    {
        // Sumamos 1 para pasar a la siguiente imagen de la lista.
        nivelActual++;

        // Verificamos si todavía quedan imágenes por revisar en la lista (menos de 5).
        if (nivelActual < nivelesPhishing.Length)
        {
            // Mostramos la nueva imagen en pantalla.
            CargarImagenNivel(nivelActual);
        }
        else
        {
            // Si ya superó las 5 imágenes, encendemos la pantalla de victoria.
            Debug.Log("¡Felicidades! Superaste los 5 niveles del Detector de Phishing.");

            // Llamamos al GestorPenalizacion para detener el reloj y mostrar estadísticas de candados
            scriptMecanicas.VictoriaPhishing();
        }
    }
}
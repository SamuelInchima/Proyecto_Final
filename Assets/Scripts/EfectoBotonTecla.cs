// Importamos las herramientas básicas de Unity.
using UnityEngine;
// NUEVO PARA TU SUSTENTACIÓN: 'UnityEngine.EventSystems' es la librería que detecta eventos del ratón
// sobre la interfaz, como cuándo presionamos el clic (PointerDown) y cuándo lo soltamos (PointerUp).
using UnityEngine.EventSystems;
// Importamos 'System.Collections' para hacer que el hundimiento del botón sea suave con una Corrutina.
using System.Collections;

// Además de 'MonoBehaviour', agregamos 'IPointerDownHandler' e 'IPointerUpHandler'.
// Estas dos interfaces le avisan automáticamente a nuestro script cuando el botón es presionado o soltado.
public class EfectoBotonTecla : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuración del Efecto de Tecla")]
    [Tooltip("Cuánto se achica el botón al hundirse (0.93 = 93% de su tamaño original)")]
    public float escalaHundido = 0.93f;

    [Tooltip("Cuántos píxeles baja el botón al presionarlo para simular que se hunde")]
    public float pixelesHundimiento = 5f;

    [Tooltip("Velocidad con la que baja y sube el botón")]
    public float velocidad = 18f;

    // Variables privadas para recordar la posición y el tamaño originales del botón antes de presionarlo.
    private Vector3 escalaOriginal;
    private Vector3 posicionOriginal;
    private bool inicializado = false;

    void Awake()
    {
        // Guardamos el tamaño y posición originales del botón apenas nace en la escena.
        escalaOriginal = transform.localScale;
        posicionOriginal = transform.localPosition;
        inicializado = true;
    }

    // Si el panel donde vive el botón se apaga y se vuelve a encender, restauramos su forma normal.
    void OnEnable()
    {
        if (inicializado)
        {
            transform.localScale = escalaOriginal;
            transform.localPosition = posicionOriginal;
        }
    }

    // 1. Se ejecuta AUTOMÁTICAMENTE cuando mantienes presionado el clic sobre el botón.
    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        // Calculamos la posición un poco más abajo en el eje Y (-pixelesHundimiento).
        Vector3 destinoPosicion = posicionOriginal + new Vector3(0f, -pixelesHundimiento, 0f);
        Vector3 destinoEscala = escalaOriginal * escalaHundido;

        StartCoroutine(AnimarTecla(destinoEscala, destinoPosicion));
    }

    // 2. Se ejecuta AUTOMÁTICAMENTE cuando sueltas el clic del ratón.
    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        // Regresamos suavemente el botón a su tamaño y posición originales.
        StartCoroutine(AnimarTecla(escalaOriginal, posicionOriginal));
    }

    // Corrutina que mueve y escala el botón fotograma a fotograma de forma suave.
    private IEnumerator AnimarTecla(Vector3 metaEscala, Vector3 metaPosicion)
    {
        while (Vector3.Distance(transform.localScale, metaEscala) > 0.005f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, metaEscala, Time.deltaTime * velocidad);
            transform.localPosition = Vector3.Lerp(transform.localPosition, metaPosicion, Time.deltaTime * velocidad);
            yield return null;
        }

        transform.localScale = metaEscala;
        transform.localPosition = metaPosicion;
    }
}
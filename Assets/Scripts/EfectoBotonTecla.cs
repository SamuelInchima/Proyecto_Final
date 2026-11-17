using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EfectoBotonTecla : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuración del Efecto de Tecla")]
    [Tooltip("Cuánto se achica el botón al hundirse (0.93 = 93% de su tamaño original)")]
    public float escalaHundido = 0.93f;

    [Tooltip("Velocidad con la que baja y sube el botón")]
    public float velocidad = 25f;

    private Vector3 escalaOriginal;
    private bool inicializado = false;

    void Awake()
    {
        escalaOriginal = transform.localScale;
        inicializado = true;
    }

    void OnEnable()
    {
        if (inicializado)
        {
            transform.localScale = escalaOriginal;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimarEscala(escalaOriginal * escalaHundido));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(AnimarEscala(escalaOriginal));
    }

    private IEnumerator AnimarEscala(Vector3 metaEscala)
    {
        while (Vector3.Distance(transform.localScale, metaEscala) > 0.005f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, metaEscala, Time.deltaTime * velocidad);
            yield return null;
        }

        transform.localScale = metaEscala;
    }
}
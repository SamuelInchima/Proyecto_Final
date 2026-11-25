using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomYMoverUI : MonoBehaviour, IDragHandler, IScrollHandler
{
    public Canvas canvasPrincipal;
    public bool permitirZoom = true;
    public bool reiniciarAlAbrir = false;
    public float pasoZoom = 0.15f;
    public float escalaMinima = 0.65f;
    public float escalaMaxima = 1.75f;
    public float limiteHorizontal = 550f;
    public float limiteVertical = 300f;

    private RectTransform rectObjeto;
    private Vector2 posicionInicial;
    private Vector3 escalaInicial;
    private bool inicializado = false;

    void Awake()
    {
        rectObjeto = GetComponent<RectTransform>();
        posicionInicial = rectObjeto.anchoredPosition;
        escalaInicial = rectObjeto.localScale;
        inicializado = true;

        if (canvasPrincipal == null)
            canvasPrincipal = GetComponentInParent<Canvas>();
    }

    void OnEnable()
    {
        if (inicializado && reiniciarAlAbrir)
            BotonRestablecer();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 nuevaPos = rectObjeto.anchoredPosition + (eventData.delta / canvasPrincipal.scaleFactor);
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, -limiteHorizontal, limiteHorizontal);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, -limiteVertical, limiteVertical);
        rectObjeto.anchoredPosition = nuevaPos;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!permitirZoom) return;

        if (eventData.scrollDelta.y > 0)
            BotonAgrandar();
        else if (eventData.scrollDelta.y < 0)
            BotonAchicar();
    }

    public void BotonAgrandar()
    {
        if (!permitirZoom) return;
        float nuevaEscala = Mathf.Clamp(rectObjeto.localScale.x + pasoZoom, escalaMinima, escalaMaxima);
        rectObjeto.localScale = new Vector3(nuevaEscala, nuevaEscala, 1f);
    }

    public void BotonAchicar()
    {
        if (!permitirZoom) return;
        float nuevaEscala = Mathf.Clamp(rectObjeto.localScale.x - pasoZoom, escalaMinima, escalaMaxima);
        rectObjeto.localScale = new Vector3(nuevaEscala, nuevaEscala, 1f);
    }

    public void BotonRestablecer()
    {
        rectObjeto.anchoredPosition = posicionInicial;
        rectObjeto.localScale = escalaInicial;
    }
}
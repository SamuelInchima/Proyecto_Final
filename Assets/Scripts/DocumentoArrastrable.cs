using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentoArrastrable : MonoBehaviour, IDragHandler
{
    public Canvas canvasPrincipal;
    private RectTransform rectDocumento;

    void Start()
    {
        rectDocumento = GetComponent<RectTransform>();
        if (canvasPrincipal == null)
            canvasPrincipal = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectDocumento.anchoredPosition += eventData.delta / canvasPrincipal.scaleFactor;
    }
}
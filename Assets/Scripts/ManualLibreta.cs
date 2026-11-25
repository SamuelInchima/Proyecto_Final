using UnityEngine;
using System.Collections;
using TMPro;

public class ManualLibreta : MonoBehaviour
{
    public GameObject[] paginas;
    public RectTransform contenedorPaginas;
    public TextMeshProUGUI textoNumeroPagina;
    public AudioSource reproductorSonido;
    public AudioClip sonidoPasarHoja;
    public float velocidadGiro = 10f;

    private int paginaActual = 0;
    private bool animando = false;

    void OnEnable()
    {
        animando = false;
        paginaActual = 0;
        if (contenedorPaginas != null)
            contenedorPaginas.localScale = Vector3.one;

        MostrarSoloPagina(0);
    }

    public void BotonPaginaSiguiente()
    {
        if (animando || paginaActual >= paginas.Length - 1) return;
        StartCoroutine(AnimarCambioHoja(paginaActual + 1));
    }

    public void BotonPaginaAnterior()
    {
        if (animando || paginaActual <= 0) return;
        StartCoroutine(AnimarCambioHoja(paginaActual - 1));
    }

    IEnumerator AnimarCambioHoja(int nuevaPagina)
    {
        animando = true;

        if (reproductorSonido != null && sonidoPasarHoja != null)
            reproductorSonido.PlayOneShot(sonidoPasarHoja);

        float x = 1f;
        while (x > 0.02f)
        {
            x -= Time.deltaTime * velocidadGiro;
            if (x < 0f) x = 0f;
            contenedorPaginas.localScale = new Vector3(x, 1f, 1f);
            yield return null;
        }

        MostrarSoloPagina(nuevaPagina);

        while (x < 0.98f)
        {
            x += Time.deltaTime * velocidadGiro;
            if (x > 1f) x = 1f;
            contenedorPaginas.localScale = new Vector3(x, 1f, 1f);
            yield return null;
        }

        contenedorPaginas.localScale = Vector3.one;
        animando = false;
    }

    void MostrarSoloPagina(int indice)
    {
        paginaActual = indice;
        for (int i = 0; i < paginas.Length; i++)
        {
            paginas[i].SetActive(i == paginaActual);
        }

        if (textoNumeroPagina != null)
            textoNumeroPagina.text = "Página " + (paginaActual + 1) + " / " + paginas.Length;
    }
}
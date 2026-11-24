using UnityEngine;

public class PistaClicable : MonoBehaviour
{
    public GameObject marcaVisual;
    public GestorTurnoEscritorio gestor;
    private bool yaEncontrada = false;

    public void AlHacerClicEnPista()
    {
        if (yaEncontrada) return;

        yaEncontrada = true;
        marcaVisual.SetActive(true);
        gestor.RegistrarPistaEncontrada();
    }

    public void ReiniciarPista()
    {
        yaEncontrada = false;
        marcaVisual.SetActive(false);
    }
}
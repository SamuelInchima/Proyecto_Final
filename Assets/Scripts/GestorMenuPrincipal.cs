using UnityEngine; // Esta línea es obligatoria, le dice al script que use las herramientas básicas de Unity.

public class GestorMenuPrincipal : MonoBehaviour
{
    // VARIABLES
    // Usamos 'public' para poder ver estas variables en el Inspector de Unity.
    // 'GameObject' representa cualquier objeto en la escena (en este caso, nuestros paneles).

    [Header("Paneles de la Interfaz")] // Esto solo pone un título bonito en el Inspector de Unity para organizar.
    public GameObject panelMenuPrincipal;   // Aquí arrastraremos el panel del menú de inicio (NexoBank).
    public GameObject panelSelectorModulos; // Aquí arrastraremos el panel donde se elige Servicio o Phishing.
    public GameObject panelInstrucciones;   // Aquí arrastraremos el panel del bloc de notas con las instrucciones.

    // MÉTODO START
    // Todo lo que esté dentro de Start() se ejecuta una sola vez justo cuando arranca el juego.
    void Start()
    {
        // Al iniciar el juego, queremos asegurarnos de que el jugador solo vea el Menú Principal.
        // SetActive(true) enciende el objeto, SetActive(false) lo apaga.
        panelMenuPrincipal.SetActive(true);

        // Apagamos los otros dos paneles por si se quedaron encendidos por accidente en el editor.
        panelSelectorModulos.SetActive(false);
        panelInstrucciones.SetActive(false);
    }

    // MÉTODOS PARA LOS BOTONES
    // Usamos 'public void' para que los botones de Unity puedan encontrar y ejecutar estas funciones.

    // 1. Esta función se conectará al botón "Jugar"
    public void BotonJugar()
    {
        // Apagamos el Menú Principal
        panelMenuPrincipal.SetActive(false);
        // Encendemos el panel para elegir el módulo
        panelSelectorModulos.SetActive(true);
    }

    // 2. Esta función se conectará al botón "Instrucciones"
    public void BotonInstrucciones()
    {
        // Apagamos el Menú Principal
        panelMenuPrincipal.SetActive(false);
        // Encendemos el panel de Instrucciones
        panelInstrucciones.SetActive(true);
    }

    // 3. Esta función se conectará a los botones "Volver" (tanto en instrucciones como en el selector)
    public void BotonVolver()
    {
        // No importa en qué pantalla estemos, apagamos ambas por seguridad...
        panelInstrucciones.SetActive(false);
        panelSelectorModulos.SetActive(false);

        // ...y volvemos a encender el Menú Principal
        panelMenuPrincipal.SetActive(true);
    }

    // 4. Esta función se conectará al botón "Salir"
    public void BotonSalir()
    {
        // Mensaje de prueba: Como Application.Quit() no cierra el editor de Unity, 
        // usamos Debug.Log para ver en la consola que el botón sí está funcionando.
        Debug.Log("Saliendo del simulador de NexoBank...");

        // Esta es la orden real que cierra el juego cuando ya está exportado como un programa final (.exe).
        Application.Quit();
    }
}
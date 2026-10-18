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

    [Header("Paneles de los Módulos")]
    public GameObject panelNivelServicio;  // Arrastra aquí el panel del juego de Servicio al Cliente.
    public GameObject panelNivelPhishing;  // Arrastra aquí el panel del juego de Phishing.

    [Header("Conexión con los otros Scripts")]
    public GestorPenalizacion scriptPenalizacion;
    public GestorServicioCliente scriptServicio;
    public GestorPhishing scriptPhishing;

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

        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);
    }

    // MÉTODOS DEL MENÚ PRINCIPAL
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

    // MÉTODOS: SELECTOR DE MÓDULOS

    // 1. Esta función se conectará al botón "Servicio al Cliente" dentro del panel Selector - Al presionar el botón "Servicio al Cliente" en el Selector
    public void BotonModuloServicio()
    {
        // Apagamos el selector para despejar la pantalla
        panelSelectorModulos.SetActive(false);

        // Encendemos el nivel de Atención al Cliente (donde luego pondrás las 3 estrellas)
        panelNivelServicio.SetActive(true);

        // Reiniciamos el módulo al nivel 0, restauramos las 3 estrellas y arrancamos el reloj
        scriptServicio.CargarNivel(0);
        scriptPenalizacion.PrepararModuloServicio();
    }

    // 2. Esta función se conectará al botón "Detector de Phishing" dentro del panel Selector - Al presionar el botón "Detector de Phishing" en el Selector
    public void BotonModuloPhishing()
    {
        // Apagamos el selector
        panelSelectorModulos.SetActive(false);

        // Encendemos el nivel de Phishing (donde luego pondrás los 3 candados)
        panelNivelPhishing.SetActive(true);

        // Reiniciamos el módulo a la imagen 0, restauramos los 3 candados y arrancamos el reloj
        scriptPhishing.CargarImagenNivel(0);
        scriptPenalizacion.PrepararModuloPhishing();
    }

    // 3. Esta función sirve para un botón de "Atrás" o "Volver" dentro de los propios niveles - Al volver al selector (desde el nivel, desde Game Over o desde Victoria)
    public void BotonVolverASelector()
    {
        scriptPenalizacion.DetenerReloj(); // Apagamos el reloj por si salió a mitad de partida

        // Apagamos cualquier nivel en el que estemos
        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);

        scriptPenalizacion.panelGameOver.SetActive(false);
        scriptPenalizacion.panelVictoria.SetActive(false);

        // Y volvemos a encender la pantalla de selección de módulos
        panelSelectorModulos.SetActive(true);
    }
}
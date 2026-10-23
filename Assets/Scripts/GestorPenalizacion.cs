using TMPro; // Librería necesaria para modificar los textos de la interfaz (reloj, mensajes finales).
using UnityEngine;

public class GestorPenalizacion : MonoBehaviour
{
    // 1. VARIABLES DE VIDAS (ESTRELLAS Y CANDADOS)
    // Usamos corchetes [] para indicar que esto es un "Array" (una lista de varios objetos, no solo uno).
    [Header("Vidas del Módulo de Servicio")]
    public GameObject[] estrellas; // Las 3 estrellas que se ven mientras juegas Servicio al Cliente.
    private int erroresServicio = 0; // Esta variable interna (privada) cuenta cuántas veces se ha equivocado el jugador en los niveles de Servicio al Cliente.

    [Header("Vidas del Módulo de Phishing")]
    public GameObject[] candados; // Los 3 candados que se ven mientras juegas Phishing.
    private int erroresPhishing = 0; // Cuenta los fallos en Phishing.


    // 2. VARIABLES DEL CONTADOR DE TIEMPO
    [Header("Configuración del Cronómetro")]
    public float tiempoPorModulo = 60f; // Tiempo máximo en segundos para completar un módulo (ej. 60 segundos).
    private float tiempoRestante; // Variable interna que irá bajando segundo a segundo.
    private bool relojActivo = false; // Interruptor: true = el reloj corre, false = el reloj se detiene.
    public TextMeshProUGUI textoReloj; // El texto en pantalla donde se verán los segundos.


    // 3. VARIABLES DE VICTORIA Y ESTADÍSTICAS
    [Header("Pantallas de Fin de Juego")]
    public GameObject panelGameOver; // Panel que se activa si llegas a 3 errores o se acaba el tiempo.
    public GameObject panelVictoria; // Panel de victoria donde mostraremos las estadísticas.

    //public GameObject panelNivelServicio;         ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
    //public GameObject panelNivelPhishing;         ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER

    [Header("Elementos de Estadísticas (Dentro del Panel Victoria)")]
    public TextMeshProUGUI textoMensajeVictoria; // Texto que cambiará según las vidas con las que termines.
    public GameObject[] iconosVictoria; // 3 iconos (estrellas) dentro del panel de victoria.


    // 4. SÍMBOLOS DE MÓDULO COMPLETADO (ESTÉTICO)
    [Header("Insignias en el Selector de Módulos")]
    public GameObject simboloCompletadoServicio; // Imagen de "Chulito/Completado" junto al botón de Servicio.
    public GameObject simboloCompletadoPhishing; //Misma imagen junto al botón de Phishing.

    void Start()
    {
        // Por seguridad, nos aseguramos de apagar las pantallas de fin de juego al iniciar.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Apagamos los símbolos de completado al iniciar el juego por primera vez.
        simboloCompletadoServicio.SetActive(false);
        simboloCompletadoPhishing.SetActive(false);

        // Escondemos el objeto de texto del reloj para que no se vea en el Menú Principal.
        textoReloj.gameObject.SetActive(false);
    }

    // MÉTODO UPDATE: Se ejecuta automáticamente en cada fotograma del juego (muchas veces por segundo). - Es el lugar ideal para manejar cronómetros.
    void Update()
    {
        // Solo descontamos tiempo si el interruptor 'relojActivo' está encendido (true).
        if (relojActivo == true)
        {
            // Time.deltaTime es el tiempo exacto en segundos que pasó desde el último fotograma.
            tiempoRestante = tiempoRestante - Time.deltaTime;

            // Convertimos el tiempo con decimales a un número entero (int) para que se vea limpio en pantalla.
            int segundosEnteros = Mathf.CeilToInt(tiempoRestante);
            textoReloj.text = "Tiempo: " + segundosEnteros.ToString() + "s";

            // Si el tiempo llega a cero o menos, el jugador pierde por tiempo agotado.
            if (tiempoRestante <= 0)
            {
                textoReloj.text = "Tiempo: 0s";
                MostrarGameOver();
            }
        }
    }

    // MÉTODOS PARA INICIAR, REINICIAR MÓDULOS Y OCULTAR EL RELOJ
    // Se llama al entrar al módulo de Servicio para preparar las vidas y arrancar el reloj.
    public void PrepararModuloServicio()
    {
        erroresServicio = 0; // Reiniciamos los errores a cero para poder jugar y volver a jugar normalmente.

        // Encendemos de nuevo las 3 estrellas por si se habían apagado en una partida anterior.
        estrellas[0].SetActive(true);
        estrellas[1].SetActive(true);
        estrellas[2].SetActive(true);

        // Reiniciamos las pantallas de victoria/derrota.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Hacemos visible el texto del reloj en pantalla y arrancamos la cuenta atrás (el cronómetro).
        textoReloj.gameObject.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    // Se llama al entrar al módulo de Phishing para preparar los candados y arrancar el reloj.
    public void PrepararModuloPhishing()
    {
        erroresPhishing = 0; // Reiniciamos los errores a cero.

        // Encendemos de nuevo los 3 candados.
        candados[0].SetActive(true);
        candados[1].SetActive(true);
        candados[2].SetActive(true);

        // Reiniciamos pantallas.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Hacemos visible el texto del reloj en pantalla y arrancamos la cuenta atrás.
        textoReloj.gameObject.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    // Método para detener y esconder el reloj si el jugador sale al menú antes de terminar (o al ganar, perder o volver al selector).
    public void DetenerReloj()
    {
        relojActivo = false; // Detiene el conteo matemático.

        //Apaga el objeto de texto en la interfaz para que desaparezca de la pantalla.
        textoReloj.gameObject.SetActive(false);
    }

    // MÉTODOS PARA RESTAR VIDAS
    // Esta función se conecta a los botones de "Respuesta Incorrecta" en Servicio al Cliente
    public void RestarEstrella()
    {
        // Verificamos si el jugador aún tiene estrellas (es decir, menos de 3 errores)
        if (erroresServicio < 3)
        {
            // Los Arrays en programación empiezan a contar desde el 0.
            // Si erroresServicio es 0, apagará la estrella en la posición 0.
            estrellas[erroresServicio].SetActive(false);

            // Sumamos 1 al contador de errores
            erroresServicio++;

            // Si después de sumar, llegamos a 3 errores, perdimos.
            if (erroresServicio >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // Esta función se conecta a los botones de "Respuesta Incorrecta" (marcar como seguro un correo falso)
    public void RestarCandado()
    {
        if (erroresPhishing < 3)
        {
            candados[erroresPhishing].SetActive(false);
            erroresPhishing++;

            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // MÉTODOS DE VICTORIA Y ESTADÍSTICAS
    // Esta función la llamará el módulo de Servicio al Cliente al superar sus 5 niveles.
    public void VictoriaServicio()
    {
        //Llamamos a DetenerReloj() para que frene el tiempo y además oculte el texto.
        DetenerReloj();

        relojActivo = false; // Detenemos el cronómetro porque ya ganó.
        panelVictoria.SetActive(true); // Mostramos la pantalla de victoria.

        // Encendemos el símbolo estético de "Completado" en el selector de módulos.
        simboloCompletadoServicio.SetActive(true);

        // Calculamos con cuántas estrellas terminó (3 menos los errores cometidos).
        int estrellasFinales = 3 - erroresServicio;
        ActualizarIconosVictoria(estrellasFinales);

        // Mostramos un mensaje diferente según las estrellas conservadas.
        if (estrellasFinales == 3)
        {
            textoMensajeVictoria.text = "¡Desempeño Perfecto (3/3 Estrellas)!\nAtendiste a todos los clientes con total profesionalidad.";
        }
        else if (estrellasFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Estrellas)!\nSuperaste el turno, pero ten más paciencia con los clientes difíciles.";
        }
        else if (estrellasFinales == 1)
        {
            textoMensajeVictoria.text = "¡Al límite (1/3 Estrellas)!\nEstuviste a punto de ser despedido.";
        }
    }

    // Esta función la llamará el módulo de Phishing al superar sus 5 niveles.
    public void VictoriaPhishing()
    {
        // Llamamos a DetenerReloj() para que frene el tiempo y además oculte el texto.
        DetenerReloj();

        relojActivo = false; // Detenemos el cronómetro.
        panelVictoria.SetActive(true); // Mostramos la pantalla de victoria.

        // Encendemos el símbolo estético de "Completado" en el selector de módulos.
        simboloCompletadoPhishing.SetActive(true);

        // Calculamos con cuántos candados terminó.
        int candadosFinales = 3 - erroresPhishing;
        ActualizarIconosVictoria(candadosFinales);

        // Mostramos un mensaje diferente según los candados conservados.
        if (candadosFinales == 3)
        {
            textoMensajeVictoria.text = "¡Seguridad Impecable (3/3 Candados)!\nDescartaste todas las amenazas sin fallar.";
        }
        else if (candadosFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Candados)!\nProtegiste la seguridad de NexoBank, pero tuviste un descuido. Revisa bien los remitentes y detalles.";
        }
        else if (candadosFinales == 1)
        {
            textoMensajeVictoria.text = "¡Alerta de Riesgo (1/3 Candados)!\nSobreviviste por poco. Un error más y NexoBank habría sido hackeado.";
        }
    }

    // Método auxiliar sencillo para encender 1, 2 o 3 iconos en la pantalla de victoria.
    private void ActualizarIconosVictoria(int cantidadVidas)
    {
        // Primero apagamos los 3 iconos de la pantalla de victoria.
        iconosVictoria[0].SetActive(false);
        iconosVictoria[1].SetActive(false);
        iconosVictoria[2].SetActive(false);

        // Encendemos el primero si terminó con al menos 1 vida.
        if (cantidadVidas >= 1)
        {
            iconosVictoria[0].SetActive(true);
        }
        // Encendemos el segundo si terminó con al menos 2 vidas.
        if (cantidadVidas >= 2)
        {
            iconosVictoria[1].SetActive(true);
        }
        // Encendemos el tercero si terminó con las 3 vidas intactas.
        if (cantidadVidas == 3)
        {
            iconosVictoria[2].SetActive(true);
        }
    }

    // Método interno para mostrar la derrota (por 3 errores o por tiempo agotado).
    private void MostrarGameOver()
    {
        //panelNivelServicio.SetActive(false);           ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
        //panelNivelPhishing.SetActive(false);           ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER

        // También ocultamos el reloj si el jugador pierde la partida.
        DetenerReloj();
        relojActivo = false; // Detenemos el reloj inmediatamente.
        panelGameOver.SetActive(true); // Mostramos la pantalla de Game Over.
    }
}
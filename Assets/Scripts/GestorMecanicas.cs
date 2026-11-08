using TMPro; // Librería necesaria para modificar los textos de la interfaz (reloj, mensajes finales).
using UnityEngine;

public class GestorMecanicas : MonoBehaviour
{
    // VARIABLES DE VIDAS (ESTRELLAS Y CANDADOS)
    // Usamos corchetes [] para indicar que esto es un "Array" (una lista de varios objetos, no solo uno).
    [Header("Vidas del Módulo de Servicio")]
    public GameObject[] estrellas; // Las 3 estrellas que se ven mientras juegas Servicio al Cliente.
    private int erroresServicio = 0; // Esta variable interna (privada) cuenta cuántas veces se ha equivocado el jugador en los niveles de Servicio al Cliente.

    [Header("Vidas del Módulo de Phishing")]
    public GameObject[] candados; // Los 3 candados que se ven mientras juegas Phishing.
    private int erroresPhishing = 0; // Cuenta los fallos en Phishing.


    // VARIABLES DEL CONTADOR DE TIEMPO
    [Header("Configuración del Cronómetro")]
    public float tiempoPorModulo = 60f; // Tiempo máximo en segundos para completar un módulo (ej. 60 segundos).
    private float tiempoRestante; // Variable interna que irá bajando segundo a segundo.
    private bool relojActivo = false; // Interruptor: true = el reloj corre, false = el reloj se detiene. 
    public TextMeshProUGUI textoReloj; // El texto en pantalla donde se verán los segundos. - Es el texto hijo que cambia los segundos
    public GameObject contenedorReloj;  // La imagen Padre que contiene al texto adentro


    // VARIABLES DE VICTORIA Y ESTADÍSTICAS
    [Header("Pantallas de Fin de Juego")]
    public GameObject panelGameOver; // Panel que se activa si llegas a 3 errores o se acaba el tiempo.
    public GameObject panelVictoria; // Panel de victoria donde mostraremos las estadísticas.


    [Header("Elementos de Estadísticas (Dentro del Panel Victoria)")]
    public TextMeshProUGUI textoMensajeVictoria; // Texto que cambiará según las vidas con las que termines.
    public GameObject[] iconosVictoria; // 3 iconos (estrellas) dentro del panel de victoria.


    // SÍMBOLOS DE MÓDULO COMPLETADO (ESTÉTICO)
    [Header("Insignias en el Selector de Módulos")]
    public GameObject simboloCompletadoServicio; // Imagen de "Chulito/Completado" junto al botón de Servicio.
    public GameObject simboloCompletadoPhishing; //Misma imagen junto al botón de Phishing.

    // AUDIO
    [Header("Conexión con el Audio")]
    public GestorAudio scriptAudio; // Referencia para disparar sonidos de error, victoria y derrota.
    void Start()
    {
        // Por seguridad, ocultamos las pantallas de Game Over y Victoria al arrancar el programa.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Ocultamos los dos símbolos de "Módulo Completado" porque el jugador apenas acaba de abrir el juego y no ha ganado ninguno todavía.
        simboloCompletadoServicio.SetActive(false);
        simboloCompletadoPhishing.SetActive(false);

        // Escondemos el recuadro padre del reloj (y su texto hijo desaparece automáticamente con él) para que no se vea en el Menú Principal.
        contenedorReloj.SetActive(false);
    }

    // todo lo que está dentro de Update() se repite sin parar en cada fotograma del juego (60 o más veces por segundo).
    void Update()
    {
        // Primero preguntamos: "¿El interruptor del reloj está encendido (relojActivo == true)?".
        // Si estamos en el menú principal, 'relojActivo' es 'false', así que Update no hace absolutamente nada.
        if (relojActivo == true)
        {
            // 'Time.deltaTime' mide exactamente cuántos milisegundos reales pasaron desde el fotograma anterior.
            // Restarle 'Time.deltaTime' a 'tiempoRestante' garantiza que el reloj baje exactamente 1 segundo por cada segundo de la vida real,
            // sin importar si el computador donde se juega es muy rápido o muy lento.
            tiempoRestante = tiempoRestante - Time.deltaTime;

            // Como 'tiempoRestante' tiene muchos decimales (ej. 58.4392), usamos 'Mathf.CeilToInt(tiempoRestante)' para redondearlo hacia arriba
            // al número entero más cercano (ej. 58.4 se muestra como 59, y 0.2 se muestra como 1) y guardarlo en la variable entera 'segundosEnteros'.
            int segundosEnteros = Mathf.CeilToInt(tiempoRestante);

            // '.ToString()' convierte ese número matemático en texto para poder unirlo con la letra "s" (segundos) y mostrarlo en pantalla (ej. "59s").
            textoReloj.text = segundosEnteros.ToString() + "s";

            // Preguntamos: "¿El tiempo restante ya llegó a cero o bajó de cero?".
            if (tiempoRestante <= 0)
            {
                // Forzamos a que el texto muestre "Tiempo: 0s" y ejecutamos inmediatamente la función de derrota 'MostrarGameOver()'.
                textoReloj.text = "Tiempo: 0s";
                MostrarGameOver();
            }
        }
    }

    // SECCIÓN 4: MÉTODOS PARA INICIAR MÓDULOS Y CONTROLAR EL RELOJ
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
        contenedorReloj.SetActive(true);
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
        contenedorReloj.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    // Método para detener y esconder el reloj si el jugador sale al menú antes de terminar (o al ganar, perder o volver al selector).
    public void DetenerReloj()
    {
        relojActivo = false; // Detiene el conteo matemático.

        //Apaga el objeto de texto en la interfaz para que desaparezca de la pantalla.
        contenedorReloj.SetActive(false);
    }

    // NUEVO: Pausa el conteo del reloj temporalmente mientras el jugador lee la ventana emergente de feedback.
    public void PausarReloj()
    {
        relojActivo = false; // Detiene la resta de segundos sin ocultar el reloj de la pantalla.
    }

    // NUEVO: Reanuda el conteo del reloj cuando el jugador cierra la ventana emergente y pasa al siguiente caso.
    public void ReanudarReloj()
    {
        relojActivo = true; // Vuelve a activar el descuento de segundos en Update().
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

            scriptAudio.ReproducirError(); // Suena la alerta de fallo al perder una vida

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

            scriptAudio.ReproducirError(); // Suena la alerta de fallo al perder una vida

            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // SECCIÓN 6: MÉTODOS DE VICTORIA, ESTADÍSTICAS Y DERROTA
    // Esta función la llamará el módulo de Servicio al Cliente al superar sus 5 niveles.
    public void VictoriaServicio()
    {
        //Llamamos a DetenerReloj() para que frene el tiempo y además oculte el texto.
        DetenerReloj();

        relojActivo = false; // Detenemos el cronómetro porque ya ganó.
        panelVictoria.SetActive(true); // Mostramos la pantalla de victoria.

        // Encendemos el símbolo estético de "Completado" en el selector de módulos.
        simboloCompletadoServicio.SetActive(true);

        scriptAudio.DetenerMusica();      // Silencia la canción del nivel
        scriptAudio.ReproducirVictoria(); // (O ReproducirGameOver en MostrarGameOver) // Suena la tonada de triunfo
        
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

        scriptAudio.DetenerMusica();      // Silencia la canción del nivel
        scriptAudio.ReproducirVictoria(); // (O ReproducirGameOver en MostrarGameOver)

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
        // También ocultamos el reloj si el jugador pierde la partida.
        DetenerReloj();
        relojActivo = false; // Detenemos el reloj inmediatamente.
        panelGameOver.SetActive(true); // Mostramos la pantalla de Game Over.

        scriptAudio.DetenerMusica();      // Silencia la canción del nivel
        scriptAudio.ReproducirGameOver(); // (O ReproducirGameOver en MostrarGameOver) // Suena la tonada de derrota
    }
}
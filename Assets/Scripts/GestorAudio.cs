using UnityEngine;

public class GestorAudio : MonoBehaviour
{
    // LOS REPRODUCTORES (PARLANTES)

    [Header("Componentes AudioSource (Parlantes)")]
    public AudioSource reproductorMusica;  // Un solo parlante para todas las músicas de fondo.
    public AudioSource reproductorEfectos; // Parlante dedicado a los efectos cortos (clics, errores, etc.).

    // LAS CANCIONES DE FONDO (UNA POR MÓDULO)
    [Header("Músicas de Fondo (MP3 o WAV)")]
    public AudioClip musicaMenu;     // Canción para el Menú Inicial, Instrucciones y Selector.
    public AudioClip musicaServicio; // Canción exclusiva para el módulo de Servicio al Cliente.
    public AudioClip musicaPhishing; // Canción exclusiva para el módulo de Detector de Phishing.

    // LOS EFECTOS DE SONIDO CORTOS
    [Header("Archivos de Audio (Arrastrar MP3 o WAV)")]
    public AudioClip sonidoClic; // Sonido corto al presionar botones.
    public AudioClip sonidoAcierto;
    public AudioClip sonidoError; 
    public AudioClip sonidoVictoria;
    public AudioClip sonidoGameOver;

    void Start()
    {
        CambiarMusica(musicaMenu);
    }

    // MÉTODOS PARA CONTROLAR LA MÚSICA

    // Esta función recibe entre paréntesis cuál de las 3 canciones queremos escuchar.
    public void CambiarMusica(AudioClip nuevaCancion)
    {
        // Si la casilla de la canción está vacía en el Inspector, no hacemos nada para evitar errores.
        if (nuevaCancion == null || reproductorMusica == null)
        {
            return;
        }

        // TRUCO PROFESIONAL SENCILLO:
        // Si la canción que queremos poner YA es la que está sonando en este momento, 
        // no la reiniciamos (así la música del menú no se corta al ir del Menú a Instrucciones o al Selector).
        if (reproductorMusica.clip == nuevaCancion && reproductorMusica.isPlaying)
        {
            return;
        }

        // Cambiamos el disco dentro del parlante por la nueva canción:
        reproductorMusica.clip = nuevaCancion;

        // Nos aseguramos de que se repita en bucle (loop) si el jugador tarda en el nivel:
        reproductorMusica.loop = true;

        // Le damos Play a la nueva canción:
        reproductorMusica.Play();
    }

    // Esta función apaga la música de fondo (muy útil cuando sale la pantalla de Victoria o Game Over
    // para que la tonada de triunfo o derrota se escuche limpia y sin música encima).
    public void DetenerMusica()
    {
        if (reproductorMusica != null)
        {
            reproductorMusica.Stop();
        }
    }
    // MÉTODOS PARA LOS EFECTOS DE SONIDO
    // Usamos 'PlayOneShot' porque permite reproducir varios sonidos cortos seguidos 
    // (como varios clics rápidos) sin que un sonido corte al anterior.

    // Se llamará al presionar botones de navegación
    public void ReproducirClic()
    {
        if (sonidoClic != null)
        {
            reproductorEfectos.PlayOneShot(sonidoClic);
        }
    }

    // Se llamará cuando el jugador elija la respuesta correcta
    public void ReproducirAcierto()
    {
        if (sonidoAcierto != null)
        {
            reproductorEfectos.PlayOneShot(sonidoAcierto);
        }
    }

    // Se llamará cuando el jugador pierda una estrella o un candado
    public void ReproducirError()
    {
        if (sonidoError != null)
        {
            reproductorEfectos.PlayOneShot(sonidoError);
        }
    }

    // Se llamará al encenderse el panel de Victoria
    public void ReproducirVictoria()
    {
        if (sonidoVictoria != null)
        {
            reproductorEfectos.PlayOneShot(sonidoVictoria);
        }
    }

    // Se llamará al encenderse el panel de Game Over
    public void ReproducirGameOver()
    {
        if (sonidoGameOver != null)
        {
            reproductorEfectos.PlayOneShot(sonidoGameOver);
        }
    }
}
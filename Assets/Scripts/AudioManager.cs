using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Música de Fundo")]
    public AudioClip musicaDeFundo;
    [Range(0f, 1f)] public float volumeMusica = 0.5f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private float volumeOriginal;
    private bool volumeOriginalDefinido = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = musicaDeFundo;
        audioSource.volume = volumeMusica;
        audioSource.loop = true;
        audioSource.Play();

        // Salva o volume original assim que tocar
        volumeOriginal = audioSource.volume;
        volumeOriginalDefinido = true;
    }

    // ----------- FADES -----------

    public void AjustarVolumeGradualmente(float volumeFinal, float duracao)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeVolume(volumeFinal, duracao));
    }

    private IEnumerator FadeVolume(float volumeFinal, float duracao)
    {
        float volumeInicial = audioSource.volume;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumeInicial, volumeFinal, tempo / duracao);
            yield return null;
        }

        audioSource.volume = volumeFinal;
        fadeCoroutine = null;
    }

    // ----------- AUMENTO E RETORNO -----------

    public void AumentarVolumeInstantaneo(float novoVolume, float duracaoEspera, float duracaoFade)
    {
        if (!volumeOriginalDefinido)
            volumeOriginal = audioSource.volume;

        // Interrompe qualquer fade anterior
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        // Sobe o volume instantaneamente (com clamp pra não passar de 1)
        audioSource.volume = Mathf.Clamp01(novoVolume);

        // Inicia a rotina de retorno ao volume original
        fadeCoroutine = StartCoroutine(FadeDeVolta(volumeOriginal, duracaoEspera, duracaoFade));
    }

    private IEnumerator FadeDeVolta(float volumeFinal, float atraso, float duracao)
    {
        // Espera antes de começar a reduzir
        yield return new WaitForSeconds(atraso);

        float volumeInicial = audioSource.volume;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumeInicial, volumeFinal, tempo / duracao);
            yield return null;
        }

        audioSource.volume = volumeFinal;
        fadeCoroutine = null;
    }
}

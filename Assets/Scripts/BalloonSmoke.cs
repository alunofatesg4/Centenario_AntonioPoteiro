using UnityEngine;
using System.Collections;

public class BalloonSmoke : MonoBehaviour
{
    [Header("Partícula de fumaça")]
    public GameObject particulaFumaca;
    public float limiteVelocidadeY = 0.05f;
    public float tempoDesaparecer = 0.6f;

    [Header("Configuração de destruição/respawn")]
    public string layerFogueira = "Bonfire";
    public GameObject particulaExplosao;
    public float tempoReiniciar = 10f;

    [Header("Sons do balão")]
    public AudioClip somVoo;       // 🔊 som da fumaça (quando sobe)
    public AudioClip somExplosao;  // 💥 som da explosão
    public float volumeSom = 0.8f;

    private Rigidbody rb;
    private GameObject instanciaFumaca;
    private ParticleSystem sistemaParticula;
    private AudioSource audioSource;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private Renderer[] renderizadores;
    private Collider[] coliders;
    private bool estourado;
    private bool somVooTocando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;

        renderizadores = GetComponentsInChildren<Renderer>(true);
        coliders = GetComponentsInChildren<Collider>(true);

        // Adiciona componente de áudio se não existir
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (rb == null || estourado) return;

        // Balão subindo → toca som + fumaça
        if (rb.linearVelocity.y > limiteVelocidadeY)
        {
            if (!somVooTocando && somVoo != null)
            {
                audioSource.PlayOneShot(somVoo, volumeSom);
                somVooTocando = true;
            }

            if (instanciaFumaca == null && particulaFumaca != null)
            {
                Quaternion rotacao = Quaternion.Euler(90f, 0f, 0f);
                instanciaFumaca = Instantiate(particulaFumaca, transform.position, rotacao, transform);
                sistemaParticula = instanciaFumaca.GetComponent<ParticleSystem>();
            }
        }
        else
        {
            somVooTocando = false;

            // Se o balão parar ou descer → fade da fumaça
            if (instanciaFumaca != null)
            {
                if (sistemaParticula != null)
                {
                    var emission = sistemaParticula.emission;
                    emission.enabled = false;
                }

                Destroy(instanciaFumaca, tempoDesaparecer);
                instanciaFumaca = null;
                sistemaParticula = null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (estourado) return;

        if (other.gameObject.layer == LayerMask.NameToLayer(layerFogueira))
        {
            EstourarBalao();
        }
    }

    void EstourarBalao()
    {
        estourado = true;

        // 💥 Som da explosão
        if (somExplosao != null)
            audioSource.PlayOneShot(somExplosao, volumeSom);

        // Partícula de explosão
        if (particulaExplosao != null)
        {
            GameObject explosao = Instantiate(particulaExplosao, transform.position, Quaternion.identity);
            Destroy(explosao, 3f);
        }

        // Apaga fumaça atual
        if (instanciaFumaca != null)
        {
            if (sistemaParticula != null)
            {
                var emission = sistemaParticula.emission;
                emission.enabled = false;
            }
            Destroy(instanciaFumaca, tempoDesaparecer);
            instanciaFumaca = null;
            sistemaParticula = null;
        }

        OcultarBalao();
        StartCoroutine(ReiniciarBalao());
    }

    IEnumerator ReiniciarBalao()
    {
        yield return new WaitForSeconds(tempoReiniciar);

        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        MostrarBalao();
        estourado = false;
    }

    void OcultarBalao()
    {
        if (renderizadores != null)
            foreach (var r in renderizadores) r.enabled = false;

        if (coliders != null)
            foreach (var c in coliders) c.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    void MostrarBalao()
    {
        if (renderizadores != null)
            foreach (var r in renderizadores) r.enabled = true;

        if (coliders != null)
            foreach (var c in coliders) c.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}

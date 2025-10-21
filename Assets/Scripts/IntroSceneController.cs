using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneController : MonoBehaviour
{
    [Header("Configurações")]
    public Image imagemIntro;          // Imagem de introdução
    public float tempoExibicao = 3f;   // Tempo que a imagem fica visível
    public float duracaoFade = 1.5f;   // Tempo do fade

    [Header("Cena seguinte")]
    public string nomeCenaSeguinte = "MenuPrincipal"; // Próxima cena

    void Start()
    {
        // Garante que a imagem começa totalmente visível
        Color c = imagemIntro.color;
        c.a = 1f;
        imagemIntro.color = c;

        StartCoroutine(ExecutarIntro());
    }

    private IEnumerator ExecutarIntro()
    {
        // Aguarda o tempo de exibição
        yield return new WaitForSeconds(tempoExibicao);

        // Faz o fade-out
        float tempo = 0f;
        Color cor = imagemIntro.color;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tempo / duracaoFade);
            imagemIntro.color = new Color(cor.r, cor.g, cor.b, alpha);
            yield return null;
        }

        // Garante que fica totalmente invisível
        imagemIntro.color = new Color(cor.r, cor.g, cor.b, 0f);

        // Carrega a próxima cena
        SceneManager.LoadScene(nomeCenaSeguinte);
    }
}

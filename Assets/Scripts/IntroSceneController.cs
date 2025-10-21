using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneController : MonoBehaviour
{
    [Header("Configura��es")]
    public Image imagemIntro;          // Imagem de introdu��o
    public float tempoExibicao = 3f;   // Tempo que a imagem fica vis�vel
    public float duracaoFade = 1.5f;   // Tempo do fade

    [Header("Cena seguinte")]
    public string nomeCenaSeguinte = "MenuPrincipal"; // Pr�xima cena

    void Start()
    {
        // Garante que a imagem come�a totalmente vis�vel
        Color c = imagemIntro.color;
        c.a = 1f;
        imagemIntro.color = c;

        StartCoroutine(ExecutarIntro());
    }

    private IEnumerator ExecutarIntro()
    {
        // Aguarda o tempo de exibi��o
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

        // Garante que fica totalmente invis�vel
        imagemIntro.color = new Color(cor.r, cor.g, cor.b, 0f);

        // Carrega a pr�xima cena
        SceneManager.LoadScene(nomeCenaSeguinte);
    }
}

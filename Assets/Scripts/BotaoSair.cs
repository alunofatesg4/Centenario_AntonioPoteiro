using UnityEngine;

public class BotaoSair : MonoBehaviour
{
    // M�todo que ser� chamado pelo bot�o
    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

#if UNITY_EDITOR
        // Se estiver testando no Editor, apenas para o modo Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Encerra o aplicativo na build
        Application.Quit();
#endif
    }
}

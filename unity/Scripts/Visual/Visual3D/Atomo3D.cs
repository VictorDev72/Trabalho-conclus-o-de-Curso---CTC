using UnityEngine;
using BalanciadorQuimico; // Usando o namespace onde está o seu ElementoInfo

public class Atomo3D : MonoBehaviour
{
    public int NumeroAtomicoAtual { get; private set; }
    
    private Renderer _renderer;
    private static MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        
        // Inicializa o bloco de propriedades apenas uma vez para todos os átomos
        if (_propBlock == null) 
            _propBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Método chamado pelo seu Visualizador de Reações ao instanciar este átomo.
    /// </summary>
    public void Inicializar(ElementoInfo info)
    {
        NumeroAtomicoAtual = info.NumeroAtomico;

        // 1. Escala Dinâmica
        // O raio atômico no seu banco de dados geralmente está em picômetros (pm)
        // Precisamos dividir por um fator para caber na tela do Unity. 
        // Exemplo: 53pm do Hidrogênio / 100 = 0.53 no Unity.
        float fatorDeEscala = 100f; 
        float escalaUnity = info.RaioAtomico / fatorDeEscala;
        
        // Proteção: se o elemento não tiver raio cadastrado (for 0), define um tamanho padrão
        if (escalaUnity <= 0.1f) escalaUnity = 1.0f;

        transform.localScale = new Vector3(escalaUnity, escalaUnity, escalaUnity);

        // 2. Cor CPK usando MaterialPropertyBlock (Alta Performance)
        Color corDoAtomo = TabelaCPK.ObterCor(info.NumeroAtomico);
        
        // Pega as propriedades atuais do material, altera a cor e aplica de volta
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_Color", corDoAtomo);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
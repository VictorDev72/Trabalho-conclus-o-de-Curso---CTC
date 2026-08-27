using UnityEngine;
using BalanciadorQuimico; // Usando o namespace onde está o seu ElementoInfo

public class Atomo3D : MonoBehaviour
{
    public int NumeroAtomicoAtual { get; private set; }

    // Fator usado para converter o raio atômico (pm) num tamanho razoável em unidades Unity.
    private const float FatorDeEscala = 100f;

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
    /// Calcula a escala (== diâmetro da esfera, já que o prefab tem escala 1 = diâmetro 1)
    /// que um átomo desse elemento deve ter. Usado tanto para escalar a esfera quanto para
    /// calcular o espaçamento entre átomos em GerenciadorVisual3D, evitando que fiquem sobrepostos.
    /// </summary>
    public static float CalcularEscala(ElementoInfo info)
    {
        float escalaUnity = (float)info.RaioAtomico / FatorDeEscala;

        // Proteção: se o elemento não tiver raio cadastrado (for 0), define um tamanho padrão
        if (escalaUnity <= 0.1f) escalaUnity = 1.0f;

        return escalaUnity;
    }

    /// <summary>
    /// Método chamado pelo seu Visualizador de Reações ao instanciar este átomo.
    /// </summary>
    public void Inicializar(ElementoInfo info)
    {
        NumeroAtomicoAtual = info.Z;

        // 1. Escala Dinâmica
        // O raio atômico no seu banco de dados geralmente está em picômetros (pm)
        // Precisamos dividir por um fator para caber na tela do Unity.
        // Exemplo: 53pm do Hidrogênio / 100 = 0.53 no Unity.
        float escalaUnity = CalcularEscala(info);

        transform.localScale = new Vector3(escalaUnity, escalaUnity, escalaUnity);

        // 2. Cor CPK usando MaterialPropertyBlock (Alta Performance)
        Color corDoAtomo = TabelaCPK.ObterCor(info.Z);
        
        // Pega as propriedades atuais do material, altera a cor e aplica de volta
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_BaseColor", corDoAtomo);
        _renderer.SetPropertyBlock(_propBlock);
    }
}

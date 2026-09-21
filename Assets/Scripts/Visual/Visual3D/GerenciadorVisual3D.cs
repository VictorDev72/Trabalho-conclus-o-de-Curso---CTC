using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BalanciadorQuimico; // Para acessar os Elementos e Atomos

public class GerenciadorVisual3D : MonoBehaviour
{
    [Header("Configurações 3D")]
    public GameObject atomoPrefab; // Arraste o ÚNICO prefab AtomoBase_Prefab aqui
    public float distanciaEntreMoleculas = 4f;

    [Header("Ligações (cilindros)")]
    public float comprimentoLigacao = 1.6f;
    public float espessuraLigacao = 0.12f;
    public Color corLigacao = new Color(0.75f, 0.75f, 0.75f);

    // Lista para guardar os átomos/ligações gerados e poder apagá-los depois
    private List<GameObject> atomosNaTela = new List<GameObject>();

    private void Start()
    {
        // Ao carregar esta cena, renderiza automaticamente a reação balanceada
        // que a UIReacao deixou pronta em ReacaoTransferData.
        RenderizarReacao(ReacaoTransferData.Reagentes, ReacaoTransferData.Produtos);
    }

    /// <summary>
    /// Função principal que a sua UI vai chamar quando a reação terminar de ser calculada
    /// </summary>
    public void RenderizarReacao(List<Dictionary<int, int>> reagentes, List<Dictionary<int, int>> produtos)
    {
        // 1. Limpa a tela de reações anteriores
        LimparTela();

        // 2. Renderiza os Reagentes (vamos colocá-los mais à esquerda da tela, X = -5)
        Vector3 posicaoAtual = new Vector3(-5, 0, 0);
        foreach (var molecula in reagentes)
        {
            RenderizarMolecula(molecula, ref posicaoAtual);
            posicaoAtual.x += distanciaEntreMoleculas;
        }

        // 3. Renderiza os Produtos (vamos colocá-los mais à direita da tela, X = 5)
        posicaoAtual = new Vector3(5, 0, 0);
        foreach (var molecula in produtos)
        {
            RenderizarMolecula(molecula, ref posicaoAtual);
            posicaoAtual.x += distanciaEntreMoleculas;
        }
    }

    /// <summary>
    /// Renderiza uma molécula como um pequeno "ball and stick": um átomo central
    /// (o de menor quantidade na fórmula) com os demais distribuídos ao redor
    /// e ligados a ele por cilindros finos.
    /// </summary>
    private void RenderizarMolecula(Dictionary<int, int> molecula, ref Vector3 posicaoBase)
    {
        // Expande o dicionário {numeroAtomico: quantidade} numa lista plana de átomos
        List<int> atomos = new List<int>();
        foreach (var par in molecula)
            for (int i = 0; i < par.Value; i++)
                atomos.Add(par.Key);

        if (atomos.Count == 0) return;

        if (atomos.Count == 1)
        {
            atomosNaTela.Add(CriarAtomo(atomos[0], posicaoBase));
            return;
        }

        if (atomos.Count == 2)
        {
            Vector3 offset = new Vector3(comprimentoLigacao / 2f, 0, 0);
            Vector3 posA = posicaoBase - offset;
            Vector3 posB = posicaoBase + offset;

            atomosNaTela.Add(CriarAtomo(atomos[0], posA));
            atomosNaTela.Add(CriarAtomo(atomos[1], posB));
            atomosNaTela.Add(CriarLigacao(posA, posB));
            return;
        }

        // 3+ átomos: o átomo com menor quantidade na fórmula vira o centro
        // (ex.: H2O -> O central; CO2 -> C central; NH3 -> N central).
        int atomoCentral = molecula.OrderBy(par => par.Value).First().Key;
        atomos.Remove(atomoCentral);

        atomosNaTela.Add(CriarAtomo(atomoCentral, posicaoBase));

        for (int i = 0; i < atomos.Count; i++)
        {
            Vector3 direcao = PontoNaEsfera(i, atomos.Count);
            Vector3 posSatelite = posicaoBase + direcao * comprimentoLigacao;

            atomosNaTela.Add(CriarAtomo(atomos[i], posSatelite));
            atomosNaTela.Add(CriarLigacao(posicaoBase, posSatelite));
        }
    }

    /// <summary>
    /// Distribui N pontos de forma razoavelmente uniforme sobre uma esfera unitária
    /// (espiral de Fibonacci), usado para posicionar os átomos ao redor do centro.
    /// </summary>
    private Vector3 PontoNaEsfera(int indice, int total)
    {
        if (total == 1) return Vector3.up;

        float offset = 2f / total;
        float y = (indice * offset - 1f) + (offset / 2f);
        float raio = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
        float anguloOuro = indice * Mathf.PI * (3f - Mathf.Sqrt(5f));

        float x = Mathf.Cos(anguloOuro) * raio;
        float z = Mathf.Sin(anguloOuro) * raio;
        return new Vector3(x, y, z);
    }

    // A FUNÇÃO QUE VOCÊ PERGUNTOU FICA AQUI!
    private GameObject CriarAtomo(int numeroAtomico, Vector3 posicaoNaTela)
    {
        // Pega as informações do elemento solicitado
        ElementoInfo info = Elementos.Tabela[numeroAtomico];

        // Instancia a esfera genérica
        GameObject novoAtomo = Instantiate(atomoPrefab, posicaoNaTela, Quaternion.identity);

        // Passa os dados matemáticos/químicos para o visual
        novoAtomo.GetComponent<Atomo3D>().Inicializar(info);

        return novoAtomo;
    }

    /// <summary>
    /// Cria um cilindro fino conectando dois pontos, representando a ligação química.
    /// </summary>
    private GameObject CriarLigacao(Vector3 pontoA, Vector3 pontoB)
    {
        GameObject ligacao = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ligacao.name = "Ligacao";

        var colisor = ligacao.GetComponent<Collider>();
        if (colisor != null) Destroy(colisor);

        Vector3 direcao = pontoB - pontoA;
        float distancia = direcao.magnitude;

        ligacao.transform.position = (pontoA + pontoB) / 2f;
        ligacao.transform.up = direcao.normalized;
        // Cilindro padrão do Unity tem 2 unidades de altura -> metade da distância no eixo Y
        ligacao.transform.localScale = new Vector3(espessuraLigacao, distancia / 2f, espessuraLigacao);

        var renderer = ligacao.GetComponent<Renderer>();
        renderer.material.color = corLigacao;

        return ligacao;
    }

    private void LimparTela()
    {
        foreach (var atomo in atomosNaTela)
        {
            Destroy(atomo); // (No futuro, trocaríamos isso por Object Pooling)
        }
        atomosNaTela.Clear();
    }
}

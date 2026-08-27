using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BalanciadorQuimico; // Para acessar os Elementos e Atomos

public class GerenciadorVisual3D : MonoBehaviour
{
    [Header("Configurações 3D")]
    public GameObject atomoPrefab; // Arraste o ÚNICO prefab AtomoBase_Prefab aqui

    // Espaço mínimo garantido entre a superfície de dois átomos vizinhos.
    private const float MargemEntreAtomos = 0.15f;

    // Lista para guardar os átomos gerados e poder apagá-los depois
    private List<GameObject> atomosNaTela = new List<GameObject>();

    private void Start()
    {
        // Se essa cena foi carregada a partir do botão "Reagir" (ver UIReacao),
        // os dados da reação já estão esperando aqui — renderiza assim que a cena abre.
        if (ReacaoTransferData.Reagentes != null && ReacaoTransferData.Produtos != null)
        {
            RenderizarReacao(ReacaoTransferData.Reagentes, ReacaoTransferData.Produtos);
        }
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
            posicaoAtual.x += 2f; // Dá um espaço entre as moléculas
        }

        // 3. Renderiza os Produtos (vamos colocá-los mais à direita da tela, X = 5)
        posicaoAtual = new Vector3(5, 0, 0);
        foreach (var molecula in produtos)
        {
            RenderizarMolecula(molecula, ref posicaoAtual);
            posicaoAtual.x += 2f; // Dá um espaço entre as moléculas
        }
    }

    private void RenderizarMolecula(Dictionary<int, int> molecula, ref Vector3 posicaoBase)
    {
        // Expande o dicionário (número atômico -> quantidade) numa lista de átomos individuais
        var atomosDaMolecula = new List<ElementoInfo>();
        foreach (var par in molecula)
        {
            ElementoInfo info = Elemento.TabelaPeriodica[par.Key];
            for (int i = 0; i < par.Value; i++)
                atomosDaMolecula.Add(info);
        }

        int total = atomosDaMolecula.Count;
        if (total == 0) return;

        if (total == 1)
        {
            atomosNaTela.Add(CriarAtomo(atomosDaMolecula[0].Z, posicaoBase));
            return;
        }

        // Distribui os átomos num círculo ao redor de posicaoBase. O raio do círculo é
        // calculado a partir do maior átomo da molécula para garantir que a distância entre
        // dois centros vizinhos seja sempre >= à soma dos seus raios (+ margem) — isso é o que
        // impede as esferas de ficarem uma dentro da outra, diferente do jitter aleatório antigo.
        float maiorDiametro = atomosDaMolecula.Max(Atomo3D.CalcularEscala);
        float raioOrbita = (maiorDiametro + MargemEntreAtomos) / (2f * Mathf.Sin(Mathf.PI / total));

        for (int i = 0; i < total; i++)
        {
            float angulo = (2f * Mathf.PI / total) * i;
            Vector3 offset = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f) * raioOrbita;

            GameObject novoAtomo = CriarAtomo(atomosDaMolecula[i].Z, posicaoBase + offset);
            atomosNaTela.Add(novoAtomo);
        }
    }

    // A FUNÇÃO QUE VOCÊ PERGUNTOU FICA AQUI!
    private GameObject CriarAtomo(int numeroAtomico, Vector3 posicaoNaTela)
    {
        // Pega as informações do elemento solicitado
        ElementoInfo info = Elemento.TabelaPeriodica[numeroAtomico]; 

        // Instancia a esfera genérica
        GameObject novoAtomo = Instantiate(atomoPrefab, posicaoNaTela, Quaternion.identity);
        
        // Passa os dados matemáticos/químicos para o visual
        novoAtomo.GetComponent<Atomo3D>().Inicializar(info);

        return novoAtomo;
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
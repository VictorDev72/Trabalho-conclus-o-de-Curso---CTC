using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico; // Para acessar os Elementos e Atomos

public class GerenciadorVisual3D : MonoBehaviour
{
    [Header("Configurações 3D")]
    public GameObject atomoPrefab; // Arraste o ÚNICO prefab AtomoBase_Prefab aqui

    // Lista para guardar os átomos gerados e poder apagá-los depois
    private List<GameObject> atomosNaTela = new List<GameObject>();

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
        // Lê os átomos dentro da molécula (Ex: H: 2, O: 1)
        foreach (var par in molecula)
        {
            int numeroAtomico = par.Key;
            int quantidade = par.Value;

            for (int i = 0; i < quantidade; i++)
            {
                // Calcula uma posição um pouco aleatória em volta da posição base
                // (No futuro você pode melhorar isso para posições geométricas precisas)
                Vector3 posicaoAtomo = posicaoBase + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                
                // Chama a função genérica para criar o átomo!
                GameObject novoAtomo = CriarAtomo(numeroAtomico, posicaoAtomo);
                
                // Guarda na lista para podermos apagar depois
                atomosNaTela.Add(novoAtomo);
            }
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
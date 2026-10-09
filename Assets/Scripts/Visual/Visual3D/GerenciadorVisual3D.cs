using System;
using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;

public class GerenciadorVisual3D : MonoBehaviour
{
    [Header("Configurações 3D")]
    public GameObject atomoPrefab;

    // Espaço livre entre uma molécula e a próxima.
    public float distanciaEntreMoleculas = 2f;

    // Metade do espaço vazio entre reagentes (esquerda)
    // e produtos (direita), em torno de X = 0.
    public float espacoCentral = 3f;

    [Header("Configurações das ligações")]
    public float espessuraLigacao = 0.12f;

    public Color corLigacao =
        new Color(0.75f, 0.75f, 0.75f);

    // Opcional: se vazio, usa o material padrão
    // do cilindro pintado com corLigacao.
    public Material materialLigacao;

    // Cada molécula fica num GameObject "grupo";
    // destruir o grupo destrói átomos e ligações.
    private List<GameObject> objetosNaTela =
        new List<GameObject>();

    private void Start()
    {
        // Ao carregar esta cena, renderiza a reação balanceada
        // que a UIReacao deixou pronta em ReacaoTransferData.
        RenderizarReacao(
            ReacaoTransferData.Reagentes,
            ReacaoTransferData.Produtos);
    }

    // ============================================================
    // REAÇÃO
    // ============================================================

    public void RenderizarReacao(
        List<Dictionary<int, int>> reagentes,
        List<Dictionary<int, int>> produtos)
    {
        LimparTela();

        if (atomoPrefab == null)
        {
            Debug.LogError(
                "[GerenciadorVisual3D] atomoPrefab não foi " +
                "atribuído no Inspector.");
            return;
        }

        List<MoleculaNaTela> moleculasReagentes =
            CriarMoleculas(reagentes);

        List<MoleculaNaTela> moleculasProdutos =
            CriarMoleculas(produtos);

        // Reagentes terminam em -espacoCentral,
        // produtos começam em +espacoCentral.
        float larguraReagentes =
            LarguraTotal(moleculasReagentes);

        PosicionarEmFila(
            moleculasReagentes,
            -espacoCentral - larguraReagentes);

        PosicionarEmFila(
            moleculasProdutos,
            espacoCentral);
    }

    // ============================================================
    // LAYOUT
    // ============================================================

    // Molécula já montada (na origem) e seus limites em X,
    // usados para enfileirar sem que uma invada a outra.
    private struct MoleculaNaTela
    {
        public GameObject Grupo;
        public float MinX;
        public float MaxX;

        public float Largura => MaxX - MinX;
    }

    private List<MoleculaNaTela> CriarMoleculas(
        List<Dictionary<int, int>> composicoes)
    {
        List<MoleculaNaTela> moleculas =
            new List<MoleculaNaTela>();

        if (composicoes == null)
            return moleculas;

        foreach (
            Dictionary<int, int> composicao
            in composicoes)
        {
            try
            {
                moleculas.Add(
                    RenderizarMolecula(
                        composicao));
            }
            catch (Exception ex)
            {
                Debug.LogWarning(
                    "[GerenciadorVisual3D] Não foi possível " +
                    "montar a molécula: " + ex.Message);
            }
        }

        return moleculas;
    }

    private float LarguraTotal(
        List<MoleculaNaTela> moleculas)
    {
        if (moleculas.Count == 0)
            return 0f;

        float total = 0f;

        foreach (MoleculaNaTela molecula in moleculas)
            total += molecula.Largura;

        return total +
               distanciaEntreMoleculas *
               (moleculas.Count - 1);
    }

    private void PosicionarEmFila(
        List<MoleculaNaTela> moleculas,
        float inicioX)
    {
        float cursor = inicioX;

        foreach (MoleculaNaTela molecula in moleculas)
        {
            // Encosta a borda esquerda da molécula no cursor.
            molecula.Grupo.transform.position =
                new Vector3(
                    cursor - molecula.MinX,
                    0,
                    0);

            cursor +=
                molecula.Largura +
                distanciaEntreMoleculas;
        }
    }

    // ============================================================
    // MOLÉCULA
    // ============================================================

    private MoleculaNaTela RenderizarMolecula(
        Dictionary<int, int> composicao)
    {
        EstruturaMolecular estrutura =
            GeometriaMolecular.Criar(
                composicao);

        GameObject grupo =
            new GameObject(
                "Molecula_" +
                Formula(composicao));

        objetosNaTela.Add(grupo);

        Dictionary<int, GameObject>
            objetosAtomos =
            new Dictionary<int, GameObject>();

        float minX = float.MaxValue;
        float maxX = float.MinValue;

        // --------------------------------------------------------
        // ÁTOMOS
        // --------------------------------------------------------

        foreach (
            AtomoMolecular atomo
            in estrutura.Atomos)
        {
            GameObject objeto =
                CriarAtomo(
                    atomo.Info,
                    atomo.Posicao,
                    grupo.transform);

            objetosAtomos.Add(
                atomo.Id,
                objeto);

            // A escala da esfera é o diâmetro.
            float raio =
                objeto.transform.localScale.x / 2f;

            minX = Mathf.Min(minX, atomo.Posicao.x - raio);
            maxX = Mathf.Max(maxX, atomo.Posicao.x + raio);
        }

        // --------------------------------------------------------
        // LIGAÇÕES
        // --------------------------------------------------------

        foreach (
            LigacaoMolecular ligacao
            in estrutura.Ligacoes)
        {
            GameObject objetoA =
                objetosAtomos[
                    ligacao.A.Id];

            GameObject objetoB =
                objetosAtomos[
                    ligacao.B.Id];

            CriarLigacao(
                objetoA.transform.localPosition,
                objetoB.transform.localPosition,
                ligacao.Ordem,
                grupo.transform);
        }

        return new MoleculaNaTela
        {
            Grupo = grupo,
            MinX = minX,
            MaxX = maxX
        };
    }

    private static string Formula(
        Dictionary<int, int> composicao)
    {
        string texto = "";

        foreach (KeyValuePair<int, int> item in composicao)
        {
            texto +=
                Elementos.Tabela.TryGetValue(
                    item.Key,
                    out ElementoInfo info)
                    ? info.Simbolo
                    : "E" + item.Key;

            if (item.Value > 1)
                texto += item.Value;
        }

        return texto;
    }

    // ============================================================
    // ÁTOMO
    // ============================================================

    private GameObject CriarAtomo(
        ElementoInfo info,
        Vector3 posicaoLocal,
        Transform pai)
    {
        GameObject novoAtomo =
            Instantiate(
                atomoPrefab,
                pai);

        novoAtomo.name =
            info.Simbolo;

        novoAtomo.transform.localPosition =
            posicaoLocal;

        Atomo3D atomo =
            novoAtomo.GetComponent<Atomo3D>();

        if (atomo == null)
        {
            Debug.LogError(
                "AtomoBase_Prefab não possui " +
                "o componente Atomo3D.");
        }
        else
        {
            atomo.Inicializar(info);
        }

        return novoAtomo;
    }

    // ============================================================
    // LIGAÇÃO
    // ============================================================

    // Cria "ordem" cilindros lado a lado entre inicio e fim
    // (1 = simples, 2 = dupla, 3 = tripla), em coordenadas
    // locais do grupo da molécula.
    private void CriarLigacao(
        Vector3 inicio,
        Vector3 fim,
        int ordem,
        Transform pai)
    {
        Vector3 direcao =
            fim - inicio;

        float distancia =
            direcao.magnitude;

        if (distancia <= 0.001f)
            return;

        Vector3 meio =
            (inicio + fim) / 2f;

        Vector3 deslocamento =
            Vector3.Cross(
                direcao.normalized,
                Vector3.up);

        if (deslocamento.sqrMagnitude < 0.01f)
        {
            deslocamento =
                Vector3.Cross(
                    direcao.normalized,
                    Vector3.forward);
        }

        deslocamento.Normalize();

        float espacamento =
            0.18f;

        float centro =
            (ordem - 1) / 2f;

        for (int i = 0;
             i < ordem;
             i++)
        {
            GameObject cilindro =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cylinder);

            cilindro.name =
                "Ligacao";

            Collider collider =
                cilindro.GetComponent<Collider>();

            if (collider != null)
                Destroy(collider);

            cilindro.transform.SetParent(
                pai,
                false);

            cilindro.transform.localPosition =
                meio +
                deslocamento *
                ((i - centro) *
                 espacamento);

            cilindro.transform.up =
                pai.rotation *
                direcao.normalized;

            // O cilindro padrão do Unity tem 2 unidades
            // de altura, por isso distancia / 2.
            cilindro.transform.localScale =
                new Vector3(
                    espessuraLigacao,
                    distancia / 2f,
                    espessuraLigacao);

            Renderer renderer =
                cilindro.GetComponent<Renderer>();

            if (materialLigacao != null)
                renderer.material = materialLigacao;
            else
                renderer.material.color = corLigacao;
        }
    }

    // ============================================================
    // LIMPAR
    // ============================================================

    private void LimparTela()
    {
        foreach (
            GameObject objeto
            in objetosNaTela)
        {
            if (objeto != null)
                Destroy(objeto);
        }

        objetosNaTela.Clear();
    }
}

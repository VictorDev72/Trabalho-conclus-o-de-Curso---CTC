using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;

public class GerenciadorVisual3D : MonoBehaviour
{
    [Header("Átomo")]
    public GameObject atomoPrefab;
    [Header("Ligações")]
    [Min(0.02f)] public float espessuraLigacao = 0.10f;
    public Material materialLigacao;
    [Header("Layout")]
    public float espacamentoMoleculas = 3.5f;
    private readonly List<GameObject> gruposNaTela = new List<GameObject>();

    public void RenderizarReacao(List<Dictionary<int, int>> reagentes, List<Dictionary<int, int>> produtos)
    {
        LimparTela();
        RenderizarLado(reagentes, -5f);
        RenderizarLado(produtos, 5f);
    }

    // Permite que a UI informe a carga da espécie ao renderizar uma molécula isolada.
    public void RenderizarMolecula(Dictionary<int, int> composicao, Vector3 posicao, int cargaTotal = 0)
    {
        if (atomoPrefab == null) { Debug.LogError("AtomoPrefab não foi atribuído no GerenciadorVisual3D."); return; }
        EstruturaMolecular estrutura;
        try { estrutura = GeometriaMolecular.Criar(composicao, cargaTotal); }
        catch (System.Exception ex) { Debug.LogError("Não foi possível gerar a estrutura: " + ex.Message); return; }
        if (!string.IsNullOrEmpty(estrutura.Aviso)) Debug.LogWarning(estrutura.Aviso);

        GameObject grupo = new GameObject("Molecula_3D"); grupo.transform.position = posicao; gruposNaTela.Add(grupo);
        var objetos = new Dictionary<int, GameObject>();
        foreach (var a in estrutura.Atomos)
        {
            GameObject go = Instantiate(atomoPrefab, posicao + a.Posicao, Quaternion.identity, grupo.transform);
            Atomo3D visual = go.GetComponent<Atomo3D>();
            if (visual == null) { Debug.LogError("O prefab de átomo precisa do componente Atomo3D."); Destroy(go); continue; }
            visual.Inicializar(a.Info); objetos[a.Id] = go;
        }
        foreach (var l in estrutura.Ligacoes)
        {
            if (!objetos.ContainsKey(l.A.Id) || !objetos.ContainsKey(l.B.Id)) continue;
            for (int i = 0; i < l.Ordem; i++) CriarCilindroLigacao(objetos[l.A.Id].transform.position, objetos[l.B.Id].transform.position, i, l.Ordem, grupo.transform);
        }
    }

    private void RenderizarLado(List<Dictionary<int, int>> moleculas, float xInicial)
    {
        if (moleculas == null) return;
        float x = xInicial;
        foreach (var composicao in moleculas)
        {
            RenderizarMolecula(composicao, new Vector3(x, 0, 0));
            x += espacamentoMoleculas;
        }
    }

    private void CriarCilindroLigacao(Vector3 inicio, Vector3 fim, int indice, int ordem, Transform pai)
    {
        Vector3 vetor = fim - inicio; float distancia = vetor.magnitude; if (distancia < 0.001f) return;
        Vector3 deslocamento = Vector3.Cross(vetor.normalized, Vector3.up);
        if (deslocamento.sqrMagnitude < 0.01f) deslocamento = Vector3.Cross(vetor.normalized, Vector3.forward);
        deslocamento.Normalize();
        float offset = (indice - (ordem - 1) / 2f) * espessuraLigacao * 1.8f;
        GameObject cilindro = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cilindro.name = "Ligacao_Ordem_" + ordem;
        cilindro.transform.position = (inicio + fim) * 0.5f + deslocamento * offset;
        cilindro.transform.rotation = Quaternion.FromToRotation(Vector3.up, vetor.normalized);
        cilindro.transform.localScale = new Vector3(espessuraLigacao, distancia * 0.5f, espessuraLigacao);
        cilindro.transform.SetParent(pai, true);
        Collider c = cilindro.GetComponent<Collider>(); if (c != null) Destroy(c);
        if (materialLigacao != null) cilindro.GetComponent<Renderer>().sharedMaterial = materialLigacao;
    }

    public void LimparTela()
    {
        foreach (var go in gruposNaTela) if (go != null) Destroy(go);
        gruposNaTela.Clear();
    }

    private void OnDestroy() { LimparTela(); }
}

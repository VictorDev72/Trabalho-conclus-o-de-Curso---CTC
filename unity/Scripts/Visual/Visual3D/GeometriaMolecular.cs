using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BalanciadorQuimico;

/// <summary>
/// Constrói uma estrutura canônica aproximada para moléculas covalentes pequenas.
/// Não é um solucionador quântico nem determina isômeros/ressonância de forma completa.
/// </summary>
public static class GeometriaMolecular
{
    private const int MaxAtomosTotais = 18;
    private const int MaxAtomosPesados = 7;
    private const float DistanciaMinima = 1.15f;

    public static EstruturaMolecular Criar(Dictionary<int, int> composicao, int cargaTotal = 0)
    {
        if (composicao == null || composicao.Count == 0) throw new ArgumentException("A composição está vazia.");
        if (composicao.Any(kv => kv.Value <= 0)) throw new ArgumentException("As quantidades de átomos devem ser positivas.");
        int total = composicao.Values.Sum();
        if (total > MaxAtomosTotais) throw new NotSupportedException("Para manter a busca limitada, o máximo atual é " + MaxAtomosTotais + " átomos.");

        var estrutura = CriarAtomos(composicao);
        estrutura.CargaTotal = cargaTotal;
        var pesados = estrutura.Atomos.Where(a => a.NumeroAtomico != Atomos.H).ToList();
        if (pesados.Count > MaxAtomosPesados) throw new NotSupportedException("A busca estrutural atual suporta até " + MaxAtomosPesados + " átomos não hidrogênio.");

        if (pesados.Count == 0)
        {
            if (estrutura.Atomos.Count == 2) estrutura.AdicionarLigacao(estrutura.Atomos[0], estrutura.Atomos[1]);
            PosicionarEstrutura(estrutura);
            return estrutura;
        }

        // Escolha de um esqueleto canônico: cadeias para compostos dominados por C/Si;
        // átomo central para estruturas com um formador de rede claramente preferido.
        ConstruirEsqueletoCanonico(estrutura, pesados);
        DistribuirHidrogenios(estrutura);
        AjustarOrdensLigacao(estrutura);
        ValidarValenciasAproximadas(estrutura);
        PosicionarEstrutura(estrutura);

        string erro;
        if (!estrutura.ValidarIntegridade(out erro)) throw new InvalidOperationException(erro);
        if (!estrutura.EhConexa()) estrutura.Aviso = "A fórmula pode representar mais de uma espécie ou a conectividade não foi inferida com confiança.";
        if (string.IsNullOrEmpty(estrutura.Aviso)) estrutura.Aviso = "Estrutura canônica aproximada; confirme fórmulas ambíguas ou espécies carregadas.";
        return estrutura;
    }

    private static EstruturaMolecular CriarAtomos(Dictionary<int, int> composicao)
    {
        var e = new EstruturaMolecular();
        foreach (var item in composicao.OrderBy(k => k.Key))
        {
            ElementoInfo info;
            if (!Elementos.Tabela.TryGetValue(item.Key, out info)) throw new ArgumentException("Elemento Z=" + item.Key + " não existe em Elementos.Tabela.");
            for (int i = 0; i < item.Value; i++) e.AdicionarAtomo(info);
        }
        return e;
    }

    private static void ConstruirEsqueletoCanonico(EstruturaMolecular e, List<AtomoMolecular> pesados)
    {
        if (pesados.Count < 2) return;
        bool cadeia = pesados.All(a => a.NumeroAtomico == Atomos.C || a.NumeroAtomico == Atomos.Si) ||
                      pesados.Select(a => a.NumeroAtomico).Distinct().Count() == 1;
        if (cadeia)
        {
            for (int i = 0; i < pesados.Count - 1; i++) e.AdicionarLigacao(pesados[i], pesados[i + 1]);
            return;
        }

        // Centralidade prioriza valência disponível e elementos menos eletronegativos.
        var centro = pesados.OrderByDescending(a => MaiorValenciaCovalente(a.Info))
            .ThenBy(a => a.Info.Eletronegatividade <= 0 ? 99 : a.Info.Eletronegatividade)
            .ThenBy(a => a.Id).First();
        foreach (var outro in pesados.Where(a => a != centro)) e.AdicionarLigacao(centro, outro);
    }

    private static int MaiorValenciaCovalente(ElementoInfo info)
    {
        // A tabela global informa valências possíveis; para esta heurística ignoramos NOX.
        return info.LigacoesPossiveis == null ? 1 : info.LigacoesPossiveis.Where(v => v > 0).DefaultIfEmpty(1).Max();
    }

    private static int Valencias(ElementoInfo info, int ordem)
    {
        var vs = info.LigacoesPossiveis == null ? new List<int>() : info.LigacoesPossiveis.Where(v => v > 0 && v <= 6).Distinct().OrderBy(v => v).ToList();
        if (vs.Count == 0) return ordem;
        return vs.FirstOrDefault(v => v >= ordem) == 0 ? vs.Max() : vs.First(v => v >= ordem);
    }

    private static void DistribuirHidrogenios(EstruturaMolecular e)
    {
        var hidrogeniosLivres = e.Atomos.Where(a => a.NumeroAtomico == Atomos.H && a.Ligacoes.Count == 0).ToList();
        var pesados = e.Atomos.Where(a => a.NumeroAtomico != Atomos.H).ToList();
        foreach (var h in hidrogeniosLivres)
        {
            var candidatos = pesados.Where(a => a.OrdemTotal < MaiorValenciaCovalente(a.Info))
                .OrderBy(a => a.OrdemTotal / (float)Math.Max(1, MaiorValenciaCovalente(a.Info)))
                .ThenByDescending(a => a.Info.Eletronegatividade)
                .ThenBy(a => a.Id).ToList();
            if (candidatos.Count == 0)
            {
                // Molécula composta apenas por hidrogênios: conectar pares quando possível.
                var outroH = e.Atomos.FirstOrDefault(a => a.NumeroAtomico == Atomos.H && a != h && a.Ligacoes.Count == 0);
                if (outroH != null) e.AdicionarLigacao(h, outroH);
                continue;
            }
            e.AdicionarLigacao(candidatos[0], h);
        }
    }

    private static void AjustarOrdensLigacao(EstruturaMolecular e)
    {
        // Aumenta ordens somente em ligações entre átomos pesados; nunca em H-X.
        // Prioriza ligações entre átomos com valência ainda incompleta.
        for (int rodada = 0; rodada < 4; rodada++)
        {
            bool mudou = false;
            foreach (var l in e.Ligacoes.Where(l => l.A.NumeroAtomico != Atomos.H && l.B.NumeroAtomico != Atomos.H).ToList())
            {
                if (l.Ordem >= 3) continue;
                int alvoA = Valencias(l.A.Info, l.A.OrdemTotal);
                int alvoB = Valencias(l.B.Info, l.B.OrdemTotal);
                if (l.A.OrdemTotal < alvoA && l.B.OrdemTotal < alvoB)
                {
                    l.DefinirOrdem(l.Ordem + 1); mudou = true;
                }
            }
            if (!mudou) break;
        }
    }

    private static void ValidarValenciasAproximadas(EstruturaMolecular e)
    {
        foreach (var a in e.Atomos)
        {
            if (a.NumeroAtomico == Atomos.H && a.OrdemTotal > 1)
                throw new InvalidOperationException("Hidrogênio com mais de uma ligação.");
            if (a.NumeroAtomico == Atomos.H) continue;
            var possiveis = a.Info.LigacoesPossiveis;
            if (possiveis != null && possiveis.Count > 0 && !possiveis.Any(v => v == a.OrdemTotal))
                e.Aviso = "Atenção: a valência inferida para " + a.Info.Simbolo + " (" + a.OrdemTotal + ") não coincide exatamente com a tabela; pode haver carga, ressonância ou conectividade alternativa.";
        }
    }

    private static void PosicionarEstrutura(EstruturaMolecular e)
    {
        if (e.Atomos.Count == 0) return;
        foreach (var a in e.Atomos) a.Posicao = Vector3.zero;
        var raiz = e.Atomos.OrderByDescending(a => a.NumeroAtomico != Atomos.H ? 1 : 0).ThenBy(a => a.Id).First();
        var posicionados = new HashSet<AtomoMolecular> { raiz };
        var fila = new Queue<AtomoMolecular>(); fila.Enqueue(raiz);
        while (fila.Count > 0)
        {
            var pai = fila.Dequeue();
            var vizinhos = pai.Ligacoes.Select(l => l.Outro(pai)).Where(a => !posicionados.Contains(a)).OrderBy(a => a.Id).ToList();
            if (vizinhos.Count == 0) continue;
            Vector3[] dirs = DirecoesVSEPR(pai, vizinhos.Count);
            for (int i = 0; i < vizinhos.Count; i++)
            {
                var filho = vizinhos[i];
                filho.Posicao = pai.Posicao + dirs[i] * Distancia(pai, filho);
                posicionados.Add(filho); fila.Enqueue(filho);
            }
        }
        Vector3 centroide = Vector3.zero;
        foreach (var a in e.Atomos) centroide += a.Posicao;
        centroide /= e.Atomos.Count;
        foreach (var a in e.Atomos) a.Posicao -= centroide;
    }

    private static Vector3[] DirecoesVSEPR(AtomoMolecular centro, int quantidade)
    {
        int esterico = centro.Ligacoes.Count + EstimarParesLivres(centro);
        Vector3[] baseDirs;
        if (esterico <= 2) baseDirs = new[] { Vector3.right, Vector3.left };
        else if (esterico == 3) baseDirs = new[] { Vector3.right, new Vector3(-0.5f, 0, 0.8660254f), new Vector3(-0.5f, 0, -0.8660254f) };
        else if (esterico == 4) baseDirs = new[] { new Vector3(1,1,1).normalized, new Vector3(1,-1,-1).normalized, new Vector3(-1,1,-1).normalized, new Vector3(-1,-1,1).normalized };
        else if (esterico == 5) baseDirs = new[] { Vector3.up, Vector3.down, Vector3.right, new Vector3(-0.5f,0,0.8660254f), new Vector3(-0.5f,0,-0.8660254f) };
        else baseDirs = new[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

        // Orienta o conjunto local usando a ligação já posicionada como eixo de referência.
        var pai = centro.Ligacoes.Select(l => l.Outro(centro)).FirstOrDefault(a => (a.Posicao - centro.Posicao).sqrMagnitude > 0.001f);
        Quaternion rotacao = Quaternion.identity;
        if (pai != null) rotacao = Quaternion.FromToRotation(Vector3.right, (centro.Posicao - pai.Posicao).normalized);
        var saida = new Vector3[quantidade];
        for (int i = 0; i < quantidade; i++) saida[i] = (rotacao * baseDirs[i % baseDirs.Length]).normalized;
        return saida;
    }

    private static int EstimarParesLivres(AtomoMolecular a)
    {
        int z = a.NumeroAtomico;
        if (z == Atomos.O) return 2;
        if (z == Atomos.N) return a.Ligacoes.Count <= 3 ? 1 : 0;
        if (z == Atomos.C || z == Atomos.Si || z == Atomos.B) return 0;
        if (z == Atomos.F || z == Atomos.Cl || z == Atomos.Br || z == Atomos.I) return 3;
        if (z == Atomos.P || z == Atomos.S) return Math.Max(0, (Valencias(a.Info, a.OrdemTotal) - a.OrdemTotal) / 2);
        return 0;
    }

    private static float Distancia(AtomoMolecular a, AtomoMolecular b)
    {
        float ra = (float)a.Info.RaioAtomico / 100f;
        float rb = (float)b.Info.RaioAtomico / 100f;
        if (ra <= 0) ra = 0.7f;
        if (rb <= 0) rb = 0.7f;
        return Mathf.Clamp(ra + rb + 0.35f, DistanciaMinima, 2.6f);
    }
}

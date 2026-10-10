using System;
using System.Collections.Generic;
using System.Linq;
using BalanciadorQuimico;

public class EstruturaMolecular
{
    public List<AtomoMolecular> Atomos { get; private set; }
    public List<LigacaoMolecular> Ligacoes { get; private set; }
    public int CargaTotal { get; set; }
    public string Aviso { get; set; }

    public EstruturaMolecular() { Atomos = new List<AtomoMolecular>(); Ligacoes = new List<LigacaoMolecular>(); }

    public AtomoMolecular AdicionarAtomo(ElementoInfo info)
    {
        var a = new AtomoMolecular(Atomos.Count, info); Atomos.Add(a); return a;
    }

    public AtomoMolecular AdicionarAtomo(int z)
    {
        ElementoInfo info;
        if (!Elementos.Tabela.TryGetValue(z, out info)) throw new ArgumentException("Elemento Z=" + z + " não existe na tabela global.");
        return AdicionarAtomo(info);
    }

    public LigacaoMolecular AdicionarLigacao(AtomoMolecular a, AtomoMolecular b, int ordem = 1)
    {
        if (!Atomos.Contains(a) || !Atomos.Contains(b)) throw new ArgumentException("Os átomos devem pertencer à estrutura.");
        var existente = ObterLigacao(a, b);
        if (existente != null) { existente.DefinirOrdem(ordem); return existente; }
        var l = new LigacaoMolecular(a, b, ordem); Ligacoes.Add(l); a.Ligacoes.Add(l); b.Ligacoes.Add(l); return l;
    }

    public LigacaoMolecular ObterLigacao(AtomoMolecular a, AtomoMolecular b)
    {
        return Ligacoes.FirstOrDefault(l => (l.A == a && l.B == b) || (l.A == b && l.B == a));
    }
    public bool PossuiLigacao(AtomoMolecular a, AtomoMolecular b) { return ObterLigacao(a, b) != null; }

    public bool EhConexa()
    {
        if (Atomos.Count <= 1) return true;
        var visitados = new HashSet<AtomoMolecular>(); var pilha = new Stack<AtomoMolecular>(); pilha.Push(Atomos[0]);
        while (pilha.Count > 0) { var a = pilha.Pop(); if (!visitados.Add(a)) continue; foreach (var l in a.Ligacoes) pilha.Push(l.Outro(a)); }
        return visitados.Count == Atomos.Count;
    }

    public bool ValidarIntegridade(out string erro)
    {
        foreach (var l in Ligacoes)
        {
            if (!Atomos.Contains(l.A) || !Atomos.Contains(l.B)) { erro = "Existe ligação apontando para átomo fora da estrutura."; return false; }
            if (!l.A.Ligacoes.Contains(l) || !l.B.Ligacoes.Contains(l)) { erro = "As listas de ligações dos átomos estão inconsistentes."; return false; }
        }
        erro = null; return true;
    }
}

using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;

public class AtomoMolecular
{
    public int Id { get; private set; }
    public ElementoInfo Info { get; private set; }
    public int NumeroAtomico { get { return Info.Z; } }
    public Vector3 Posicao { get; set; }
    public List<LigacaoMolecular> Ligacoes { get; private set; }
    public int OrdemTotal
    {
        get { int total = 0; foreach (var l in Ligacoes) total += l.Ordem; return total; }
    }

    public AtomoMolecular(int id, ElementoInfo info)
    {
        if (info == null) throw new System.ArgumentNullException("info");
        Id = id; Info = info; Posicao = Vector3.zero; Ligacoes = new List<LigacaoMolecular>();
    }
}

using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;

public class AtomoMolecular
{
    public int Id { get; private set; }

    public int NumeroAtomico { get; private set; }

    public ElementoInfo Info { get; private set; }

    public Vector3 Posicao { get; set; }

    public List<LigacaoMolecular> Ligacoes { get; private set; }

    public int OrdemTotal
    {
        get
        {
            int total = 0;

            foreach (LigacaoMolecular ligacao in Ligacoes)
                total += ligacao.Ordem;

            return total;
        }
    }

    public AtomoMolecular(
        int id,
        ElementoInfo info)
    {
        Id = id;

        Info = info;

        NumeroAtomico = info.Z;

        Posicao = Vector3.zero;

        Ligacoes =
            new List<LigacaoMolecular>();
    }
}
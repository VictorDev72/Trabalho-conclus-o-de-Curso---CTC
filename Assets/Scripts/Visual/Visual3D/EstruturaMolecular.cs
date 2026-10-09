using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;

public class EstruturaMolecular
{
    public List<AtomoMolecular> Atomos { get; private set; }

    public List<LigacaoMolecular> Ligacoes { get; private set; }

    // Anéis da molécula (ex.: benzeno, sacarose), cada um com os átomos em ordem.
    public List<List<AtomoMolecular>> Aneis { get; private set; }

    public EstruturaMolecular()
    {
        Atomos =
            new List<AtomoMolecular>();

        Ligacoes =
            new List<LigacaoMolecular>();

        Aneis =
            new List<List<AtomoMolecular>>();
    }

    public AtomoMolecular AdicionarAtomo(
        ElementoInfo info)
    {
        AtomoMolecular atomo =
            new AtomoMolecular(
                Atomos.Count,
                info);

        Atomos.Add(atomo);

        return atomo;
    }

    public AtomoMolecular AdicionarAtomo(
        int numeroAtomico)
    {
        if (!Elementos.Tabela.TryGetValue(
            numeroAtomico,
            out ElementoInfo info))
        {
            throw new System.Exception(
                "Elemento não encontrado: " +
                numeroAtomico);
        }

        return AdicionarAtomo(info);
    }

    public LigacaoMolecular AdicionarLigacao(
        AtomoMolecular a,
        AtomoMolecular b,
        int ordem = 1)
    {
        if (a == b)
            return null;

        LigacaoMolecular existente =
            ObterLigacao(a, b);

        if (existente != null)
        {
            existente.Ordem = ordem;
            return existente;
        }

        LigacaoMolecular ligacao =
            new LigacaoMolecular(
                a,
                b,
                ordem);

        Ligacoes.Add(ligacao);

        a.Ligacoes.Add(ligacao);
        b.Ligacoes.Add(ligacao);

        return ligacao;
    }

    public LigacaoMolecular ObterLigacao(
        AtomoMolecular a,
        AtomoMolecular b)
    {
        foreach (LigacaoMolecular ligacao
                 in Ligacoes)
        {
            if (
                (ligacao.A == a &&
                 ligacao.B == b) ||

                (ligacao.A == b &&
                 ligacao.B == a)
            )
            {
                return ligacao;
            }
        }

        return null;
    }

    public bool PossuiLigacao(
        AtomoMolecular a,
        AtomoMolecular b)
    {
        return ObterLigacao(a, b) != null;
    }

    public int OrdemTotal(
        AtomoMolecular atomo)
    {
        return atomo.OrdemTotal;
    }

    // Copia os átomos e ligações de outra estrutura para esta,
    // deslocando as posições. Usado para juntar os íons de um sal.
    public void Anexar(
        EstruturaMolecular outra,
        Vector3 deslocamento)
    {
        Dictionary<AtomoMolecular, AtomoMolecular> copias =
            new Dictionary<AtomoMolecular, AtomoMolecular>();

        foreach (AtomoMolecular atomo in outra.Atomos)
        {
            AtomoMolecular copia =
                AdicionarAtomo(atomo.Info);

            copia.Posicao =
                atomo.Posicao + deslocamento;

            copias[atomo] = copia;
        }

        foreach (LigacaoMolecular ligacao in outra.Ligacoes)
        {
            AdicionarLigacao(
                copias[ligacao.A],
                copias[ligacao.B],
                ligacao.Ordem);
        }
    }
}

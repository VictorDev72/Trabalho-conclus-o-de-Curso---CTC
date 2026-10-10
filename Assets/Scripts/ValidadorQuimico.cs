using System.Collections.Generic;
using System.Linq;
using BalanciadorQuimico;

// Regras químicas usadas pelo GeradorDeProduto. Trabalha com os íons de cada
// substância (DadosSubstancia), nunca com o texto do nome.
public static class ValidadorQuimico
{
    // Fila de reatividade dos metais (+ H), do mais reativo para o menos reativo.
    private static readonly List<int> ReatividadeMetais = new List<int>
    {
        Atomos.K, Atomos.Ba, Atomos.Ca, Atomos.Na, Atomos.Mg, Atomos.Al,
        Atomos.Zn, Atomos.Fe, Atomos.Pb,
        Atomos.H,
        Atomos.Cu, Atomos.Hg, Atomos.Ag, Atomos.Au
    };

    // Fila de reatividade dos halogênios.
    private static readonly List<int> ReatividadeHalogenios = new List<int>
    {
        Atomos.F, Atomos.Cl, Atomos.Br, Atomos.I
    };

    // Metais que reagem com água fria formando hidróxido + H2.
    private static readonly HashSet<int> ReagemComAgua = new HashSet<int>
    {
        Atomos.K, Atomos.Na, Atomos.Ca, Atomos.Ba
    };

    // Metais cujos óxidos reagem com água formando base.
    private static readonly HashSet<int> OxidoBasicoReativo = new HashSet<int>
    {
        Atomos.Li, Atomos.Na, Atomos.K, Atomos.Ca, Atomos.Ba, Atomos.Mg
    };

    private static readonly HashSet<int> Alcalinos = new HashSet<int>
    {
        Atomos.Li, Atomos.Na, Atomos.K, Atomos.Rb, Atomos.Cs
    };

    private static readonly HashSet<int> Ametais = new HashSet<int>
    {
        Atomos.H, Atomos.He, Atomos.B, Atomos.C, Atomos.N, Atomos.O, Atomos.F, Atomos.Ne,
        Atomos.Si, Atomos.P, Atomos.S, Atomos.Cl, Atomos.Ar, Atomos.As, Atomos.Se,
        Atomos.Br, Atomos.Kr, Atomos.Te, Atomos.I, Atomos.Xe, Atomos.At, Atomos.Rn
    };

    // ── Classificação ────────────────────────────────────────────────────────

    public static bool EhMetal(int z) => !Ametais.Contains(z);

    public static bool EhHalogenio(int z) => ReatividadeHalogenios.Contains(z);

    // Elemento único de um íon monoatômico ({Na:1} → Na); 0 se for poliatômico.
    public static int ElementoDoIon(Dictionary<int, int> ion) =>
        ion != null && ion.Count == 1 && ion.Values.First() == 1 ? ion.Keys.First() : 0;

    public static bool EhAgua(DadosSubstancia s) =>
        s != null && s.Nome == "H2O";

    public static bool EhAmonio(Dictionary<int, int> cation) =>
        DadosSubstancia.Chave(cation) == DadosSubstancia.Chave(BancoSubstancias.NH4);

    public static bool EhOxidoMetalico(DadosSubstancia s) =>
        s.Funcao == FuncaoQuimica.Oxido && EhMetal(ElementoDoIon(s.Cation));

    public static bool EhOxidoBasicoReativo(DadosSubstancia s) =>
        EhOxidoMetalico(s) && OxidoBasicoReativo.Contains(ElementoDoIon(s.Cation));

    public static bool EhIonComposto(DadosSubstancia s) =>
        s.EhIonico && s.Funcao != FuncaoQuimica.Oxido;

    // ── Reatividade ──────────────────────────────────────────────────────────

    // true se o metal (ou H) livre é mais reativo que o que está no composto.
    public static bool DeslocaMetal(int livre, int noComposto)
    {
        int iLivre = ReatividadeMetais.IndexOf(livre);
        int iComposto = ReatividadeMetais.IndexOf(noComposto);
        return iLivre >= 0 && iComposto >= 0 && iLivre < iComposto;
    }

    public static bool DeslocaHalogenio(int livre, int noComposto)
    {
        int iLivre = ReatividadeHalogenios.IndexOf(livre);
        int iComposto = ReatividadeHalogenios.IndexOf(noComposto);
        return iLivre >= 0 && iComposto >= 0 && iLivre < iComposto;
    }

    public static bool ReageComAgua(int metal) => ReagemComAgua.Contains(metal);

    // ── Solubilidade (tabela prática) ────────────────────────────────────────

    public static bool EhInsoluvel(Dictionary<int, int> cation, Dictionary<int, int> anion)
    {
        int metal = ElementoDoIon(cation);

        // Sais de metais alcalinos e de amônio são solúveis.
        if (Alcalinos.Contains(metal) || EhAmonio(cation) || metal == Atomos.H)
            return false;

        string a = DadosSubstancia.Chave(anion);

        // Nitratos: sempre solúveis.
        if (a == DadosSubstancia.Chave(BancoSubstancias.NO3))
            return false;

        int elementoAnion = ElementoDoIon(anion);

        // Cloretos, brometos, iodetos: insolúveis com Ag, Pb, Hg.
        if (elementoAnion == Atomos.Cl || elementoAnion == Atomos.Br || elementoAnion == Atomos.I)
            return metal == Atomos.Ag || metal == Atomos.Pb || metal == Atomos.Hg;

        // Fluoretos: insolúveis com Ca, Mg, Ba.
        if (elementoAnion == Atomos.F)
            return metal == Atomos.Ca || metal == Atomos.Mg || metal == Atomos.Ba;

        // Sulfatos: insolúveis com Ba, Ca, Sr, Pb.
        if (a == DadosSubstancia.Chave(BancoSubstancias.SO4))
            return metal == Atomos.Ba || metal == Atomos.Ca || metal == Atomos.Sr || metal == Atomos.Pb;

        // Hidróxidos: insolúveis exceto alcalinos, Ca, Ba, Sr.
        if (a == DadosSubstancia.Chave(BancoSubstancias.OH))
            return !(metal == Atomos.Ca || metal == Atomos.Ba || metal == Atomos.Sr);

        // Carbonatos, fosfatos, sulfitos e sulfetos: insolúveis (exceto alcalinos/amônio, já tratados).
        if (a == DadosSubstancia.Chave(BancoSubstancias.CO3) ||
            a == DadosSubstancia.Chave(BancoSubstancias.PO4) ||
            a == DadosSubstancia.Chave(BancoSubstancias.SO3) ||
            elementoAnion == Atomos.S)
            return true;

        return false;
    }

    // pKa aproximado (1ª ionização). Quanto menor, mais forte o ácido.
    private static readonly Dictionary<string, double> PKa = new Dictionary<string, double>
    {
        { "HCl", -7 }, { "HBr", -9 }, { "HI", -10 }, { "HNO3", -1.4 }, { "H2SO4", -3 },
        { "H2SO3", 1.8 }, { "H3PO4", 2.1 }, { "HF", 3.2 }, { "HNO2", 3.3 },
        { "H2CO3", 6.4 }, { "H2S", 7.0 }
    };

    public static bool EhAcidoForte(string acido) =>
        PKa.TryGetValue(acido, out double pka) && pka < 0;

    // true se o ácido "a" é mais forte que "b" (ácidos fortes empatam entre si).
    public static bool AcidoMaisForte(string a, string b)
    {
        if (!PKa.TryGetValue(a, out double pa) || !PKa.TryGetValue(b, out double pb))
            return false;
        bool aForte = pa < 0, bForte = pb < 0;
        if (aForte && bForte) return false;
        return pa < pb;
    }

    // Precipitados que não se dissolvem em ácido: podem se formar mesmo
    // quando a reação libera um ácido forte (HCl + AgNO3 → AgCl↓ + HNO3).
    public static bool InsoluvelEmAcido(Dictionary<int, int> cation, Dictionary<int, int> anion)
    {
        int metal = ElementoDoIon(cation);
        int elementoAnion = ElementoDoIon(anion);
        string a = DadosSubstancia.Chave(anion);

        if (metal == Atomos.Ag && (elementoAnion == Atomos.Cl || elementoAnion == Atomos.Br || elementoAnion == Atomos.I))
            return true;
        if (metal == Atomos.Ba && a == DadosSubstancia.Chave(BancoSubstancias.SO4))
            return true;
        if (elementoAnion == Atomos.S && (metal == Atomos.Cu || metal == Atomos.Ag || metal == Atomos.Pb || metal == Atomos.Hg))
            return true;
        return false;
    }

    // Gases que escapam numa dupla troca e servem de "força motriz".
    public static bool EhGas(DadosSubstancia s) =>
        s != null && (s.Nome == "CO2" || s.Nome == "SO2" || s.Nome == "H2S" || s.Nome == "NH3");
}

using System.Collections.Generic;
using UnityEngine;
using BalanciadorQuimico;
using static FuncaoQuimica;

// Banco de substâncias do simulador. Também é o CATÁLOGO de produtos:
// o GeradorDeProduto só aceita um produto cuja fórmula exista aqui,
// o que impede que moléculas inexistentes (NaCl2, Al2O9...) apareçam.
//
// Para adicionar uma substância basta incluir uma linha na lista abaixo.
// Compostos iônicos recebem a unidade de cada íon e sua carga; a fórmula
// é calculada automaticamente.
public static class BancoSubstancias
{
    // ── Íons usados nos compostos ───────────────────────────────────────────
    private static Dictionary<int, int> Ion(int z, int n = 1) => new Dictionary<int, int> { { z, n } };
    private static Dictionary<int, int> Ion(int z1, int n1, int z2, int n2) =>
        new Dictionary<int, int> { { z1, n1 }, { z2, n2 } };
    private static Dictionary<int, int> Ion(int z1, int n1, int z2, int n2, int z3, int n3) =>
        new Dictionary<int, int> { { z1, n1 }, { z2, n2 }, { z3, n3 } };

    public static Dictionary<int, int> OH   => Ion(Atomos.O, 1, Atomos.H, 1);
    public static Dictionary<int, int> SO4  => Ion(Atomos.S, 1, Atomos.O, 4);
    public static Dictionary<int, int> SO3  => Ion(Atomos.S, 1, Atomos.O, 3);
    public static Dictionary<int, int> CO3  => Ion(Atomos.C, 1, Atomos.O, 3);
    public static Dictionary<int, int> HCO3 => Ion(Atomos.H, 1, Atomos.C, 1, Atomos.O, 3);
    public static Dictionary<int, int> NO3  => Ion(Atomos.N, 1, Atomos.O, 3);
    public static Dictionary<int, int> NO2  => Ion(Atomos.N, 1, Atomos.O, 2);
    public static Dictionary<int, int> PO4  => Ion(Atomos.P, 1, Atomos.O, 4);
    public static Dictionary<int, int> NH4  => Ion(Atomos.N, 1, Atomos.H, 4);

    private static DadosSubstancia S(string nome, int z, int atomicidade, int[] cargas, bool combustivel = false) =>
        DadosSubstancia.Simples(nome, z, atomicidade, cargas, combustivel);

    private static DadosSubstancia I(string nome, FuncaoQuimica funcao,
                                     Dictionary<int, int> cation, int qc,
                                     Dictionary<int, int> anion, int qa, bool combustivel = false) =>
        DadosSubstancia.Ionico(nome, funcao, cation, qc, anion, qa, combustivel);

    private static DadosSubstancia M(string nome, Dictionary<int, int> formula, bool combustivel = false) =>
        DadosSubstancia.Molecular(nome, formula, combustivel);

    // ── Banco ────────────────────────────────────────────────────────────────
    public static readonly List<DadosSubstancia> Todas = new List<DadosSubstancia>
    {
        // Substâncias simples: cargas em ordem de preferência na síntese.
        S("H2",  Atomos.H,  2, new[] { +1 }, combustivel: true),
        S("O2",  Atomos.O,  2, new[] { -2 }),
        S("N2",  Atomos.N,  2, new[] { -3, +2, +4, +1 }),
        S("F2",  Atomos.F,  2, new[] { -1 }),
        S("Cl2", Atomos.Cl, 2, new[] { -1 }),
        S("Br2", Atomos.Br, 2, new[] { -1 }),
        S("I2",  Atomos.I,  2, new[] { -1 }),
        S("C",   Atomos.C,  1, new[] { +4, +2, -4 }, combustivel: true),
        S("S",   Atomos.S,  1, new[] { +4, +6, -2 }, combustivel: true),
        S("P",   Atomos.P,  1, new[] { +5, -3 }),
        S("Na",  Atomos.Na, 1, new[] { +1 }),
        S("K",   Atomos.K,  1, new[] { +1 }),
        S("Ca",  Atomos.Ca, 1, new[] { +2 }),
        S("Mg",  Atomos.Mg, 1, new[] { +2 }),
        S("Al",  Atomos.Al, 1, new[] { +3 }),
        S("Fe",  Atomos.Fe, 1, new[] { +3, +2 }),
        S("Cu",  Atomos.Cu, 1, new[] { +2, +1 }),
        S("Zn",  Atomos.Zn, 1, new[] { +2 }),
        S("Ag",  Atomos.Ag, 1, new[] { +1 }),

        // Óxidos
        I("H2O",   Oxido, Ion(Atomos.H),  +1, Ion(Atomos.O), -2),
        I("CO2",   Oxido, Ion(Atomos.C),  +4, Ion(Atomos.O), -2),
        I("CO",    Oxido, Ion(Atomos.C),  +2, Ion(Atomos.O), -2, combustivel: true),
        I("SO2",   Oxido, Ion(Atomos.S),  +4, Ion(Atomos.O), -2),
        I("SO3",   Oxido, Ion(Atomos.S),  +6, Ion(Atomos.O), -2),
        I("NO",    Oxido, Ion(Atomos.N),  +2, Ion(Atomos.O), -2),
        I("NO2",   Oxido, Ion(Atomos.N),  +4, Ion(Atomos.O), -2),
        I("N2O",   Oxido, Ion(Atomos.N),  +1, Ion(Atomos.O), -2),
        I("P2O5",  Oxido, Ion(Atomos.P),  +5, Ion(Atomos.O), -2),
        I("Fe2O3", Oxido, Ion(Atomos.Fe), +3, Ion(Atomos.O), -2),
        I("CaO",   Oxido, Ion(Atomos.Ca), +2, Ion(Atomos.O), -2),
        I("MgO",   Oxido, Ion(Atomos.Mg), +2, Ion(Atomos.O), -2),
        I("Al2O3", Oxido, Ion(Atomos.Al), +3, Ion(Atomos.O), -2),
        I("Na2O",  Oxido, Ion(Atomos.Na), +1, Ion(Atomos.O), -2),
        I("K2O",   Oxido, Ion(Atomos.K),  +1, Ion(Atomos.O), -2),
        I("ZnO",   Oxido, Ion(Atomos.Zn), +2, Ion(Atomos.O), -2),
        I("CuO",   Oxido, Ion(Atomos.Cu), +2, Ion(Atomos.O), -2),

        // Ácidos
        I("HCl",   Acido, Ion(Atomos.H), +1, Ion(Atomos.Cl), -1),
        I("HF",    Acido, Ion(Atomos.H), +1, Ion(Atomos.F),  -1),
        I("HBr",   Acido, Ion(Atomos.H), +1, Ion(Atomos.Br), -1),
        I("HI",    Acido, Ion(Atomos.H), +1, Ion(Atomos.I),  -1),
        I("H2S",   Acido, Ion(Atomos.H), +1, Ion(Atomos.S),  -2, combustivel: true),
        I("H2SO4", Acido, Ion(Atomos.H), +1, SO4, -2),
        I("H2SO3", Acido, Ion(Atomos.H), +1, SO3, -2),
        I("HNO3",  Acido, Ion(Atomos.H), +1, NO3, -1),
        I("HNO2",  Acido, Ion(Atomos.H), +1, NO2, -1),
        I("H3PO4", Acido, Ion(Atomos.H), +1, PO4, -3),
        I("H2CO3", Acido, Ion(Atomos.H), +1, CO3, -2),

        // Bases
        I("NaOH",    Base, Ion(Atomos.Na), +1, OH, -1),
        I("KOH",     Base, Ion(Atomos.K),  +1, OH, -1),
        I("Ca(OH)2", Base, Ion(Atomos.Ca), +2, OH, -1),
        I("Mg(OH)2", Base, Ion(Atomos.Mg), +2, OH, -1),
        I("Ba(OH)2", Base, Ion(Atomos.Ba), +2, OH, -1),
        I("Al(OH)3", Base, Ion(Atomos.Al), +3, OH, -1),
        I("Fe(OH)2", Base, Ion(Atomos.Fe), +2, OH, -1),
        I("Fe(OH)3", Base, Ion(Atomos.Fe), +3, OH, -1),
        I("Cu(OH)2", Base, Ion(Atomos.Cu), +2, OH, -1),
        I("Zn(OH)2", Base, Ion(Atomos.Zn), +2, OH, -1),
        M("NH3", new Dictionary<int, int> { { Atomos.N, 1 }, { Atomos.H, 3 } }, combustivel: true),

        // Sais — cloretos, fluoretos, brometos, iodetos
        I("NaCl",  Sal, Ion(Atomos.Na), +1, Ion(Atomos.Cl), -1),
        I("KCl",   Sal, Ion(Atomos.K),  +1, Ion(Atomos.Cl), -1),
        I("CaCl2", Sal, Ion(Atomos.Ca), +2, Ion(Atomos.Cl), -1),
        I("MgCl2", Sal, Ion(Atomos.Mg), +2, Ion(Atomos.Cl), -1),
        I("BaCl2", Sal, Ion(Atomos.Ba), +2, Ion(Atomos.Cl), -1),
        I("AlCl3", Sal, Ion(Atomos.Al), +3, Ion(Atomos.Cl), -1),
        I("FeCl2", Sal, Ion(Atomos.Fe), +2, Ion(Atomos.Cl), -1),
        I("FeCl3", Sal, Ion(Atomos.Fe), +3, Ion(Atomos.Cl), -1),
        I("CuCl2", Sal, Ion(Atomos.Cu), +2, Ion(Atomos.Cl), -1),
        I("ZnCl2", Sal, Ion(Atomos.Zn), +2, Ion(Atomos.Cl), -1),
        I("AgCl",  Sal, Ion(Atomos.Ag), +1, Ion(Atomos.Cl), -1),
        I("NH4Cl", Sal, NH4, +1, Ion(Atomos.Cl), -1),
        I("NaF",   Sal, Ion(Atomos.Na), +1, Ion(Atomos.F),  -1),
        I("CaF2",  Sal, Ion(Atomos.Ca), +2, Ion(Atomos.F),  -1),
        I("NaBr",  Sal, Ion(Atomos.Na), +1, Ion(Atomos.Br), -1),
        I("KBr",   Sal, Ion(Atomos.K),  +1, Ion(Atomos.Br), -1),
        I("AgBr",  Sal, Ion(Atomos.Ag), +1, Ion(Atomos.Br), -1),
        I("KI",    Sal, Ion(Atomos.K),  +1, Ion(Atomos.I),  -1),
        I("AgI",   Sal, Ion(Atomos.Ag), +1, Ion(Atomos.I),  -1),

        // Sais — sulfetos
        I("Na2S", Sal, Ion(Atomos.Na), +1, Ion(Atomos.S), -2),
        I("FeS",  Sal, Ion(Atomos.Fe), +2, Ion(Atomos.S), -2),
        I("ZnS",  Sal, Ion(Atomos.Zn), +2, Ion(Atomos.S), -2),
        I("CuS",  Sal, Ion(Atomos.Cu), +2, Ion(Atomos.S), -2),

        // Sais — sulfatos e sulfitos
        I("Na2SO4",    Sal, Ion(Atomos.Na), +1, SO4, -2),
        I("K2SO4",     Sal, Ion(Atomos.K),  +1, SO4, -2),
        I("CaSO4",     Sal, Ion(Atomos.Ca), +2, SO4, -2),
        I("MgSO4",     Sal, Ion(Atomos.Mg), +2, SO4, -2),
        I("BaSO4",     Sal, Ion(Atomos.Ba), +2, SO4, -2),
        I("ZnSO4",     Sal, Ion(Atomos.Zn), +2, SO4, -2),
        I("FeSO4",     Sal, Ion(Atomos.Fe), +2, SO4, -2),
        I("CuSO4",     Sal, Ion(Atomos.Cu), +2, SO4, -2),
        I("Al2(SO4)3", Sal, Ion(Atomos.Al), +3, SO4, -2),
        I("(NH4)2SO4", Sal, NH4, +1, SO4, -2),
        I("Na2SO3",    Sal, Ion(Atomos.Na), +1, SO3, -2),
        I("CaSO3",     Sal, Ion(Atomos.Ca), +2, SO3, -2),

        // Sais — carbonatos
        I("Na2CO3", Sal, Ion(Atomos.Na), +1, CO3, -2),
        I("K2CO3",  Sal, Ion(Atomos.K),  +1, CO3, -2),
        I("CaCO3",  Sal, Ion(Atomos.Ca), +2, CO3, -2),
        I("MgCO3",  Sal, Ion(Atomos.Mg), +2, CO3, -2),
        I("BaCO3",  Sal, Ion(Atomos.Ba), +2, CO3, -2),
        I("NaHCO3", Sal, Ion(Atomos.Na), +1, HCO3, -1),

        // Sais — nitratos e fosfatos
        I("KNO3",      Sal, Ion(Atomos.K),  +1, NO3, -1),
        I("NaNO3",     Sal, Ion(Atomos.Na), +1, NO3, -1),
        I("AgNO3",     Sal, Ion(Atomos.Ag), +1, NO3, -1),
        I("KNO2",      Sal, Ion(Atomos.K),  +1, NO2, -1),
        I("NaNO2",     Sal, Ion(Atomos.Na), +1, NO2, -1),
        I("Ca(NO3)2",  Sal, Ion(Atomos.Ca), +2, NO3, -1),
        I("Cu(NO3)2",  Sal, Ion(Atomos.Cu), +2, NO3, -1),
        I("Zn(NO3)2",  Sal, Ion(Atomos.Zn), +2, NO3, -1),
        I("NH4NO3",    Sal, NH4, +1, NO3, -1),
        I("Na3PO4",    Sal, Ion(Atomos.Na), +1, PO4, -3),
        I("Ca3(PO4)2", Sal, Ion(Atomos.Ca), +2, PO4, -3),

        // Compostos orgânicos
        M("CH4",       new Dictionary<int, int> { { Atomos.C, 1 },  { Atomos.H, 4 } },  combustivel: true),
        M("C2H6",      new Dictionary<int, int> { { Atomos.C, 2 },  { Atomos.H, 6 } },  combustivel: true),
        M("C3H8",      new Dictionary<int, int> { { Atomos.C, 3 },  { Atomos.H, 8 } },  combustivel: true),
        M("C4H10",     new Dictionary<int, int> { { Atomos.C, 4 },  { Atomos.H, 10 } }, combustivel: true),
        M("C2H4",      new Dictionary<int, int> { { Atomos.C, 2 },  { Atomos.H, 4 } },  combustivel: true),
        M("C2H2",      new Dictionary<int, int> { { Atomos.C, 2 },  { Atomos.H, 2 } },  combustivel: true),
        M("C6H6",      new Dictionary<int, int> { { Atomos.C, 6 },  { Atomos.H, 6 } },  combustivel: true),
        M("CH3OH",     new Dictionary<int, int> { { Atomos.C, 1 },  { Atomos.H, 4 },  { Atomos.O, 1 } },  combustivel: true),
        M("C2H5OH",    new Dictionary<int, int> { { Atomos.C, 2 },  { Atomos.H, 6 },  { Atomos.O, 1 } },  combustivel: true),
        M("C6H12O6",   new Dictionary<int, int> { { Atomos.C, 6 },  { Atomos.H, 12 }, { Atomos.O, 6 } },  combustivel: true),
        M("C12H22O11", new Dictionary<int, int> { { Atomos.C, 12 }, { Atomos.H, 22 }, { Atomos.O, 11 } }, combustivel: true),
    };

    private static readonly Dictionary<string, DadosSubstancia> porNome = new Dictionary<string, DadosSubstancia>();
    private static readonly Dictionary<string, DadosSubstancia> porFormula = new Dictionary<string, DadosSubstancia>();
    private static readonly Dictionary<int, DadosSubstancia> simplesPorElemento = new Dictionary<int, DadosSubstancia>();

    static BancoSubstancias()
    {
        foreach (var s in Todas)
        {
            porNome[s.Nome] = s;

            string chave = DadosSubstancia.Chave(s.Formula);
            if (porFormula.TryGetValue(chave, out var existente))
                Debug.LogWarning($"[BancoSubstancias] {s.Nome} e {existente.Nome} têm a mesma fórmula.");
            else
                porFormula[chave] = s;

            if (s.EhSimples)
                simplesPorElemento[s.Elemento] = s;
        }
    }

    public static DadosSubstancia PorNome(string nome) =>
        porNome.TryGetValue(nome, out var s) ? s : null;

    // Procura no catálogo a substância com exatamente essa composição.
    public static DadosSubstancia PorFormula(Dictionary<int, int> composicao) =>
        composicao != null && porFormula.TryGetValue(DadosSubstancia.Chave(composicao), out var s) ? s : null;

    public static DadosSubstancia SimplesDoElemento(int z) =>
        simplesPorElemento.TryGetValue(z, out var s) ? s : null;

    // Composto iônico cátion + ânion, se existir no catálogo.
    public static DadosSubstancia Combinar(Dictionary<int, int> cation, int cargaCation,
                                           Dictionary<int, int> anion, int cargaAnion)
    {
        if (cargaCation <= 0 || cargaAnion >= 0) return null;
        return PorFormula(DadosSubstancia.MontarFormulaIonica(cation, cargaCation, anion, cargaAnion));
    }
}

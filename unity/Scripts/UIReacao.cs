using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ideias;
using unity.Scripts;
using Elementos;

public class UIReacao : MonoBehaviour
{
    public TMP_Dropdown dropPart1;
    public TMP_Dropdown dropPart2;
    public TMP_Dropdown dropTipoReacao;

    private readonly Dictionary<string, Dictionary<int, int>> banco = new()
    {
        // ── Substâncias simples ────────────────────────────────────
        { "H2",   new Dictionary<int, int> { { Atomos.H,  2 } } },
        { "O2",   new Dictionary<int, int> { { Atomos.O,  2 } } },
        { "N2",   new Dictionary<int, int> { { Atomos.N,  2 } } },
        { "F2",   new Dictionary<int, int> { { Atomos.F,  2 } } },
        { "Cl2",  new Dictionary<int, int> { { Atomos.Cl, 2 } } },
        { "Br2",  new Dictionary<int, int> { { Atomos.Br, 2 } } },
        { "I2",   new Dictionary<int, int> { { Atomos.I,  2 } } },
        { "C",    new Dictionary<int, int> { { Atomos.C,  1 } } },
        { "S",    new Dictionary<int, int> { { Atomos.S,  1 } } },
        { "P",    new Dictionary<int, int> { { Atomos.P,  1 } } },
        { "Na",   new Dictionary<int, int> { { Atomos.Na, 1 } } },
        { "K",    new Dictionary<int, int> { { Atomos.K,  1 } } },
        { "Ca",   new Dictionary<int, int> { { Atomos.Ca, 1 } } },
        { "Mg",   new Dictionary<int, int> { { Atomos.Mg, 1 } } },
        { "Al",   new Dictionary<int, int> { { Atomos.Al, 1 } } },
        { "Fe",   new Dictionary<int, int> { { Atomos.Fe, 1 } } },
        { "Cu",   new Dictionary<int, int> { { Atomos.Cu, 1 } } },
        { "Zn",   new Dictionary<int, int> { { Atomos.Zn, 1 } } },

        // ── Óxidos ────────────────────────────────────────────────
        { "H2O",  new Dictionary<int, int> { { Atomos.H,  2 }, { Atomos.O, 1 } } },
        { "CO2",  new Dictionary<int, int> { { Atomos.C,  1 }, { Atomos.O, 2 } } },
        { "CO",   new Dictionary<int, int> { { Atomos.C,  1 }, { Atomos.O, 1 } } },
        { "SO2",  new Dictionary<int, int> { { Atomos.S,  1 }, { Atomos.O, 2 } } },
        { "SO3",  new Dictionary<int, int> { { Atomos.S,  1 }, { Atomos.O, 3 } } },
        { "NO",   new Dictionary<int, int> { { Atomos.N,  1 }, { Atomos.O, 1 } } },
        { "NO2",  new Dictionary<int, int> { { Atomos.N,  1 }, { Atomos.O, 2 } } },
        { "N2O",  new Dictionary<int, int> { { Atomos.N,  2 }, { Atomos.O, 1 } } },
        { "Fe2O3",new Dictionary<int, int> { { Atomos.Fe, 2 }, { Atomos.O, 3 } } },
        { "Fe3O4",new Dictionary<int, int> { { Atomos.Fe, 3 }, { Atomos.O, 4 } } },
        { "CaO",  new Dictionary<int, int> { { Atomos.Ca, 1 }, { Atomos.O, 1 } } },
        { "MgO",  new Dictionary<int, int> { { Atomos.Mg, 1 }, { Atomos.O, 1 } } },
        { "Al2O3",new Dictionary<int, int> { { Atomos.Al, 2 }, { Atomos.O, 3 } } },
        { "Na2O", new Dictionary<int, int> { { Atomos.Na, 2 }, { Atomos.O, 1 } } },
        { "K2O",  new Dictionary<int, int> { { Atomos.K,  2 }, { Atomos.O, 1 } } },
        { "ZnO",  new Dictionary<int, int> { { Atomos.Zn, 1 }, { Atomos.O, 1 } } },
        { "CuO",  new Dictionary<int, int> { { Atomos.Cu, 1 }, { Atomos.O, 1 } } },
        { "P2O5", new Dictionary<int, int> { { Atomos.P,  2 }, { Atomos.O, 5 } } },

        // ── Ácidos ────────────────────────────────────────────────
        { "HCl",  new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.Cl, 1 } } },
        { "HF",   new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.F,  1 } } },
        { "HBr",  new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.Br, 1 } } },
        { "HI",   new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.I,  1 } } },
        { "H2S",  new Dictionary<int, int> { { Atomos.H,  2 }, { Atomos.S,  1 } } },
        { "H2SO4",new Dictionary<int, int> { { Atomos.H,  2 }, { Atomos.S,  1 }, { Atomos.O, 4 } } },
        { "H2SO3",new Dictionary<int, int> { { Atomos.H,  2 }, { Atomos.S,  1 }, { Atomos.O, 3 } } },
        { "HNO3", new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.N,  1 }, { Atomos.O, 3 } } },
        { "HNO2", new Dictionary<int, int> { { Atomos.H,  1 }, { Atomos.N,  1 }, { Atomos.O, 2 } } },
        { "H3PO4",new Dictionary<int, int> { { Atomos.H,  3 }, { Atomos.P,  1 }, { Atomos.O, 4 } } },
        { "H2CO3",new Dictionary<int, int> { { Atomos.H,  2 }, { Atomos.C,  1 }, { Atomos.O, 3 } } },

        // ── Bases ─────────────────────────────────────────────────
        { "NaOH", new Dictionary<int, int> { { Atomos.Na, 1 }, { Atomos.O,  1 }, { Atomos.H, 1 } } },
        { "KOH",  new Dictionary<int, int> { { Atomos.K,  1 }, { Atomos.O,  1 }, { Atomos.H, 1 } } },
        { "Ca(OH)2", new Dictionary<int, int> { { Atomos.Ca, 1 }, { Atomos.O, 2 }, { Atomos.H, 2 } } },
        { "Mg(OH)2", new Dictionary<int, int> { { Atomos.Mg, 1 }, { Atomos.O, 2 }, { Atomos.H, 2 } } },
        { "Al(OH)3", new Dictionary<int, int> { { Atomos.Al, 1 }, { Atomos.O, 3 }, { Atomos.H, 3 } } },
        { "NH3",  new Dictionary<int, int> { { Atomos.N,  1 }, { Atomos.H,  3 } } },

        // ── Sais ──────────────────────────────────────────────────
        { "NaCl", new Dictionary<int, int> { { Atomos.Na, 1 }, { Atomos.Cl, 1 } } },
        { "KCl",  new Dictionary<int, int> { { Atomos.K,  1 }, { Atomos.Cl, 1 } } },
        { "CaCl2",new Dictionary<int, int> { { Atomos.Ca, 1 }, { Atomos.Cl, 2 } } },
        { "MgCl2",new Dictionary<int, int> { { Atomos.Mg, 1 }, { Atomos.Cl, 2 } } },
        { "AlCl3",new Dictionary<int, int> { { Atomos.Al, 1 }, { Atomos.Cl, 3 } } },
        { "FeCl2",new Dictionary<int, int> { { Atomos.Fe, 1 }, { Atomos.Cl, 2 } } },
        { "FeCl3",new Dictionary<int, int> { { Atomos.Fe, 1 }, { Atomos.Cl, 3 } } },
        { "CuCl2",new Dictionary<int, int> { { Atomos.Cu, 1 }, { Atomos.Cl, 2 } } },
        { "ZnCl2",new Dictionary<int, int> { { Atomos.Zn, 1 }, { Atomos.Cl, 2 } } },
        { "Na2SO4",new Dictionary<int, int> { { Atomos.Na, 2 }, { Atomos.S, 1 }, { Atomos.O, 4 } } },
        { "CaSO4", new Dictionary<int, int> { { Atomos.Ca, 1 }, { Atomos.S, 1 }, { Atomos.O, 4 } } },
        { "MgSO4", new Dictionary<int, int> { { Atomos.Mg, 1 }, { Atomos.S, 1 }, { Atomos.O, 4 } } },
        { "Na2CO3",new Dictionary<int, int> { { Atomos.Na, 2 }, { Atomos.C, 1 }, { Atomos.O, 3 } } },
        { "CaCO3", new Dictionary<int, int> { { Atomos.Ca, 1 }, { Atomos.C, 1 }, { Atomos.O, 3 } } },
        { "NaHCO3",new Dictionary<int, int> { { Atomos.Na, 1 }, { Atomos.H, 1 }, { Atomos.C, 1 }, { Atomos.O, 3 } } },
        { "NH4Cl", new Dictionary<int, int> { { Atomos.N,  1 }, { Atomos.H, 4 }, { Atomos.Cl, 1 } } },
        { "KNO3",  new Dictionary<int, int> { { Atomos.K,  1 }, { Atomos.N, 1 }, { Atomos.O, 3 } } },
        { "NaNO3", new Dictionary<int, int> { { Atomos.Na, 1 }, { Atomos.N, 1 }, { Atomos.O, 3 } } },

        // ── Compostos orgânicos simples ────────────────────────────
        { "CH4",  new Dictionary<int, int> { { Atomos.C,  1 }, { Atomos.H,  4 } } },
        { "C2H6", new Dictionary<int, int> { { Atomos.C,  2 }, { Atomos.H,  6 } } },
        { "C3H8", new Dictionary<int, int> { { Atomos.C,  3 }, { Atomos.H,  8 } } },
        { "C4H10",new Dictionary<int, int> { { Atomos.C,  4 }, { Atomos.H, 10 } } },
        { "C2H4", new Dictionary<int, int> { { Atomos.C,  2 }, { Atomos.H,  4 } } },
        { "C2H2", new Dictionary<int, int> { { Atomos.C,  2 }, { Atomos.H,  2 } } },
        { "C6H6", new Dictionary<int, int> { { Atomos.C,  6 }, { Atomos.H,  6 } } },
        { "CH3OH",new Dictionary<int, int> { { Atomos.C,  1 }, { Atomos.H,  4 }, { Atomos.O, 1 } } },
        { "C2H5OH",new Dictionary<int,int> { { Atomos.C,  2 }, { Atomos.H,  6 }, { Atomos.O, 1 } } },
        { "C6H12O6",new Dictionary<int,int>{ { Atomos.C,  6 }, { Atomos.H, 12 }, { Atomos.O, 6 } } },
        { "C12H22O11",new Dictionary<int,int>{{ Atomos.C,12 }, { Atomos.H, 22 }, { Atomos.O,11 } } },
    };

    private List<string> opcoes = new List<string>()
    {
        "H2", "O2", "H2O", "CO2", "C"
    };

    private List<string> tiposReacao = new List<string>()
    {
        "Sintese", "Decomposicao", "Combustao", "Simples Troca", "Dupla Troca"
    };

    // Modelo de partes/cargas usado só pelo GeradorDeProduto (cargaB = 0 => espécie simples, sem ParteB)
    private readonly Dictionary<string, EspecieQuimica> bancoEspecies = new()
    {
        { "H2",  new EspecieQuimica(new Dictionary<int, int> { { Atomos.H, 2 } }, null, 1) },
        { "O2",  new EspecieQuimica(new Dictionary<int, int> { { Atomos.O, 2 } }, null, 2) },
        { "C",   new EspecieQuimica(new Dictionary<int, int> { { Atomos.C, 1 } }, null, 4) },
        { "H2O", new EspecieQuimica(new Dictionary<int, int> { { Atomos.H, 2 } }, new Dictionary<int, int> { { Atomos.O, 1 } }, 1, 2) },
        { "CO2", new EspecieQuimica(new Dictionary<int, int> { { Atomos.C, 1 } }, new Dictionary<int, int> { { Atomos.O, 2 } }, 4, 2) },
    };

    private static readonly Dictionary<int, string> simbolos = typeof(Atomos)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(f => (int)f.GetValue(null), f => f.Name);

    void Start()
    {
        dropPart1.ClearOptions();
        dropPart2.ClearOptions();
        dropTipoReacao.ClearOptions();

        dropPart1.AddOptions(opcoes);
        dropPart2.AddOptions(opcoes);
        dropTipoReacao.AddOptions(tiposReacao);
    }

    public void Reagir()
    {
        string r1 = dropPart1.options[dropPart1.value].text;
        string r2 = dropPart2.options[dropPart2.value].text;
        string tipo = dropTipoReacao.options[dropTipoReacao.value].text;

        if (!bancoEspecies.TryGetValue(r1, out var especieA) || !bancoEspecies.TryGetValue(r2, out var especieB))
        {
            Debug.LogError($"Espécie química não cadastrada para '{r1}' ou '{r2}'.");
            return;
        }

        try
        {
            var produtos = GeradorDeProduto.GerarProduto(tipo, especieA, especieB);

            var eq = new EquacaoQuimica();

            // Decomposição só consome a primeira espécie (AB -> A + B); a segunda é ignorada.
            var reagentesFormula = tipo == "Decomposicao" ? new[] { r1 } : new[] { r1, r2 };
            foreach (var formula in reagentesFormula)
                eq.Reagentes.Add(banco[formula]);

            eq.Produtos.AddRange(produtos);

            var resultado = eq.Balancear();

            var ladoEsquerdo = string.Join(" + ", reagentesFormula.Select((f, i) => $"{resultado[i]}{f}"));
            var ladoDireito = string.Join(" + ", produtos.Select((p, i) => $"{resultado[reagentesFormula.Length + i]}{FormatarFormula(p)}"));

            Debug.Log($"{ladoEsquerdo} -> {ladoDireito}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Não foi possível balancear a reação: {ex.Message}");
        }
    }

    private static string FormatarFormula(Dictionary<int, int> atomos)
    {
        return string.Join("", atomos.OrderBy(a => a.Key)
            .Select(a => simbolos[a.Key] + (a.Value > 1 ? a.Value.ToString() : "")));
    }
}

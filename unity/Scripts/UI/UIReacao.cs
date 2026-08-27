using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using BalanciadorQuimico;
using System;
using System.Linq;
using NUnit.Framework;

// Estrutura para armazenar os dados de cada substância do banco parteA, parteB e cargas.
public struct DadosSubstancia
{
    public Dictionary<int, int> ParteA;
    public Dictionary<int, int> ParteB;   // null para substâncias simples
    public int CargaA;
    public int CargaB;

    public DadosSubstancia(Dictionary<int, int> parteA, int cargaA,
                           Dictionary<int, int> parteB = null, int cargaB = 0)
    {
        ParteA = parteA;
        ParteB = parteB;
        CargaA = cargaA;
        CargaB = cargaB;
    }

    
    public Dictionary<int, int> ToFormula()
    {
        var res = new Dictionary<int, int>(ParteA);
        if (ParteB != null)
        {
            foreach (var par in ParteB)
            {
                if (res.ContainsKey(par.Key)) res[par.Key] += par.Value;
                else                          res[par.Key]  = par.Value;
            }
        }
        return res;
    }

    //Constrói o EspecieQuimica correspondente.
    //construtor de EspecieQuimica tem default cargaB=0 MAS lança
    //(sem parteB) recebem cargaB = cargaA para satisfazer o construtor
    public EspecieQuimica ToEspecieQuimica()
    {
        int cargaBEfetiva = (CargaB == 0) ? CargaA : CargaB;
        return new EspecieQuimica(
            ParteA,
            ParteB ?? new Dictionary<int, int>(),
            CargaA,
            cargaBEfetiva   
        );
    }
}

public class UIReacao : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown dropReagente1;
    public TMP_Dropdown dropReagente2;
    public TMP_Dropdown dropReacao;
    public Button btnReagir;

    // ─────────────────────────────────────────────────────────────────────────
    // Banco reestruturado: cada substância agora carrega parteA, parteB e cargas.
    //
    // Convenção de cargas iônicas usada aqui:
    //   Substâncias simples  → cargaA = +1 ou +2 (valor nominal), cargaB = 0
    //                          (cargaB será espelhada de cargaA em ToEspecieQuimica)
    //   Compostos iônicos    → cargaA = carga do cátion, cargaB = carga do ânion
    //
    // Ajuste os valores de carga conforme a implementação real de EspecieQuimica
    // e GeradorDeProduto no seu projeto — os valores abaixo são os padrão da
    // química inorgânica e devem coincidir com o que GetCargaA/GetCargaB retornam.
    // ─────────────────────────────────────────────────────────────────────────



    //+++++-----Pode trocar por Solução: Transforme isso em um ScriptableObject. 
    //+++++-----Crie um script SubstanciaSO : ScriptableObject que contenha as listas de ParteA, ParteB e Cargas.
    //+++++-----Assim, você pode criar as moléculas diretamente pelo Inspector do Unity como "Assets", facilitando a expansão do jogo sem mexer no código.



    private readonly Dictionary<string, DadosSubstancia> banco = new()
    {
        // ── Substâncias simples ──────────────────────────────────────────────
        { "H2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}},  cargaA: +1) },
        { "O2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.O, 2}},  cargaA: -2) },
        { "N2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.N, 2}},  cargaA: -3) },
        { "F2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.F, 2}},  cargaA: -1) },
        { "Cl2", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Cl,2}},  cargaA: -1) },
        { "Br2", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Br,2}},  cargaA: -1) },
        { "I2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.I, 2}},  cargaA: -1) },
        { "C",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.C, 1}},  cargaA: +4) },
        { "S",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.S, 1}},  cargaA: -2) },
        { "P",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.P, 1}},  cargaA: -3) },
        { "Na",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,1}},  cargaA: +1) },
        { "K",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.K, 1}},  cargaA: +1) },
        { "Ca",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}},  cargaA: +2) },
        { "Mg",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Mg,1}},  cargaA: +2) },
        { "Al",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Al,1}},  cargaA: +3) },
        { "Fe",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Fe,1}},  cargaA: +3) },
        { "Cu",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Cu,1}},  cargaA: +2) },
        { "Zn",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Zn,1}},  cargaA: +2) },

        // ── Óxidos ───────────────────────────────────────────────────────────
        { "H2O",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "CO2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.C, 1}}, +4,
                                       new Dictionary<int,int>{{Atomos.O, 2}}, -2) },
        { "CO",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.C, 1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "SO2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.S, 1}}, +4,
                                       new Dictionary<int,int>{{Atomos.O, 2}}, -2) },
        { "SO3",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.S, 1}}, +6,
                                       new Dictionary<int,int>{{Atomos.O, 3}}, -2) },
        { "NO",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.N, 1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "NO2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.N, 1}}, +4,
                                       new Dictionary<int,int>{{Atomos.O, 2}}, -2) },
        { "N2O",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.N, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "Fe2O3", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Fe,2}}, +3,
                                       new Dictionary<int,int>{{Atomos.O, 3}}, -2) },
        { "Fe3O4", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Fe,3}}, +8,
                                       new Dictionary<int,int>{{Atomos.O, 4}}, -2) },
        { "CaO",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "MgO",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Mg,1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "Al2O3", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Al,2}}, +3,
                                       new Dictionary<int,int>{{Atomos.O, 3}}, -2) },
        { "Na2O",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,2}}, +1,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "K2O",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.K, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "ZnO",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Zn,1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "CuO",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Cu,1}}, +2,
                                       new Dictionary<int,int>{{Atomos.O, 1}}, -2) },
        { "P2O5",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.P, 2}}, +5,
                                       new Dictionary<int,int>{{Atomos.O, 5}}, -2) },

        // ── Ácidos ───────────────────────────────────────────────────────────
        { "HCl",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.Cl,1}}, -1) },
        { "HF",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.F, 1}}, -1) },
        { "HBr",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.Br,1}}, -1) },
        { "HI",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.I, 1}}, -1) },
        { "H2S",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.S, 1}}, -2) },
        { "H2SO4", new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.S,1},{Atomos.O,4}}, -2) },
        { "H2SO3", new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.S,1},{Atomos.O,3}}, -2) },
        { "HNO3",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.N,1},{Atomos.O,3}}, -1) },
        { "HNO2",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 1}}, +1,
                                       new Dictionary<int,int>{{Atomos.N,1},{Atomos.O,2}}, -1) },
        { "H3PO4", new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 3}}, +1,
                                       new Dictionary<int,int>{{Atomos.P,1},{Atomos.O,4}}, -3) },
        { "H2CO3", new DadosSubstancia(new Dictionary<int,int>{{Atomos.H, 2}}, +1,
                                       new Dictionary<int,int>{{Atomos.C,1},{Atomos.O,3}}, -2) },

        // ── Bases ────────────────────────────────────────────────────────────
        { "NaOH",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,1}}, +1,
                                         new Dictionary<int,int>{{Atomos.O,1},{Atomos.H,1}}, -1) },
        { "KOH",     new DadosSubstancia(new Dictionary<int,int>{{Atomos.K, 1}}, +1,
                                         new Dictionary<int,int>{{Atomos.O,1},{Atomos.H,1}}, -1) },
        { "Ca(OH)2", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.O,2},{Atomos.H,2}}, -1) },
        { "Mg(OH)2", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Mg,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.O,2},{Atomos.H,2}}, -1) },
        { "Al(OH)3", new DadosSubstancia(new Dictionary<int,int>{{Atomos.Al,1}}, +3,
                                         new Dictionary<int,int>{{Atomos.O,3},{Atomos.H,3}}, -1) },
        { "NH3",     new DadosSubstancia(new Dictionary<int,int>{{Atomos.N, 1}}, -3,
                                         new Dictionary<int,int>{{Atomos.H, 3}}, +1) },

        // ── Sais ─────────────────────────────────────────────────────────────
        { "NaCl",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,1}}, +1,
                                         new Dictionary<int,int>{{Atomos.Cl,1}}, -1) },
        { "KCl",     new DadosSubstancia(new Dictionary<int,int>{{Atomos.K, 1}}, +1,
                                         new Dictionary<int,int>{{Atomos.Cl,1}}, -1) },
        { "CaCl2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.Cl,2}}, -1) },
        { "MgCl2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Mg,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.Cl,2}}, -1) },
        { "AlCl3",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Al,1}}, +3,
                                         new Dictionary<int,int>{{Atomos.Cl,3}}, -1) },
        { "FeCl2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Fe,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.Cl,2}}, -1) },
        { "FeCl3",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Fe,1}}, +3,
                                         new Dictionary<int,int>{{Atomos.Cl,3}}, -1) },
        { "CuCl2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Cu,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.Cl,2}}, -1) },
        { "ZnCl2",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Zn,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.Cl,2}}, -1) },
        { "Na2SO4",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,2}}, +1,
                                         new Dictionary<int,int>{{Atomos.S,1},{Atomos.O,4}}, -2) },
        { "CaSO4",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.S,1},{Atomos.O,4}}, -2) },
        { "MgSO4",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Mg,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.S,1},{Atomos.O,4}}, -2) },
        { "Na2CO3",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,2}}, +1,
                                         new Dictionary<int,int>{{Atomos.C,1},{Atomos.O,3}}, -2) },
        { "CaCO3",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Ca,1}}, +2,
                                         new Dictionary<int,int>{{Atomos.C,1},{Atomos.O,3}}, -2) },
        { "NaHCO3",  new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,1}}, +1,
                                         new Dictionary<int,int>{{Atomos.H,1},{Atomos.C,1},{Atomos.O,3}}, -1) },
        { "NH4Cl",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.N,1},{Atomos.H,4}}, +1,
                                         new Dictionary<int,int>{{Atomos.Cl,1}}, -1) },
        { "KNO3",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.K, 1}}, +1,
                                         new Dictionary<int,int>{{Atomos.N,1},{Atomos.O,3}}, -1) },
        { "NaNO3",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.Na,1}}, +1,
                                         new Dictionary<int,int>{{Atomos.N,1},{Atomos.O,3}}, -1) },

        // ── Compostos orgânicos ───────────────────────────────────────────────
        { "CH4",       new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,1}}, +4,
                                           new Dictionary<int,int>{{Atomos.H,4}}, -1) },
        { "C2H6",      new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,2}}, +3,
                                           new Dictionary<int,int>{{Atomos.H,6}}, -1) },
        { "C3H8",      new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,3}}, +8,
                                           new Dictionary<int,int>{{Atomos.H,8}}, -1) },
        { "C4H10",     new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,4}}, +10,
                                           new Dictionary<int,int>{{Atomos.H,10}}, -1) },
        { "C2H4",      new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,2}}, +4,
                                           new Dictionary<int,int>{{Atomos.H,4}}, -1) },
        { "C2H2",      new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,2}}, +2,
                                           new Dictionary<int,int>{{Atomos.H,2}}, -1) },
        { "C6H6",      new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,6}}, +6,
                                           new Dictionary<int,int>{{Atomos.H,6}}, -1) },
        { "CH3OH",     new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,1},{Atomos.H,4}}, +2,
                                           new Dictionary<int,int>{{Atomos.O,1}}, -2) },
        { "C2H5OH",    new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,2},{Atomos.H,6}}, +2,
                                           new Dictionary<int,int>{{Atomos.O,1}}, -2) },
        { "C6H12O6",   new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,6},{Atomos.H,12}}, +12,
                                           new Dictionary<int,int>{{Atomos.O,6}}, -2) },
        { "C12H22O11", new DadosSubstancia(new Dictionary<int,int>{{Atomos.C,12},{Atomos.H,22}}, +22,
                                           new Dictionary<int,int>{{Atomos.O,11}}, -2) },
    };

    private readonly Dictionary<string, string> tiposReacao = new()
    {
        { "Combustão",    "Combustao"    },
        { "Decomposição", "Decomposicao" },
        { "Síntese",      "Sintese"      },
        { "Simples Troca","Simples Troca"},
        { "Dupla Troca",  "Dupla Troca"  }
    };

    private List<string> opcoes;
    private List<string> opcoesReacao;

    void Start()
    {
        opcoes       = new List<string>(banco.Keys);
        opcoesReacao = new List<string>(tiposReacao.Keys);

        dropReagente1.ClearOptions();
        dropReagente2.ClearOptions();
        dropReacao.ClearOptions();

        dropReagente1.AddOptions(opcoes);
        dropReagente2.AddOptions(opcoes);
        dropReacao.AddOptions(opcoesReacao);

        btnReagir.onClick.AddListener(Reagir);
    }

    public void Reagir()
    {
        string nomeR1     = opcoes[dropReagente1.value];
        string nomeR2     = opcoes[dropReagente2.value];
        string nomeReacao = opcoesReacao[dropReacao.value];
        string tipoChave  = tiposReacao[nomeReacao];

    
        if (nomeR1 == nomeR2)
        {
            Debug.LogWarning("[UIReacao] Os dois reagentes são iguais.");
            return;
        }

        Debug.Log($"[UIReacao] Reagindo: {nomeR1} + {nomeR2} | Tipo: {nomeReacao}");

        try
        {
            DadosSubstancia dadosR1 = banco[nomeR1];
            DadosSubstancia dadosR2 = banco[nomeR2];

            
            ValidarTipo(tipoChave, nomeR1, nomeR2, dadosR1, dadosR2);

            
            EspecieQuimica especieA = dadosR1.ToEspecieQuimica();
            EspecieQuimica especieB = dadosR2.ToEspecieQuimica();

            
            List<Dictionary<int, int>> produtos =
                GeradorDeProduto.GerarProduto(tipoChave, especieA, especieB);

            if (produtos == null || produtos.Count == 0)
            {
                Debug.LogWarning("[UIReacao] Nenhum produto gerado para essa combinação.");
                return;
            }

            var eq = new EquacaoQuimica();
            eq.Reagentes.Add(dadosR1.ToFormula());
            eq.Reagentes.Add(dadosR2.ToFormula());
            foreach (var p in produtos)
                eq.Produtos.Add(p);

            var coef = eq.Balancear();

            
            if (coef == null || coef.Exists(c => c == 0))
            {
                Debug.LogWarning("[UIReacao] Reação não pôde ser balanceada (coeficiente zero ou nulo).");
                return;
            }

            
            string parteReagentes = $"{coef[0]} {nomeR1} + {coef[1]} {nomeR2}";
            string parteProdutos = "";
            foreach (var dic in produtos)
            {
                parteProdutos += $"{coef[2 + produtos.IndexOf(dic)]} ";
                foreach(var par in dic)
                {
                    
                    string simbolo = Simbolos.Elemento[par.Key];
                    int quantidade = par.Value;
                    if (quantidade <= 1)
                    {
                        parteProdutos += $"{simbolo}";
                    }
                    else
                    {
                        parteProdutos += $"{simbolo}{quantidade}";
                    }
                    
                }
                if (produtos.IndexOf(dic) < produtos.Count - 1)
                    parteProdutos += " + ";
            }
            Debug.Log($"[UIReacao] Equação balanceada: {parteReagentes}  →  {parteProdutos}");

            // Entrega os dados já balanceados para a cena de visualização 3D e abre ela.
            ReacaoTransferData.Reagentes = eq.Reagentes;
            ReacaoTransferData.Produtos = produtos;
            SceneManager.LoadScene(ReacaoTransferData.NomeCena);
        }
        catch (InvalidOperationException ex)
        {
            Debug.LogWarning($"[UIReacao] Reação inválida: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void ValidarTipo(string tipoChave, string nomeR1, string nomeR2,
                             DadosSubstancia d1, DadosSubstancia d2)
    {
        
        bool r1Simples  = d1.ParteB == null || d1.ParteB.Count == 0;
        bool r2Simples  = d2.ParteB == null || d2.ParteB.Count == 0;
        bool r1Composto = !r1Simples;
        bool r2Composto = !r2Simples;

        switch (tipoChave)
        {
            case "Combustao":
                bool r1Combustivel = d1.ParteA.ContainsKey(Atomos.C) || d1.ParteA.ContainsKey(Atomos.H);
                bool r2Combustivel = d2.ParteA.ContainsKey(Atomos.C) || d2.ParteA.ContainsKey(Atomos.H);

        
                bool r1EhO2 = r1Simples && d1.ParteA.ContainsKey(Atomos.O);
                bool r2EhO2 = r2Simples && d2.ParteA.ContainsKey(Atomos.O);

                if (!((r1Combustivel && r2EhO2) || (r2Combustivel && r1EhO2)))
                    throw new InvalidOperationException(
                        "Combustão requer um combustível (com C ou H) e O2 como reagentes.");
                break;

            case "Decomposicao":
                
                if (!r1Composto && !r2Composto)
                    throw new InvalidOperationException(
                        "Decomposição requer pelo menos um reagente composto.");
                break;

            case "Sintese":
                
                if (!r1Simples || !r2Simples)
                    throw new InvalidOperationException(
                        "Síntese requer que ambos os reagentes sejam substâncias simples.");
                break;

            case "Simples Troca":
                
                if (!((r1Simples && r2Composto) || (r2Simples && r1Composto)))
                    throw new InvalidOperationException(
                        "Simples Troca requer um elemento puro e um composto.");
                break;

            case "Dupla Troca":
                
                if (!r1Composto || !r2Composto)
                    throw new InvalidOperationException(
                        "Dupla Troca requer que ambos os reagentes sejam compostos.");
                break;

            default:
                throw new InvalidOperationException($"Tipo de reação não reconhecido: '{tipoChave}'.");
        }
    }
}

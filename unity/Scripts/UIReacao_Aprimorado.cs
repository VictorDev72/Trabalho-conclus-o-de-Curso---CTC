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
    private readonly List<string> historico = new();

    [Header("Feedback Visual")]
    [Tooltip("Texto principal para mensagens, estado e erros.")]
    public TMP_Text txtStatus;
    [Tooltip("Pré-visualização da equação antes de abrir a cena 3D.")]
    public TMP_Text txtEquacao;
    [Tooltip("Lista visual do histórico de reações executadas.")]
    public TMP_Text txtHistorico;
    [Tooltip("Indicador opcional do número de reações.")]
    public TMP_Text txtContadorReacoes;

    [Header("Controles Extras")]
    public Button btnInverter;
    public Button btnLimpar;
    public Button btnAleatorio;
    public Button btnHistoricoLimpar;

    [Header("Configuração da UI")]
    [SerializeField] private bool atualizarPreviewAutomaticamente = true;
    [SerializeField] private bool bloquearReagentesIguais = true;
    [SerializeField] private bool salvarHistorico = true;
    [SerializeField] private int limiteHistorico = 8;

    private int totalReacoes;
    private bool inicializando;

    private void Start()
    {
        inicializando = true;

        opcoes = new List<string>(banco.Keys);
        opcoesReacao = new List<string>(tiposReacao.Keys);

        ConfigurarDropdown(dropReagente1, opcoes);
        ConfigurarDropdown(dropReagente2, opcoes);
        ConfigurarDropdown(dropReacao, opcoesReacao);

        btnReagir?.onClick.AddListener(Reagir);
        btnInverter?.onClick.AddListener(InverterReagentes);
        btnLimpar?.onClick.AddListener(LimparSelecao);
        btnAleatorio?.onClick.AddListener(ReacaoAleatoria);
        btnHistoricoLimpar?.onClick.AddListener(LimparHistorico);

        dropReagente1?.onValueChanged.AddListener(_ => AtualizarInterface());
        dropReagente2?.onValueChanged.AddListener(_ => AtualizarInterface());
        dropReacao?.onValueChanged.AddListener(_ => AtualizarInterface());

        inicializando = false;
        LimparSelecao(false);
        AtualizarInterface();
    }

    private void OnDestroy()
    {
        btnReagir?.onClick.RemoveListener(Reagir);
        btnInverter?.onClick.RemoveListener(InverterReagentes);
        btnLimpar?.onClick.RemoveListener(LimparSelecao);
        btnAleatorio?.onClick.RemoveListener(ReacaoAleatoria);
        btnHistoricoLimpar?.onClick.RemoveListener(LimparHistorico);
    }

    private void ConfigurarDropdown(TMP_Dropdown dropdown, List<string> options)
    {
        if (dropdown == null) return;
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();
    }

    public void Reagir()
    {
        if (!TryObterSelecao(out string nomeR1, out string nomeR2, out string nomeReacao, out string tipoChave))
            return;

        if (bloquearReagentesIguais && nomeR1 == nomeR2)
        {
            MostrarStatus("Escolha dois reagentes diferentes.", true);
            return;
        }

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
                MostrarStatus("Essa combinação não possui produto definido no banco.", true);
                return;
            }

            var eq = new EquacaoQuimica();
            eq.Reagentes.Add(dadosR1.ToFormula());
            eq.Reagentes.Add(dadosR2.ToFormula());
            foreach (var produto in produtos)
                eq.Produtos.Add(produto);

            var coef = eq.Balancear();
            if (coef == null || coef.Count < 2 + produtos.Count || coef.Exists(c => c <= 0))
            {
                MostrarStatus("Não foi possível balancear essa reação.", true);
                return;
            }

            string equacao = MontarEquacao(nomeR1, nomeR2, produtos, coef);
            AtualizarEquacao(equacao);
            AdicionarAoHistorico(equacao, nomeReacao);
            totalReacoes++;
            AtualizarContador();

            MostrarStatus("Reação válida e balanceada. Abrindo visualização 3D...", false);
            Debug.Log($"[UIReacao] Equação balanceada: {equacao}");

            ReacaoTransferData.Reagentes = eq.Reagentes;
            ReacaoTransferData.Produtos = produtos;
            SceneManager.LoadScene(ReacaoTransferData.NomeCena);
        }
        catch (InvalidOperationException ex)
        {
            MostrarStatus(ex.Message, true);
            Debug.LogWarning($"[UIReacao] {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            MostrarStatus("Uma substância necessária não foi encontrada no banco.", true);
            Debug.LogException(ex);
        }
        catch (Exception ex)
        {
            MostrarStatus("Ocorreu um erro inesperado ao processar a reação.", true);
            Debug.LogException(ex);
        }
    }

    private bool TryObterSelecao(out string nomeR1, out string nomeR2, out string nomeReacao, out string tipoChave)
    {
        nomeR1 = nomeR2 = nomeReacao = tipoChave = string.Empty;

        if (dropReagente1 == null || dropReagente2 == null || dropReacao == null)
        {
            MostrarStatus("Configure os três dropdowns da reação no Inspector.", true);
            return false;
        }

        if (opcoes == null || opcoes.Count == 0 || opcoesReacao == null || opcoesReacao.Count == 0)
        {
            MostrarStatus("O banco de substâncias ou os tipos de reação estão vazios.", true);
            return false;
        }

        if (dropReagente1.value < 0 || dropReagente1.value >= opcoes.Count ||
            dropReagente2.value < 0 || dropReagente2.value >= opcoes.Count ||
            dropReacao.value < 0 || dropReacao.value >= opcoesReacao.Count)
        {
            MostrarStatus("Seleção inválida. Escolha os reagentes e o tipo de reação novamente.", true);
            return false;
        }

        nomeR1 = opcoes[dropReagente1.value];
        nomeR2 = opcoes[dropReagente2.value];
        nomeReacao = opcoesReacao[dropReacao.value];
        tipoChave = tiposReacao[nomeReacao];
        return true;
    }

    private string MontarEquacao(string nomeR1, string nomeR2,
                                  List<Dictionary<int, int>> produtos, List<int> coef)
    {
        string reagentes = FormatSlot(nomeR1, coef[0]) + " + " + FormatSlot(nomeR2, coef[1]);
        List<string> listaProdutos = new();

        for (int i = 0; i < produtos.Count; i++)
            listaProdutos.Add(FormatFormula(produtos[i], coef[2 + i]));

        return $"{reagentes}  →  {string.Join(" + ", listaProdutos)}";
    }

    private string FormatSlot(string nome, int coeficiente)
    {
        return coeficiente == 1 ? nome : $"{coeficiente} {nome}";
    }

    private string FormatFormula(Dictionary<int, int> formula, int coeficiente)
    {
        string formulaTexto = "";

        foreach (var par in formula)
        {
            if (!Simbolos.Elemento.TryGetValue(par.Key, out string simbolo))
                simbolo = $"E{par.Key}";

            formulaTexto += simbolo;
            if (par.Value > 1)
                formulaTexto += par.Value.ToString();
        }

        return FormatSlot(formulaTexto, coeficiente);
    }

    public void InverterReagentes()
    {
        if (dropReagente1 == null || dropReagente2 == null) return;

        int valor1 = dropReagente1.value;
        int valor2 = dropReagente2.value;

        dropReagente1.SetValueWithoutNotify(valor2);
        dropReagente2.SetValueWithoutNotify(valor1);
        dropReagente1.RefreshShownValue();
        dropReagente2.RefreshShownValue();
        AtualizarInterface();
        MostrarStatus("Reagentes invertidos.", false);
    }

    public void LimparSelecao()
    {
        LimparSelecao(true);
    }

    private void LimparSelecao(bool mostrarMensagem)
    {
        if (dropReagente1 != null)
            dropReagente1.SetValueWithoutNotify(0);
        if (dropReagente2 != null)
            dropReagente2.SetValueWithoutNotify(Mathf.Min(1, Math.Max(0, opcoes?.Count - 1 ?? 0)));
        if (dropReacao != null)
            dropReacao.SetValueWithoutNotify(0);

        dropReagente1?.RefreshShownValue();
        dropReagente2?.RefreshShownValue();
        dropReacao?.RefreshShownValue();

        AtualizarEquacao("Selecione os reagentes para visualizar a equação.");
        if (mostrarMensagem && !inicializando)
            MostrarStatus("Seleção limpa.", false);
    }

    public void ReacaoAleatoria()
    {
        if (opcoes == null || opcoes.Count < 2 || opcoesReacao == null || opcoesReacao.Count == 0)
            return;

        const int maxTentativas = 100;
        for (int tentativa = 0; tentativa < maxTentativas; tentativa++)
        {
            int r1 = UnityEngine.Random.Range(0, opcoes.Count);
            int r2 = UnityEngine.Random.Range(0, opcoes.Count);
            int tipo = UnityEngine.Random.Range(0, opcoesReacao.Count);

            if (r1 == r2) continue;

            dropReagente1.SetValueWithoutNotify(r1);
            dropReagente2.SetValueWithoutNotify(r2);
            dropReacao.SetValueWithoutNotify(tipo);

            dropReagente1.RefreshShownValue();
            dropReagente2.RefreshShownValue();
            dropReacao.RefreshShownValue();
            AtualizarInterface();
            MostrarStatus("Combinação aleatória gerada. Verifique a prévia antes de reagir.", false);
            return;
        }
    }

    private void AtualizarInterface()
    {
        if (inicializando) return;

        if (!TryObterSelecaoSemMensagem(out string nomeR1, out string nomeR2, out string nomeReacao, out string tipoChave))
            return;

        bool iguais = nomeR1 == nomeR2;
        bool podeValidar = banco.TryGetValue(nomeR1, out DadosSubstancia d1) &&
                           banco.TryGetValue(nomeR2, out DadosSubstancia d2);

        if (!podeValidar)
        {
            if (btnReagir != null) btnReagir.interactable = false;
            AtualizarEquacao("Não foi possível carregar os reagentes.");
            return;
        }

        try
        {
            ValidarTipo(tipoChave, nomeR1, nomeR2, d1, d2);
            if (btnReagir != null) btnReagir.interactable = !iguais || !bloquearReagentesIguais;

            if (iguais && bloquearReagentesIguais)
            {
                MostrarStatus("Escolha reagentes diferentes.", true);
            }
            else
            {
                AtualizarEquacao($"{nomeR1} + {nomeR2}  →  ?");
                MostrarStatus($"Combinação válida para {nomeReacao}.", false);
            }
        }
        catch (InvalidOperationException ex)
        {
            if (btnReagir != null) btnReagir.interactable = false;
            AtualizarEquacao($"{nomeR1} + {nomeR2}  →  ?");
            MostrarStatus(ex.Message, true);
        }
    }

    private bool TryObterSelecaoSemMensagem(out string nomeR1, out string nomeR2,
                                             out string nomeReacao, out string tipoChave)
    {
        nomeR1 = nomeR2 = nomeReacao = tipoChave = string.Empty;
        if (dropReagente1 == null || dropReagente2 == null || dropReacao == null ||
            opcoes == null || opcoesReacao == null || opcoes.Count == 0 || opcoesReacao.Count == 0)
            return false;

        if (dropReagente1.value < 0 || dropReagente1.value >= opcoes.Count ||
            dropReagente2.value < 0 || dropReagente2.value >= opcoes.Count ||
            dropReacao.value < 0 || dropReacao.value >= opcoesReacao.Count)
            return false;

        nomeR1 = opcoes[dropReagente1.value];
        nomeR2 = opcoes[dropReagente2.value];
        nomeReacao = opcoesReacao[dropReacao.value];
        tipoChave = tiposReacao[nomeReacao];
        return true;
    }

    private void MostrarStatus(string mensagem, bool erro)
    {
        if (txtStatus != null)
            txtStatus.text = (erro ? "⚠ " : "✓ ") + mensagem;
    }

    private void AtualizarEquacao(string equacao)
    {
        if (txtEquacao != null && atualizarPreviewAutomaticamente)
            txtEquacao.text = equacao;
    }

    private void AdicionarAoHistorico(string equacao, string tipo)
    {
        if (!salvarHistorico || txtHistorico == null) return;

        historico.Insert(0, $"• {tipo}: {equacao}");
        if (historico.Count > Mathf.Max(1, limiteHistorico))
            historico.RemoveAt(historico.Count - 1);

        txtHistorico.text = string.Join("\n", historico);
    }

    public void LimparHistorico()
    {
        historico.Clear();
        if (txtHistorico != null)
            txtHistorico.text = "Nenhuma reação realizada ainda.";
    }

    private void AtualizarContador()
    {
        if (txtContadorReacoes != null)
            txtContadorReacoes.text = $"Reações realizadas: {totalReacoes}";
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
                    throw new InvalidOperationException("Combustão requer um combustível (com C ou H) e O2 como reagentes.");
                break;

            case "Decomposicao":
                if (!r1Composto && !r2Composto)
                    throw new InvalidOperationException("Decomposição requer pelo menos um reagente composto.");
                break;

            case "Sintese":
                if (!r1Simples || !r2Simples)
                    throw new InvalidOperationException("Síntese requer que ambos os reagentes sejam substâncias simples.");
                break;

            case "Simples Troca":
                if (!((r1Simples && r2Composto) || (r2Simples && r1Composto)))
                    throw new InvalidOperationException("Simples Troca requer um elemento puro e um composto.");
                break;

            case "Dupla Troca":
                if (!r1Composto || !r2Composto)
                    throw new InvalidOperationException("Dupla Troca requer que ambos os reagentes sejam compostos.");
                break;

            default:
                throw new InvalidOperationException($"Tipo de reação não reconhecido: '{tipoChave}'.");
        }
    }
}

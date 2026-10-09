using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// Tela de escolha da reação.
//
// O filtro dos dropdowns usa a MESMA função que executa a reação
// (GeradorDeProduto.Reagir): o Reagente 1 só lista substâncias que têm
// alguma reação do tipo escolhido, e o Reagente 2 só lista parceiros com os
// quais a reação realmente ocorre. Assim o filtro nunca aprova uma combinação
// que falha ao clicar em REAGIR.
//
// As substâncias ficam em BancoSubstancias.
public class UIReacao : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown dropReagente1;
    public TMP_Dropdown dropReagente2;
    public TMP_Dropdown dropReacao;
    public Button btnReagir;

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
    [SerializeField] private bool salvarHistorico = true;
    [SerializeField] private int limiteHistorico = 8;

    private static readonly (string nome, TipoReacao tipo)[] tiposReacao =
    {
        ("Síntese",       TipoReacao.Sintese),
        ("Decomposição",  TipoReacao.Decomposicao),
        ("Simples Troca", TipoReacao.SimplesTroca),
        ("Dupla Troca",   TipoReacao.DuplaTroca),
        ("Combustão",     TipoReacao.Combustao),
    };

    private List<DadosSubstancia> opcoesR1 = new List<DadosSubstancia>();
    private List<DadosSubstancia> opcoesR2 = new List<DadosSubstancia>();
    private ResultadoReacao resultadoAtual;

    // Resultados do filtro, calculados uma vez por tipo / reagente.
    private readonly Dictionary<TipoReacao, List<DadosSubstancia>> cacheReagentes =
        new Dictionary<TipoReacao, List<DadosSubstancia>>();
    private readonly Dictionary<(TipoReacao, DadosSubstancia), List<DadosSubstancia>> cacheParceiros =
        new Dictionary<(TipoReacao, DadosSubstancia), List<DadosSubstancia>>();

    private readonly List<string> historico = new List<string>();
    private int totalReacoes;

    private TipoReacao TipoSelecionado =>
        tiposReacao[Mathf.Clamp(dropReacao != null ? dropReacao.value : 0, 0, tiposReacao.Length - 1)].tipo;

    private string NomeTipoSelecionado =>
        tiposReacao[Mathf.Clamp(dropReacao != null ? dropReacao.value : 0, 0, tiposReacao.Length - 1)].nome;

    private DadosSubstancia Reagente1 => Selecionado(dropReagente1, opcoesR1);
    private DadosSubstancia Reagente2 => Selecionado(dropReagente2, opcoesR2);

    private void Start()
    {
        if (dropReagente1 == null || dropReagente2 == null || dropReacao == null)
        {
            MostrarStatus("Configure os três dropdowns da reação no Inspector.", true);
            return;
        }

        dropReacao.ClearOptions();
        dropReacao.AddOptions(tiposReacao.Select(t => t.nome).ToList());
        dropReacao.SetValueWithoutNotify(0);
        dropReacao.RefreshShownValue();

        btnReagir?.onClick.AddListener(Reagir);
        btnInverter?.onClick.AddListener(InverterReagentes);
        btnLimpar?.onClick.AddListener(LimparSelecao);
        btnAleatorio?.onClick.AddListener(ReacaoAleatoria);
        btnHistoricoLimpar?.onClick.AddListener(LimparHistorico);

        dropReacao.onValueChanged.AddListener(_ => AoMudarTipo());
        dropReagente1.onValueChanged.AddListener(_ => AoMudarReagente1(null));
        dropReagente2.onValueChanged.AddListener(_ => AtualizarPreview());

        AoMudarTipo();
        AtualizarContador();
    }

    private void OnDestroy()
    {
        btnReagir?.onClick.RemoveListener(Reagir);
        btnInverter?.onClick.RemoveListener(InverterReagentes);
        btnLimpar?.onClick.RemoveListener(LimparSelecao);
        btnAleatorio?.onClick.RemoveListener(ReacaoAleatoria);
        btnHistoricoLimpar?.onClick.RemoveListener(LimparHistorico);
    }

    // ── Filtro ───────────────────────────────────────────────────────────────

    // Substâncias que têm pelo menos uma reação do tipo.
    private List<DadosSubstancia> ReagentesValidos(TipoReacao tipo)
    {
        if (cacheReagentes.TryGetValue(tipo, out var lista))
            return lista;

        lista = tipo == TipoReacao.Decomposicao
            ? BancoSubstancias.Todas.Where(s => GeradorDeProduto.Reagir(tipo, s, null).Valida).ToList()
            : BancoSubstancias.Todas.Where(s => Parceiros(tipo, s).Count > 0).ToList();

        cacheReagentes[tipo] = lista;
        return lista;
    }

    // Substâncias com as quais "reagente" de fato reage nesse tipo.
    private List<DadosSubstancia> Parceiros(TipoReacao tipo, DadosSubstancia reagente)
    {
        if (reagente == null || tipo == TipoReacao.Decomposicao)
            return new List<DadosSubstancia>();

        if (cacheParceiros.TryGetValue((tipo, reagente), out var lista))
            return lista;

        lista = BancoSubstancias.Todas
            .Where(outro => GeradorDeProduto.Reagir(tipo, reagente, outro).Valida)
            .ToList();

        cacheParceiros[(tipo, reagente)] = lista;
        return lista;
    }

    // ── Atualização dos dropdowns ────────────────────────────────────────────

    private void AoMudarTipo(DadosSubstancia preferido1 = null, DadosSubstancia preferido2 = null)
    {
        opcoesR1 = ReagentesValidos(TipoSelecionado);
        PreencherDropdown(dropReagente1, opcoesR1, preferido1 ?? Reagente1,
                          "Nenhuma substância disponível");
        AoMudarReagente1(preferido2);
    }

    private void AoMudarReagente1(DadosSubstancia preferido2)
    {
        if (TipoSelecionado == TipoReacao.Decomposicao)
        {
            opcoesR2 = new List<DadosSubstancia>();
            PreencherDropdown(dropReagente2, opcoesR2, null, "— (apenas um reagente)");
        }
        else
        {
            opcoesR2 = Parceiros(TipoSelecionado, Reagente1);
            PreencherDropdown(dropReagente2, opcoesR2, preferido2 ?? Reagente2,
                              "Sem combinação válida");
        }

        AtualizarPreview();
    }

    private static void PreencherDropdown(TMP_Dropdown dropdown, List<DadosSubstancia> opcoes,
                                          DadosSubstancia preferido, string textoVazio)
    {
        dropdown.ClearOptions();

        if (opcoes.Count == 0)
        {
            dropdown.AddOptions(new List<string> { textoVazio });
            dropdown.SetValueWithoutNotify(0);
            dropdown.interactable = false;
        }
        else
        {
            dropdown.AddOptions(opcoes.Select(s => s.Nome).ToList());
            dropdown.SetValueWithoutNotify(Mathf.Max(0, opcoes.IndexOf(preferido)));
            dropdown.interactable = true;
        }

        dropdown.RefreshShownValue();
    }

    private static DadosSubstancia Selecionado(TMP_Dropdown dropdown, List<DadosSubstancia> opcoes)
    {
        if (dropdown == null || opcoes == null || opcoes.Count == 0)
            return null;
        if (dropdown.value < 0 || dropdown.value >= opcoes.Count)
            return null;
        return opcoes[dropdown.value];
    }

    private void AtualizarPreview()
    {
        DadosSubstancia r1 = Reagente1;
        DadosSubstancia r2 = Reagente2;

        resultadoAtual = r1 == null
            ? ResultadoReacao.Falha($"Nenhuma substância do banco participa de {NomeTipoSelecionado}.")
            : GeradorDeProduto.Reagir(TipoSelecionado, r1, r2);

        if (btnReagir != null)
            btnReagir.interactable = resultadoAtual.Valida;

        if (resultadoAtual.Valida)
        {
            AtualizarEquacao(resultadoAtual.Equacao);
            MostrarStatus($"Reação de {NomeTipoSelecionado} válida e balanceada.", false);
        }
        else
        {
            AtualizarEquacao(r1 == null ? "" : r2 == null ? $"{r1.Nome}  →  ?" : $"{r1.Nome} + {r2.Nome}  →  ?");
            MostrarStatus(resultadoAtual.Motivo, true);
        }
    }

    // ── Ações ────────────────────────────────────────────────────────────────

    public void Reagir()
    {
        if (resultadoAtual == null || !resultadoAtual.Valida)
        {
            MostrarStatus(resultadoAtual?.Motivo ?? "Escolha uma reação válida.", true);
            return;
        }

        AdicionarAoHistorico(resultadoAtual.Equacao, NomeTipoSelecionado);
        totalReacoes++;
        AtualizarContador();

        MostrarStatus("Reação válida e balanceada. Abrindo visualização 3D...", false);
        Debug.Log($"[UIReacao] Equação balanceada: {resultadoAtual.Equacao}");

        ReacaoTransferData.Reagentes = resultadoAtual.Reagentes
            .Select(s => new Dictionary<int, int>(s.Formula)).ToList();
        ReacaoTransferData.Produtos = resultadoAtual.Produtos
            .Select(s => new Dictionary<int, int>(s.Formula)).ToList();
        ReacaoTransferData.Equacao = resultadoAtual.Equacao;

        SceneManager.LoadScene(ReacaoTransferData.NomeCena);
    }

    public void InverterReagentes()
    {
        DadosSubstancia r1 = Reagente1;
        DadosSubstancia r2 = Reagente2;

        if (r1 == null || r2 == null)
        {
            MostrarStatus("Não há dois reagentes para inverter.", true);
            return;
        }

        // As reações são simétricas: r2 também está na lista do Reagente 1
        // e r1 entre os parceiros de r2.
        AoMudarTipo(preferido1: r2, preferido2: r1);
        if (resultadoAtual != null && resultadoAtual.Valida)
            MostrarStatus("Reagentes invertidos.", false);
    }

    public void LimparSelecao()
    {
        if (dropReacao == null) return;

        dropReacao.SetValueWithoutNotify(0);
        dropReacao.RefreshShownValue();
        opcoesR1 = ReagentesValidos(TipoSelecionado);
        PreencherDropdown(dropReagente1, opcoesR1, null, "Nenhuma substância disponível");
        AoMudarReagente1(null);
    }

    public void ReacaoAleatoria()
    {
        if (dropReacao == null) return;

        var tiposDisponiveis = Enumerable.Range(0, tiposReacao.Length)
            .Where(i => ReagentesValidos(tiposReacao[i].tipo).Count > 0)
            .ToList();
        if (tiposDisponiveis.Count == 0) return;

        int indiceTipo = tiposDisponiveis[Random.Range(0, tiposDisponiveis.Count)];
        dropReacao.SetValueWithoutNotify(indiceTipo);
        dropReacao.RefreshShownValue();

        var reagentes = ReagentesValidos(TipoSelecionado);
        DadosSubstancia r1 = reagentes[Random.Range(0, reagentes.Count)];

        var parceiros = Parceiros(TipoSelecionado, r1);
        DadosSubstancia r2 = parceiros.Count > 0 ? parceiros[Random.Range(0, parceiros.Count)] : null;

        AoMudarTipo(preferido1: r1, preferido2: r2);
        if (resultadoAtual != null && resultadoAtual.Valida)
            MostrarStatus("Combinação aleatória gerada. Clique em REAGIR para visualizar.", false);
    }

    public void LimparHistorico()
    {
        historico.Clear();
        if (txtHistorico != null)
            txtHistorico.text = "Nenhuma reação realizada ainda.";
    }

    // ── Feedback ─────────────────────────────────────────────────────────────

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

    private void AtualizarContador()
    {
        if (txtContadorReacoes != null)
            txtContadorReacoes.text = $"Reações realizadas: {totalReacoes}";
    }
}

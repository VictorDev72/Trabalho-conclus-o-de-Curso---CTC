using System;
using System.Collections.Generic;
using System.Linq;
using BalanciadorQuimico;

public enum TipoReacao
{
    Combustao,
    Decomposicao,
    Sintese,
    SimplesTroca,
    DuplaTroca
}

public class ResultadoReacao
{
    public bool Valida;
    public string Motivo;      // por que a reação não ocorre (quando Valida == false)
    public List<DadosSubstancia> Reagentes = new List<DadosSubstancia>();
    public List<DadosSubstancia> Produtos = new List<DadosSubstancia>();
    public List<int> Coeficientes = new List<int>();   // reagentes e depois produtos
    public string Equacao = "";

    public static ResultadoReacao Falha(string motivo) =>
        new ResultadoReacao { Valida = false, Motivo = motivo };
}

// Gera os produtos de uma reação a partir dos íons dos reagentes.
//
// Regras gerais:
//  - Compostos iônicos são recombinados pelas cargas (MDC), nunca somando átomos.
//  - Todo produto precisa existir no catálogo (BancoSubstancias); caso contrário
//    a reação é recusada — nenhuma molécula inexistente chega à tela.
//  - A equação é balanceada e o resultado é conferido átomo por átomo.
//
// A mesma função Reagir é usada pela UI para filtrar os dropdowns e para executar
// a reação, então o filtro nunca aprova algo que depois falha.
public static class GeradorDeProduto
{
    // Óxido ácido → oxoácido correspondente (e vice-versa na decomposição).
    private static readonly Dictionary<string, string> OxoacidoDoOxido = new Dictionary<string, string>
    {
        { "CO2",  "H2CO3" },
        { "SO2",  "H2SO3" },
        { "SO3",  "H2SO4" },
        { "P2O5", "H3PO4" },
    };

    // Óxidos que reagem com O2 formando o óxido de NOX maior.
    private static readonly Dictionary<string, string> OxidacaoComO2 = new Dictionary<string, string>
    {
        { "CO",  "CO2" },
        { "SO2", "SO3" },
        { "NO",  "NO2" },
    };

    // Decomposições que não seguem um padrão geral.
    private static readonly Dictionary<string, string[]> DecomposicoesEspeciais = new Dictionary<string, string[]>
    {
        { "NH4NO3", new[] { "N2O", "H2O" } },
        { "HNO3",   new[] { "NO2", "O2", "H2O" } },
    };

    // ── Entrada principal ───────────────────────────────────────────────────

    public static ResultadoReacao Reagir(TipoReacao tipo, DadosSubstancia a, DadosSubstancia b)
    {
        if (a == null)
            return ResultadoReacao.Falha("Escolha o reagente.");

        var reagentes = new List<DadosSubstancia> { a };

        if (tipo != TipoReacao.Decomposicao)
        {
            if (b == null)
                return ResultadoReacao.Falha("Escolha o segundo reagente.");
            if (a == b)
                return ResultadoReacao.Falha("Escolha dois reagentes diferentes.");
            reagentes.Add(b);
        }

        string motivo;
        List<DadosSubstancia> produtos = tipo switch
        {
            TipoReacao.Combustao    => Combustao(a, b, out motivo),
            TipoReacao.Decomposicao => Decomposicao(a, out motivo),
            TipoReacao.Sintese      => Sintese(a, b, out motivo),
            TipoReacao.SimplesTroca => SimplesTroca(a, b, out motivo),
            TipoReacao.DuplaTroca   => DuplaTroca(a, b, out motivo),
            _ => Erro("Tipo de reação desconhecido.", out motivo)
        };

        if (produtos == null)
            return ResultadoReacao.Falha(motivo);

        if (produtos.Any(p => p == null))
            return ResultadoReacao.Falha("Um dos produtos não existe no catálogo de substâncias.");

        produtos = produtos.Distinct().ToList();

        if (produtos.Any(p => reagentes.Contains(p)))
            return ResultadoReacao.Falha("Os produtos seriam iguais aos reagentes.");

        return Balancear(reagentes, produtos);
    }

    // ── Balanceamento ───────────────────────────────────────────────────────

    private static ResultadoReacao Balancear(List<DadosSubstancia> reagentes, List<DadosSubstancia> produtos)
    {
        var eq = new EquacaoQuimica();
        foreach (var r in reagentes) eq.Reagentes.Add(new Dictionary<int, int>(r.Formula));
        foreach (var p in produtos) eq.Produtos.Add(new Dictionary<int, int>(p.Formula));

        List<int> coef;
        try
        {
            coef = eq.Balancear();
        }
        catch (Exception)
        {
            return ResultadoReacao.Falha("Não foi possível balancear essa reação.");
        }

        int total = reagentes.Count + produtos.Count;
        if (coef == null || coef.Count != total || coef.Any(c => c <= 0) ||
            !ConservaAtomos(reagentes, produtos, coef))
            return ResultadoReacao.Falha("Não foi possível balancear essa reação.");

        var resultado = new ResultadoReacao
        {
            Valida = true,
            Reagentes = reagentes,
            Produtos = produtos,
            Coeficientes = coef
        };
        resultado.Equacao = MontarEquacao(resultado);
        return resultado;
    }

    // Confere, átomo por átomo, que os coeficientes realmente balanceiam a equação.
    private static bool ConservaAtomos(List<DadosSubstancia> reagentes, List<DadosSubstancia> produtos, List<int> coef)
    {
        var saldo = new Dictionary<int, int>();
        for (int i = 0; i < reagentes.Count; i++)
            DadosSubstancia.Somar(saldo, reagentes[i].Formula, coef[i]);
        for (int i = 0; i < produtos.Count; i++)
            DadosSubstancia.Somar(saldo, produtos[i].Formula, -coef[reagentes.Count + i]);
        return saldo.Values.All(v => v == 0);
    }

    private static string MontarEquacao(ResultadoReacao r)
    {
        string Termo(DadosSubstancia s, int c) => c == 1 ? s.Nome : $"{c} {s.Nome}";

        var esquerda = r.Reagentes.Select((s, i) => Termo(s, r.Coeficientes[i]));
        var direita = r.Produtos.Select((s, i) => Termo(s, r.Coeficientes[r.Reagentes.Count + i]));
        return $"{string.Join(" + ", esquerda)}  →  {string.Join(" + ", direita)}";
    }

    // ── Combustão ───────────────────────────────────────────────────────────

    private static List<DadosSubstancia> Combustao(DadosSubstancia a, DadosSubstancia b, out string motivo)
    {
        DadosSubstancia o2 = BancoSubstancias.PorNome("O2");
        DadosSubstancia combustivel = a == o2 ? b : b == o2 ? a : null;

        if (combustivel == null)
            return Erro("A combustão precisa de O2 como um dos reagentes.", out motivo);
        if (!combustivel.Combustivel)
            return Erro($"{combustivel.Nome} não é um combustível.", out motivo);

        var produtos = new List<DadosSubstancia>();
        foreach (int z in combustivel.Formula.Keys)
        {
            if (z == Atomos.O) continue;

            string produto =
                z == Atomos.C ? "CO2" :
                z == Atomos.H ? "H2O" :
                z == Atomos.S ? "SO2" :
                z == Atomos.N ? "N2"  : null;

            if (produto == null)
                return Erro($"A combustão de {combustivel.Nome} não é suportada.", out motivo);

            produtos.Add(BancoSubstancias.PorNome(produto));
        }

        motivo = null;
        return produtos;
    }

    // ── Decomposição ────────────────────────────────────────────────────────

    private static List<DadosSubstancia> Decomposicao(DadosSubstancia s, out string motivo)
    {
        motivo = null;

        if (s.EhSimples)
            return Erro("Substâncias simples não se decompõem.", out motivo);

        if (DecomposicoesEspeciais.TryGetValue(s.Nome, out string[] especiais))
            return especiais.Select(BancoSubstancias.PorNome).ToList();

        // NH3 → N2 + H2 (e outros binários moleculares não orgânicos)
        if (s.Funcao == FuncaoQuimica.Molecular)
        {
            if (s.Formula.ContainsKey(Atomos.C))
                return Erro($"{s.Nome} não se decompõe por um processo simples.", out motivo);
            return EmElementos(s, out motivo);
        }

        string anion = DadosSubstancia.Chave(s.Anion);
        int metal = ValidadorQuimico.ElementoDoIon(s.Cation);

        // Sais de amônio: NH4X → NH3 + HX
        if (ValidadorQuimico.EhAmonio(s.Cation))
        {
            var acido = BancoSubstancias.Combinar(Ion(Atomos.H), +1, s.Anion, s.CargaAnion);
            return Lista(BancoSubstancias.PorNome("NH3"), acido);
        }

        // Bicarbonato: 2 NaHCO3 → Na2CO3 + H2O + CO2
        if (anion == DadosSubstancia.Chave(BancoSubstancias.HCO3))
        {
            var carbonato = BancoSubstancias.Combinar(s.Cation, s.CargaCation, BancoSubstancias.CO3, -2);
            return Lista(carbonato, BancoSubstancias.PorNome("H2O"), BancoSubstancias.PorNome("CO2"));
        }

        // Carbonato (não alcalino): CaCO3 → CaO + CO2
        if (anion == DadosSubstancia.Chave(BancoSubstancias.CO3) && metal != Atomos.Na && metal != Atomos.K)
        {
            var oxido = BancoSubstancias.Combinar(s.Cation, s.CargaCation, Ion(Atomos.O), -2);
            if (oxido != null)
                return Lista(oxido, BancoSubstancias.PorNome("CO2"));
        }

        // Hidróxido (não alcalino): Ca(OH)2 → CaO + H2O
        if (s.Funcao == FuncaoQuimica.Base && metal != Atomos.Na && metal != Atomos.K)
        {
            var oxido = BancoSubstancias.Combinar(s.Cation, s.CargaCation, Ion(Atomos.O), -2);
            if (oxido != null)
                return Lista(oxido, BancoSubstancias.PorNome("H2O"));
        }

        // Nitrato alcalino: 2 KNO3 → 2 KNO2 + O2
        if (anion == DadosSubstancia.Chave(BancoSubstancias.NO3) && (metal == Atomos.Na || metal == Atomos.K))
        {
            var nitrito = BancoSubstancias.Combinar(s.Cation, s.CargaCation, BancoSubstancias.NO2, -1);
            if (nitrito != null)
                return Lista(nitrito, BancoSubstancias.PorNome("O2"));
        }

        // Oxoácido: H2CO3 → CO2 + H2O
        if (s.Funcao == FuncaoQuimica.Acido)
        {
            string oxido = OxoacidoDoOxido.FirstOrDefault(p => p.Value == s.Nome).Key;
            if (oxido != null)
                return Lista(BancoSubstancias.PorNome(oxido), BancoSubstancias.PorNome("H2O"));
        }

        // Binários em seus elementos (eletrólise / termólise):
        // água, hidrácidos (HCl, H2S...) e haletos (NaCl, AgBr...).
        int elementoAnion = ValidadorQuimico.ElementoDoIon(s.Anion);
        bool binario = metal != 0 && elementoAnion != 0;
        bool permitido = ValidadorQuimico.EhAgua(s) ||
                         (binario && metal == Atomos.H) ||
                         (binario && ValidadorQuimico.EhHalogenio(elementoAnion));
        if (permitido)
            return EmElementos(s, out motivo);

        return Erro($"{s.Nome} não se decompõe por um processo simples.", out motivo);
    }

    private static List<DadosSubstancia> EmElementos(DadosSubstancia s, out string motivo)
    {
        var elementos = s.Formula.Keys.Select(BancoSubstancias.SimplesDoElemento).ToList();
        if (elementos.Count < 2 || elementos.Any(e => e == null))
            return Erro($"{s.Nome} não se decompõe por um processo simples.", out motivo);

        motivo = null;
        return elementos;
    }

    // ── Síntese ─────────────────────────────────────────────────────────────

    private static List<DadosSubstancia> Sintese(DadosSubstancia a, DadosSubstancia b, out string motivo)
    {
        motivo = null;

        // Duas substâncias simples: o elemento mais eletronegativo vira ânion.
        if (a.EhSimples && b.EhSimples)
        {
            if (a.Elemento == b.Elemento)
                return Erro("Os dois reagentes são do mesmo elemento.", out motivo);

            double enA = Elementos.Tabela[a.Elemento].Eletronegatividade;
            double enB = Elementos.Tabela[b.Elemento].Eletronegatividade;
            DadosSubstancia positivo = enA <= enB ? a : b;
            DadosSubstancia negativo = positivo == a ? b : a;

            int cargaNeg = negativo.CargasPossiveis.FirstOrDefault(c => c < 0);
            if (cargaNeg == 0)
                return Erro($"{negativo.Nome} não forma ânion com {positivo.Nome}.", out motivo);

            foreach (int cargaPos in positivo.CargasPossiveis.Where(c => c > 0))
            {
                var produto = BancoSubstancias.Combinar(Ion(positivo.Elemento), cargaPos, Ion(negativo.Elemento), cargaNeg);
                if (produto != null)
                    return Lista(produto);
            }
            return Erro($"{a.Nome} e {b.Nome} não formam um composto conhecido.", out motivo);
        }

        // Óxido + O2 → óxido de NOX maior (2 SO2 + O2 → 2 SO3)
        DadosSubstancia o2 = BancoSubstancias.PorNome("O2");
        if (a == o2 || b == o2)
        {
            DadosSubstancia outro = a == o2 ? b : a;
            if (OxidacaoComO2.TryGetValue(outro.Nome, out string maior))
                return Lista(BancoSubstancias.PorNome(maior));
            return Erro($"{outro.Nome} não reage com O2 por síntese.", out motivo);
        }

        // NH3 + ácido → sal de amônio
        DadosSubstancia nh3 = BancoSubstancias.PorNome("NH3");
        if (a == nh3 || b == nh3)
        {
            DadosSubstancia acido = a == nh3 ? b : a;
            if (acido.Funcao != FuncaoQuimica.Acido)
                return Erro("NH3 só forma sal de amônio com um ácido.", out motivo);
            var sal = BancoSubstancias.Combinar(BancoSubstancias.NH4, +1, acido.Anion, acido.CargaAnion);
            return sal != null ? Lista(sal) : Erro("O sal de amônio desse ácido não está no catálogo.", out motivo);
        }

        // Reações com água
        DadosSubstancia agua = BancoSubstancias.PorNome("H2O");
        if (a == agua || b == agua)
        {
            DadosSubstancia oxido = a == agua ? b : a;

            if (ValidadorQuimico.EhOxidoBasicoReativo(oxido))
            {
                var baseFormada = BancoSubstancias.Combinar(oxido.Cation, oxido.CargaCation, BancoSubstancias.OH, -1);
                return baseFormada != null ? Lista(baseFormada) : Erro("A base formada não está no catálogo.", out motivo);
            }

            if (OxoacidoDoOxido.TryGetValue(oxido.Nome, out string acido))
                return Lista(BancoSubstancias.PorNome(acido));

            return Erro($"{oxido.Nome} não reage com água por síntese.", out motivo);
        }

        // Óxido básico + óxido ácido → sal (CaO + CO2 → CaCO3)
        DadosSubstancia basico = ValidadorQuimico.EhOxidoBasicoReativo(a) ? a : ValidadorQuimico.EhOxidoBasicoReativo(b) ? b : null;
        DadosSubstancia acidoOx = basico == null ? null : (basico == a ? b : a);
        if (basico != null && OxoacidoDoOxido.TryGetValue(acidoOx.Nome, out string nomeAcido))
        {
            var oxoacido = BancoSubstancias.PorNome(nomeAcido);
            var sal = BancoSubstancias.Combinar(basico.Cation, basico.CargaCation, oxoacido.Anion, oxoacido.CargaAnion);
            return sal != null ? Lista(sal) : Erro("O sal formado não está no catálogo.", out motivo);
        }

        return Erro("Essa combinação não forma um produto por síntese.", out motivo);
    }

    // ── Simples troca ───────────────────────────────────────────────────────

    private static List<DadosSubstancia> SimplesTroca(DadosSubstancia a, DadosSubstancia b, out string motivo)
    {
        motivo = null;

        if (a.EhSimples == b.EhSimples)
            return Erro("A simples troca precisa de uma substância simples e um composto.", out motivo);

        DadosSubstancia simples = a.EhSimples ? a : b;
        DadosSubstancia composto = a.EhSimples ? b : a;

        if (!composto.EhIonico)
            return Erro($"{composto.Nome} não participa de simples troca.", out motivo);

        int livre = simples.Elemento;

        // Halogênio desloca halogênio menos reativo: Cl2 + 2 KI → 2 KCl + I2
        if (ValidadorQuimico.EhHalogenio(livre))
        {
            int halogenioComposto = ValidadorQuimico.ElementoDoIon(composto.Anion);
            if (!ValidadorQuimico.EhHalogenio(halogenioComposto))
                return Erro($"{simples.Nome} só desloca halogênios de haletos.", out motivo);
            if (composto.Funcao == FuncaoQuimica.Sal && ValidadorQuimico.EhInsoluvel(composto.Cation, composto.Anion))
                return Erro($"{composto.Nome} é insolúvel; não há íons livres para a troca.", out motivo);
            if (!ValidadorQuimico.DeslocaHalogenio(livre, halogenioComposto))
                return Erro($"{simples.Nome} é menos reativo que o halogênio de {composto.Nome}; a reação não ocorre.", out motivo);

            var novo = BancoSubstancias.Combinar(composto.Cation, composto.CargaCation, Ion(livre), -1);
            return Lista(novo, BancoSubstancias.SimplesDoElemento(halogenioComposto));
        }

        if (!ValidadorQuimico.EhMetal(livre) && livre != Atomos.H)
            return Erro($"{simples.Nome} não participa de simples troca neste simulador.", out motivo);

        // Metal + água: 2 Na + 2 H2O → 2 NaOH + H2
        if (ValidadorQuimico.EhAgua(composto))
        {
            if (!ValidadorQuimico.ReageComAgua(livre))
                return Erro($"{simples.Nome} não reage com água fria.", out motivo);

            var hidroxido = CombinarComMenorCarga(simples, BancoSubstancias.OH, -1);
            return Lista(hidroxido, BancoSubstancias.PorNome("H2"));
        }

        int deslocado = ValidadorQuimico.ElementoDoIon(composto.Cation);
        if (deslocado == 0 || deslocado == livre)
            return Erro($"{simples.Nome} não tem o que deslocar em {composto.Nome}.", out motivo);

        switch (composto.Funcao)
        {
            case FuncaoQuimica.Acido:
                // Metal + ácido → sal + H2 (só metais acima do H)
                if (livre == Atomos.H)
                    return Erro("H2 não reage com ácidos.", out motivo);
                if (composto.Nome == "HNO3")
                    return Erro("HNO3 é um ácido oxidante e não libera H2 com metais.", out motivo);
                break;

            case FuncaoQuimica.Sal:
                // Metal + sal em solução: o sal precisa ser solúvel, e metais que
                // reagem com água (Na, K, Ca, Ba) reagiriam com ela, não com o sal.
                if (livre == Atomos.H)
                    return Erro("H2 só reduz óxidos metálicos.", out motivo);
                if (ValidadorQuimico.ReageComAgua(livre))
                    return Erro($"{simples.Nome} reage com a água da solução, não com o sal.", out motivo);
                if (ValidadorQuimico.EhInsoluvel(composto.Cation, composto.Anion))
                    return Erro($"{composto.Nome} é insolúvel; não há íons livres para a troca.", out motivo);
                break;

            case FuncaoQuimica.Oxido:
                // Redução de óxido metálico a quente: H2, Al ou Mg (ex.: aluminotermia).
                if (livre != Atomos.H && livre != Atomos.Al && livre != Atomos.Mg)
                    return Erro($"{simples.Nome} não é usado para reduzir óxidos.", out motivo);
                if (!ValidadorQuimico.EhMetal(deslocado))
                    return Erro($"{composto.Nome} não é um óxido metálico.", out motivo);
                break;

            default:
                return Erro($"{simples.Nome} não desloca o metal de {composto.Nome}.", out motivo);
        }

        if (!ValidadorQuimico.DeslocaMetal(livre, deslocado))
            return Erro($"{simples.Nome} é menos reativo que o cátion de {composto.Nome}; a reação não ocorre.", out motivo);

        var sal = CombinarComMenorCarga(simples, composto.Anion, composto.CargaAnion);
        return Lista(sal, BancoSubstancias.SimplesDoElemento(deslocado));
    }

    // Na simples troca o metal assume, em geral, seu menor NOX (Fe → Fe²⁺).
    private static DadosSubstancia CombinarComMenorCarga(DadosSubstancia metal, Dictionary<int, int> anion, int cargaAnion)
    {
        foreach (int carga in metal.CargasPossiveis.Where(c => c > 0).OrderBy(c => c))
        {
            var produto = BancoSubstancias.Combinar(Ion(metal.Elemento), carga, anion, cargaAnion);
            if (produto != null)
                return produto;
        }
        return null;
    }

    // ── Dupla troca ─────────────────────────────────────────────────────────

    private static List<DadosSubstancia> DuplaTroca(DadosSubstancia a, DadosSubstancia b, out string motivo)
    {
        motivo = null;

        if (!a.EhIonico || !b.EhIonico)
            return Erro("A dupla troca precisa de dois compostos iônicos (ácidos, bases, sais ou óxidos).", out motivo);

        if (ValidadorQuimico.EhAgua(a) || ValidadorQuimico.EhAgua(b))
            return Erro("A água não participa de dupla troca neste simulador.", out motivo);

        DadosSubstancia agua = BancoSubstancias.PorNome("H2O");

        // Óxido metálico + ácido → sal + água (CaO + 2 HCl → CaCl2 + H2O)
        DadosSubstancia oxidoMetalico = ValidadorQuimico.EhOxidoMetalico(a) ? a : ValidadorQuimico.EhOxidoMetalico(b) ? b : null;
        if (oxidoMetalico != null)
        {
            DadosSubstancia acido = oxidoMetalico == a ? b : a;
            if (acido.Funcao != FuncaoQuimica.Acido)
                return Erro("Um óxido metálico só faz dupla troca com ácido.", out motivo);
            var sal = BancoSubstancias.Combinar(oxidoMetalico.Cation, oxidoMetalico.CargaCation, acido.Anion, acido.CargaAnion);
            return Lista(sal, agua);
        }

        // Óxido ácido + base → sal + água (CO2 + 2 NaOH → Na2CO3 + H2O)
        DadosSubstancia oxidoAcido = a.Funcao == FuncaoQuimica.Oxido ? a : b.Funcao == FuncaoQuimica.Oxido ? b : null;
        if (oxidoAcido != null)
        {
            DadosSubstancia baseQ = oxidoAcido == a ? b : a;
            if (baseQ.Funcao != FuncaoQuimica.Base || !OxoacidoDoOxido.TryGetValue(oxidoAcido.Nome, out string nomeAcido))
                return Erro($"{oxidoAcido.Nome} não reage com {baseQ.Nome}.", out motivo);
            var oxoacido = BancoSubstancias.PorNome(nomeAcido);
            var sal = BancoSubstancias.Combinar(baseQ.Cation, baseQ.CargaCation, oxoacido.Anion, oxoacido.CargaAnion);
            return Lista(sal, agua);
        }

        // AB + CD → AD + CB
        if (DadosSubstancia.Chave(a.Cation) == DadosSubstancia.Chave(b.Cation) ||
            DadosSubstancia.Chave(a.Anion) == DadosSubstancia.Chave(b.Anion))
            return Erro("Os reagentes têm o mesmo cátion ou o mesmo ânion; não há o que trocar.", out motivo);

        // Um reagente insolúvel só reage se o outro for ácido.
        bool aInsoluvel = a.Funcao != FuncaoQuimica.Acido && ValidadorQuimico.EhInsoluvel(a.Cation, a.Anion);
        bool bInsoluvel = b.Funcao != FuncaoQuimica.Acido && ValidadorQuimico.EhInsoluvel(b.Cation, b.Anion);
        if ((aInsoluvel && b.Funcao != FuncaoQuimica.Acido) || (bInsoluvel && a.Funcao != FuncaoQuimica.Acido))
            return Erro("Um dos reagentes é insolúvel e não reage com esse composto.", out motivo);

        // Ácido + sal: o ácido formado precisa ser mais fraco que o reagente,
        // a menos que se forme um precipitado insolúvel em ácido (AgCl, BaSO4, CuS).
        DadosSubstancia acidoReagente = a.Funcao == FuncaoQuimica.Acido ? a : b.Funcao == FuncaoQuimica.Acido ? b : null;
        DadosSubstancia salReagente = acidoReagente == null ? null : (acidoReagente == a ? b : a);
        if (acidoReagente != null && salReagente.Funcao == FuncaoQuimica.Sal)
        {
            var acidoFormado = BancoSubstancias.Combinar(Ion(Atomos.H), +1, salReagente.Anion, salReagente.CargaAnion);
            bool formaAcidoMaisFraco = acidoFormado != null &&
                                       ValidadorQuimico.AcidoMaisForte(acidoReagente.Nome, acidoFormado.Nome);
            bool precipitadoResistente = ValidadorQuimico.InsoluvelEmAcido(salReagente.Cation, acidoReagente.Anion);

            if (ValidadorQuimico.InsoluvelEmAcido(salReagente.Cation, salReagente.Anion))
                return Erro($"{salReagente.Nome} não se dissolve em ácido.", out motivo);
            if (!formaAcidoMaisFraco && !precipitadoResistente)
                return Erro($"{acidoReagente.Nome} não é forte o suficiente para deslocar o ácido de {salReagente.Nome}.", out motivo);
        }

        var produtos = new List<DadosSubstancia>();
        bool forcaMotriz = false;
        bool gasOuAgua = false;

        foreach (var (cation, cargaC, anion, cargaA) in new[]
        {
            (a.Cation, a.CargaCation, b.Anion, b.CargaAnion),
            (b.Cation, b.CargaCation, a.Anion, a.CargaAnion)
        })
        {
            var formula = DadosSubstancia.MontarFormulaIonica(cation, cargaC, anion, cargaA);
            string nome = BancoSubstancias.PorFormula(formula)?.Nome;

            // Produtos instáveis se decompõem na hora.
            if (nome == "H2CO3")
            {
                produtos.Add(BancoSubstancias.PorNome("CO2"));
                produtos.Add(agua);
                forcaMotriz = gasOuAgua = true;
                continue;
            }
            if (nome == "H2SO3")
            {
                produtos.Add(BancoSubstancias.PorNome("SO2"));
                produtos.Add(agua);
                forcaMotriz = gasOuAgua = true;
                continue;
            }
            if (ValidadorQuimico.EhAmonio(cation) && DadosSubstancia.Chave(anion) == DadosSubstancia.Chave(BancoSubstancias.OH))
            {
                produtos.Add(BancoSubstancias.PorNome("NH3"));
                produtos.Add(agua);
                forcaMotriz = gasOuAgua = true;
                continue;
            }

            var produto = BancoSubstancias.PorFormula(formula);
            if (produto == null)
                return Erro("Um dos produtos não existe no catálogo de substâncias.", out motivo);

            if (produto == agua || ValidadorQuimico.EhGas(produto))
                forcaMotriz = gasOuAgua = true;
            else if (ValidadorQuimico.EhInsoluvel(cation, anion))
                forcaMotriz = true;

            produtos.Add(produto);
        }

        if (!forcaMotriz)
            return Erro("Não se forma precipitado, gás nem água; os íons ficam em solução e a reação não ocorre.", out motivo);

        // Sólido insolúvel + ácido fraco: só reage se liberar gás ou formar água.
        if ((aInsoluvel || bInsoluvel) && !gasOuAgua && !ValidadorQuimico.EhAcidoForte(acidoReagente?.Nome ?? ""))
            return Erro("Um ácido fraco não dissolve esse sólido insolúvel.", out motivo);

        return produtos;
    }

    // ── Utilitários ─────────────────────────────────────────────────────────

    private static Dictionary<int, int> Ion(int z) => new Dictionary<int, int> { { z, 1 } };

    private static List<DadosSubstancia> Lista(params DadosSubstancia[] itens) => itens.ToList();

    private static List<DadosSubstancia> Erro(string mensagem, out string motivo)
    {
        motivo = mensagem;
        return null;
    }
}

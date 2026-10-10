using System;
using System.Collections.Generic;
using System.Linq;
using BalanciadorQuimico;

public enum FuncaoQuimica
{
    SubstanciaSimples,
    Oxido,
    Acido,
    Base,
    Sal,
    Molecular   // compostos moleculares sem modelo cátion/ânion (orgânicos, NH3)
}

// Dados de uma substância do banco.
//
// Compostos iônicos (óxidos, ácidos, bases e sais) guardam a UNIDADE de cada íon
// e a carga dela — ex.: Fe2O3 = cátion {Fe:1} +3 e ânion {O:1} -2. Os índices da
// fórmula (Fe2, O3) são calculados pelas cargas via MDC em Formula, nunca guardados.
// Isso permite ao GeradorDeProduto recombinar íons sem multiplicar índices duas vezes.
public class DadosSubstancia
{
    public string Nome { get; }
    public FuncaoQuimica Funcao { get; }

    // Iônicos
    public Dictionary<int, int> Cation { get; }
    public int CargaCation { get; }
    public Dictionary<int, int> Anion { get; }
    public int CargaAnion { get; }

    // Substância simples
    public int Elemento { get; }
    public int Atomicidade { get; }
    // Cargas que o elemento assume ao formar íons, em ordem de preferência.
    public int[] CargasPossiveis { get; }

    public bool Combustivel { get; }

    public Dictionary<int, int> Formula { get; }

    public bool EhSimples => Funcao == FuncaoQuimica.SubstanciaSimples;
    public bool EhIonico => Cation != null && Anion != null;

    public int QtdCation => EhIonico ? Math.Abs(CargaAnion) / Mdc(CargaCation, CargaAnion) : 0;
    public int QtdAnion  => EhIonico ? Math.Abs(CargaCation) / Mdc(CargaCation, CargaAnion) : 0;

    private DadosSubstancia(string nome, FuncaoQuimica funcao,
                            Dictionary<int, int> cation, int cargaCation,
                            Dictionary<int, int> anion, int cargaAnion,
                            int elemento, int atomicidade, int[] cargas,
                            Dictionary<int, int> formulaMolecular, bool combustivel)
    {
        Nome = nome;
        Funcao = funcao;
        Cation = cation;
        CargaCation = cargaCation;
        Anion = anion;
        CargaAnion = cargaAnion;
        Elemento = elemento;
        Atomicidade = atomicidade;
        CargasPossiveis = cargas ?? Array.Empty<int>();
        Combustivel = combustivel;

        if (funcao == FuncaoQuimica.SubstanciaSimples)
            Formula = new Dictionary<int, int> { { elemento, atomicidade } };
        else if (cation != null && anion != null)
            Formula = MontarFormulaIonica(cation, cargaCation, anion, cargaAnion);
        else
            Formula = new Dictionary<int, int>(formulaMolecular);
    }

    // ── Fábricas ─────────────────────────────────────────────────────────────

    public static DadosSubstancia Simples(string nome, int elemento, int atomicidade,
                                          int[] cargas, bool combustivel = false)
    {
        return new DadosSubstancia(nome, FuncaoQuimica.SubstanciaSimples,
            null, 0, null, 0, elemento, atomicidade, cargas, null, combustivel);
    }

    public static DadosSubstancia Ionico(string nome, FuncaoQuimica funcao,
                                         Dictionary<int, int> cation, int cargaCation,
                                         Dictionary<int, int> anion, int cargaAnion,
                                         bool combustivel = false)
    {
        if (cargaCation <= 0 || cargaAnion >= 0)
            throw new ArgumentException($"{nome}: o cátion precisa de carga positiva e o ânion de carga negativa.");

        return new DadosSubstancia(nome, funcao, cation, cargaCation, anion, cargaAnion,
            0, 0, null, null, combustivel);
    }

    public static DadosSubstancia Molecular(string nome, Dictionary<int, int> formula,
                                            bool combustivel = false)
    {
        return new DadosSubstancia(nome, FuncaoQuimica.Molecular,
            null, 0, null, 0, 0, 0, null, formula, combustivel);
    }

    // ── Fórmula ──────────────────────────────────────────────────────────────

    public static Dictionary<int, int> MontarFormulaIonica(Dictionary<int, int> cation, int cargaCation,
                                                          Dictionary<int, int> anion, int cargaAnion)
    {
        int mdc = Mdc(cargaCation, cargaAnion);
        int qtdCation = Math.Abs(cargaAnion) / mdc;
        int qtdAnion = Math.Abs(cargaCation) / mdc;

        var formula = new Dictionary<int, int>();
        Somar(formula, cation, qtdCation);
        Somar(formula, anion, qtdAnion);
        return formula;
    }

    public static void Somar(Dictionary<int, int> destino, Dictionary<int, int> parte, int fator)
    {
        foreach (var par in parte)
        {
            destino.TryGetValue(par.Key, out int atual);
            destino[par.Key] = atual + par.Value * fator;
        }
    }

    // Chave canônica de uma composição (independe da ordem do dicionário),
    // usada para procurar uma fórmula no banco.
    public static string Chave(Dictionary<int, int> composicao)
    {
        return string.Join("|", composicao
            .Where(p => p.Value > 0)
            .OrderBy(p => p.Key)
            .Select(p => p.Key + ":" + p.Value));
    }

    public static int Mdc(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (b != 0)
        {
            int t = b;
            b = a % b;
            a = t;
        }
        return a == 0 ? 1 : a;
    }

    public override string ToString() => Nome;
}

// Ponte estática para levar reagentes/produtos balanceados até a cena de visualização 3D.
public static class ReacaoTransferData
{
    public static List<Dictionary<int, int>> Reagentes = new List<Dictionary<int, int>>();
    public static List<Dictionary<int, int>> Produtos = new List<Dictionary<int, int>>();
    public static string Equacao = "";   // texto da equação balanceada, exibido no título da cena 3D
    public static string NomeCena = "Reacao3D";
}

using System;
using System.Collections.Generic;
using System.Linq;
using BalanciadorQuimico;

// Estruturas de Lewis cadastradas à mão para as moléculas e íons em que a
// regra de valência não basta: ligações duplas/triplas com NOX variável
// (S, N, P), cargas formais (NO3⁻, HNO3, CO) e anéis (benzeno).
//
// Formato:
//   átomos  → símbolos separados por espaço; o índice de cada um é a posição.
//   ligações → "i-j" simples, "i=j" dupla, "i#j" tripla.
//   anéis   → (opcional) índices dos átomos de cada anel, em ordem;
//             vários anéis separados por "|".
public static class EstruturasConhecidas
{
    public class Definicao
    {
        public int[] Atomos;
        public List<(int a, int b, int ordem)> Ligacoes = new List<(int, int, int)>();
        public List<int[]> Aneis = new List<int[]>();
    }

    // Moléculas neutras.
    private static readonly Dictionary<string, Definicao> moleculas = new Dictionary<string, Definicao>();

    // Íons poliatômicos (usados para montar sais, bases e ácidos iônicos).
    private static readonly Dictionary<string, Definicao> ions = new Dictionary<string, Definicao>();

    static EstruturasConhecidas()
    {
        // ── Diatômicas ──
        Molecula("O O", "0=1");
        Molecula("N N", "0#1");
        Molecula("C O", "0#1");                                     // C≡O
        Molecula("N O", "0=1");                                     // NO (radical)

        // ── Óxidos ──
        Molecula("C O O", "0=1 0=2");                               // O=C=O
        Molecula("S O O", "0=1 0=2");                               // O=S=O (angular)
        Molecula("S O O O", "0=1 0=2 0=3");                         // SO3 trigonal
        Molecula("N O O", "0=1 0-2");                               // NO2 (angular)
        Molecula("N N O", "0=1 1=2");                               // N=N=O
        Molecula("P P O O O O O", "0=2 0=3 0-4 1-4 1=5 1=6");       // O2P–O–PO2

        // ── Oxoácidos ──
        Molecula("S O O O O H H", "0=1 0=2 0-3 0-4 3-5 4-6");       // H2SO4
        Molecula("S O O O H H", "0=1 0-2 0-3 2-4 3-5");             // H2SO3
        Molecula("N O O O H", "0=1 0-2 0-3 3-4");                   // HNO3
        Molecula("N O O H", "0=1 0-2 2-3");                         // HNO2
        Molecula("P O O O O H H H", "0=1 0-2 0-3 0-4 2-5 3-6 4-7"); // H3PO4
        Molecula("C O O O H H", "0=1 0-2 0-3 2-4 3-5");             // H2CO3

        // ── Orgânicos com ligação múltipla ou anel ──
        Molecula("C C H H H H", "0=1 0-2 0-3 1-4 1-5");             // eteno
        Molecula("C C H H", "0#1 0-2 1-3");                         // etino
        Molecula("C C C C C C H H H H H H",
                 "0=1 1-2 2=3 3-4 4=5 5-0 0-6 1-7 2-8 3-9 4-10 5-11",
                 aneis: "0 1 2 3 4 5");                             // benzeno

        // Sacarose: glicose (anel de 6, índices 0–5) + frutose (anel de 5,
        // índices 13–17) unidas pelo O glicosídico (7). Sem ligações duplas.
        Molecula("C C C C C O C O O O O O C C C C C O C O O O O " +
                 "H H H H H H H H H H H H H H H H H H H H H H",
                 "0-1 1-2 2-3 3-4 4-5 5-0 " +                       // anel da glicose
                 "4-6 0-7 1-8 2-9 3-10 6-11 " +
                 "13-14 14-15 15-16 16-17 17-13 " +                 // anel da frutose
                 "7-13 13-12 12-19 14-20 15-21 16-18 18-22 " +
                 "0-23 1-24 2-25 3-26 4-27 6-28 6-29 8-30 9-31 10-32 11-33 " +
                 "12-34 12-35 14-36 15-37 16-38 18-39 18-40 19-41 20-42 21-43 22-44",
                 aneis: "0 1 2 3 4 5 | 13 14 15 16 17");

        // ── Íons poliatômicos ──
        Ion("O H", "0-1");                                          // OH⁻
        Ion("N H H H H", "0-1 0-2 0-3 0-4");                        // NH4⁺
        Ion("S O O O O", "0=1 0=2 0-3 0-4");                        // SO4²⁻
        Ion("S O O O", "0=1 0-2 0-3");                              // SO3²⁻ (piramidal)
        Ion("C O O O", "0=1 0-2 0-3");                              // CO3²⁻
        Ion("C O O O H", "0=1 0-2 0-3 3-4");                        // HCO3⁻
        Ion("N O O O", "0=1 0-2 0-3");                              // NO3⁻
        Ion("N O O", "0=1 0-2");                                    // NO2⁻
        Ion("P O O O O", "0=1 0-2 0-3 0-4");                        // PO4³⁻
    }

    public static bool TryObterMolecula(Dictionary<int, int> composicao, out Definicao def) =>
        moleculas.TryGetValue(DadosSubstancia.Chave(composicao), out def);

    public static bool TryObterIon(Dictionary<int, int> composicao, out Definicao def) =>
        ions.TryGetValue(DadosSubstancia.Chave(composicao), out def);

    // ── Cadastro ─────────────────────────────────────────────────────────────

    private static void Molecula(string atomos, string ligacoes, string aneis = null) =>
        Registrar(moleculas, atomos, ligacoes, aneis);

    private static void Ion(string atomos, string ligacoes) =>
        Registrar(ions, atomos, ligacoes, null);

    private static void Registrar(Dictionary<string, Definicao> destino,
                                  string atomos, string ligacoes, string aneis)
    {
        var def = new Definicao
        {
            Atomos = atomos.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Select(NumeroAtomico).ToArray()
        };

        if (aneis != null)
        {
            foreach (string anel in aneis.Split('|'))
            {
                def.Aneis.Add(anel.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(int.Parse).ToArray());
            }
        }

        foreach (string token in ligacoes.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int ordem = token.Contains('#') ? 3 : token.Contains('=') ? 2 : 1;
            string[] partes = token.Split('-', '=', '#');
            def.Ligacoes.Add((int.Parse(partes[0]), int.Parse(partes[1]), ordem));
        }

        var composicao = new Dictionary<int, int>();
        foreach (int z in def.Atomos)
        {
            composicao.TryGetValue(z, out int n);
            composicao[z] = n + 1;
        }
        destino[DadosSubstancia.Chave(composicao)] = def;
    }

    private static int NumeroAtomico(string simbolo)
    {
        foreach (var par in Elementos.Tabela)
            if (par.Value.Simbolo == simbolo)
                return par.Key;
        throw new ArgumentException("Símbolo desconhecido: " + simbolo);
    }
}

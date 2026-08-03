using System;
using System.Collections.Generic;
using Elementos;

public static class GeradorDeProduto
{
    public static List<Dictionary<int, int>> GerarProduto(string tipoReacao, EspecieQuimica especieA, EspecieQuimica especieB)
    {
        var responce = new List<Dictionary<int, int>>();

        switch (tipoReacao)
        {
            case "Combustao":
                var composicaoCombustivel = especieA.ToDictionary();
                bool temCarbono = composicaoCombustivel.ContainsKey(Atomos.C);
                bool temHidrogenio = composicaoCombustivel.ContainsKey(Atomos.H);

                if (!temCarbono && !temHidrogenio)
                    throw new ArgumentException("Combustível deve conter carbono e/ou hidrogênio para gerar CO2/H2O.");

                if (temCarbono)
                    responce.Add(new Dictionary<int, int>() { { Atomos.C, 1 }, { Atomos.O, 2 } }); // CO2
                if (temHidrogenio)
                    responce.Add(new Dictionary<int, int>() { { Atomos.H, 2 }, { Atomos.O, 1 } }); // H2O
                break;

            case "Decomposicao":
                responce.Add(especieA.GetParteA());
                if (especieA.GetCargaB() != 0)
                    responce.Add(especieA.GetParteB());
                break;

            case "Sintese":
                var sintese = especieA.ToDictionary();
                foreach (var par in especieB.ToDictionary())
                {
                    if (sintese.ContainsKey(par.Key))
                        sintese[par.Key] += par.Value;
                    else
                        sintese[par.Key] = par.Value;
                }
                responce.Add(sintese);
                break;

            case "Simples Troca":
                // AB + C -> AC + B
                if (especieA.GetCargaB() == 0)
                    throw new ArgumentException("Simples Troca exige que a primeira espécie seja um composto AB.");

                int cargaA = Math.Abs(especieA.GetCargaA()); //Especie A
                int cargaB = Math.Abs(especieA.GetCargaB()); //Especie A
                int cargaC = Math.Abs(especieB.GetCargaA()); //Especie B

                int qtdA_noAC = cargaC; // quantidade de A no produto AC
                int qtdC_noAC = cargaA; // quantidade de C no produto AC

                var ac = MultiplicarDicionario(especieA.GetParteA(), qtdA_noAC);
                foreach(var par in MultiplicarDicionario(especieB.GetParteA(), qtdC_noAC))
                {
                    if (ac.ContainsKey(par.Key))
                        ac[par.Key] += par.Value;
                    else
                        ac[par.Key] = par.Value;
                }

                int qtdB_saindo = cargaA * cargaC / cargaB; // quantidade de B saindo da reação

                responce.Add(ac);
                responce.Add(MultiplicarDicionario(especieA.GetParteB(), qtdB_saindo));
                break;

            case "Dupla Troca":
                // AB + CD -> AD + CB
                if (especieA.GetCargaB() == 0 || especieB.GetCargaB() == 0)
                    throw new ArgumentException("Dupla Troca exige que as duas espécies sejam compostos AB e CD.");

                int cargaA_Dt = Math.Abs(especieA.GetCargaA());
                int cargaD_Dt = Math.Abs(especieB.GetCargaB());
                int cargaC_Dt = Math.Abs(especieB.GetCargaA());
                int cargaB_Dt = Math.Abs(especieA.GetCargaB());

                int qtdA_noAD = cargaD_Dt; // quantidade de A no produto AD
                int qtdD_noAD = cargaA_Dt; // quantidade de D no produto AD
                int qtdC_noCB = cargaB_Dt; // quantidade de C no produto CB
                int qtdB_noCB = cargaC_Dt; // quantidade de B no produto CB

                var ad = MultiplicarDicionario(especieA.GetParteA(), qtdA_noAD);
                foreach (var par in MultiplicarDicionario(especieB.GetParteB(), qtdD_noAD))
                {
                    if (ad.ContainsKey(par.Key)) ad[par.Key] += par.Value;
                    else ad[par.Key] = par.Value;
                }

                var cb = MultiplicarDicionario(especieB.GetParteA(), qtdC_noCB);
                foreach (var par in MultiplicarDicionario(especieA.GetParteB(), qtdB_noCB))
                {
                    if (cb.ContainsKey(par.Key)) cb[par.Key] += par.Value;
                    else cb[par.Key] = par.Value;
                }

                responce.Add(ad);
                responce.Add(cb);
                break;

            default:
                throw new ArgumentException($"Tipo de reação desconhecido: {tipoReacao}");
        }
        return responce;
    }
    private static Dictionary<int, int> MultiplicarDicionario(Dictionary<int, int> dict, int fator)
    {
        var resultado = new Dictionary<int, int>();
        foreach (var par in dict)
            resultado[par.Key] = par.Value * fator;
        return resultado;
    }
}

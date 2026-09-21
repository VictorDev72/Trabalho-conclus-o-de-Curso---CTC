using System.Collections.Generic;
using BalanciadorQuimico;

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

// Ponte estática para levar reagentes/produtos balanceados até a cena de visualização 3D.
public static class ReacaoTransferData
{
    public static List<Dictionary<int, int>> Reagentes = new List<Dictionary<int, int>>();
    public static List<Dictionary<int, int>> Produtos = new List<Dictionary<int, int>>();
    public static string NomeCena = "Reacao3D";
}

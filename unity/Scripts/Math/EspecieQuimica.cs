using System;
using System.Collections.Generic;

namespace BalanciadorQuimico
{
public class EspecieQuimica
{
    private Dictionary<int, int> ParteA { get; set; }
    private Dictionary<int, int> ParteB { get; set; }

    private readonly int CargaA;
    private readonly int CargaB;

    // cargaB = 0 representa uma espécie simples, sem ParteB (ex: elementos como O2, Na, Cl2)
    public EspecieQuimica(Dictionary<int, int> parteA, Dictionary<int, int> parteB, int cargaA, int cargaB = 0)
    {
        if (parteA == null)
            throw new ArgumentException("ParteA não pode ser nula");
        if(cargaA == 0)
            throw new ArgumentException("CargaA não pode ser zero");

        ParteA = parteA;
        ParteB = parteB ?? new Dictionary<int, int>();

        CargaA = cargaA;
        CargaB = cargaB;

    }

    public Dictionary<int,int> ToDictionary()
    {

        var res = new Dictionary<int, int>();

        foreach (var x in ParteA)
        {
            res[x.Key] = x.Value;
        }

        foreach (var x in ParteB)
        {
            if (res.ContainsKey(x.Key))
            {
                res[x.Key] += x.Value;
            }
            else
            {
                res[x.Key] = x.Value;
            }
        }
        return res;
    }
    public int GetCargaTotal()
    {
        return CargaA + CargaB;
    }
    public int GetCargaA()
    {
        return CargaA;
    }
    public int GetCargaB()
    {
        return CargaB;
    }

    public Dictionary<int, int> GetParteA()
    {
        return new Dictionary<int, int>(ParteA);
    }

    public Dictionary<int, int> GetParteB()
    {
        return new Dictionary<int, int>(ParteB);
    }
}
}

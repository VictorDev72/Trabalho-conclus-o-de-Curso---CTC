public static class GeradorDeProduto
{
    public static List<Dictionary<int, int>> GerarProduto(String tipoReacao,EspecieQuimica especieA, EspecieQuimica especieB)
    {
        responce= new List<Dictionary<int, int>>();

        switch(tipoReacao)
        {
            case "Combustao":
                responce.Add(new Dictionary<int, int>() { { 6, 1 }, { 8, 2 } });
                responce.Add(new Dictionary<int, int>() { { 1, 2 }, { 8, 1 } });
            case "Decomposicao":
                    responce.Add(especieA.GetParteA());
                    responce.Add(especieA.GetParteB());
                    break;
            case "Sintese":
                var cargaA = especieA.GetCargaA();
                var cargaB = especieB.GetCargaB();
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
                var ac = especieA.GetParteA();
                foreach(var par in especieB.ToDictionary())
                {
                    if (ac.ContainsKey(par.Key))
                        ac[par.Key] += par.Value;
                    else
                        ac[par.Key] = par.Value;
                }
                responce.Add(ac);
                responce.Add(especieB.GetParteB());
                break;
            case "Dupla Troca":
                // AB + CD -> AD + CB
                var ad = especieA.GetParteA();
                var cb = especieB.GetParteB();
                foreach(var par in especieB.GetParteB())
                {
                    if (ad.ContainsKey(par.Key))
                        ad[par.Key] += par.Value;
                    else
                        ad[par.Key] = par.Value;
                }
                foreach(var par in especieA.GetParteB())
                {
                    if (cb.ContainsKey(par.Key))
                        cb[par.Key] += par.Value;
                    else
                        cb[par.Key] = par.Value;
                }
                responce.Add(ad);
                responce.Add(cb);
                break;
        }
        return responce;
    }
}
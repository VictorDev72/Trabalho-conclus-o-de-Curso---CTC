public static class GeradorDeProduto
{
    public static List<Dictionary<int, int>> GerarProduto(String tipoReacao,EspecieQuimica especieA, EspecieQuimica especieB)
    {
        var responce= new List<Dictionary<int, int>>();

        switch(tipoReacao)
        {
            case "Combustao":
                responce.Add(new Dictionary<int, int>() { { 6, 1 }, { 8, 2 } });
                responce.Add(new Dictionary<int, int>() { { 1, 2 }, { 8, 1 } });
                break;
            case "Decomposicao":
                responce.Add(especieA.GetParteA());
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
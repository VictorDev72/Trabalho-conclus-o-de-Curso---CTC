public static class GeradorDeProduto
{
    public static List<Dictionary<int, int>> GerarProduto(String tipoReacao,EspecieQuimica especieA, EspecieQuimica especieB)
    {
        var responce= new List<Dictionary<int, int>>();

        switch(tipoReacao)
        {
            case "Combustao": // Completa
                responce.Add(new Dictionary<int, int>() { { Atomos.C, 1 }, { Atomos.O, 2 } }); // CO2
                responce.Add(new Dictionary<int, int>() { { Atomos.H, 2 }, { Atomos.O, 1 } }); // H2O
                break;

            case "Combustao_Incompleta_CO":
                responce.Add(new Dictionary<int, int>() { { Atomos.C, 1 }, { Atomos.O, 1 } }); // CO
                responce.Add(new Dictionary<int, int>() { { Atomos.H, 2 }, { Atomos.O, 1 } }); // H2O
                break;

            case "Combustao_Incompleta_C":
                responce.Add(new Dictionary<int, int>() { { Atomos.C, 1 } });                  // C (Fuligem)
                responce.Add(new Dictionary<int, int>() { { Atomos.H, 2 }, { Atomos.O, 1 } }); // H2O
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
                // 1. Identifica quem é a substância simples (que não tem Parte B) e quem é a composta
                bool a_eh_simples = (especieA.GetParteB() == null || especieA.GetParteB().Count == 0);
                EspecieQuimica simples = a_eh_simples ? especieA : especieB;
                EspecieQuimica composta = a_eh_simples ? especieB : especieA;

                int cargaSimples = Math.Abs(simples.GetCargaA());
                int cargaCompA = Math.Abs(composta.GetCargaA());
                int cargaCompB = Math.Abs(composta.GetCargaB());

                Dictionary<int, int> novoComposto = new Dictionary<int, int>();
                Dictionary<int, int> deslocado = new Dictionary<int, int>();

                // 2. Se a substância simples é POSITIVA (Metal, ex: Zn), substitui a Parte A (H do HCl)
                if (simples.GetCargaA() > 0) 
                {
                    // Forma CB: Parte A da Simples + Parte B da Composta (Ex: Zn + Cl)
                    int mdc = CalcularMDC(cargaSimples, cargaCompB);
                    int qtdSimples = cargaCompB / mdc;
                    int qtdCompB = cargaSimples / mdc;

                    novoComposto = MultiplicarDicionario(simples.GetParteA(), qtdSimples);
                    foreach (var par in MultiplicarDicionario(composta.GetParteB(), qtdCompB))
                    {
                        if (novoComposto.ContainsKey(par.Key)) novoComposto[par.Key] += par.Value;
                        else novoComposto[par.Key] = par.Value;
                    }

                    // O elemento chutado para fora é a Parte A da Composta (O H)
                    deslocado = MultiplicarDicionario(composta.GetParteA(), 1); 
                }
                // 3. Se a substância simples é NEGATIVA (Ametais como F2), substitui a Parte B
                else 
                {
                    // Forma AC: Parte A da Composta + Parte A da Simples (Ex: Na + F)
                    int mdc = CalcularMDC(cargaCompA, cargaSimples);
                    int qtdCompA = cargaSimples / mdc;
                    int qtdSimples = cargaCompA / mdc;

                    novoComposto = MultiplicarDicionario(composta.GetParteA(), qtdCompA);
                    foreach (var par in MultiplicarDicionario(simples.GetParteA(), qtdSimples))
                    {
                        if (novoComposto.ContainsKey(par.Key)) novoComposto[par.Key] += par.Value;
                        else novoComposto[par.Key] = par.Value;
                    }

                    // O elemento chutado para fora é a Parte B da Composta
                    deslocado = MultiplicarDicionario(composta.GetParteB(), 1);
                }

                // 4. Regra de Ouro da Química: Elementos que viram gás diatômico sozinhos (H2, N2, O2, F2, Cl2, Br2, I2)
                int numAtomicoDeslocado = 0;
                foreach (var chave in deslocado.Keys) { numAtomicoDeslocado = chave; break; } // Pega o ID do elemento

                if (numAtomicoDeslocado == 1 || numAtomicoDeslocado == 7 || numAtomicoDeslocado == 8 || 
                    numAtomicoDeslocado == 9 || numAtomicoDeslocado == 17 || numAtomicoDeslocado == 35 || numAtomicoDeslocado == 53)
                {
                    deslocado[numAtomicoDeslocado] = 2; // Força a ser uma molécula dupla (Ex: H2)
                }
                else 
                {
                    deslocado[numAtomicoDeslocado] = 1; // Metais ficam sozinhos (Ex: Fe, Zn)
                }

                // 5. Adiciona os produtos na lista final
                responce.Add(novoComposto);
                responce.Add(deslocado);
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
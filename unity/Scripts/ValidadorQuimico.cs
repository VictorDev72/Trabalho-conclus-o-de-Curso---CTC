using System.Collections.Generic;

public static class ValidadorQuimico
{
    // Fila de Reatividade dos Cátions (Metais + Hidrogênio) - Do Mais Reativo pro Menos Reativo
    private static readonly List<int> ReatividadeCations = new List<int>
    {
        Atomos.K, Atomos.Na, Atomos.Ca, Atomos.Mg, Atomos.Al, // Metais Alcalinos / Alcalino-Terrosos
        Atomos.Zn, Atomos.Fe,                                 // Metais Comuns
        Atomos.H,                                             // HIDROGÊNIO (Ponto de corte)
        Atomos.Cu, Atomos.Ag, Atomos.Au                       // Metais Nobres (Inertes frente ao H)
    };

    // Fila de Reatividade dos Ânions (Ametais / Halogênios)
    private static readonly List<int> ReatividadeAnions = new List<int>
    {
        Atomos.F, Atomos.O, Atomos.Cl, Atomos.Br, Atomos.I, Atomos.S
    };

    public static bool PodeOcorrerSimplesTroca(int atomoLivre, int atomoSubstituido, bool ehCation)
    {
        List<int> fila = ehCation ? ReatividadeCations : ReatividadeAnions;

        int idxLivre = fila.IndexOf(atomoLivre);
        int idxSubstituido = fila.IndexOf(atomoSubstituido);

        // Se algum elemento não estiver na lista, assume viável ou trata exceção
        if (idxLivre == -1 || idxSubstituido == -1) return true; 

        // No índice da List: Menor Índice = Mais Reativo
        // A reação SÓ ocorre se o elemento livre for MAIS reativo (índice menor)
        return idxLivre < idxSubstituido;
    }

    public static bool PodeOcorrerDuplaTroca(DadosSubstancia subA, DadosSubstancia subB, string formulaA, string formulaB)
    {
        // 1. Ácido + Base -> Sal + Água (Sempre ocorre / Neutralização)
        bool aEhAcido = formulaA.StartsWith("H") && formulaA != "H2O";
        bool bEhAcido = formulaB.StartsWith("H") && formulaB != "H2O";
        bool aEhBase = formulaA.EndsWith("OH");
        bool bEhBase = formulaB.EndsWith("OH");

        if ((aEhAcido && bEhBase) || (bEhAcido && aEhBase)) 
            return true; // Forma H2O

        // 2. Formação de Gases Voláteis (Ex: Carbonatos/Sulfetos reagindo com Ácidos)
        bool temCarbonato = formulaA.Contains("CO3") || formulaB.Contains("CO3");
        bool temSulfeto = formulaA.EndsWith("S") || formulaB.EndsWith("S");
        if ((aEhAcido || bEhAcido) && (temCarbonato || temSulfeto)) 
            return true; // Libera CO2 ou H2S

        // 3. Formação de Precipitados Insolúveis (Tabela Prática de Solubilidade)
        // Sais Insolúveis Clássicos: AgCl, PbCl2, BaSO4, CaCO3, Fe(OH)3, etc.
        if (IraFormarPrecipitado(formulaA, formulaB)) 
            return true;

        // Se ambos os produtos forem aquosos e solúveis, os íons só 'boiam' na solução e a reação NÃO ocorre.
        return false; 
    }

    private static bool IraFormarPrecipitado(string fA, string fB)
    {
        // Troca de pares simples para checar insolúveis clássicos do seu banco
        if ((fA.Contains("Ag") || fB.Contains("Ag")) && (fA.Contains("Cl") || fB.Contains("Cl"))) return true; // AgCl é insolúvel
        if ((fA.Contains("Ba") || fB.Contains("Ba")) && (fA.Contains("SO4") || fB.Contains("SO4"))) return true; // BaSO4 é insolúvel
        if ((fA.Contains("Ca") || fB.Contains("Ca")) && (fA.Contains("CO3") || fB.Contains("CO3"))) return true; // CaCO3 é insolúvel

        return false;
    }
}
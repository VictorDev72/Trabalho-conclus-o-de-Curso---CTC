using UnityEngine;

public static class TabelaCPK
{
    /// <summary>
    /// Retorna a cor baseada no padrão CPK (e Jmol) cobrindo os 118 elementos.
    /// </summary>
    public static Color ObterCor(int numeroAtomico)
    {
        return numeroAtomico switch
        {
            // ── Não-Metais Principais (Cores clássicas CPK) ──
            1  => Color.white,                                 // H - Hidrogênio
            6  => new Color(0.1f, 0.1f, 0.1f),                 // C - Carbono (Preto/Cinza Escuro)
            7  => Color.blue,                                  // N - Nitrogênio
            8  => Color.red,                                   // O - Oxigênio
            15 => new Color(1.0f, 0.65f, 0.0f),                // P - Fósforo (Laranja)
            16 => Color.yellow,                                // S - Enxofre
            5  => new Color(1.0f, 0.7f, 0.7f),                 // B - Boro (Pêssego)
            
            // ── Halogênios ──
            9  => Color.green,                                 // F - Flúor (Verde Claro)
            17 => new Color(0.12f, 0.94f, 0.12f),              // Cl - Cloro (Verde)
            35 => new Color(0.65f, 0.16f, 0.16f),              // Br - Bromo (Vermelho Escuro)
            53 => new Color(0.58f, 0.0f, 0.58f),               // I - Iodo (Roxo Escuro)
            85 or 117 => new Color(0.46f, 0.31f, 0.27f),       // At, Ts (Marrom Escuro)

            // ── Gases Nobres (Ciano / Azul Claro) ──
            2 or 10 or 18 or 36 or 54 or 86 or 118 => Color.cyan, 

            // ── Metais Alcalinos (Violeta / Roxo) ──
            3 or 11 or 19 or 37 or 55 or 87 => new Color(0.6f, 0.2f, 0.8f), 

            // ── Metais Alcalino-Terrosos (Verde Escuro) ──
            4 or 12 or 20 or 38 or 56 or 88 => new Color(0.13f, 0.54f, 0.13f),

            // ── Semimetais / Metaloides (Cinza) ──
            14 or 32 or 33 or 51 or 52 => new Color(0.6f, 0.6f, 0.6f), // Si, Ge, As, Sb, Te

            // ── Metais de Transição Específicos ──
            26 or 44 or 76 => new Color(0.87f, 0.4f, 0.2f),    // Fe, Ru, Os (Ferrugem/Alaranjado)
            29 => new Color(0.78f, 0.45f, 0.2f),               // Cu - Cobre
            47 => new Color(0.75f, 0.75f, 0.75f),              // Ag - Prata
            79 => new Color(1.0f, 0.84f, 0.0f),                // Au - Ouro
            22 => new Color(0.75f, 0.76f, 0.78f),              // Ti - Titânio (Cinza prateado)

            // ── Agrupamentos de Blocos (Metais de transição restantes, Lantanídeos, Actinídeos) ──
            >= 21 and <= 30 => new Color(0.9f, 0.6f, 0.6f),    // Transição Período 4 (Rosa claro)
            >= 39 and <= 48 => new Color(0.9f, 0.6f, 0.6f),    // Transição Período 5
            >= 72 and <= 80 => new Color(0.9f, 0.6f, 0.6f),    // Transição Período 6
            >= 57 and <= 71 => new Color(0.4f, 1.0f, 0.8f),    // Lantanídeos (Verde água)
            >= 89 and <= 103 => new Color(0.4f, 0.8f, 1.0f),   // Actinídeos (Azul celeste)
            
            // ── Superpesados (Elementos 104 a 116) ──
            >= 104 and <= 116 => new Color(0.8f, 0.5f, 0.8f),  // Metais pós-transição superpesados (Rosa escuro)

            // ── Outros metais representativos (Al, Ga, In, Sn, Tl, Pb, Bi, Po) ──
            13 or 31 or 49 or 50 or 81 or 82 or 83 or 84 => new Color(0.65f, 0.65f, 0.65f),

            // Fallback: se por algum motivo receber um número inválido (ex: 0 ou 119+)
            _  => Color.magenta 
        };
    }
}
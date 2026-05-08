namespace Elementos
{
    /// <summary>
    /// Constantes de número atômico para todos os elementos da tabela periódica.
    /// Use estas constantes no lugar de magic numbers ao montar dicionários de composição química.
    /// Exemplo: new Dictionary&lt;int, int&gt; { { Elemento.H, 2 }, { Elemento.O, 1 } } // H2O
    /// </summary>
    public static class Atomos
    {
        // ── Período 1 ──────────────────────────────────────────────
        public const int H  = 1;   // Hidrogênio
        public const int He = 2;   // Hélio

        // ── Período 2 ──────────────────────────────────────────────
        public const int Li = 3;   // Lítio
        public const int Be = 4;   // Berílio
        public const int B  = 5;   // Boro
        public const int C  = 6;   // Carbono
        public const int N  = 7;   // Nitrogênio
        public const int O  = 8;   // Oxigênio
        public const int F  = 9;   // Flúor
        public const int Ne = 10;  // Neônio

        // ── Período 3 ──────────────────────────────────────────────
        public const int Na = 11;  // Sódio
        public const int Mg = 12;  // Magnésio
        public const int Al = 13;  // Alumínio
        public const int Si = 14;  // Silício
        public const int P  = 15;  // Fósforo
        public const int S  = 16;  // Enxofre
        public const int Cl = 17;  // Cloro
        public const int Ar = 18;  // Argônio

        // ── Período 4 ──────────────────────────────────────────────
        public const int K  = 19;  // Potássio
        public const int Ca = 20;  // Cálcio
        public const int Sc = 21;  // Escândio
        public const int Ti = 22;  // Titânio
        public const int V  = 23;  // Vanádio
        public const int Cr = 24;  // Cromo
        public const int Mn = 25;  // Manganês
        public const int Fe = 26;  // Ferro
        public const int Co = 27;  // Cobalto
        public const int Ni = 28;  // Níquel
        public const int Cu = 29;  // Cobre
        public const int Zn = 30;  // Zinco
        public const int Ga = 31;  // Gálio
        public const int Ge = 32;  // Germânio
        public const int As = 33;  // Arsênio
        public const int Se = 34;  // Selênio
        public const int Br = 35;  // Bromo
        public const int Kr = 36;  // Criptônio

        // ── Período 5 ──────────────────────────────────────────────
        public const int Rb = 37;  // Rubídio
        public const int Sr = 38;  // Estrôncio
        public const int Y  = 39;  // Ítrio
        public const int Zr = 40;  // Zircônio
        public const int Nb = 41;  // Nióbio
        public const int Mo = 42;  // Molibdênio
        public const int Tc = 43;  // Tecnécio
        public const int Ru = 44;  // Rutênio
        public const int Rh = 45;  // Ródio
        public const int Pd = 46;  // Paládio
        public const int Ag = 47;  // Prata
        public const int Cd = 48;  // Cádmio
        public const int In = 49;  // Índio
        public const int Sn = 50;  // Estanho
        public const int Sb = 51;  // Antimônio
        public const int Te = 52;  // Telúrio
        public const int I  = 53;  // Iodo
        public const int Xe = 54;  // Xenônio

        // ── Período 6 ──────────────────────────────────────────────
        public const int Cs = 55;  // Césio
        public const int Ba = 56;  // Bário
        public const int La = 57;  // Lantânio
        public const int Ce = 58;  // Cério
        public const int Pr = 59;  // Praseodímio
        public const int Nd = 60;  // Neodímio
        public const int Pm = 61;  // Promécio
        public const int Sm = 62;  // Samário
        public const int Eu = 63;  // Európio
        public const int Gd = 64;  // Gadolínio
        public const int Tb = 65;  // Térbio
        public const int Dy = 66;  // Disprósio
        public const int Ho = 67;  // Hólmio
        public const int Er = 68;  // Érbio
        public const int Tm = 69;  // Túlio
        public const int Yb = 70;  // Itérbio
        public const int Lu = 71;  // Lutécio
        public const int Hf = 72;  // Háfnio
        public const int Ta = 73;  // Tântalo
        public const int W  = 74;  // Tungstênio
        public const int Re = 75;  // Rênio
        public const int Os = 76;  // Ósmio
        public const int Ir = 77;  // Irídio
        public const int Pt = 78;  // Platina
        public const int Au = 79;  // Ouro
        public const int Hg = 80;  // Mercúrio
        public const int Tl = 81;  // Tálio
        public const int Pb = 82;  // Chumbo
        public const int Bi = 83;  // Bismuto
        public const int Po = 84;  // Polônio
        public const int At = 85;  // Ástato
        public const int Rn = 86;  // Radônio

        // ── Período 7 ──────────────────────────────────────────────
        public const int Fr = 87;  // Frâncio
        public const int Ra = 88;  // Rádio
        public const int Ac = 89;  // Actínio
        public const int Th = 90;  // Tório
        public const int Pa = 91;  // Protactínio
        public const int U  = 92;  // Urânio
        public const int Np = 93;  // Netúnio
        public const int Pu = 94;  // Plutônio
        public const int Am = 95;  // Amerício
        public const int Cm = 96;  // Cúrio
        public const int Bk = 97;  // Berquélio
        public const int Cf = 98;  // Califórnio
        public const int Es = 99;  // Einstênio
        public const int Fm = 100; // Férmio
        public const int Md = 101; // Mendelévio
        public const int No = 102; // Nobélio
        public const int Lr = 103; // Laurêncio
        public const int Rf = 104; // Rutherfórdio
        public const int Db = 105; // Dúbnio
        public const int Sg = 106; // Seabórgio
        public const int Bh = 107; // Bóhrio
        public const int Hs = 108; // Hássio
        public const int Mt = 109; // Meitnério
        public const int Ds = 110; // Darmstádtio
        public const int Rg = 111; // Roentgênio
        public const int Cn = 112; // Copernício
        public const int Nh = 113; // Nihônio
        public const int Fl = 114; // Fleróvio
        public const int Mc = 115; // Moscóvio
        public const int Lv = 116; // Livermório
        public const int Ts = 117; // Tenessino
        public const int Og = 118; // Oganessônio
    }
}

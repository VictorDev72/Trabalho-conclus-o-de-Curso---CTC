
using System.Collections.Generic;
using System.Linq;
namespace BalanciadorQuimico
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
public class ElementoInfo
{
    public int Z { get; }
    public string Simbolo { get; }
    public string Nome { get; }
    
    // Raio atômico empírico/calculado (pm)
    public double RaioAtomico { get; }
    
    // Escala de Pauling
    public double Eletronegatividade { get; }
    
    // Valências/ligações mais comuns
    public List<int> LigacoesPossiveis { get; }
    
    // Números de oxidação mais comuns
    public List<int> NOX { get; }

    public ElementoInfo(
        int z,
        string simbolo,
        string nome,
        double raio = 0,
        double eletronegatividade = 0,
        List<int> ligacoes = null,
        List<int> nox = null)
    {
        Z = z;
        Simbolo = simbolo;
        Nome = nome;
        RaioAtomico = raio;
        Eletronegatividade = eletronegatividade;
        LigacoesPossiveis = ligacoes ?? new List<int>();
        NOX = nox ?? new List<int>();
    }
}

public static class Elementos
{
    public static readonly Dictionary<int, ElementoInfo> Tabela = new Dictionary<int, ElementoInfo>()
    {
        { 1,  new ElementoInfo(1,  "H",  "Hidrogênio", 53, 2.20, new List<int> { 1 }, new List<int> { 1, -1 }) },
        { 2,  new ElementoInfo(2,  "He", "Hélio", 31, 0, new List<int> { 0 }, new List<int> { 0 }) },
        { 3,  new ElementoInfo(3,  "Li", "Lítio", 167, 0.98, new List<int> { 1 }, new List<int> { 1 }) },
        { 4,  new ElementoInfo(4,  "Be", "Berílio", 112, 1.57, new List<int> { 2 }, new List<int> { 2 }) },
        { 5,  new ElementoInfo(5,  "B",  "Boro", 87, 2.04, new List<int> { 3 }, new List<int> { 3 }) },
        { 6,  new ElementoInfo(6,  "C",  "Carbono", 67, 2.55, new List<int> { 4 }, new List<int> { 4, 2, -4 }) },
        { 7,  new ElementoInfo(7,  "N",  "Nitrogênio", 56, 3.04, new List<int> { 3, 4 }, new List<int> { 5, 4, 3, 2, 1, -3 }) },
        { 8,  new ElementoInfo(8,  "O",  "Oxigênio", 48, 3.44, new List<int> { 2 }, new List<int> { -2, -1 }) },
        { 9,  new ElementoInfo(9,  "F",  "Flúor", 42, 3.98, new List<int> { 1 }, new List<int> { -1 }) },
        { 10, new ElementoInfo(10, "Ne", "Neônio", 38, 0, new List<int> { 0 }, new List<int> { 0 }) },
        { 11, new ElementoInfo(11, "Na", "Sódio", 190, 0.93, new List<int> { 1 }, new List<int> { 1 }) },
        { 12, new ElementoInfo(12, "Mg", "Magnésio", 145, 1.31, new List<int> { 2 }, new List<int> { 2 }) },
        { 13, new ElementoInfo(13, "Al", "Alumínio", 118, 1.61, new List<int> { 3 }, new List<int> { 3 }) },
        { 14, new ElementoInfo(14, "Si", "Silício", 111, 1.90, new List<int> { 4 }, new List<int> { 4, -4 }) },
        { 15, new ElementoInfo(15, "P",  "Fósforo", 98, 2.19, new List<int> { 3, 5 }, new List<int> { 5, 3, -3 }) },
        { 16, new ElementoInfo(16, "S",  "Enxofre", 88, 2.58, new List<int> { 2, 4, 6 }, new List<int> { 6, 4, 2, -2 }) },
        { 17, new ElementoInfo(17, "Cl", "Cloro", 79, 3.16, new List<int> { 1 }, new List<int> { 7, 5, 3, 1, -1 }) },
        { 18, new ElementoInfo(18, "Ar", "Argônio", 71, 0, new List<int> { 0 }, new List<int> { 0 }) },
        { 19, new ElementoInfo(19, "K",  "Potássio", 243, 0.82, new List<int> { 1 }, new List<int> { 1 }) },
        { 20, new ElementoInfo(20, "Ca", "Cálcio", 194, 1.00, new List<int> { 2 }, new List<int> { 2 }) },
        { 21, new ElementoInfo(21, "Sc", "Escândio", 184, 1.36, new List<int> { 3 }, new List<int> { 3 }) },
        { 22, new ElementoInfo(22, "Ti", "Titânio", 176, 1.54, new List<int> { 4 }, new List<int> { 4, 3 }) },
        { 23, new ElementoInfo(23, "V",  "Vanádio", 171, 1.63, new List<int> { 5 }, new List<int> { 5, 4, 3, 2 }) },
        { 24, new ElementoInfo(24, "Cr", "Cromo", 166, 1.66, new List<int> { 6, 3 }, new List<int> { 6, 3, 2 }) },
        { 25, new ElementoInfo(25, "Mn", "Manganês", 161, 1.55, new List<int> { 2, 4, 7 }, new List<int> { 7, 6, 4, 3, 2 }) },
        { 26, new ElementoInfo(26, "Fe", "Ferro", 156, 1.83, new List<int> { 2, 3 }, new List<int> { 3, 2 }) },
        { 27, new ElementoInfo(27, "Co", "Cobalto", 152, 1.88, new List<int> { 2, 3 }, new List<int> { 3, 2 }) },
        { 28, new ElementoInfo(28, "Ni", "Níquel", 149, 1.91, new List<int> { 2 }, new List<int> { 2, 3 }) },
        { 29, new ElementoInfo(29, "Cu", "Cobre", 145, 1.90, new List<int> { 1, 2 }, new List<int> { 2, 1 }) },
        { 30, new ElementoInfo(30, "Zn", "Zinco", 142, 1.65, new List<int> { 2 }, new List<int> { 2 }) },
        { 31, new ElementoInfo(31, "Ga", "Gálio", 136, 1.81, new List<int> { 3 }, new List<int> { 3 }) },
        { 32, new ElementoInfo(32, "Ge", "Germânio", 125, 2.01, new List<int> { 4 }, new List<int> { 4, 2 }) },
        { 33, new ElementoInfo(33, "As", "Arsênio", 114, 2.18, new List<int> { 3, 5 }, new List<int> { 5, 3, -3 }) },
        { 34, new ElementoInfo(34, "Se", "Selênio", 103, 2.55, new List<int> { 2, 4, 6 }, new List<int> { 6, 4, -2 }) },
        { 35, new ElementoInfo(35, "Br", "Bromo", 94, 2.96, new List<int> { 1 }, new List<int> { 5, 3, 1, -1 }) },
        { 36, new ElementoInfo(36, "Kr", "Criptônio", 88, 3.00, new List<int> { 0 }, new List<int> { 2 }) },
        { 37, new ElementoInfo(37, "Rb", "Rubídio", 265, 0.82, new List<int> { 1 }, new List<int> { 1 }) },
        { 38, new ElementoInfo(38, "Sr", "Estrôncio", 219, 0.95, new List<int> { 2 }, new List<int> { 2 }) },
        { 39, new ElementoInfo(39, "Y",  "Ítrio", 212, 1.22, new List<int> { 3 }, new List<int> { 3 }) },
        { 40, new ElementoInfo(40, "Zr", "Zircônio", 206, 1.33, new List<int> { 4 }, new List<int> { 4 }) },
        { 41, new ElementoInfo(41, "Nb", "Nióbio", 198, 1.6, new List<int> { 5 }, new List<int> { 5, 3 }) },
        { 42, new ElementoInfo(42, "Mo", "Molibdênio", 190, 2.16, new List<int> { 6 }, new List<int> { 6, 5, 4, 3, 2 }) },
        { 43, new ElementoInfo(43, "Tc", "Tecnécio", 183, 1.9, new List<int> { 7 }, new List<int> { 7, 4 }) },
        { 44, new ElementoInfo(44, "Ru", "Rutênio", 178, 2.2, new List<int> { 3, 4 }, new List<int> { 8, 4, 3 }) },
        { 45, new ElementoInfo(45, "Rh", "Ródio", 173, 2.28, new List<int> { 3 }, new List<int> { 3 }) },
        { 46, new ElementoInfo(46, "Pd", "Paládio", 169, 2.20, new List<int> { 2, 4 }, new List<int> { 4, 2 }) },
        { 47, new ElementoInfo(47, "Ag", "Prata", 165, 1.93, new List<int> { 1 }, new List<int> { 1 }) },
        { 48, new ElementoInfo(48, "Cd", "Cádmio", 161, 1.69, new List<int> { 2 }, new List<int> { 2 }) },
        { 49, new ElementoInfo(49, "In", "Índio", 156, 1.78, new List<int> { 3 }, new List<int> { 3 }) },
        { 50, new ElementoInfo(50, "Sn", "Estanho", 145, 1.96, new List<int> { 2, 4 }, new List<int> { 4, 2 }) },
        { 51, new ElementoInfo(51, "Sb", "Antimônio", 133, 2.05, new List<int> { 3, 5 }, new List<int> { 5, 3, -3 }) },
        { 52, new ElementoInfo(52, "Te", "Telúrio", 123, 2.1, new List<int> { 2, 4, 6 }, new List<int> { 6, 4, -2 }) },
        { 53, new ElementoInfo(53, "I",  "Iodo", 115, 2.66, new List<int> { 1 }, new List<int> { 7, 5, 1, -1 }) },
        { 54, new ElementoInfo(54, "Xe", "Xenônio", 108, 2.6, new List<int> { 0 }, new List<int> { 6, 4, 2 }) },
        { 55, new ElementoInfo(55, "Cs", "Césio", 298, 0.79, new List<int> { 1 }, new List<int> { 1 }) },
        { 56, new ElementoInfo(56, "Ba", "Bário", 253, 0.89, new List<int> { 2 }, new List<int> { 2 }) },
        { 57, new ElementoInfo(57, "La", "Lantânio", 195, 1.1, new List<int> { 3 }, new List<int> { 3 }) },
        { 58, new ElementoInfo(58, "Ce", "Cério", 185, 1.12, new List<int> { 3, 4 }, new List<int> { 4, 3 }) },
        { 59, new ElementoInfo(59, "Pr", "Praseodímio", 182, 1.13, new List<int> { 3 }, new List<int> { 3 }) },
        { 60, new ElementoInfo(60, "Nd", "Neodímio", 181, 1.14, new List<int> { 3 }, new List<int> { 3 }) },
        { 61, new ElementoInfo(61, "Pm", "Promécio", 183, 1.13, new List<int> { 3 }, new List<int> { 3 }) },
        { 62, new ElementoInfo(62, "Sm", "Samário", 180, 1.17, new List<int> { 3 }, new List<int> { 3, 2 }) },
        { 63, new ElementoInfo(63, "Eu", "Európio", 199, 1.2, new List<int> { 2, 3 }, new List<int> { 3, 2 }) },
        { 64, new ElementoInfo(64, "Gd", "Gadolínio", 180, 1.2, new List<int> { 3 }, new List<int> { 3 }) },
        { 65, new ElementoInfo(65, "Tb", "Térbio", 177, 1.2, new List<int> { 3 }, new List<int> { 4, 3 }) },
        { 66, new ElementoInfo(66, "Dy", "Disprósio", 175, 1.22, new List<int> { 3 }, new List<int> { 3 }) },
        { 67, new ElementoInfo(67, "Ho", "Hólmio", 176, 1.23, new List<int> { 3 }, new List<int> { 3 }) },
        { 68, new ElementoInfo(68, "Er", "Érbio", 176, 1.24, new List<int> { 3 }, new List<int> { 3 }) },
        { 69, new ElementoInfo(69, "Tm", "Túlio", 176, 1.25, new List<int> { 3 }, new List<int> { 3 }) },
        { 70, new ElementoInfo(70, "Yb", "Itérbio", 173, 1.1, new List<int> { 2, 3 }, new List<int> { 3, 2 }) },
        { 71, new ElementoInfo(71, "Lu", "Lutécio", 174, 1.27, new List<int> { 3 }, new List<int> { 3 }) },
        { 72, new ElementoInfo(72, "Hf", "Háfnio", 175, 1.3, new List<int> { 4 }, new List<int> { 4 }) },
        { 73, new ElementoInfo(73, "Ta", "Tântalo", 170, 1.5, new List<int> { 5 }, new List<int> { 5 }) },
        { 74, new ElementoInfo(74, "W",  "Tungstênio", 137, 2.36, new List<int> { 6 }, new List<int> { 6, 5, 4, 3, 2 }) },
        { 75, new ElementoInfo(75, "Re", "Rênio", 137, 1.9, new List<int> { 7 }, new List<int> { 7, 6, 4, 2 }) },
        { 76, new ElementoInfo(76, "Os", "Ósmio", 135, 2.2, new List<int> { 4, 8 }, new List<int> { 8, 4 }) },
        { 77, new ElementoInfo(77, "Ir", "Irídio", 136, 2.2, new List<int> { 3, 4 }, new List<int> { 4, 3 }) },
        { 78, new ElementoInfo(78, "Pt", "Platina", 139, 2.28, new List<int> { 2, 4 }, new List<int> { 4, 2 }) },
        { 79, new ElementoInfo(79, "Au", "Ouro", 144, 2.54, new List<int> { 1, 3 }, new List<int> { 3, 1 }) },
        { 80, new ElementoInfo(80, "Hg", "Mercúrio", 151, 2.00, new List<int> { 1, 2 }, new List<int> { 2, 1 }) },
        { 81, new ElementoInfo(81, "Tl", "Tálio", 170, 1.62, new List<int> { 1, 3 }, new List<int> { 3, 1 }) },
        { 82, new ElementoInfo(82, "Pb", "Chumbo", 175, 2.33, new List<int> { 2, 4 }, new List<int> { 4, 2 }) },
        { 83, new ElementoInfo(83, "Bi", "Bismuto", 170, 2.02, new List<int> { 3, 5 }, new List<int> { 5, 3 }) },
        { 84, new ElementoInfo(84, "Po", "Polônio", 168, 2.0, new List<int> { 2, 4 }, new List<int> { 4, 2 }) },
        { 85, new ElementoInfo(85, "At", "Ástato", 202, 2.2, new List<int> { 1 }, new List<int> { 1, -1 }) },
        { 86, new ElementoInfo(86, "Rn", "Radônio", 220, 2.2, new List<int> { 0 }, new List<int> { 2 }) },
        { 87, new ElementoInfo(87, "Fr", "Frâncio", 348, 0.7, new List<int> { 1 }, new List<int> { 1 }) },
        { 88, new ElementoInfo(88, "Ra", "Rádio", 283, 0.9, new List<int> { 2 }, new List<int> { 2 }) },
        { 89, new ElementoInfo(89, "Ac", "Actínio", 260, 1.1, new List<int> { 3 }, new List<int> { 3 }) },
        { 90, new ElementoInfo(90, "Th", "Tório", 179, 1.3, new List<int> { 4 }, new List<int> { 4 }) },
        { 91, new ElementoInfo(91, "Pa", "Protactínio", 161, 1.5, new List<int> { 5 }, new List<int> { 5, 4 }) },
        { 92, new ElementoInfo(92, "U",  "Urânio", 175, 1.38, new List<int> { 6 }, new List<int> { 6, 5, 4, 3 }) },
        { 93, new ElementoInfo(93, "Np", "Netúnio", 175, 1.36, new List<int> { 5 }, new List<int> { 6, 5, 4, 3 }) },
        { 94, new ElementoInfo(94, "Pu", "Plutônio", 175, 1.28, new List<int> { 4 }, new List<int> { 6, 5, 4, 3 }) },
        { 95, new ElementoInfo(95, "Am", "Amerício", 173, 1.13, new List<int> { 3 }, new List<int> { 6, 5, 4, 3 }) },
        { 96, new ElementoInfo(96, "Cm", "Cúrio", 171, 1.28, new List<int> { 3 }, new List<int> { 4, 3 }) },
        { 97, new ElementoInfo(97, "Bk", "Berquélio", 170, 1.3, new List<int> { 3 }, new List<int> { 4, 3 }) },
        { 98, new ElementoInfo(98, "Cf", "Califórnio", 169, 1.3, new List<int> { 3 }, new List<int> { 4, 3 }) },
        { 99, new ElementoInfo(99, "Es", "Einstênio", 165, 1.3, new List<int> { 3 }, new List<int> { 3 }) },
        { 100, new ElementoInfo(100, "Fm", "Férmio", 167, 1.3, new List<int> { 3 }, new List<int> { 3 }) },
        { 101, new ElementoInfo(101, "Md", "Mendelévio", 173, 1.3, new List<int> { 3 }, new List<int> { 3, 2 }) },
        { 102, new ElementoInfo(102, "No", "Nobélio", 176, 1.3, new List<int> { 2, 3 }, new List<int> { 3, 2 }) },
        { 103, new ElementoInfo(103, "Lr", "Laurêncio", 161, 1.3, new List<int> { 3 }, new List<int> { 3 }) },
        { 104, new ElementoInfo(104, "Rf", "Rutherfórdio", 0, 0, new List<int> { 4 }, new List<int> { 4 }) },
        { 105, new ElementoInfo(105, "Db", "Dúbnio", 0, 0, new List<int> { 5 }, new List<int> { 5 }) },
        { 106, new ElementoInfo(106, "Sg", "Seabórgio", 0, 0, new List<int> { 6 }, new List<int> { 6 }) },
        { 107, new ElementoInfo(107, "Bh", "Bóhrio", 0, 0, new List<int> { 7 }, new List<int> { 7 }) },
        { 108, new ElementoInfo(108, "Hs", "Hássio", 0, 0, new List<int> { 8 }, new List<int> { 8 }) },
        { 109, new ElementoInfo(109, "Mt", "Meitnério", 0, 0, new List<int> { 9 }, new List<int> { 9 }) }, // Predições teóricas
        { 110, new ElementoInfo(110, "Ds", "Darmstádtio", 0, 0, new List<int> { 8 }, new List<int> { 8 }) },
        { 111, new ElementoInfo(111, "Rg", "Roentgênio", 0, 0, new List<int> { 3 }, new List<int> { 3, -1 }) },
        { 112, new ElementoInfo(112, "Cn", "Copernício", 0, 0, new List<int> { 2 }, new List<int> { 2 }) },
        { 113, new ElementoInfo(113, "Nh", "Nihônio", 0, 0, new List<int> { 1 }, new List<int> { 1 }) },
        { 114, new ElementoInfo(114, "Fl", "Fleróvio", 0, 0, new List<int> { 2 }, new List<int> { 2 }) },
        { 115, new ElementoInfo(115, "Mc", "Moscóvio", 0, 0, new List<int> { 1, 3 }, new List<int> { 3, 1 }) },
        { 116, new ElementoInfo(116, "Lv", "Livermório", 0, 0, new List<int> { 2 }, new List<int> { 2 }) },
        { 117, new ElementoInfo(117, "Ts", "Tenessino", 0, 0, new List<int> { 1 }, new List<int> { -1, 1, 3 }) },
        { 118, new ElementoInfo(118, "Og", "Oganessônio", 0, 0, new List<int> { 0 }, new List<int> { 0, 2, 4 }) }
    };
}
    public static class Simbolos
    {
        public static readonly Dictionary<int, string> Elemento =
            Elementos.Tabela.ToDictionary(kv => kv.Key, kv => kv.Value.Simbolo);
    }
}

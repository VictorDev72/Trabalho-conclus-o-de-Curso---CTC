public class LigacaoMolecular
{
    public AtomoMolecular A { get; private set; }

    public AtomoMolecular B { get; private set; }

    // 1 = simples
    // 2 = dupla
    // 3 = tripla
    public int Ordem { get; set; }

    public LigacaoMolecular(
        AtomoMolecular a,
        AtomoMolecular b,
        int ordem = 1)
    {
        A = a;
        B = b;
        Ordem = ordem;
    }

    public AtomoMolecular Outro(
        AtomoMolecular atomo)
    {
        if (atomo == A)
            return B;

        return A;
    }
}
public class LigacaoMolecular
{
    public AtomoMolecular A { get; private set; }
    public AtomoMolecular B { get; private set; }
    public int Ordem { get; private set; }

    public LigacaoMolecular(AtomoMolecular a, AtomoMolecular b, int ordem = 1)
    {
        if (a == null || b == null) throw new System.ArgumentNullException("Os átomos da ligação não podem ser nulos.");
        if (a == b) throw new System.ArgumentException("Um átomo não pode se ligar a si mesmo.");
        if (ordem < 1 || ordem > 3) throw new System.ArgumentOutOfRangeException("ordem", "A ordem deve ser 1, 2 ou 3.");
        A = a; B = b; Ordem = ordem;
    }

    public void DefinirOrdem(int ordem)
    {
        if (ordem < 1 || ordem > 3) throw new System.ArgumentOutOfRangeException("ordem");
        Ordem = ordem;
    }

    public AtomoMolecular Outro(AtomoMolecular atomo)
    {
        if (atomo == A) return B;
        if (atomo == B) return A;
        throw new System.ArgumentException("O átomo informado não pertence a esta ligação.");
    }
}

Conversa aberta. 1 mensagem lida.

Pular para o conteúdo
Como usar o E-mail de Unicamp com leitores de tela

9 de 846
Arquivo txt pro meu amigo
Caixa de entrada

JOAO VICTOR CUSSOLIM <cc24136@g.unicamp.br>
qua., 11 de mar., 19:50 (há 5 dias)
para mim

Parece que esta mensagem está em inglês
public static class ChemicalEquationBalancer
{
    public static int[] Balance(
        List<Dictionary<string, int>> reactants,
        List<Dictionary<string, int>> products)
    {
        var allMolecules = new List<Dictionary<string, int>>();
        allMolecules.AddRange(reactants);
        allMolecules.AddRange(products);

        var elements = allMolecules
            .SelectMany(m => m.Keys)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        int rows = elements.Count;
        int cols = allMolecules.Count;

        Fraction[,] matrix = new Fraction[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            string element = elements[i];

            for (int j = 0; j < reactants.Count; j++)
            {
                int count = reactants[j].ContainsKey(element) ? reactants[j][element] : 0;
                matrix[i, j] = new Fraction(count);
            }

            for (int j = 0; j < products.Count; j++)
            {
                int count = products[j].ContainsKey(element) ? products[j][element] : 0;
                matrix[i, reactants.Count + j] = new Fraction(-count);
            }
        }

        return SolveHomogeneousSystem(matrix);
    }

    private static int[] SolveHomogeneousSystem(Fraction[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        Fraction[,] rref = (Fraction[,])matrix.Clone();
        List<int> pivotCols = new();

        int pivotRow = 0;

        for (int col = 0; col < cols && pivotRow < rows; col++)
        {
            int bestRow = -1;
            for (int r = pivotRow; r < rows; r++)
            {
                if (!rref[r, col].IsZero)
                {
                    bestRow = r;
                    break;
                }
            }

            if (bestRow == -1)
                continue;

            SwapRows(rref, pivotRow, bestRow);

            Fraction pivot = rref[pivotRow, col];
            for (int c = 0; c < cols; c++)
                rref[pivotRow, c] = rref[pivotRow, c] / pivot;

            for (int r = 0; r < rows; r++)
            {
                if (r == pivotRow) continue;
                if (rref[r, col].IsZero) continue;

                Fraction factor = rref[r, col];
                for (int c = 0; c < cols; c++)
                    rref[r, c] = rref[r, c] - factor * rref[pivotRow, c];
            }

            pivotCols.Add(col);
            pivotRow++;
        }

        var freeCols = Enumerable.Range(0, cols).Except(pivotCols).ToList();
        if (freeCols.Count == 0)
            throw new InvalidOperationException("Sistema só possui solução trivial.");

        Fraction[] solution = Enumerable.Repeat(Fraction.Zero, cols).ToArray();

        int freeCol = freeCols.Last();
        solution[freeCol] = Fraction.One;

        for (int i = pivotCols.Count - 1; i >= 0; i--)
        {
            int col = pivotCols[i];
            Fraction sum = Fraction.Zero;

            for (int c = col + 1; c < cols; c++)
                sum += rref[i, c] * solution[c];

            solution[col] = -sum;
        }

        return NormalizeToIntegers(solution);
    }

    private static int[] NormalizeToIntegers(Fraction[] solution)
    {
        BigInteger lcm = 1;

        foreach (var f in solution)
            lcm = Lcm(lcm, f.Den);

        BigInteger[] integers = solution
            .Select(f => f.Num * (lcm / f.Den))
            .ToArray();

        BigInteger gcd = integers
            .Where(x => x != 0)
            .Aggregate((a, b) => BigInteger.GreatestCommonDivisor(BigInteger.Abs(a), BigInteger.Abs(b)));

        for (int i = 0; i < integers.Length; i++)
            integers[i] /= gcd;

        bool hasNegative = integers.Any(x => x < 0);
        if (hasNegative)
        {
            for (int i = 0; i < integers.Length; i++)
                integers[i] = -integers[i];
        }

        return integers.Select(x => (int)x).ToArray();
    }

    private static void SwapRows(Fraction[,] matrix, int r1, int r2)
    {
        if (r1 == r2) return;

        int cols = matrix.GetLength(1);
        for (int c = 0; c < cols; c++)
        {
            (matrix[r1, c], matrix[r2, c]) = (matrix[r2, c], matrix[r1, c]);
        }
    }

    private static BigInteger Lcm(BigInteger a, BigInteger b)
    {
        return BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);
    }
}

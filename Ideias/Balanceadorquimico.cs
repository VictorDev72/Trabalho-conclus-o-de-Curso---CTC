using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace Ideias
{
    public class EquacaoQuimica
    {
        public List<Dictionary<int, int>> Reagentes;
        public List<Dictionary<int, int>> Produtos;

        public EquacaoQuimica()
        {
            Reagentes = new List<Dictionary<int, int>>();
            Produtos = new List<Dictionary<int, int>>();
        }

        public List<int> Balancear()
        {
            var atomos = IdentificaAtomosUnicos(Reagentes, Produtos);

            var matrizInt = MontarMatriz(atomos);

            // converter para Fracao
            int rows = matrizInt.Count;
            int cols = matrizInt[0].Count;

            Fracao[,] matriz = new Fracao[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matriz[i, j] = new Fracao(matrizInt[i][j], 1);

            return ResolverMatriz(matriz);
        }

        public List<int> IdentificaAtomosUnicos(List<Dictionary<int, int>> reag, List<Dictionary<int, int>> prod)
        {
            List<int> atomosUnicos = new List<int>();
            foreach (var dici in reag)
            {
                foreach(var item in dici)
                {
                    if (!atomosUnicos.Contains(item.Key))
                        atomosUnicos.Add(item.Key);
                }
            }
            foreach (var dici in prod)
            {
                foreach (var item in dici){
                if (!atomosUnicos.Contains(item.Key))
                    atomosUnicos.Add(item.Key);                       
                }
            }
            return atomosUnicos;
        }

        public List<List<int>> MontarMatriz(List<int> atomosUnicos){   
  
            List<List<int>> matriz = new List<List<int>>();

            for (int i = 0; i < atomosUnicos.Count; i++)
            {
                
                List<int> linha = new List<int>();
                foreach (var r in Reagentes)
                    linha.Add(r.ContainsKey(atomosUnicos[i]) ? r[atomosUnicos[i]] : 0);

                foreach (var p in Produtos)
                    linha.Add(-(p.ContainsKey(atomosUnicos[i]) ? p[atomosUnicos[i]] : 0));
                

                matriz.Add(linha);
            }

            return matriz;
        }
        public List<int> ResolverMatriz(Fracao[,] matriz)
        {
            int rows = matriz.GetLength(0);
            int cols = matriz.GetLength(1);
            List<int> pivoCols = new();
            Fracao[,] referencia = (Fracao[,]) matriz.Clone();

            int pivoRow = 0;
            for (int col = 0; col < cols && pivoRow < rows; col++)
            {
                int bestRow = -1;
                for (int linha = pivoRow; linha < rows; linha++)
                {
                    
                    if (!referencia[linha, col].IsZero)
                    {
                        bestRow = linha;
                        break;
                    }
                }

                if (bestRow == -1)
                    continue;

                TrocaLinha(referencia, pivoRow, bestRow);

                Fracao pivo = referencia[pivoRow, col];
                for (int c = 0; c < cols; c++)
                {
                    referencia[pivoRow, c] = referencia[pivoRow, c] / pivo;
                }
                for (int r = 0; r < rows; r++)
                {
                    if (r == pivoRow) continue;
                    if (referencia[r, col].IsZero) continue;

                    Fracao factor = referencia[r, col];
                    for (int c = 0; c < cols; c++)
                        referencia[r, c] = referencia[r, c] - factor * referencia[pivoRow, c];
                }

                pivoCols.Add(col);
                pivoRow++;
            }

            var variaveisLivres = Enumerable.Range(0, cols).Except(pivoCols).ToList();
            if (variaveisLivres.Count == 0)
            {
                throw new InvalidOperationException("a matriz so tem solução trivial");
            }

            Fracao[] solucao = Enumerable.Repeat(Fracao.Zero, cols).ToArray();

            int varLivre = variaveisLivres.Last();
            solucao[varLivre] = Fracao.One;
            for (int i = pivoCols.Count - 1; i >= 0; i--)
            {
                int col = pivoCols[i];
                Fracao soma = Fracao.Zero;

                for (int c = col + 1; c < cols; c++)
                    soma += referencia[i, c] * solucao[c];

                solucao[col] = -soma;
            }

            return NaturalizaInteitros(solucao);
        }
        private void TrocaLinha(Fracao[,] matriz, int original, int troca)
        {
            List<Fracao> beckup = new List<Fracao>();
            for (int i = 0; i < matriz.GetLength(1); i++)
            {
                beckup.Add(matriz[original,i]);
            }
            for(int t = 0;t < matriz.GetLength(1); t++)
            {
                matriz[original,t] = matriz[troca,t];
                matriz[troca,t] = beckup[t];
            }
        }
        private List<int> NaturalizaInteitros(Fracao[] modelo)
        {
            //cria os resultados inteiros
            BigInteger mmc = 1;
            List<int> resultado = new List<int>();
            List<int> res = new List<int>();
            foreach (var item in modelo)            {
                mmc = MMC(mmc, item.Denominator);
            }
            foreach (var f in modelo)
                res.Add((int)(f.Numerator * (mmc / f.Denominator)));
            

            //cria os resultados absolutos
            BigInteger mdc = 1;
            foreach (var item in res)            {
                if (item != 0)
                    mdc = BigInteger.GreatestCommonDivisor(mdc, BigInteger.Abs(item));

            }
            foreach (var f in modelo)
                resultado.Add((int)(f.Numerator * (mmc / f.Denominator)) / (int)mdc);
            for (int i = 0; i < resultado.Count; i++)
            {
                resultado[i] /= (int)mdc;
                if(resultado[i] < 0)
                {
                    resultado[i] = -resultado[i];
                }
            }
              
             return resultado;
        }
        private BigInteger MMC(BigInteger x,BigInteger y)
        {
            return BigInteger.Abs(x*y)/BigInteger.GreatestCommonDivisor(x,y);
        }

    }
}

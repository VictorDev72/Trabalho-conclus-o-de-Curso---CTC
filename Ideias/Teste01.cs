using System;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Balanceador
{
    class Elemento
    {
        public int Z { get; set; }//Numero Atomico se representa com Z
        public float A { get; set; }//Numero de massa se representa com A
        public int E { get; set; }//Numero de Eletrons se representa com E
        public double Raio { get; set; }
        public int Quantidade { get; set; }
        public float Eletronegatividade { get; set; }

        public Elemento(int z, float a, int e, double raio, int quantidade, float eletronegatividade)
        {
            Z = z;
            A = a;
            E = e;
            Raio = raio;
            Quantidade = quantidade;
            Eletronegatividade = eletronegatividade;
        }

        public void ionizar(int valor)
        {
            E += valor;
        }
        public void mudarQuantidade(int valor)
        {
            Quantidade += valor;
        }
    }

    class Molecula
    {

        public List<Elemento> Elementos { get; set; }
        public int Coeficiente { get; set; } // número na frente (ex: 2H2O)

        public Molecula()
        {
            Elementos = new List<Elemento>();
            Coeficiente = 1;
        }
        public void AdicionarElemento(Elemento elemento)
        {
            Elementos.Add(elemento);
        }

        public int contarAtomos(int nZ)
        {
            int numAtomos = 0;

            foreach (var i in Elementos)
            {
                if (i.Z == nZ) numAtomos += i.Quantidade * Coeficiente;
            }
            return numAtomos;
        }
    }

    class ParteEq
    {
        public List<Molecula> Moleculas { get; set; }

        public ParteEq()
        {
            Moleculas = new List<Molecula>();
        }

        public void AdicionarMolecula(Molecula M)
        {
            Moleculas.Add(M);
        }

        public int contarAtomos(int Z)
        {
            int atomos = 0;
            foreach (var mol in Moleculas)
            {
                atomos += mol.contarAtomos(Z);
            }
            return atomos;
        }


    }

    class EquacaoQuimica
    {
        public ParteEq Reagente;
        public ParteEq Produto;

        public EquacaoQuimica()
        {
            Reagente = new ParteEq();
            Produto = new ParteEq();
        }

        public List<int> IndentificaAtomosUnicos(ParteEq reag, ParteEq prod)
        {
            List<int> atomosUnicos = new List<int>();
            foreach (var item in reag.Moleculas)
            {
                foreach (var itemP in item.Elementos)
                {
                    if (!atomosUnicos.Contains(itemP.Z))
                    {
                        atomosUnicos.Add(itemP.Z);
                    }
                }
            }
            foreach (var item in prod.Moleculas)
            {
                foreach (var itemP in item.Elementos)
                {
                    if (!atomosUnicos.Contains(itemP.Z))
                    {
                        atomosUnicos.Add(itemP.Z);
                    }
                }
            }

            return atomosUnicos;
        }
        public List<List<int>> ObtemMatriz(List<int> atomosUnicos)
        {
            List<List<int>> matriz = new List<List<int>>();

            foreach (var atomo in atomosUnicos)
            {

                List<int> linha = new List<int>();
                foreach (var reagente in Reagente.Moleculas)
                {
                    linha.Add(reagente.contarAtomos(atomo));
                }
                foreach (var produto in Produto.Moleculas)
                {
                    linha.Add(-produto.contarAtomos(atomo));
                }

                matriz.Add(linha);
            }

            return matriz;
        }
        public List<int> ResolverMatriz(Fracao[,] matriz)
        {
            int rows = matriz.GetLength(0);
            int cols = matriz.GetLength(1);
            List<int> pivoCols = new();
            Fracao[,] referencia = matriz.Clone();

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
                for (int c = 0; c > cols; c++)
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

            var varLivre = Enumerable.Range(0, cols).Except(pivoCols).ToList();
            if (varLivre.Count == 0)
            {
                throw new InvalidOperationException("a matriz so tem solução trivial");
            }

            Fracao[] solucao = Enumerable.Repeat(Fracao.Zero, cols).ToArray();
        }


    

        /*Hoje 9 de março em campinas-COTUCA-LabLapa as 17:00 Rafael Schmal
        fiz o esqueleto de um programa que balanceia equaçoes
        tentei fazer com C# em POO com as classes Elemento, Molecula, ParteEq e EquaçãoQuimica
        a ideia é ter os metodos que serao acessados pela interação do usuario
        em que os Elementos ja vao ser definidos previamente com a tabela periodica ou banco de dados;
        e as Moleculas tambem; o que possibilitaria o facil acesso as funcionalidades;

        18:30 - ADD Vitor Ohland por Watszap
        fiz todo o esqueleto agora passei para os metodos vou fazer o metodo ContarAtomo para Elemento,molecula e ParteQe;
        fazer o metodo indentificarAtomosUnicos para Equaçãoquimica porque tive a idaia de Balanciar por matriz e resolver a matriz por gauss
        {
            gera lista de elementos únicos

            gera matriz

            resolve sistema

            define coeficientes das moléculas
        }
        H2O + CO2 -> H2CO3
        [2,0,-2]
        [1,2,-3]
        [0,1,-1]
        
        */

        //vai pegar um input de uma equação e transformala nas classes -X
        //nao seria mais facil so fazer direto do input?-X
        //armazenar todos os atomos nesse formato?-X
        //armazenar as moleculas uteis e ir adicionando com o tempo??-X
        //ter as reaçoes ja montadas??-X
        //Gerar por matriz, classificar todos os elementos unicos e montar sistema de equação 
        /*
            gera lista de elementos únicos

            gera matriz

            resolve sistema

            define coeficientes das moléculas
        */
        private void TrocaLinha(Fracao[,] matriz, int original, int troca)
        {

        }

    }
    
    public class Fracao
    {

    }

}


using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
            if(Quantidade >= 0) { Quantidade += valor; }
            else {Console.WriteLine("Quantidade de átomos não pode ser negativa.");}
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
        public List<List<double>> ObtemMatriz(List<int> atomosUnicos)
        {
            List<List<double>> matriz = new List<List<double>>();

            foreach (var atomo in atomosUnicos)
            {

                List<double> linha = new List<double>();
                foreach (var reagente in Reagente.Moleculas) 
                {
                    linha.Add(reagente.contarAtomos(atomo));
                }
                foreach (var produto in Produto.Moleculas) 
                {
                    linha.Add(produto.contarAtomos(atomo));
                }

                matriz.Add(linha);
            }

            return matriz;
        }


        /*Hoje 9 de março em campinas-COTUCA-LabLapa as 17:00 Rafael Schmal
        fiz o esqueleto de um programa que balanceia equaçoes
        tentei fazer com C# em POO com as classes Elemento, Molecula, ParteEq e EquaçãoQuimica
        a ideia é ter os metodos que serao acessados pela interação do usuario
        em que os Elementos ja vao ser definidos previamente com a tabela periodica ou banco de dados;
        e as Moleculas tambem; o que possibilitaria o facil acesso as funcionalidades;

        18:30 - ADD Vitor Ohland por Watszap
        fiz todo o esqueleto agora passei para os metodos vou fazer o metodo ContarAtomo para Elemento,molecula e ParteQe;
        fazer o metodo indentificarAtomosUnicos para EquaçãoQuimica porque tive a idaia de Balanciar por matriz e resolver a matriz por gauss
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

        /*
        hoje as 00:20 Victor fernandes
        fiz algumas alterações no codigo, coloquei .Moleculas para acessar a lista de moleculas dentro da classe ParteEq, pórem ainda a duvidas se funciona;
        bom tava pensando em realmente deixar completo a lista de atomos, pois acredito eu q ficaria mais facil;
        li o codigo é tenho algumas duvidas e muito a esclarecer, mudei algumas coisas como:

        .Moleculas para acessar a lista de moleculas dentro da classe ParteEq, pórem ainda a duvidas se funciona;
        troquei o tipo da matriz para double, pois acredito que seja mais facil de resolver o sistema de equação depois;
        coloquei um if para evitar que a quantidade de átomos fique negativa, pois isso não faz sentido;
        começei a fazer a matriz de gauss jordan;

        amanha possivelmente vou terminar a matriz de gauss jordan;

        Encerro aqui as 1:37 pm
         */

    }
}

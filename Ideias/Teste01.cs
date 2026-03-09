using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Balanceador
{
  class Elemento{
    private int z;//Numero Atomico se representa com Z
    private float a;//Numero de massa se representa com Z
    private int e;//Numero de Eletrons se representa com Z
    private double raio;
    private int quantidade;//O2
    private float eletronegatividade;


    public int Z {get;set;}
    public float A {get;set;}
    public int E {get;set;}
    public double Raio {get;set;}
    public int Quantidade {get;set;}
    public float Eletronegatividade{get;set;}

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
        private int coeficiente;
        private List<Elemento> elementos;
        
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

            foreach(var i in elementos)
            {
                if(i.Z == nZ)numAtomos+= i.Quantidade * Coeficiente;
            }
            return numAtomos;
        } 
    }

    class ParteEq
    {
        public List<Molecula> Moleculas {get; set;}

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
            foreach(var mol in Moleculas)
            {
                atomos += mol.contarAtomos(Z);
            }
            return atomos;
        }


    }

    class EquacaoQuimica
    {
        public List<ParteEq> Reagente;
        public List<ParteEq> Produto;

        public EquacaoQuimica(){
            Reagente = new List<ParteEq>();
            Produto = new List<ParteEq>();
        }

        public List<int> IndentificaAtomosUnicos(ParteEq parte)
        {
            List<int> atomosUnicos = new List<int>(); 
            foreach (var item in parte.Moleculas)
            {
                foreach(var itemP in item.Elementos)
                {
                 if(!atomosUnicos.Contains(itemP.Z))
                    {
                        atomosUnicos.Add(itemP.Z);
                    } 
                }
            }

            return atomosUnicos;
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

    }
}

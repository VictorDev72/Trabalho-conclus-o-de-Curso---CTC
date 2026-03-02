using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Reagentes
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

    class Reagente
    {
        public List<Molecula> Moleculas {get; set;}

        public Reagente()
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
        //vai pegar um input de uma equação e transformala nas classes
        //nao seria mais facil so fazer direto do input?
        //armazenar todos os atomos nesse formato?
        //armazenar as moleculas uteis e ir adicionando com o tempo??
        //ter as reaçoes ja montadas??
        //Gerar por matriz, classificar todos os elementos unicos e montar sistema de equação 
        /*
            gera lista de elementos únicos

            gera matriz

            resolve sistema

            define coeficientes das moléculas
        */

    }
}

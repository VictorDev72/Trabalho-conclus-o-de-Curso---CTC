using System;

public class Fracao
{
    private int numerador;
    private int denominador;

    public Fracao(int numerador, int denominador)
    {
        if (denominador == 0)
            throw new Exception("Denominador não pode ser zero.");

        this.numerador = numerador;
        this.denominador = denominador;

        Simplificar();
    }

    public int Numerador => numerador;
    public int Denominador => denominador;

    // metodo para simplificar a fração

    private void Simplificar()
    {
        int mdc = MDC(Math.Abs(numerador), Math.Abs(denominador));

        numerador /= mdc;
        denominador /= mdc;
    }

    // máximo Divisor Comum

    private int MDC(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    // Soma

    public Fracao Somar(Fracao outra)
    {
        int num = numerador * outra.denominador + outra.numerador * denominador;
        int den = denominador * outra.denominador;

        return new Fracao(num, den);
    }

    // Subtração

    public Fracao Subtrair(Fracao outra)
    {
        int num = numerador * outra.denominador - outra.numerador * denominador;
        int den = denominador * outra.denominador;

        return new Fracao(num, den);
    }

    // Multiplicação

    public Fracao Multiplicar(Fracao outra)
    {
        return new Fracao(numerador * outra.numerador,denominador * outra.denominador);
    }

    // Divisão

    public Fracao Dividir(Fracao outra)
    {
        if (outra.numerador == 0)
            throw new Exception("Não pode dividir por zero.");

        return new Fracao(numerador * outra.denominador,denominador * outra.numerador);
    }

    public Fracao Clonar()
    {
        return new Fracao(this.denominador, this.numerador);
    }

    public override string ToString()
    {
        return $"{numerador}/{denominador}";
    }

}

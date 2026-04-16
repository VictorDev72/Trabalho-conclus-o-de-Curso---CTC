using System;
using System.Collections.Generic;

namespace Ideias
{
    class Program
{
    static void Main()
    {
        var eq = new EquacaoQuimica();

        // =========================
        // EXEMPLO: H2 + O2 → H2O
        // =========================

        // H2
        eq.Reagentes.Add(new Dictionary<int, int>
        {
            { 1, 2 } // Hidrogênio
        });

        // O2
        eq.Reagentes.Add(new Dictionary<int, int>
        {
            { 8, 2 } // Oxigênio
        });

        // H2O
        eq.Produtos.Add(new Dictionary<int, int>
        {
            { 1, 2 }, // Hidrogênio
            { 8, 1 }  // Oxigênio
        });

        try
        {
            var resultado = eq.Balancear();

            Console.WriteLine("=== Resultado ===");

            for (int i = 0; i < resultado.Count; i++)
            {
                Console.WriteLine($"Coeficiente {i}: {resultado[i]}");
            }

            Console.WriteLine("\nForma balanceada:");
            Console.WriteLine($"{resultado[0]}H2 + {resultado[1]}O2 -> {resultado[2]}H2O");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao balancear:");
            Console.WriteLine(ex.Message);
        }
    }
}
}

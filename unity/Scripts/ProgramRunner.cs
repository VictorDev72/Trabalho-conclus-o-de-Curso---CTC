using System;
using System.Collections.Generic;
using UnityEngine;
using Ideias;

public class ProgramRunner : MonoBehaviour
{
    public void Start()
    {
        ExecutarPrograma();
    }

    private void ExecutarPrograma()
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

            Debug.Log("=== Resultado ===");

            for (int i = 0; i < resultado.Count; i++)
            {
                Debug.Log($"Coeficiente {i}: {resultado[i]}");
            }

            Debug.Log("\nForma balanceada:");
            Debug.Log($"{resultado[0]}H2 + {resultado[1]}O2 -> {resultado[2]}H2O");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao balancear: {ex.Message}");
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Ideias;

public class UIReacao : MonoBehaviour
{
    public TMP_Dropdown dropPart1;
    public TMP_Dropdown dropPart2;

    private Dictionary<string, Dictionary<int, int>> banco = new Dictionary<string, Dictionary<int, int>>()
    {
        { "H2", new Dictionary<int, int> { {1, 2} } },
        { "O2", new Dictionary<int, int> { {8, 2} } },
        { "H2O", new Dictionary<int, int> { {1, 2}, {8, 1} } },
        { "CO2", new Dictionary<int, int> { {6, 1}, {8, 2} } },
        { "C", new Dictionary<int, int> { {6, 1} } }
    };

    private List<string> opcoes = new List<string>()
    {
        "H2", "O2", "H2O", "CO2", "C"
    };

    void Start()
    {
        dropPart1.ClearOptions();
        dropPart2.ClearOptions();

        dropPart1.AddOptions(opcoes);
        dropPart2.AddOptions(opcoes);
    }

    public void Reagir()
    {
        string r1 = dropPart1.options[dropPart1.value].text;
        string r2 = dropPart2.options[dropPart2.value].text;


        var eq = new EquacaoQuimica();

        eq.Reagentes.Add(banco[r1]);
        eq.Reagentes.Add(banco[r2]);

        //produto fixo
        eq.Produtos.Add(banco["H2O"]);

        var resultado = eq.Balancear();

        Debug.Log($"{resultado[0]}{r1} + {resultado[1]}{r2} -> {resultado[2]}H2O");
    }
}
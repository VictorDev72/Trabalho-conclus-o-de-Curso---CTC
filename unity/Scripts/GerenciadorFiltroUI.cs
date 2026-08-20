using System.Collections.Generic;
using UnityEngine;
using TMPro; // Substitua por UnityEngine.UI se usar o Dropdown legado

public class GerenciadorFiltroUI : MonoBehaviour
{
    [Header("Componentes UI")]
    public TMP_Dropdown dropdownTipoReacao;
    public TMP_Dropdown dropdownReagenteA;
    public TMP_Dropdown dropdownReagenteB;

    // Dicionário de banco de dados fornecido no seu código
    private Dictionary<string, DadosSubstancia> banco; 

    private void Start()
    {
        // Registra os ouvintes para atualizar o Dropdown B quando a reação ou Reagente A mudarem
        dropdownTipoReacao.onValueChanged.AddListener(delegate { AtualizarDropdownB(); });
        dropdownReagenteA.onValueChanged.AddListener(delegate { AtualizarDropdownB(); });

        PreencherDropdownsIniciais();
    }

    private void PreencherDropdownsIniciais()
    {
        dropdownReagenteA.ClearOptions();
        dropdownReagenteA.AddOptions(new List<string>(banco.Keys));
        dropdownReagenteA.RefreshShownValue();
        
        AtualizarDropdownB();
    }

    /// <summary>
    /// Reconstroem o Dropdown B filtrando apenas as opções válidas
    /// </summary>
    public void AtualizarDropdownB()
    {
        string tipoReacao = dropdownTipoReacao.options[dropdownTipoReacao.value].text;
        string formulaA = dropdownReagenteA.options[dropdownReagenteA.value].text;

        dropdownReagenteB.ClearOptions();
        List<string> opcoesValidasB = new List<string>();

        // 1. Caso Especial: Decomposição exige apenas 1 reagente
        if (tipoReacao == "Decomposição")
        {
            dropdownReagenteB.interactable = false;
            opcoesValidasB.Add("N/A (Apenas 1 Reagente)");
            dropdownReagenteB.AddOptions(opcoesValidasB);
            dropdownReagenteB.RefreshShownValue();
            return;
        }

        dropdownReagenteB.interactable = true;

        // 2. Itera sobre todo o banco validando quais substâncias podem reagir com 'A'
        foreach (var par in banco)
        {
            string formulaB = par.Key;

            if (EhCombinaçãoValida(tipoReacao, formulaA, formulaB))
            {
                opcoesValidasB.Add(formulaB);
            }
        }

        // Se não houver opções compatíveis no banco
        if (opcoesValidasB.Count == 0)
        {
            opcoesValidasB.Add("Sem combinação válida");
            dropdownReagenteB.interactable = false;
        }

        dropdownReagenteB.AddOptions(opcoesValidasB);
        dropdownReagenteB.RefreshShownValue();
    }

    /// <summary>
    /// Avalia a viabilidade química com base nos dados do seu banco
    /// </summary>
    private bool EhCombinaçãoValida(string tipoReacao, string formulaA, string formulaB)
    {
        DadosSubstancia subA = banco[formulaA];
        DadosSubstancia subB = banco[formulaB];

        // Uma substância é simples se NÃO possui 'Parte B' cadastrada
        bool aEhSimples = subA.GetParteB() == null || subA.GetParteB().Count == 0;
        bool bEhSimples = subB.GetParteB() == null || subB.GetParteB().Count == 0;

        switch (tipoReacao)
        {
            case "Combustão":
                // Regra: Reagente A é O2 e B é combustível, OU Reagente A é combustível e B é O2
                if (formulaA == "O2") 
                    return formulaB != "O2" && !EhIncombustivel(formulaB);
                return formulaB == "O2";

            case "Simples Troca":
                if ((aEhSimples && !bEhSimples) || (!aEhSimples && bEhSimples))
                {
                    string formulaSimples = aEhSimples ? formulaA : formulaB;
                    string formulaComposta = aEhSimples ? formulaB : formulaA;

                    // Extrai os átomos chaves do dicionário para testar
                    int atomoLivre = ObterAtomoPrincipal(banco[formulaSimples].GetParteA());
                    int atomoSubstituido = ObterAtomoPrincipal(banco[formulaComposta].GetParteA());

                    // Valida na Fila de Reatividade antes de permitir no Dropdown
                    return ValidadorQuimico.PodeOcorrerSimplesTroca(atomoLivre, atomoSubstituido, true);
                }
                return false;

            case "Dupla Troca":
                if (!aEhSimples && !bEhSimples)
                {
                    return ValidadorQuimico.PodeOcorrerDuplaTroca(subA, subB, formulaA, formulaB);
                }
                return false;

            case "Síntese":
                // Regra 1: Duas substâncias SIMPLES (Ex: Na + Cl2, H2 + O2)
                if (aEhSimples && bEhSimples)
                {
                    if (formulaA == formulaB) return false;
                    // Evita juntar dois Cátions metálicos (Ex: Na + K)
                    bool aEhCation = subA.GetCargaA() > 0;
                    bool bEhCation = subB.GetCargaA() > 0;
                    return !(aEhCation && bEhCation); 
                }
                // Regra 2: Síntese de Óxido + Água (Ex: SO3 + H2O -> H2SO4)
                if (formulaA == "H2O" || formulaB == "H2O")
                {
                    string outro = (formulaA == "H2O") ? formulaB : formulaA;
                    return EhOxido(outro);
                }
                return false;

            default:
                return false;
        }
    }

    private bool EhOxido(string formula)
    {
        // Verifica se a substância é um Óxido (Parte B possui Oxigênio)
        DadosSubstancia sub = banco[formula];
        return sub.GetParteB() != null && sub.GetParteB().ContainsKey(Atomos.O);
    }

    private bool EhIncombustivel(string formula)
    {
        // Evita tentar queimar produtos de combustão já oxidados
        return formula == "CO2" || formula == "H2O" || formula == "N2";
    }

}
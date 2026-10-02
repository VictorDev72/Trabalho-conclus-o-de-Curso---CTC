using UnityEngine;
using TMPro;

// Mostra no título da cena 3D a equação balanceada que a UIReacao
// deixou em ReacaoTransferData antes de trocar de cena.
[RequireComponent(typeof(TextMeshProUGUI))]
public class TituloReacao3D : MonoBehaviour
{
    private void Start()
    {
        if (string.IsNullOrEmpty(ReacaoTransferData.Equacao))
            return;

        GetComponent<TextMeshProUGUI>().text = ReacaoTransferData.Equacao;
    }
}

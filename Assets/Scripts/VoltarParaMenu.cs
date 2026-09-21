using UnityEngine;
using UnityEngine.SceneManagement;

// Componente simples para o botão "Voltar" da cena de visualização 3D.
public class VoltarParaMenu : MonoBehaviour
{
    public string cena = "SampleScene";

    public void Voltar()
    {
        SceneManager.LoadScene(cena);
    }
}

using System.Collections.Generic;

namespace BalanciadorQuimico
{
    /// <summary>
    /// Ponte de dados entre a cena de UI (onde o usuário monta a reação e clica em "Reagir")
    /// e a cena "CenaMoleculas" (que só existe para mostrar a visualização 3D).
    /// Campos estáticos sobrevivem à troca de cena, então não precisa de DontDestroyOnLoad
    /// nem de um singleton — UIReacao escreve aqui antes de trocar de cena, e
    /// GerenciadorVisual3D lê no Start() da cena nova.
    /// </summary>
    public static class ReacaoTransferData
    {
        public const string NomeCena = "CenaMoleculas";

        public static List<Dictionary<int, int>> Reagentes;
        public static List<Dictionary<int, int>> Produtos;
    }
}

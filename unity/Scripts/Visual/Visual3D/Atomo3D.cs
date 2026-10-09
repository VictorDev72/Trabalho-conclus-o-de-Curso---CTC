using UnityEngine;
using BalanciadorQuimico;

[RequireComponent(typeof(Renderer))]
public class Atomo3D : MonoBehaviour
{
    public int NumeroAtomicoAtual { get; private set; }
    public string SimboloAtual { get; private set; }
    public string NomeAtual { get; private set; }
    private Renderer _renderer;
    private static MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
        if (GetComponent<Collider>() == null) gameObject.AddComponent<SphereCollider>();
    }

    public void Inicializar(ElementoInfo info)
    {
        if (info == null) { Debug.LogError("Atomo3D recebeu ElementoInfo nulo."); return; }
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        NumeroAtomicoAtual = info.Z; SimboloAtual = info.Simbolo; NomeAtual = info.Nome;
        float escala = (float)info.RaioAtomico / 100f;
        if (escala <= 0.1f) escala = 0.55f;
        transform.localScale = Vector3.one * escala;
        Color cor = TabelaCPK.ObterCor(info.Z);
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_BaseColor", cor);
        _propBlock.SetColor("_Color", cor);
        _renderer.SetPropertyBlock(_propBlock);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BalanciadorQuimico;

// Monta a estrutura 3D (átomos, ligações e posições) de uma substância.
//
//  1. Compostos iônicos (sais, bases, óxidos metálicos): cada íon é montado
//     separadamente e os íons são dispostos lado a lado, sem ligação covalente
//     entre eles (evita estruturas como Fe–Fe–O no Fe2O3).
//  2. Moléculas cadastradas em EstruturasConhecidas: usa a estrutura de Lewis
//     (garante O=C=O, O=S=O, C=C, C≡C, anel do benzeno...).
//  3. Demais moléculas: regra de valência — monta o esqueleto, testa as
//     valências possíveis de cada elemento e distribui ligações múltiplas e
//     hidrogênios até que nenhum átomo fique com valência sobrando.
//
// A geometria segue o VSEPR considerando também os pares de elétrons livres
// (H2O angular, NH3 piramidal, SO2 angular).
public static class GeometriaMolecular
{
    private const float DistanciaBase = 1.35f;

    // Espaço livre entre íons vizinhos num composto iônico.
    private const float EspacoEntreIons = 0.6f;

    // ============================================================
    // ENTRADA PRINCIPAL
    // ============================================================

    public static EstruturaMolecular Criar(
        Dictionary<int, int> composicao)
    {
        if (composicao == null ||
            composicao.Count == 0)
        {
            throw new ArgumentException(
                "A composição da molécula está vazia.");
        }

        DadosSubstancia substancia =
            BancoSubstancias.PorFormula(composicao);

        if (substancia != null &&
            EhIonica(substancia))
        {
            return CriarIonica(substancia);
        }

        return CriarCovalente(
            composicao,
            ehIon: false);
    }

    // Cátion metálico ou amônio → composto iônico.
    // Ácidos (cátion H) e óxidos de ametais são moleculares.
    private static bool EhIonica(
        DadosSubstancia s)
    {
        if (!s.EhIonico)
            return false;

        int cation =
            ValidadorQuimico.ElementoDoIon(s.Cation);

        return (cation != 0 && ValidadorQuimico.EhMetal(cation)) ||
               ValidadorQuimico.EhAmonio(s.Cation);
    }

    private static EstruturaMolecular CriarCovalente(
        Dictionary<int, int> composicao,
        bool ehIon)
    {
        bool cadastrada = ehIon
            ? EstruturasConhecidas.TryObterIon(composicao, out var def)
            : EstruturasConhecidas.TryObterMolecula(composicao, out def);

        EstruturaMolecular estrutura = cadastrada
            ? CriarDaDefinicao(def)
            : CriarPorValencia(composicao);

        CalcularGeometria(estrutura);

        return estrutura;
    }

    // ============================================================
    // COMPOSTOS IÔNICOS
    // ============================================================

    private static EstruturaMolecular CriarIonica(
        DadosSubstancia s)
    {
        EstruturaMolecular cation =
            CriarCovalente(s.Cation, ehIon: true);

        EstruturaMolecular anion =
            CriarCovalente(s.Anion, ehIon: true);

        Centralizar(cation);
        Centralizar(anion);

        float raioCation = Raio(cation);
        float raioAnion = Raio(anion);

        int qtdCation = s.QtdCation;
        int qtdAnion = s.QtdAnion;

        EstruturaMolecular resultado =
            new EstruturaMolecular();

        if (qtdCation == 1 || qtdAnion == 1)
        {
            // Um íon no centro e os outros ao redor (CaCl2, Na2O, Al(OH)3).
            bool cationNoCentro =
                qtdCation == 1;

            EstruturaMolecular centro = cationNoCentro ? cation : anion;
            EstruturaMolecular vizinho = cationNoCentro ? anion : cation;
            int qtdVizinhos = cationNoCentro ? qtdAnion : qtdCation;

            float distancia =
                raioCation + raioAnion + EspacoEntreIons;

            resultado.Anexar(centro, Vector3.zero);

            foreach (Vector3 direcao in ObterDirecoes(qtdVizinhos).Take(qtdVizinhos))
                resultado.Anexar(vizinho, direcao * distancia);
        }
        else
        {
            // Fila alternada, começando pelo íon mais numeroso
            // (Fe2O3: O Fe O Fe O; Ca3(PO4)2: Ca PO4 Ca PO4 Ca).
            int restantesCation = qtdCation;
            int restantesAnion = qtdAnion;
            bool vezDoCation = qtdCation >= qtdAnion;

            float x = 0f;
            float raioAnterior = 0f;
            bool primeiro = true;

            while (restantesCation > 0 || restantesAnion > 0)
            {
                if (vezDoCation && restantesCation == 0) vezDoCation = false;
                if (!vezDoCation && restantesAnion == 0) vezDoCation = true;

                EstruturaMolecular unidade = vezDoCation ? cation : anion;
                float raio = vezDoCation ? raioCation : raioAnion;

                if (!primeiro)
                    x += raioAnterior + raio + EspacoEntreIons;

                resultado.Anexar(unidade, new Vector3(x, 0, 0));

                if (vezDoCation) restantesCation--;
                else restantesAnion--;

                raioAnterior = raio;
                primeiro = false;
                vezDoCation = !vezDoCation;
            }
        }

        return resultado;
    }

    private static void Centralizar(
        EstruturaMolecular estrutura)
    {
        if (estrutura.Atomos.Count == 0)
            return;

        Vector3 centro = Vector3.zero;

        foreach (AtomoMolecular atomo in estrutura.Atomos)
            centro += atomo.Posicao;

        centro /= estrutura.Atomos.Count;

        foreach (AtomoMolecular atomo in estrutura.Atomos)
            atomo.Posicao -= centro;
    }

    // Raio da estrutura centralizada, incluindo o tamanho das esferas.
    private static float Raio(
        EstruturaMolecular estrutura)
    {
        float raio = 0f;

        foreach (AtomoMolecular atomo in estrutura.Atomos)
        {
            raio = Mathf.Max(
                raio,
                atomo.Posicao.magnitude + RaioVisual(atomo.Info));
        }

        return raio;
    }

    // Mesmo cálculo de escala do Atomo3D (a escala é o diâmetro).
    private static float RaioVisual(
        ElementoInfo info)
    {
        float escala = (float)(info.RaioAtomico / 100f);

        if (escala <= 0.1f)
            escala = 1f;

        return escala / 2f;
    }

    // ============================================================
    // ESTRUTURA CADASTRADA
    // ============================================================

    private static EstruturaMolecular CriarDaDefinicao(
        EstruturasConhecidas.Definicao def)
    {
        EstruturaMolecular estrutura =
            new EstruturaMolecular();

        foreach (int z in def.Atomos)
            estrutura.AdicionarAtomo(z);

        foreach (var (a, b, ordem) in def.Ligacoes)
        {
            estrutura.AdicionarLigacao(
                estrutura.Atomos[a],
                estrutura.Atomos[b],
                ordem);
        }

        foreach (int[] anel in def.Aneis)
            estrutura.Aneis.Add(anel.Select(i => estrutura.Atomos[i]).ToList());

        return estrutura;
    }

    // ============================================================
    // REGRA DE VALÊNCIA
    // ============================================================

    private static EstruturaMolecular CriarPorValencia(
        Dictionary<int, int> composicao)
    {
        EstruturaMolecular estrutura =
            new EstruturaMolecular();

        composicao.TryGetValue(
            Atomos.H,
            out int quantidadeH);

        // Só hidrogênio: H2.
        if (composicao.All(p => p.Key == Atomos.H || p.Value <= 0))
        {
            for (int i = 0; i < quantidadeH; i++)
                estrutura.AdicionarAtomo(Atomos.H);

            for (int i = 0; i + 1 < quantidadeH; i += 2)
                estrutura.AdicionarLigacao(estrutura.Atomos[i], estrutura.Atomos[i + 1]);

            return estrutura;
        }

        foreach (KeyValuePair<int, int> item in composicao)
        {
            if (item.Key == Atomos.H)
                continue;

            for (int i = 0; i < item.Value; i++)
                estrutura.AdicionarAtomo(item.Key);
        }

        List<AtomoMolecular> pesados =
            estrutura.Atomos.ToList();

        ConstruirEsqueleto(
            estrutura,
            pesados);

        if (!DistribuirValencias(
                estrutura,
                pesados,
                quantidadeH))
        {
            DistribuirHidrogeniosSimples(
                estrutura,
                pesados,
                quantidadeH);
        }

        return estrutura;
    }

    // O elemento de maior valência forma a cadeia principal
    // (C–C–C...); os demais átomos pesados se ligam a ela,
    // um por átomo da cadeia sempre que possível.
    private static void ConstruirEsqueleto(
        EstruturaMolecular estrutura,
        List<AtomoMolecular> pesados)
    {
        int elementoCadeia =
            pesados
                .Select(a => a.Info)
                .OrderByDescending(MaiorValencia)
                .ThenBy(i => i.Eletronegatividade)
                .First().Z;

        List<AtomoMolecular> cadeia =
            pesados
                .Where(a => a.NumeroAtomico == elementoCadeia)
                .ToList();

        for (int i = 0; i < cadeia.Count - 1; i++)
        {
            estrutura.AdicionarLigacao(
                cadeia[i],
                cadeia[i + 1]);
        }

        Dictionary<AtomoMolecular, int> agregados =
            cadeia.ToDictionary(a => a, a => 0);

        foreach (AtomoMolecular atomo in pesados.Except(cadeia))
        {
            AtomoMolecular alvo =
                cadeia
                    .Where(c => Capacidade(c) > 0)
                    .OrderBy(c => agregados[c])
                    .ThenByDescending(Capacidade)
                    .FirstOrDefault()
                ?? estrutura.Atomos
                    .Where(c => c != atomo && Capacidade(c) > 0 && c.Ligacoes.Count > 0)
                    .OrderByDescending(Capacidade)
                    .FirstOrDefault()
                ?? cadeia[0];

            estrutura.AdicionarLigacao(alvo, atomo);

            if (agregados.ContainsKey(alvo))
                agregados[alvo]++;
        }
    }

    private static int Capacidade(
        AtomoMolecular atomo)
    {
        return MaiorValencia(atomo.Info) - atomo.OrdemTotal;
    }

    // Testa combinações de valência (ex.: S = 2, 4 ou 6) e, para cada uma,
    // promove ligações a dupla/tripla até sobrar exatamente a valência
    // dos hidrogênios. Retorna false se nenhuma combinação fecha.
    private static bool DistribuirValencias(
        EstruturaMolecular estrutura,
        List<AtomoMolecular> pesados,
        int quantidadeH)
    {
        List<int> elementos =
            pesados.Select(a => a.NumeroAtomico).Distinct().ToList();

        Dictionary<int, List<int>> opcoes =
            new Dictionary<int, List<int>>();

        foreach (int z in elementos)
        {
            int maiorGrau =
                pesados.Where(a => a.NumeroAtomico == z).Max(a => a.Ligacoes.Count);

            List<int> valencias =
                Elementos.Tabela[z].LigacoesPossiveis
                    .Where(v => v >= maiorGrau && v > 0)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList();

            if (valencias.Count == 0)
                valencias.Add(Math.Max(1, maiorGrau));

            opcoes[z] = valencias;
        }

        List<LigacaoMolecular> ligacoesPesadas =
            estrutura.Ligacoes.ToList();

        foreach (Dictionary<int, int> combinacao in Combinacoes(elementos, opcoes).Take(500))
        {
            Dictionary<AtomoMolecular, int> livre =
                pesados.ToDictionary(
                    a => a,
                    a => combinacao[a.NumeroAtomico] - a.Ligacoes.Count);

            int sobra = livre.Values.Sum() - quantidadeH;

            if (livre.Values.Any(v => v < 0) || sobra < 0 || sobra % 2 != 0)
                continue;

            Dictionary<LigacaoMolecular, int> ordens =
                ligacoesPesadas.ToDictionary(l => l, l => 1);

            bool fechou = true;

            for (int i = 0; i < sobra / 2; i++)
            {
                LigacaoMolecular melhor =
                    ligacoesPesadas
                        .Where(l => ordens[l] < 3 && livre[l.A] > 0 && livre[l.B] > 0)
                        .OrderByDescending(l => livre[l.A] + livre[l.B])
                        .ThenByDescending(l => l.A.Info.Eletronegatividade + l.B.Info.Eletronegatividade)
                        .FirstOrDefault();

                if (melhor == null)
                {
                    fechou = false;
                    break;
                }

                ordens[melhor]++;
                livre[melhor.A]--;
                livre[melhor.B]--;
            }

            if (!fechou)
                continue;

            foreach (var par in ordens)
                par.Key.Ordem = par.Value;

            foreach (AtomoMolecular atomo in pesados)
            {
                for (int i = 0; i < livre[atomo]; i++)
                {
                    estrutura.AdicionarLigacao(
                        atomo,
                        estrutura.AdicionarAtomo(Atomos.H));
                }
            }

            return true;
        }

        return false;
    }

    // Produto cartesiano das valências de cada elemento, da menor soma para a maior.
    private static IEnumerable<Dictionary<int, int>> Combinacoes(
        List<int> elementos,
        Dictionary<int, List<int>> opcoes)
    {
        IEnumerable<Dictionary<int, int>> resultado =
            new[] { new Dictionary<int, int>() };

        foreach (int z in elementos)
        {
            resultado = resultado
                .SelectMany(parcial => opcoes[z].Select(v =>
                    new Dictionary<int, int>(parcial) { [z] = v }))
                .ToList();
        }

        return resultado.OrderBy(c => c.Values.Sum());
    }

    // Plano B: liga cada H ao átomo pesado com mais valência livre.
    private static void DistribuirHidrogeniosSimples(
        EstruturaMolecular estrutura,
        List<AtomoMolecular> pesados,
        int quantidadeH)
    {
        for (int i = 0; i < quantidadeH; i++)
        {
            AtomoMolecular alvo =
                pesados
                    .OrderByDescending(Capacidade)
                    .ThenBy(a => a.Ligacoes.Count)
                    .First();

            estrutura.AdicionarLigacao(
                alvo,
                estrutura.AdicionarAtomo(Atomos.H));
        }
    }

    private static int MaiorValencia(
        ElementoInfo info)
    {
        if (info.LigacoesPossiveis == null ||
            info.LigacoesPossiveis.Count == 0)
        {
            return 1;
        }

        return info.LigacoesPossiveis
            .Where(v => v > 0)
            .DefaultIfEmpty(1)
            .Max();
    }

    // ============================================================
    // GEOMETRIA
    // ============================================================

    private static void CalcularGeometria(
        EstruturaMolecular estrutura)
    {
        Contexto ctx =
            new Contexto(estrutura);

        HashSet<AtomoMolecular> posicionados =
            ctx.Posicionados;

        Queue<AtomoMolecular> fila =
            ctx.Fila;

        // Primeiro anel: polígono regular no plano XY (de frente para a câmera).
        // Os demais anéis são posicionados quando a busca chega neles.
        if (estrutura.Aneis.Count > 0)
        {
            PosicionarAnel(
                ctx,
                estrutura.Aneis[0],
                inicio: null,
                direcao: Vector3.right);

            ProcessarFila(ctx);
        }

        // Percorre cada parte conectada, começando pelo
        // átomo com mais ligações.
        while (posicionados.Count <
               estrutura.Atomos.Count)
        {
            AtomoMolecular raiz =
                estrutura.Atomos
                    .Where(a =>
                        !posicionados.Contains(a))
                    .OrderByDescending(
                        a => a.Ligacoes.Count)
                    .First();

            float deslocamentoX =
                posicionados.Count == 0
                    ? 0f
                    : posicionados.Max(a => a.Posicao.x) + DistanciaBase * 2f;

            raiz.Posicao =
                new Vector3(deslocamentoX, 0, 0);

            posicionados.Add(raiz);
            fila.Enqueue(raiz);

            ProcessarFila(ctx);
        }
    }

    // Estado da busca em largura que posiciona os átomos.
    private class Contexto
    {
        public readonly HashSet<AtomoMolecular> Posicionados = new HashSet<AtomoMolecular>();
        public readonly Queue<AtomoMolecular> Fila = new Queue<AtomoMolecular>();
        public readonly Dictionary<AtomoMolecular, List<AtomoMolecular>> AnelDoAtomo =
            new Dictionary<AtomoMolecular, List<AtomoMolecular>>();

        public Contexto(EstruturaMolecular estrutura)
        {
            foreach (List<AtomoMolecular> anel in estrutura.Aneis)
                foreach (AtomoMolecular atomo in anel)
                    AnelDoAtomo[atomo] = anel;
        }
    }

    private static void ProcessarFila(
        Contexto ctx)
    {
        while (ctx.Fila.Count > 0)
        {
            PosicionarVizinhos(
                ctx.Fila.Dequeue(),
                ctx);
        }
    }

    // Posiciona um anel inteiro como polígono regular.
    // Sem "inicio": centrado na origem, no plano XY.
    // Com "inicio": o átomo de entrada fica num vértice e o anel
    // cresce na "direcao" (saindo do átomo que levou até ele).
    private static void PosicionarAnel(
        Contexto ctx,
        List<AtomoMolecular> anel,
        AtomoMolecular inicio,
        Vector3 direcao)
    {
        int n = anel.Count;

        float lado =
            DistanciaEntre(anel[0], anel[1]);

        float raio =
            lado / (2f * Mathf.Sin(Mathf.PI / n));

        int primeiro =
            inicio == null ? 0 : anel.IndexOf(inicio);

        Vector3 d = direcao.normalized;

        Vector3 perpendicular =
            Vector3.Cross(d, Vector3.forward);

        if (perpendicular.sqrMagnitude < 0.01f)
            perpendicular = Vector3.Cross(d, Vector3.up);

        perpendicular.Normalize();

        Vector3 centro =
            inicio == null
                ? Vector3.zero
                : inicio.Posicao + d * raio;

        for (int k = 0; k < n; k++)
        {
            AtomoMolecular atomo =
                anel[(primeiro + k) % n];

            float angulo = 2f * Mathf.PI * k / n;

            atomo.Posicao =
                centro +
                raio * (-d * Mathf.Cos(angulo) + perpendicular * Mathf.Sin(angulo));

            if (ctx.Posicionados.Add(atomo))
                ctx.Fila.Enqueue(atomo);
        }
    }

    // ============================================================
    // POSICIONAMENTO VSEPR
    // ============================================================

    // Distribui os vizinhos ainda sem posição nas direções VSEPR,
    // levando em conta os pares de elétrons livres do átomo central
    // e as direções já ocupadas por vizinhos posicionados.
    private static void PosicionarVizinhos(
        AtomoMolecular centro,
        Contexto ctx)
    {
        HashSet<AtomoMolecular> posicionados =
            ctx.Posicionados;

        List<AtomoMolecular> vizinhos =
            centro.Ligacoes
                .Select(l => l.Outro(centro))
                .ToList();

        List<AtomoMolecular> livres =
            vizinhos.Where(v => !posicionados.Contains(v)).ToList();

        if (livres.Count == 0)
            return;

        List<Vector3> ocupadas =
            vizinhos
                .Where(v => posicionados.Contains(v))
                .Select(v => (v.Posicao - centro.Posicao).normalized)
                .ToList();

        List<Vector3> escolhidas =
            new List<Vector3>();

        List<AtomoMolecular> vizinhosNoAnel =
            ctx.AnelDoAtomo.TryGetValue(centro, out var anelCentro)
                ? vizinhos.Where(v => anelCentro.Contains(v) && posicionados.Contains(v)).ToList()
                : new List<AtomoMolecular>();

        if (vizinhosNoAnel.Count == 2)
        {
            // Átomo de anel: substituintes saem para fora do anel.
            // Com 3 ligações (C do benzeno) a direção é a bissetriz externa;
            // com 4 (C da sacarose) os dois substituintes ficam acima e abaixo
            // do plano do anel, como num tetraedro.
            Vector3 a = (vizinhosNoAnel[0].Posicao - centro.Posicao).normalized;
            Vector3 b = (vizinhosNoAnel[1].Posicao - centro.Posicao).normalized;
            Vector3 fora = -(a + b).normalized;
            Vector3 normal = Vector3.Cross(a, b).normalized;

            List<Vector3> candidatas = vizinhos.Count <= 3
                ? new List<Vector3> { fora }
                : new List<Vector3>
                {
                    (fora * 0.577f + normal * 0.816f).normalized,
                    (fora * 0.577f - normal * 0.816f).normalized
                };

            List<Vector3> usadas = new List<Vector3>(ocupadas);

            for (int i = 0; i < livres.Count; i++)
            {
                Vector3 melhor =
                    candidatas
                        .OrderBy(d => usadas.Max(u => Vector3.Dot(d, u)))
                        .First();

                escolhidas.Add(melhor);
                usadas.Add(melhor);

                if (candidatas.Count > 1)
                    candidatas.Remove(melhor);
            }
        }
        else if (ocupadas.Count >= 2 && livres.Count == 1)
        {
            // Ex.: H do benzeno — aponta para fora, oposto aos vizinhos.
            Vector3 soma = Vector3.zero;

            foreach (Vector3 d in ocupadas)
                soma += d;

            escolhidas.Add(soma.sqrMagnitude > 0.0001f
                ? -soma.normalized
                : Vector3.forward);
        }
        else
        {
            Vector3[] direcoes =
                ObterDirecoes(
                    vizinhos.Count + ParesIsolados(centro));

            // Gira a figura para que uma direção aponte para o vizinho já posicionado.
            if (ocupadas.Count > 0)
            {
                Quaternion rotacao =
                    Quaternion.FromToRotation(direcoes[0], ocupadas[0]);

                for (int i = 0; i < direcoes.Length; i++)
                    direcoes[i] = rotacao * direcoes[i];
            }

            // Escolhe, para cada vizinho livre, a direção mais afastada das já usadas.
            List<Vector3> usadas = new List<Vector3>(ocupadas);
            List<Vector3> disponiveis = direcoes.ToList();

            for (int i = 0; i < livres.Count; i++)
            {
                Vector3 melhor =
                    disponiveis
                        .OrderBy(d => usadas.Count == 0 ? 0f : usadas.Max(u => Vector3.Dot(d, u)))
                        .First();

                escolhidas.Add(melhor);
                usadas.Add(melhor);
                disponiveis.Remove(melhor);

                if (disponiveis.Count == 0)
                    disponiveis = direcoes.ToList();
            }
        }

        for (int i = 0; i < livres.Count; i++)
        {
            livres[i].Posicao =
                centro.Posicao +
                escolhidas[i] *
                DistanciaEntre(
                    centro,
                    livres[i]);

            posicionados.Add(livres[i]);
            ctx.Fila.Enqueue(livres[i]);

            // Chegou num anel ainda não desenhado: posiciona o anel inteiro.
            if (ctx.AnelDoAtomo.TryGetValue(livres[i], out var anel) &&
                anel.Any(a => !posicionados.Contains(a)))
            {
                PosicionarAnel(
                    ctx,
                    anel,
                    inicio: livres[i],
                    direcao: escolhidas[i]);
            }
        }
    }

    // Pares de elétrons não ligantes do átomo (só importa para átomos centrais).
    private static int ParesIsolados(
        AtomoMolecular atomo)
    {
        if (atomo.Ligacoes.Count < 2)
            return 0;

        int eletrons =
            EletronsDeValencia(atomo.NumeroAtomico);

        return Math.Max(0, (eletrons - atomo.OrdemTotal) / 2);
    }

    // Elétrons na camada de valência (elementos representativos).
    private static int EletronsDeValencia(
        int z)
    {
        if (z <= 2) return z;
        if (z <= 10) return z - 2;
        if (z <= 18) return z - 10;
        if (z <= 20) return z - 18;
        if (z >= 31 && z <= 36) return z - 28;
        if (z >= 37 && z <= 38) return z - 36;
        if (z >= 49 && z <= 54) return z - 46;
        return 0; // metais de transição e demais: sem pares considerados
    }

    private static float DistanciaEntre(
        AtomoMolecular a,
        AtomoMolecular b)
    {
        float raioA =
            (float)a.Info.RaioAtomico /
            100f;

        float raioB =
            (float)b.Info.RaioAtomico /
            100f;

        float distancia =
            raioA +
            raioB +
            0.55f;

        return Mathf.Clamp(
            distancia,
            0.9f,
            2.2f);
    }

    // ============================================================
    // DIREÇÕES VSEPR
    // ============================================================

    private static Vector3[] ObterDirecoes(
        int quantidade)
    {
        switch (quantidade)
        {
            case 0:
            case 1:
                return new[]
                {
                    Vector3.right
                };

            case 2:
                // Linear — 180°
                return new[]
                {
                    Vector3.right,
                    Vector3.left
                };

            case 3:
                // Trigonal planar — 120°
                return CriarTrigonalPlanar();

            case 4:
                // Tetraédrica
                return CriarTetraedrica();

            case 5:
                // Bipiramidal trigonal
                return CriarBipiramidalTrigonal();

            case 6:
                // Octaédrica
                return CriarOctaedrica();

            default:
                return CriarDistribuicaoEsferica(
                    quantidade);
        }
    }

    private static Vector3[] CriarTrigonalPlanar()
    {
        Vector3[] resultado =
            new Vector3[3];

        for (int i = 0; i < 3; i++)
        {
            float angulo =
                i *
                120f *
                Mathf.Deg2Rad;

            resultado[i] =
                new Vector3(
                    Mathf.Cos(angulo),
                    Mathf.Sin(angulo),
                    0
                ).normalized;
        }

        return resultado;
    }

    private static Vector3[] CriarTetraedrica()
    {
        return new[]
        {
            new Vector3(1, 1, 1).normalized,
            new Vector3(1, -1, -1).normalized,
            new Vector3(-1, 1, -1).normalized,
            new Vector3(-1, -1, 1).normalized
        };
    }

    private static Vector3[] CriarBipiramidalTrigonal()
    {
        return new[]
        {
            Vector3.up,
            Vector3.down,
            new Vector3(1, 0, 0),
            new Vector3(-0.5f, 0, 0.8660254f),
            new Vector3(-0.5f, 0, -0.8660254f)
        };
    }

    private static Vector3[] CriarOctaedrica()
    {
        return new[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,
            Vector3.forward,
            Vector3.back
        };
    }

    private static Vector3[] CriarDistribuicaoEsferica(
        int quantidade)
    {
        Vector3[] resultado =
            new Vector3[quantidade];

        float goldenAngle =
            Mathf.PI *
            (3f - Mathf.Sqrt(5f));

        for (int i = 0;
             i < quantidade;
             i++)
        {
            float y =
                1f -
                (i /
                Mathf.Max(
                    1f,
                    quantidade - 1f)) *
                2f;

            float raio =
                Mathf.Sqrt(
                    Mathf.Max(
                        0f,
                        1f - y * y));

            float theta =
                goldenAngle * i;

            resultado[i] =
                new Vector3(
                    Mathf.Cos(theta) * raio,
                    y,
                    Mathf.Sin(theta) * raio
                ).normalized;
        }

        return resultado;
    }
}

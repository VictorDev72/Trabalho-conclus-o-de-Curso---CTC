# Guia de montagem da UI – UIReacao_Aprimorado

O script continua compatível com o fluxo existente: `GeradorDeProduto`, `EquacaoQuimica`, `ReacaoTransferData` e carregamento da cena 3D permanecem responsáveis pela lógica da reação.

## Hierarquia 

Canvas
- Background
- Header
  - Titulo
  - Subtitulo
- PainelReacao
  - LabelReagente1
  - DropdownReagente1
  - BotaoInverter
  - LabelReagente2
  - DropdownReagente2
  - LabelTipo
  - DropdownTipo
  - PreviewPanel
    - LabelPreview
    - TextoEquacao
  - StatusPanel
    - TextoStatus
  - Botoes
    - BotaoAleatorio
    - BotaoLimpar
    - BotaoReagir
- PainelHistorico
  - TituloHistorico
  - TextoHistorico
  - BotaoLimparHistorico
  - TextoContador

## Campos do Inspector

No objeto que possui `UIReacao_Aprimorado`:

- `Drop Reagente 1` → `DropdownReagente1`
- `Drop Reagente 2` → `DropdownReagente2`
- `Drop Reacao` → `DropdownTipo`
- `Btn Reagir` → `BotaoReagir`
- `Txt Status` → `TextoStatus`
- `Txt Equacao` → `TextoEquacao`
- `Txt Historico` → `TextoHistorico`
- `Txt Contador Reacoes` → `TextoContador`
- `Btn Inverter` → `BotaoInverter`
- `Btn Limpar` → `BotaoLimpar`
- `Btn Aleatorio` → `BotaoAleatorio`
- `Btn Historico Limpar` → `BotaoLimparHistorico`

## Layout 

Use `Horizontal Layout Group` e `Vertical Layout Group` em vez de posicionar cada componente manualmente.

Para o painel principal, uma largura entre 850 e 1100 px e uma altura entre 650 e 800 px funciona bem em 1080p.

Deixe o Preview da equação visualmente maior que os demais textos. O status deve ficar imediatamente abaixo dele para que erros não passem despercebidos.

O botão **Reagir** deve ser o elemento de maior destaque do painel. Os botões secundários devem ter tamanho menor.

## Comportamentos 

### Validação em tempo real
A combinação é conferida a cada alteração dos dropdowns. Combinações incompatíveis desabilitam `Btn Reagir` e mostram o motivo no `Txt Status`.

### Pré-visualização
`Txt Equacao` mostra os reagentes selecionados e indica o produto como `?` antes da execução. Depois do balanceamento, mostra a equação final.

### Inverter
`Btn Inverter` troca Reagente 1 e Reagente 2.

### Limpar
`Btn Limpar` restaura os valores iniciais.

### Aleatório
`Btn Aleatorio` escolhe dois reagentes diferentes e um tipo de reação aleatoriamente.

### Histórico
Cada reação executada pode ser registrada em `Txt Historico`, com limite configurável por `Limite Historico`.

### Contador
`Txt Contador Reacoes` mostra quantas reações foram executadas desde o carregamento da cena.

## Observação importante

O script não cria automaticamente os objetos visuais, cores, fontes ou animações da cena. A parte visual precisa ser montada no Canvas do Unity. O código fornece os pontos de ligação e os comportamentos para essa interface.

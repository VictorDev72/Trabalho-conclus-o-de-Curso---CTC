print('esse é um programa para solucionar sistemas lineares de equações por meio de matriz ')

def obtemMatriz(documento):
    arquivo = open(documento,'r')#abre o arquivo
    matriz=[]

    numlin = 0 
    while True:

        linha = arquivo.readline()#ler linha do arquivo
        if linha == '': break#para de ler o arquivo em uma linha vasia
        #print(linha)
        parte = linha.split()#transforma a linha em lista
        print(parte)
        matriz.append([])#cria uma lista vasia
        numcol=0
        while numcol < len(parte):#enquanto a coluna estiver no range da lista coloca coisa na lista
            try:
                matriz[numlin].append(float(parte[numcol]))
            except ValueError:
                raise ValueError ('A matriz não tem somente numeros, logo não é valida')#tem um valor nao numerico na lista
                
            #print(matriz)
            numcol += 1
        numlin += 1

    arquivo.close()

    if  len(matriz)<2: raise ValueError ('A matriz não pode ter uma linha só')#verifica se a matrz tem 1 linha só

    lin = 0 
    while lin < len(matriz):# verifica se todas as linhas tem o mesmo tamanho 
        lin2 = lin+1
        
        while lin2 < len(matriz):

            if len(matriz[lin]) != len(matriz[lin2]): raise ValueError ('Linhas com numero de colunas diferentes')
                

            lin2 +=1
        lin += 1


    if len(matriz) < len(matriz[0])-1:
        raise ValueError ('Linhas insuficientes, não é um quadrado perfeito')
    elif len(matriz) > len(matriz[0])-1:    
        raise ValueError ('Muitas linhas, não é um quadrado perfeito')

    print()
    return matriz

def tirar_0_dos_pretos(matriz_des):
    posicao = 0
    for i in matriz_des:# para cada linha da matriz 
        if matriz_des[posicao] [posicao] == 0:# ou i[posicao]/numero do quadrado perfeito (1,1),(2,2)
            if posicao == len(matriz_des)-1:# estou na ultima linha?
                troca = 0 #se sim a linha que vai ser trocada é a primeira
            else:
                troca = posicao+1 #se nao é a prosima linha 
            while matriz_des[posicao] [posicao] == 0 and troca < len(matriz_des):#troca as linhas
                
                backup = matriz_des[posicao]
                matriz_des[posicao] = matriz_des[troca]
                matriz_des[troca] = backup
                troca += 1

            #print(matriz)
        posicao += 1  
    posicao = 0   
    for i in matriz_des:# para cada linha da matriz 
        if matriz_des[posicao] [posicao] == 0:# se continuar zero na diagonal da erro
            raise ValueError ('Não é posivel calcular esse sistema de equações porque não é possivel tirar os 0s da diagonal.......... Tente outro sistema de equações')
        posicao += 1 
    #print(matriz)
    #print(matriz_des)   
    return matriz_des

def verificaMatriz(matriz_ver):
    
    lin1 = 0 
    while lin1 < len(matriz_ver):#para cada linha da matriz
        lin2 = lin1+1
        
        while lin2<len(matriz_ver):#paracada linha da matriz ver todas as linhas depois dela
            divisao = []
            coluna = 0

            while coluna<len(matriz_ver)+1:
                try:
                    x = matriz_ver[lin1][coluna]/matriz_ver[lin2][coluna]#divide cada elemento entre si
                except ZeroDivisionError:
                    x = divisao.append(0.0001)
                else:
                    divisao.append(x)
                #print(divisao)
                coluna += 1

            if divisao.count(divisao[0]) == len(divisao):#verifica se toda a linha é igual
                
                return False
            
            lin2 += 1
        lin1 += 1 
    return True


def resto_vira_0_cima(mat,nl,col):
    lin = nl-1
    while lin > -1: # começa na linha e vai para as anteriores
        c = 0 #para preservar o numero original para repetição
        listaNegativa = []#armazena a lista que sera descartada
        negativo = -mat[lin][col]#numero que zera os bagui
        for x in mat[nl]:# faz a lista negativa
            x *= negativo
            #listaNegativa.append(x * negativo)
            listaNegativa.append(x)
        for x in mat[lin]:# Atualiza a lista com o numero zerado 
            x += listaNegativa[c]
            mat[lin][c] = x
            c += 1
        #print(mat)
        lin -= 1

def resto_vira_0_baixo(mat,nl,col): 
    lin = nl+1
    while lin < len(mat): # começa na linha e vai para baixo 
        c = 0 #para preservar o numero original para repetição
        listaNegativa = []#armazena a lista que sera descartada
        negativo = -mat[lin][col]#numero que zera os bagui
        for x in mat[nl]:# faz a lista negativa
            x *= negativo
            #listaNegativa.append(x * negativo)
            listaNegativa.append(x)
        for x in mat[lin]:# Atualiza a lista com o numero zerado 
            x += listaNegativa[c]
            mat[lin][c] = x
            c += 1
        #print(mat)
        lin += 1
        
    



def poe1numaLinhaDaRegiaoPretav2(mat):
    nroDaLinha = 0 
    for i in mat:

        coluna=0
        divisor=mat[nroDaLinha][nroDaLinha]

        while coluna<len(mat)+1:
            mat[nroDaLinha][coluna]/=divisor
            coluna+=1

        resto_vira_0_baixo(mat,nroDaLinha,nroDaLinha)#pegar a linha e a coluna para tirar o resto dos zeros para cada vez que transformar em 1
        resto_vira_0_cima(mat,nroDaLinha,nroDaLinha)
        nroDaLinha += 1
    #print(mat)
    return mat

def calcula(doc,xingamento_para_linhafalsa):
    try:
        mat = obtemMatriz(doc)
    except ValueError as erro:
        print(str(erro))
    else:   
        if not verificaMatriz(mat):
            print(xingamento_para_linhafalsa)
        
        else:
            try:
                matSem0noPreto = tirar_0_dos_pretos(mat)
            except ValueError as erro:
                print(str(erro))
            else:
                matriz_resposta = poe1numaLinhaDaRegiaoPretav2(matSem0noPreto)
                print('A Solução da matriz é:')
                resposta_bonita(matriz_resposta)
                #for i in matriz_resposta:
                    #print(i)


def resposta_bonita(mat):
    
    letras = ['x','y','z','w','p','u','q','a','b']
    posicao = 0
    for i in letras:
        if posicao < len(mat[0])-1:
            print(f'{i} = {round(mat[posicao][len(mat[posicao])-1],2)}')
            posicao += 1
    print('as letras estão em ordem de aparecimento e podem nao estar iguais a que fornecido')


matri =[[0,3,2,28],\
         [4,0,2,24],\
         [2,3,0,16]]

m = [[1,1.5,0,8],\
     [0,3,2,28],\
     [4,0,2,24]]

ma = [[1, 1, 1, 25],
      [5, 3, 2 , 0],
      [0, 1, -1, 6]]

mat = [[-1, 2, -5],[1, -1, 3]]

a = [
    [1, 1, 1, 1, 1, 15],
    [2, 3, 1, 4, 0, 26],
    [3, 1, 2, 2, 1, 25],
    [4, 2, 3, 1, 3, 39],
    [1, 1, 1, 0, 2, 17]
]

matriz_nao_resolvel = [
    [1, 1, 1, 1, 1, 10],
    [2, 2, 2, 2, 2, 20],
    [3, 3, 3, 3, 3, 30],
    [4, 4, 4, 4, 4, 40],
    [5, 5, 5, 5, 5, 45]  # linha "falsa"
]

o = [[0,1,7,0],
     [0,2,13,0],
     [0,3,5,0]]

calcula(input('Qual é o arquivo?'), 'Não é posivel calcular esse sistema de equações porque alguma linha é falsa e igual a uma anterior.......... Tente outro sistema de equações')

# é só fazer o resto do programa
# quando conseguir fazer o programa funcionar com esta matriz, mude para outra
# com a mesma, com menos ou com mais equações.

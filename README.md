# ForcaMiddleware

Jogo da forca simulando o jogo Roda a Roda do SBT a pedido do Professor Ricardo Inacio Alvares e Silva, para a disciplina de Tópicos Avançados em Computação, do curso de Ciência da Computação da Unifil Londrina.


## Versão 1.0 - Monoprocesso
Jogo para três jogadores, implementado em **C#** com interface **CLI**.

Para rodar o jogo usando o Visual Studio, baixe o código, abra o arquivo "ForcaMiddleware.sln", selecione o projeto "ForcaMonoprocesso" como projeto de startup e clique em iniciar.

*Para rodar diretamente execute o arquivo "\ForcaMiddleware\ForcaMonoProcesso\bin\Debug\netcoreapp3.1\ForcaMonoProcesso.exe"

## Versão 2.0 - Sockets
Jogo para até 5 jogadores implementado em **C#** com interface **CLI**.

**Iniciando o Servidor**
Para rodar o jogo usando o Visual Studio, baixe o código, abra o arquivo "ForcaMiddleware.sln", selecione o projeto "SocketServer" como projeto de startup e clique em iniciar.

*Para rodar diretamente execute o arquivo "\ForcaMiddleware\SocketServer\bin\Debug\netcoreapp3.1\SocketServer.exe"

**Iniciando Clientes**
Para cada cliente a ser iniciado, clique com o botão direito no projeto "SocketClient", no menu que aparece selecione Debug > Start a New Instance.

*Para rodar diretamente execute o arquivo "\ForcaMiddleware\SocketClient\bin\Debug\netcoreapp3.1\SocketClient.exe"

**Pontuação**
Cada palpite irá valer uma pontuação específica, para cada letra certa, o valor irá somar ao placar do jogador. Caso a letra não exista nas palavras, a pontuação sorteada será reduzida do placar. 

## Versão 3.0 - RPC
Em breve

## Versão 4.0 - AMQP
Jogo para N jogadores simultâneos, implementado em **C#** com interface **WPF** (Windows Presentation Fondation) com comunicação intermediada pelo **RabbitMQ**, servidor implementado em **C#**.

**Pré Requisito**
RabbitMQ instalado e redando no localhost. (download em https://www.rabbitmq.com/download.html)

**Iniciando o Servidor**
Para rodar o jogo usando o Visual Studio, baixe o código, abra o arquivo "ForcaMiddleware.sln", selecione o projeto "QueueServer" como projeto de startup e clique em iniciar.

*Para rodar diretamente execute o arquivo "\ForcaMiddleware\QueueServer\bin\Debug\netcoreapp3.1\QueueServer.exe"

**Iniciando Clientes**
Para cada cliente a ser iniciado, clique com o botão direito no projeto "VisualQueueClient", no menu que aparece selecione Debug > Start a New Instance.

*Para rodar diretamente execute o arquivo "\ForcaMiddleware\VisualQueueClient\bin\Debug\netcoreapp3.1\VisualQueueClient.exe"

**Pontuação**
Cada palpite irá valer uma pontuação específica, para cada letra certa, o valor irá somar ao placar do jogador. Caso a letra não exista nas palavras, a pontuação sorteada será reduzida do placar. 

# Streaming Client

Um cliente desktop para gerenciamento de conteúdo de streaming, desenvolvido em C# com Avalonia UI.

Sistema construído para o PIM VIII

## 📋 Funcionalidades

- **Gerenciamento de Criadores**: Criar, visualizar e excluir criadores de conteúdo
- **Gerenciamento de Conteúdo**: Adicionar, editar e excluir conteúdos (áudios)
- **Upload de Arquivos**: Suporte para upload de arquivos MP3
- **Reprodução de Mídia**: Reproduzir arquivos de áudio diretamente no cliente
- **Edição Inline**: Sistema de edição com ícone de lápis para modificar conteúdos
- **Download de Arquivos**: Baixar arquivos de mídia através do navegador

## 🚀 Pré-requisitos

- **.NET 8.0 SDK** ou superior
- **Sistema operacional**: Windows, Linux
- **API Streaming**: O cliente se conecta a uma API REST para gerenciar dados

### Para reprodução de mídia (opcional):
- **Windows**: VLC Media Player (recomendado)
- **Linux**: `mpv`, `vlc` ou `mplayer`

## 📦 Instalação

1. **Clone o repositório**:
   ```bash
   git clone <seu-repositorio-url>
   cd StreamingClient
   ```

2. **Restaure as dependências**:
   ```bash
   dotnet restore
   ```

3. **Compile o projeto**:
   ```bash
   dotnet build
   ```

## 🎮 Como usar

### Executar o cliente:
```bash
dotnet run
```

### Configurar a API:
1. Abra o cliente
2. Na barra superior, configure a URL da API (ex: `http://localhost:5011` OBS: Ela está rodando no render com o url de `https://streamingapi-pim-viii.onrender.com/`)
3. Clique em "Aplicar"
4. Clique em "Carregar" para buscar criadores

### Gerenciar Criadores:
- **Adicionar**: Digite o nome e clique em "Adicionar"
- **Excluir**: Clique no botão "Excluir" ao lado do criador
- **Visualizar conteúdo**: Clique no criador na lista

### Gerenciar Conteúdo:
- **Adicionar**: Preencha título, tipo, selecione criador e arquivo, clique "Adicionar"
- **Editar**: Clique no ícone de lápis (✏️) ao lado do título
- **Upload de arquivo**: No modo de edição, clique em "Upload" para selecionar arquivo
- **Reproduzir**: Clique em "Play" para reproduzir mídia
- **Download**: Clique em "Download" para baixar arquivo
- **Salvar alterações**: No modo de edição, clique em "Salvar"
- **Cancelar edição**: No modo de edição, clique em "Cancelar"

## 🏗️ Arquitetura

### Tecnologias utilizadas:
- **.NET 8.0**: Framework principal
- **Avalonia UI**: Framework de UI cross-platform
- **CommunityToolkit.Mvvm**: Padrão MVVM
- **HttpClient**: Comunicação com API REST

### Estrutura do projeto:
```
StreamingClient/
├── Models/
│   └── Dto.cs              # Modelos de dados
├── Services/
│   └── ApiClient.cs        # Cliente HTTP para API
├── ViewModels/
│   └── MainViewModel.cs    # Lógica da interface
├── Views/
│   ├── MainView.axaml      # Interface principal
│   ├── MainView.axaml.cs   # Code-behind
│   ├── MainWindow.axaml    # Janela principal
│   └── ConfirmDialog.axaml # Diálogo de confirmação
└── StreamingClient.csproj  # Arquivo de projeto
```

## 🔧 Configuração

### Variáveis de ambiente (opcional):
- `STREAMING_API_URL`: URL padrão da API

### Personalização:
- Modifique `MainView.axaml` para alterar a interface
- Ajuste `ApiClient.cs` para mudanças na comunicação com API
- Configure `MainViewModel.cs` para lógica de negócio

## 🐛 Solução de problemas

### Cliente não inicia:
- Verifique se o .NET 8.0 SDK está instalado
- Execute `dotnet restore` e `dotnet build`

### Não consegue conectar com API:
- Verifique se a API está rodando
- Confirme a URL da API na interface
- Teste a conectividade com `curl` ou navegador

### Reprodução de mídia não funciona:
- **Windows**: Instale VLC Media Player
- **Linux**: Execute `sudo apt install mpv vlc`
- **macOS**: Instale VLC Media Player
- Use o botão "Download" como alternativa

### Upload de arquivos falha:
- Verifique se o arquivo é suportado (.mp3, .mp4, .avi, .mov, .wav)
- Confirme se há espaço em disco suficiente
- Verifique se a API está funcionando

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 📞 Suporte

Se você encontrar problemas ou tiver dúvidas:

1. Verifique a seção de [Solução de problemas](#-solução-de-problemas)
2. Procure por issues similares no repositório
3. Abra uma nova issue com detalhes do problema

## 🔄 Changelog

### v1.0.0
- ✅ Gerenciamento completo de criadores e conteúdos
- ✅ Sistema de edição inline com ícone de lápis
- ✅ Upload e reprodução de arquivos de mídia
- ✅ Interface responsiva e intuitiva
- ✅ Suporte cross-platform (Windows, Linux, macOS)

---


using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using StreamingClient.ViewModels;
using StreamingClient.Models;
using System.Threading.Tasks;
using Avalonia.VisualTree;

namespace StreamingClient.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private async void OnLoaded(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.CarregarCriadoresAsync();
            }
        }

        private void OnAplicarBaseUrl(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.AplicarBaseUrl();
            }
        }

        private async void OnCarregarCriadores(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.CarregarCriadoresAsync();
            }
        }

        private async void OnCriarCriador(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.CriarCriadorAsync();
            }
        }

        private async void OnExcluirCriador(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is CriadorDto criador)
            {
                var dlg = new ConfirmDialog($"Tem certeza que deseja excluir o criador '{criador.Nome}'?\nIsso também excluirá todos os conteúdos vinculados a ele.");
                var owner = this.FindAncestorOfType<Window>();
                var result = await dlg.ShowDialog<bool>(owner!);
                if (result)
                {
                    await vm.ExcluirCriadorAsync(criador);
                }
            }
        }

        private async void OnCriarConteudo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                await vm.CriarConteudoAsync();
            }
        }

        private async void OnSalvarConteudo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                await vm.AtualizarConteudoAsync(conteudo);
            }
        }

        private async void OnExcluirConteudo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                await vm.ExcluirConteudoAsync(conteudo);
            }
        }

        private async void OnEscolherArquivo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel != null)
                {
                    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Selecionar arquivo de mídia",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("Vídeos") { Patterns = new[] { "*.mp4", "*.avi", "*.mov" } },
                            new FilePickerFileType("Áudios") { Patterns = new[] { "*.mp3", "*.wav" } },
                            new FilePickerFileType("Todos os arquivos") { Patterns = new[] { "*.*" } }
                        }
                    });

                    if (files.Count > 0)
                    {
                        vm.ArquivoSelecionado = files[0].Path.LocalPath;
                    }
                }
            }
        }

        private async void OnUploadArquivo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                // Abrir file picker diretamente
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel != null)
                {
                    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Selecionar arquivo de mídia",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("Vídeos") { Patterns = new[] { "*.mp4", "*.avi", "*.mov" } },
                            new FilePickerFileType("Áudios") { Patterns = new[] { "*.mp3", "*.wav" } },
                            new FilePickerFileType("Todos os arquivos") { Patterns = new[] { "*.*" } }
                        }
                    });

                    if (files.Count > 0)
                    {
                        var filePath = files[0].Path.LocalPath;
                        await vm.UploadArquivoDiretoAsync(conteudo, filePath);
                    }
                }
            }
        }

        private void OnPlayArquivo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                vm.PlayArquivo(conteudo);
            }
        }

        private void OnDownloadArquivo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                vm.DownloadArquivo(conteudo);
            }
        }

        private void OnEditarConteudo(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                vm.EditarConteudo(conteudo);
            }
        }

        private void OnCancelarEdicao(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && sender is Button btn && btn.Tag is ConteudoDto conteudo)
            {
                vm.CancelarEdicao(conteudo);
            }
        }
    }
}


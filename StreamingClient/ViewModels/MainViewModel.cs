using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StreamingClient.Models;
using StreamingClient.Services;

namespace StreamingClient.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private ApiClient _api;

        [ObservableProperty]
        private ObservableCollection<CriadorDto> criadores = new();

        [ObservableProperty]
        private CriadorDto? criadorSelecionado;

        [ObservableProperty]
        private ObservableCollection<ConteudoDto> conteudos = new();

        [ObservableProperty]
        private string novoCriadorNome = string.Empty;

        [ObservableProperty]
        private string novoConteudoTitulo = string.Empty;

        [ObservableProperty]
        private string novoConteudoTipo = string.Empty;

        [ObservableProperty]
        private string baseUrl = "http://localhost:5011/";

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private string arquivoSelecionado = string.Empty;

        [ObservableProperty]
        private CriadorDto? criadorSelecionadoParaConteudo;

        public MainViewModel()
        {
            _api = new ApiClient(BaseUrl);
        }

        [RelayCommand]
        public async Task CarregarCriadoresAsync()
        {
            try
            {
                var lista = await _api.GetCriadoresAsync();
                Criadores.Clear();
                foreach (var item in lista)
                {
                    // Carregar contagem de conteúdos para cada criador
                    var conteudos = await _api.GetConteudosByCriadorAsync(item.Id);
                    item.ContagemConteudos = conteudos.Count;
                    Criadores.Add(item);
                }
                StatusMessage = $"Carregados {Criadores.Count} criadores.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar criadores: {ex.Message}";
            }
        }

        partial void OnCriadorSelecionadoChanged(CriadorDto? value)
        {
            _ = CarregarConteudosAsync();
        }

        [RelayCommand]
        public async Task CarregarConteudosAsync()
        {
            if (CriadorSelecionado == null) return;
            try
            {
                var lista = await _api.GetConteudosByCriadorAsync(CriadorSelecionado.Id);
                Conteudos.Clear();
                foreach (var item in lista)
                {
                    Conteudos.Add(item);
                }
                StatusMessage = $"Carregados {Conteudos.Count} conteúdos para {CriadorSelecionado.Nome}.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar conteúdos: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task CriarCriadorAsync()
        {
            if (string.IsNullOrWhiteSpace(NovoCriadorNome)) return;
            try
            {
                var created = await _api.CreateCriadorAsync(NovoCriadorNome);
                if (created != null)
                {
                    created.ContagemConteudos = 0; // Novo criador começa com 0 conteúdos
                    Criadores.Add(created);
                    NovoCriadorNome = string.Empty;
                    StatusMessage = "Criador adicionado.";
                }
                else
                {
                    StatusMessage = "Falha ao adicionar criador.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao adicionar criador: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task ExcluirCriadorAsync(CriadorDto? criador)
        {
            if (criador == null) return;
            try
            {
                if (await _api.DeleteCriadorAsync(criador.Id))
                {
                    Criadores.Remove(criador);
                    if (CriadorSelecionado?.Id == criador.Id)
                    {
                        CriadorSelecionado = null;
                        Conteudos.Clear();
                    }
                    StatusMessage = "Criador removido.";
                }
                else
                {
                    StatusMessage = "Falha ao remover criador.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao remover criador: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task CriarConteudoAsync()
        {
            var criadorParaUsar = CriadorSelecionadoParaConteudo ?? CriadorSelecionado;
            if (criadorParaUsar == null) 
            {
                StatusMessage = "Selecione um criador para o conteúdo.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NovoConteudoTitulo) || string.IsNullOrWhiteSpace(NovoConteudoTipo)) return;
            try
            {
                var created = await _api.CreateConteudoAsync(NovoConteudoTitulo, NovoConteudoTipo, criadorParaUsar.Id);
                if (created != null)
                {
                    // Se o conteúdo foi criado para o criador selecionado, adiciona à lista
                    if (CriadorSelecionado?.Id == criadorParaUsar.Id)
                    {
                        Conteudos.Add(created);
                    }
                    
                    // Atualizar contagem do criador
                    criadorParaUsar.ContagemConteudos++;
                    
                    // Notificar a UI sobre a mudança na propriedade
                    OnPropertyChanged(nameof(Criadores));
                    
                    NovoConteudoTitulo = string.Empty;
                    NovoConteudoTipo = string.Empty;
                    CriadorSelecionadoParaConteudo = null;
                    StatusMessage = $"Conteúdo adicionado para {criadorParaUsar.Nome}.";
                }
                else
                {
                    StatusMessage = "Falha ao adicionar conteúdo.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao adicionar conteúdo: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task AtualizarConteudoAsync(ConteudoDto? conteudo)
        {
            if (conteudo == null) return;
            try
            {
                var updated = await _api.UpdateConteudoAsync(conteudo.Id, conteudo.Titulo, conteudo.Varchar);
                if (updated != null)
                {
                    var idx = Conteudos.IndexOf(conteudo);
                    if (idx >= 0) Conteudos[idx] = updated;
                    
                    // Desativar modo de edição
                    conteudo.IsEditing = false;
                    
                    StatusMessage = "Conteúdo atualizado.";
                }
                else
                {
                    StatusMessage = "Falha ao atualizar conteúdo.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao atualizar conteúdo: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task ExcluirConteudoAsync(ConteudoDto? conteudo)
        {
            if (conteudo == null) return;
            try
            {
                if (await _api.DeleteConteudoAsync(conteudo.Id))
                {
                    Conteudos.Remove(conteudo);
                    
                    // Atualizar contagem do criador
                    var criador = Criadores.FirstOrDefault(c => c.Id == conteudo.CriadorId);
                    if (criador != null)
                    {
                        criador.ContagemConteudos--;
                        // Notificar a UI sobre a mudança na propriedade
                        OnPropertyChanged(nameof(Criadores));
                    }
                    
                    StatusMessage = "Conteúdo removido.";
                }
                else
                {
                    StatusMessage = "Falha ao remover conteúdo.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao remover conteúdo: {ex.Message}";
            }
        }

        [RelayCommand]
        public void AplicarBaseUrl()
        {
            _api = new ApiClient(BaseUrl);
            StatusMessage = $"Base URL atualizada: {BaseUrl}";
        }

        [RelayCommand]
        public async Task UploadArquivoAsync(ConteudoDto? conteudo)
        {
            if (conteudo == null)
            {
                StatusMessage = "Nenhum conteúdo selecionado para upload.";
                return;
            }
            
            if (string.IsNullOrEmpty(ArquivoSelecionado))
            {
                StatusMessage = "Nenhum arquivo selecionado. Clique em 'Escolher Arquivo' primeiro.";
                return;
            }
            
            try
            {
                StatusMessage = "Enviando arquivo...";
                var success = await _api.UploadArquivoAsync(conteudo.Id, ArquivoSelecionado);
                if (success)
                {
                    // Recarregar conteúdos para atualizar a UI
                    await CarregarConteudosAsync();
                    ArquivoSelecionado = string.Empty;
                    StatusMessage = "Arquivo enviado com sucesso!";
                }
                else
                {
                    StatusMessage = "Falha ao enviar arquivo.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao enviar arquivo: {ex.Message}";
            }
        }

        public async Task UploadArquivoDiretoAsync(ConteudoDto? conteudo, string filePath)
        {
            if (conteudo == null)
            {
                StatusMessage = "Nenhum conteúdo selecionado para upload.";
                return;
            }
            
            if (string.IsNullOrEmpty(filePath))
            {
                StatusMessage = "Nenhum arquivo selecionado.";
                return;
            }
            
            try
            {
                StatusMessage = "Enviando arquivo...";
                var success = await _api.UploadArquivoAsync(conteudo.Id, filePath);
                if (success)
                {
                    // Recarregar conteúdos para atualizar a UI
                    await CarregarConteudosAsync();
                    StatusMessage = "Arquivo enviado com sucesso!";
                }
                else
                {
                    StatusMessage = "Falha ao enviar arquivo.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao enviar arquivo: {ex.Message}";
            }
        }

        [RelayCommand]
        public void PlayArquivo(ConteudoDto? conteudo)
        {
            if (conteudo == null || string.IsNullOrEmpty(conteudo.ArquivoPath)) return;
            
            try
            {
                var url = $"{BaseUrl}api/arquivos/download/{conteudo.ArquivoPath}";
                var extension = Path.GetExtension(conteudo.ArquivoPath).ToLowerInvariant();
                
                // Verificar se estamos no WSL2
                var isWsl = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null;
                
                if (isWsl)
                {
                    // No WSL2, tentar usar players específicos
                    if (extension.Equals(".mp3") || extension.Equals(".wav"))
                    {
                        // Para áudio, tentar usar mpv primeiro
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "mpv",
                                Arguments = $"--no-video {url}",
                                UseShellExecute = false
                            });
                            StatusMessage = $"Reproduzindo áudio: {conteudo.Titulo}";
                            return;
                        }
                        catch
                        {
                            // Fallback para vlc
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "vlc",
                                    Arguments = $"--intf dummy {url}",
                                    UseShellExecute = false
                                });
                                StatusMessage = $"Reproduzindo áudio: {conteudo.Titulo}";
                                return;
                            }
                            catch
                            {
                                // Fallback para mplayer
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = "mplayer",
                                        Arguments = url,
                                        UseShellExecute = false
                                    });
                                    StatusMessage = $"Reproduzindo áudio: {conteudo.Titulo}";
                                    return;
                                }
                                catch
                                {
                                    StatusMessage = "Nenhum player de áudio encontrado. Use o botão Download.";
                                    return;
                                }
                            }
                        }
                    }
                    else if (extension.Equals(".mp4") || extension.Equals(".avi") || extension.Equals(".mov"))
                    {
                        // Para vídeo, tentar usar mpv primeiro
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "mpv",
                                Arguments = url,
                                UseShellExecute = false
                            });
                            StatusMessage = $"Reproduzindo vídeo: {conteudo.Titulo}";
                            return;
                        }
                        catch
                        {
                            // Fallback para vlc
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "vlc",
                                    Arguments = url,
                                    UseShellExecute = false
                                });
                                StatusMessage = $"Reproduzindo vídeo: {conteudo.Titulo}";
                                return;
                            }
                            catch
                            {
                                StatusMessage = "Nenhum player de vídeo encontrado. Use o botão Download.";
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Em ambiente Linux nativo, usar xdg-open
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = url,
                        UseShellExecute = false
                    });
                    StatusMessage = $"Abrindo arquivo: {conteudo.Titulo}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao reproduzir arquivo: {ex.Message}. Use o botão Download.";
            }
        }

        [RelayCommand]
        public void DownloadArquivo(ConteudoDto? conteudo)
        {
            if (conteudo == null || string.IsNullOrEmpty(conteudo.ArquivoPath)) return;
            
            try
            {
                var url = $"{BaseUrl}api/arquivos/download/{conteudo.ArquivoPath}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                StatusMessage = $"Download iniciado: {conteudo.Titulo}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao baixar arquivo: {ex.Message}";
            }
        }

        [RelayCommand]
        public void EditarConteudo(ConteudoDto? conteudo)
        {
            if (conteudo == null) return;

            // Salvar valores originais
            conteudo.TituloOriginal = conteudo.Titulo;
            conteudo.VarcharOriginal = conteudo.Varchar;
            
            // Ativar modo de edição
            conteudo.IsEditing = true;
            
            // Notificar a UI sobre a mudança
            OnPropertyChanged(nameof(Conteudos));
            
            StatusMessage = $"Editando: {conteudo.Titulo}";
        }

        [RelayCommand]
        public void CancelarEdicao(ConteudoDto? conteudo)
        {
            if (conteudo == null) return;

            // Restaurar valores originais
            conteudo.Titulo = conteudo.TituloOriginal;
            conteudo.Varchar = conteudo.VarcharOriginal;
            
            // Desativar modo de edição
            conteudo.IsEditing = false;
            
            // Notificar a UI sobre a mudança
            OnPropertyChanged(nameof(Conteudos));
            
            StatusMessage = "Edição cancelada";
        }
    }
}


using System.ComponentModel;

namespace StreamingClient.Models
{
    public class CriadorDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int ContagemConteudos { get; set; }
    }

    public class ConteudoDto : INotifyPropertyChanged
    {
        public int Id { get; set; }
        
        private string _titulo = string.Empty;
        public string Titulo 
        { 
            get => _titulo; 
            set 
            { 
                _titulo = value; 
                OnPropertyChanged(nameof(Titulo)); 
            } 
        }
        
        private string _varchar = string.Empty;
        public string Varchar 
        { 
            get => _varchar; 
            set 
            { 
                _varchar = value; 
                OnPropertyChanged(nameof(Varchar)); 
            } 
        }
        
        public string? ArquivoPath { get; set; }
        public string? ContentType { get; set; }
        public int CriadorId { get; set; }
        
        private bool _isEditing = false;
        public bool IsEditing 
        { 
            get => _isEditing; 
            set 
            { 
                _isEditing = value; 
                OnPropertyChanged(nameof(IsEditing)); 
            } 
        }
        
        public string TituloOriginal { get; set; } = string.Empty;
        public string VarcharOriginal { get; set; } = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


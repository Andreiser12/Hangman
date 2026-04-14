using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangmanGame.Models
{
    public class LetterButton: ViewModels.ViewModelBase
    {
        public char Letter { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public LetterButton(char letter)
        {
            Letter = letter;
            IsEnabled = true;
        }
    }
}

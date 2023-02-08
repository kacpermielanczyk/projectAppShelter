using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace projectApp
{
    class Pet
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; } //gatunek
        public string? Breed { get; set; } //rasa
        public string? Gender { get; set; }
        public string? Size { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public DateTime DateOfAdmission { get; set; }
        public ImageSource? Image { get; set; }
    }
}

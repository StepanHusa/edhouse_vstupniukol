using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vstupniukol
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dobrý den,");
            Console.WriteLine("Jmenuji se Štěpán Husa a tento program jsem napsal jako vstupní úkol pro stáž ve firmě Edhouse");
            Console.WriteLine("Nyní stiskněte Enter pro načtení .txt souboru pomocí windows dialogu");
            Console.ReadKey();

            OpenFileDialog openFileDialog = new OpenFileDialog();

        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace vstupniukol
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //úvod
            Console.WriteLine("Dobrý den,");
            Console.WriteLine("Jmenuji se Štěpán Husa a tento program jsem napsal jako vstupní úkol pro stáž ve firmě Edhouse");

            string file = null;
            //pokud je progtam spuštěn pomocí txt souboru dá mu přednost
            if (args.Length > 0 && File.Exists(args[0]) && Path.GetExtension(args[0]).ToLower() == ".txt")
                file = args[0];
            //pokusí se otevřít soubor s názvem test.txt který je ve stejné složce jako program
            else if (File.Exists("test.txt"))
                file = "vstup.txt";

            //pokud ani jedno, otevře dialog
            else {

                Console.WriteLine("Stiskněte Enter pro načtení .txt souboru pomocí windows dialogu");
                Console.ReadKey();
                //otevře dialog pro výběr txt souboru
                OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = false, Filter= $"Textový soubor |*.TXT|All files (*.*)|*.*" };
                //pokud není otevřen soubor, aplikace nemůže načíst data a nemá důvod pokračovat k algoritmu
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    Console.WriteLine("Nebyl vybrán žádný soubor");
                    ExitProgram();
                }
                else
                    file = openFileDialog.FileName;
            }

            Console.WriteLine("načetli jste soubor "+ file);


            //načítání dat

            //soubor je nutné vkládat ve formátu:
            //první řádek dvě čísla oddělená pouze pomlčkou,
            //druhý řádek trasa prvního řidiče, třetí řádek trasa druhého řidiče,
            //kroky trasy oddělené čárkou, na konci ani na začátku nepřebývají znaky (např. "50W,80S,99E" )

            //protože jednodenní trasy nemohou být takového rozsahu, aby překročily operační paměť současných počítačů, načítám všechna data najedou
            try
            {
                //načte řádky textu
                string[] lines = File.ReadAllLines(file);

                //načte interval
                string[] range = lines[0].Split('-');
                int lowerBound = Int32.Parse(range[0]);
                int upperBound = Int32.Parse(range[1]);

                //načte trasy a převede na vektory
                var pathOne = lines[1].TranslatePathToVectors();
            }
            catch
            {
                Console.WriteLine("načtený soubor je špatného formátu a nezle provést načtení dat");
                ExitProgram();
            }







            ExitProgram();
        }


        //metoda pro vystoupení z algoritmu a ukončení programu
        private static void ExitProgram()
        {
            Console.WriteLine();
            Console.WriteLine("Stiskněte libovolnou klávesu pro ukončení aplikace");
            Console.ReadKey();
            Environment.Exit(0);

        }



    }
    public static class Extensions
    {
        //metoda pro převod textového vstupu vektorovou trasu
        public static Point[] TranslatePathToVectors(this string path)
        {
            //rozdělí string na jednotlivé kroky
            string[] moves = path.Split(',');

            //deklaruje pole celočíselných dvourozměrných vektorů
            Point[] vectors = new Point[moves.Length];
            //projde postupně každý krok, převede jej na vektor a uloží do pole
            for (int i = 0; i < moves.Length; i++)
            {
                char lastChar = moves[i][moves.Length - 1];
                int number = Int32.Parse(moves[i].Remove(moves.Length - 1));

                //podle písmena určí směr a do daného směru vkládá dálku
                Point vector;
                switch (lastChar)
                {
                    case 'E':
                        vector = new Point(number, 0);
                        break;
                    case 'N':
                        vector = new Point(0, number);
                        break;
                    case 'W':
                        vector = new Point(-number, 0);
                        break;
                    case 'S':
                        vector = new Point(0, -number);
                        break;
                    default:
                        //k tomuto kodu by nemělo dojít proto vypisuji varování
                        Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                        vector = new Point(0, 0);
                        break;
                }

                vectors[i] = vector;
            }

            return vectors;
        }
    }
}

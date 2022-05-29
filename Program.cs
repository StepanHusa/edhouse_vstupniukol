using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


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
            string defoultFileName = "vstup.txt";
            //pokud je progtam spuštěn pomocí txt souboru dá mu přednost
            if (args.Length > 0 && File.Exists(args[0]) && Path.GetExtension(args[0]).ToLower() == ".txt")
                file = args[0];
            //pokusí se otevřít soubor s názvem test.txt který je ve stejné složce jako program
            else if (File.Exists(defoultFileName))
                file = defoultFileName;

            //pokud ani jedno, otevře dialog
            else
            {

                Console.WriteLine("Stiskněte Enter pro načtení .txt souboru pomocí windows dialogu");
                Console.ReadKey();
                //otevře dialog pro výběr txt souboru
                OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = false, Filter = $"Textový soubor |*.TXT|All files (*.*)|*.*" };
                //pokud není otevřen soubor, aplikace nemůže načíst data a nemá důvod pokračovat k algoritmu
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    Console.WriteLine("Nebyl vybrán žádný soubor");
                    ExitProgram();
                }
                else
                    file = openFileDialog.FileName;
            }

            Console.WriteLine("načetli jste soubor " + file);

            //pokud by se vyskytla chyba program nespadne, ale vypíše hlášení do konzole
            try
            {
                //načítání dat

                //soubor je nutné vkládat ve formátu:
                //první řádek dvě čísla oddělená pouze pomlčkou,
                //druhý řádek trasa prvního řidiče, třetí řádek trasa druhého řidiče,
                //kroky trasy oddělené čárkou, na konci ani na začátku nepřebývají znaky (např. "50W,80S,99E" )

                //protože jednodenní trasy nemohou být takového rozsahu, aby překročily operační paměť současných počítačů, načítám všechna data najedou

                //načte řádky textu
                string[] lines = File.ReadAllLines(file);

                //načte interval
                string[] range = lines[0].Split('-');
                int lowerBound = int.Parse(range[0]);
                int upperBound = int.Parse(range[1]);

                //načte trasy a převede na vektory
                var pathOne = lines[1].TranslateStringPathToPathSections(lowerBound, upperBound);
                var pathTwo = lines[2].TranslateStringPathToPathSections(lowerBound, upperBound);

                //porovná dvě trasy a najde společný bod (pokud existuje)
                bool pointFound = false;
                Point finalPoint = new Point();

                //metoda pro projití všech možností protnutí tras, díky uzavření do metody se pomocí return dostanu pryč z kodu po nalezení bodu
                LookForCrossSection();
                void LookForCrossSection()
                {
                    //deklarování proměnných úseků
                    PathSection sectionOne;
                    PathSection sectionTwo;
                    //kolmé úseky
                    //dva cykly, ketré projdou všechny kombinace kolmých úseků a pokud najdou průnik ukončí hledání
                    for (int i = 0; i < pathOne.Verticals.Length; i++)
                    {
                        sectionOne = pathOne.Verticals[i];
                        for (int j = 0; j < pathTwo.Horizontals.Length; j++)
                        {
                            sectionTwo = pathTwo.Horizontals[j];

                            if (ComparePerpendicular(sectionOne,sectionTwo))
                            {
                                finalPoint = new Point(sectionTwo.Position, sectionOne.Position);
                                pointFound = true;
                                //return;
                            }
                        }
                    }
                    for (int i = 0; i < pathTwo.Verticals.Length; i++)
                    {
                        sectionOne = pathTwo.Verticals[i];
                        for (int j = 0; j < pathOne.Horizontals.Length; j++)
                        {
                            sectionTwo = pathOne.Horizontals[j];

                            if (ComparePerpendicular(sectionOne, sectionTwo))
                            {
                                finalPoint = new Point(sectionOne.Position, sectionTwo.Position);
                                pointFound = true;
                                //return;

                            }
                        }
                    }

                    //krajní úseky
                    //horizontální
                    if (!(pathOne.Horizontals.Length == 0 || pathTwo.Horizontals.Length == 0)) //kontrola prázdnosti polí
                    {
                        sectionOne = pathOne.Horizontals[0];
                        sectionTwo = pathTwo.Horizontals[0];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Horizontals[0];
                        sectionTwo = pathTwo.Horizontals[pathTwo.Horizontals.Length - 1];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Horizontals[pathOne.Horizontals.Length - 1];
                        sectionTwo = pathTwo.Horizontals[0];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Horizontals[pathOne.Horizontals.Length - 1];
                        sectionTwo = pathTwo.Horizontals[pathTwo.Horizontals.Length - 1];
                        if (TryFindParallel()) return;
                    }
                    //vertikální
                    if (!(pathOne.Verticals.Length == 0 || pathTwo.Verticals.Length == 0))
                    {
                        sectionOne = pathOne.Verticals[0];
                        sectionTwo = pathTwo.Verticals[0];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Verticals[0];
                        sectionTwo = pathTwo.Verticals[pathTwo.Verticals.Length - 1];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Verticals[pathOne.Verticals.Length - 1];
                        sectionTwo = pathTwo.Verticals[0];
                        if (TryFindParallel()) return;

                        sectionOne = pathOne.Verticals[pathOne.Verticals.Length - 1];
                        sectionTwo = pathTwo.Verticals[pathTwo.Verticals.Length - 1];
                        if (TryFindParallel()) return;

                    }
                    // metoda pro srovnání krajních úseků
                    bool TryFindParallel()
                    {
                        if (CompareParallel(sectionOne, sectionTwo)) //pozná pokud jsou na stejné přímce a prolínají se
                        {
                            if (sectionOne.Lower > sectionTwo.Lower) //vybere ten správný bod (teo
                            {
                                finalPoint = new Point(sectionOne.Position, sectionOne.Lower);
                                pointFound = true;
                            }
                            else
                            {
                                finalPoint = new Point(sectionOne.Position, sectionTwo.Lower);
                                pointFound = true;
                            }
                            return true;
                        }
                        return false;
                    }


                }
                if (pointFound)
                {
                    Console.WriteLine($"BOD NALEZEN: {finalPoint.X}, {finalPoint.Y}");
                }
                else
                {
                    Console.WriteLine("BOD NENALEZEN");
                }

            }
            catch
            {
                Console.WriteLine("načtený soubor je špatného formátu, nebo neobsahuje správná data a nezle provést načtení a výpočet");
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


        // logika pro srovnání úseků
        private static bool CompareParallel(PathSection sectionOne, PathSection sectionTwo)
        {
            return sectionOne.Position == sectionTwo.Position && sectionOne.Lower <= sectionTwo.Higher && sectionOne.Higher >= sectionTwo.Lower;  
        }
        private static bool ComparePerpendicular(PathSection sectionOne, PathSection sectionTwo)
        {
            return sectionOne.Position >= sectionTwo.Lower && sectionOne.Position <= sectionTwo.Higher && sectionTwo.Position >= sectionOne.Lower && sectionTwo.Position <= sectionOne.Higher;
        }



    }
    public static class Extensions
    {
        //metoda pro převod textového vstupu na rohové body trasy mezi kraji intervalu (nepoužito)
        public static Point[] TranslateStringPathToPointsInRange(this string path, int lowerBound, int upperBound)
        {
            //rozdělí string na jednotlivé kroky
            string[] moves = path.Split(',');

            //deklaruje pole celočíselných dvourozměrných vektorů (nakonec nevyužito tohoto postupu a ulkádají se už přímo body, ve ktrých řidič mění směr)
            Point[] vectors = new Point[moves.Length];

            //deklaruje počítadlo ujeté trasy a aktuální pozici 
            int distance = 0;
            Point position = new Point(0, 0);
            char lastChar = ' ';
            int number;

            int i = 0;

            //projde postupně každý krok, převede jej na vektor a uloží do pole
            while (distance < lowerBound && i < moves.Length) //TODO pohlídat vyjímku, kdy dolní hranice je větší než celková vzdálenost
            {
                lastChar = moves[i][moves[i].Length - 1];
                number = int.Parse(moves[i].Remove(moves[i].Length - 1));

                distance += number;

                //podle písmena určí směr a do daného směru vkládá dálku
                //Point vector;
                switch (lastChar)
                {
                    case 'E':
                        //vector = new Point(number, 0);
                        position.X += number;
                        break;
                    case 'N':
                        //vector = new Point(0, number);
                        position.Y += number;
                        break;
                    case 'W':
                        //vector = new Point(-number, 0);
                        position.X -= number;
                        break;
                    case 'S':
                        //vector = new Point(0, -number);
                        position.Y -= number;
                        break;
                    default:
                        //k tomuto kodu by nemělo dojít proto vypisuji varování
                        Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                        //vector = new Point(0, 0);
                        break;
                }
                i++;
            }

            //pokud se vstoupí do intervalu, uloží se místo vstupu a přeruší se cylkus 
            //uložení vstupního bodu
            Point lowerboundPoint = position;
            switch (lastChar)
            {
                case 'E':
                    lowerboundPoint.Offset(lowerBound - distance, 0);
                    break;
                case 'N':
                    lowerboundPoint.Offset(0, lowerBound - distance);
                    break;
                case 'W':
                    lowerboundPoint.Offset(distance - lowerBound, 0);
                    break;
                case 'S':
                    lowerboundPoint.Offset(0, distance - lowerBound);
                    break;
                default:
                    Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                    break;
            }
            //deklarace seznamu bodů
            List<Point> points = new List<Point>();
            points.Add(lowerboundPoint);

            //cyklus pro hodnoty v intervalu
            while (distance < upperBound && i < moves.Length) //TODO pohlídat vyjímku, kdy horní hranice je větší než celková vzdálenost
            {
                lastChar = moves[i][moves[i].Length - 1];
                number = int.Parse(moves[i].Remove(moves[i].Length - 1));

                distance += number;

                switch (lastChar)
                {
                    case 'E':
                        position.X += number;
                        break;
                    case 'N':
                        position.Y += number;
                        break;
                    case 'W':
                        position.X -= number;
                        break;
                    case 'S':
                        position.Y -= number;
                        break;
                    default:
                        Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                        break;
                }
                points.Add(position);

                i++;
            }

            //uložení hraničního bodu
            Point upperboundPoint = position;
            switch (lastChar)
            {
                case 'E':
                    upperboundPoint.Offset(lowerBound - distance, 0);
                    break;
                case 'N':
                    upperboundPoint.Offset(0, lowerBound - distance);
                    break;
                case 'W':
                    upperboundPoint.Offset(distance - lowerBound, 0);
                    break;
                case 'S':
                    upperboundPoint.Offset(0, distance - lowerBound);
                    break;
                default:
                    Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                    break;
            }

            points.Add(upperboundPoint);

            //hodnoty za intervalem mě nezajímají

            return points.ToArray();
        }

        //převod textu na pole úseků (menší prostor, lépe přístupné při porovnávání),
        //pro úseky je definován typ PathSection a jsou zabaleny do proměnné TranslatedPath obsahující 2 pole horizontálních a vertikálních úseků
        public static TranslatedPath TranslateStringPathToPathSections(this string path, int lowerBound, int upperBound)
        {

            TranslatedPath allElements = new TranslatedPath();
            //rozdělí string na jednotlivé kroky
            string[] moves = path.Split(',');

            //deklaruje pole celočíselných dvourozměrných vektorů (nakonec nevyužito tohoto postupu a ulkádají se už přímo body, ve ktrých řidič mění směr)
            Point[] vectors = new Point[moves.Length];

            //deklaruje počítadlo ujeté trasy a aktuální pozici 
            int distance = 0;
            Point position = new Point(0, 0);
            char lastChar = ' ';
            int number;

            int i = 0;

            //projde postupně každý krok, převede jej na vektor a uloží do pole
            while (distance < lowerBound && i < moves.Length) //TODO pohlídat vyjímku, kdy dolní hranice je větší než celková vzdálenost
            {
                lastChar = moves[i][moves[i].Length - 1];
                number = int.Parse(moves[i].Remove(moves[i].Length - 1));

                distance += number;

                //podle písmena určí směr a do daného směru vkládá dálku
                //Point vector;
                switch (lastChar)
                {
                    case 'E':
                        //vector = new Point(number, 0);
                        position.X += number;
                        break;
                    case 'N':
                        //vector = new Point(0, number);
                        position.Y += number;
                        break;
                    case 'W':
                        //vector = new Point(-number, 0);
                        position.X -= number;
                        break;
                    case 'S':
                        //vector = new Point(0, -number);
                        position.Y -= number;
                        break;
                    default:
                        //k tomuto kodu by nemělo dojít proto vypisuji varování
                        Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                        //vector = new Point(0, 0);
                        break;
                }
                i++;
            }

            //deklarace seznamu bodů
            List<PathSection> sectionsHor = new List<PathSection>();
            List<PathSection> sectionsVer = new List<PathSection>();
            //pokud se vstoupí do intervalu, uloží se místo vstupu a přeruší se cylkus 
            //uložení hraničního úseku (od prvního bodu, který se počítá)
            //Point lowerboundPoint = position;
            switch (lastChar)
            {
                case 'E':
                    //lowerboundPoint.Offset(lowerBound - distance, 0);

                    sectionsVer.Add(new PathSection(position.X - distance + lowerBound, position.X, position.Y));
                    break;
                case 'N':
                    sectionsHor.Add(new PathSection(position.Y - distance + lowerBound, position.Y, position.X));
                    //lowerboundPoint.Offset(0, lowerBound - distance);
                    break;
                case 'W':
                    sectionsVer.Add(new PathSection(position.X, position.X - distance + lowerBound, position.Y));
                    //lowerboundPoint.Offset(distance - lowerBound, 0);
                    break;
                case 'S':
                    sectionsHor.Add(new PathSection(position.Y, position.Y - distance + lowerBound, position.X));
                    //lowerboundPoint.Offset(0, distance - lowerBound);
                    break;
                default:
                    //Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                    break;
            }

            //cyklus pro hodnoty v intervalu
            while (distance < upperBound && i < moves.Length) //TODO pohlídat vyjímku, kdy horní hranice je větší než celková vzdálenost
            {
                lastChar = moves[i][moves[i].Length - 1];
                number = int.Parse(moves[i].Remove(moves[i].Length - 1));

                distance += number;

                //uloží úsek do správného seznamu a posune pozici
                switch (lastChar)
                {
                    case 'E':
                        sectionsHor.Add(new PathSection(position.X, position.X + number, position.Y));
                        position.X += number;
                        break;
                    case 'N':
                        sectionsVer.Add(new PathSection(position.Y, position.Y + number, position.X));
                        position.Y += number;
                        break;
                    case 'W':
                        sectionsHor.Add(new PathSection(position.X - number, position.X , position.Y));
                        position.X -= number;
                        break;
                    case 'S':
                        sectionsVer.Add(new PathSection(position.Y - number, position.Y, position.X));
                        position.Y -= number;
                        break;
                    default:
                        Console.WriteLine($"Varování: při převodu kroků na vektory nebyl u kroku {i} upřesněn směr");
                        break;
                }
                //kod použitý pro kontrolu výsledku
                //try
                //{
                //    var v = sectionsVer[sectionsVer.Count - 1];
                //    Point p = new Point(4649, 6047);

                //    if (p.X == v.Position && p.Y < v.Higher && p.Y > v.Lower)
                //        v = new PathSection();
                //    var hor = sectionsHor[sectionsHor.Count - 1];

                //    if (p.Y == v.Position && p.X < v.Higher && p.X > v.Lower)
                //        v = new PathSection();
                //}
                //catch { }

                i++;
            }

            //uložení hraničního úseku
            switch (lastChar)
            {
                case 'E':
                    sectionsVer.Add(new PathSection(position.X - distance + upperBound, position.X, position.Y));
                    break;
                case 'N':
                    sectionsHor.Add(new PathSection(position.Y - distance + upperBound, position.Y, position.X));
                    break;
                case 'W':
                    sectionsVer.Add(new PathSection(position.X, position.X - distance + upperBound, position.Y));
                    break;
                case 'S':
                    sectionsHor.Add(new PathSection(position.Y, position.Y - distance + upperBound, position.X));
                    break;
                default:
                    Console.WriteLine($"Varování: při převodu kroků na vektory nebyl při posledním kroku upřesněn směr");
                    break;
            }

            allElements.Horizontals = sectionsHor.ToArray();
            allElements.Verticals = sectionsVer.ToArray();

            //hodnoty za intervalem mě nezajímají

            return allElements;
        }

    }
}

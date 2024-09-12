using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaLiga
{
    internal class Program
    {
        static string filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "liga.csv");
        static Dictionary<string, int> score = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            LeerArchivo();
            while (true)
            {
                switch (Menu())
                {
                    case 1:
                        while (!AñadirScore()) ;
                        DictionaryToCSV();
                        break;
                    case 2:
                        while (!EliminarScore()) ;
                        DictionaryToCSV();
                        break;
                    case 3:
                        while (!ModificarScore()) ;
                        DictionaryToCSV();
                        break;
                    case 0:
                        return;
                }
            }
        }

        static int Menu()
        {
            int option;

            do
            {
                Console.WriteLine("┌───────────────────────────────────┐");
                Console.WriteLine("│        MENU  PRINCIPAL            │");
                Console.WriteLine("├───────────────────────────────────┤");
                Console.WriteLine("│  (1)  - Añadir equipo             │");
                Console.WriteLine("│  (2)  - Eliminar equipo           │");
                Console.WriteLine("│  (3)  - Modificar score           │");
                Console.WriteLine("│  (0)  - Salir                     │");
                Console.WriteLine("└───────────────────────────────────┘");

                if (!int.TryParse(Console.ReadLine(), out option))
                    Console.WriteLine("Opcion invalida");

            } while (option != 0 && option != 1 && option != 2 && option != 3);

            return option;
        }

        static void LeerArchivo()
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string linea;

                linea = sr.ReadLine();

                while (linea != null)
                {
                    string[] datos = linea.Split(',');

                    if (int.TryParse(datos[1], out int puntos))
                        score.Add(datos[0], puntos);

                    linea = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static void MostrarScore()
        {
            foreach (var equipo in score)
            {
                Console.WriteLine("Nombre: " + equipo.Key);
                Console.WriteLine("Score: " + equipo.Value);
                Console.WriteLine("-----------------------------------");
            }
        }

        static bool AñadirScore()
        {
            MostrarScore();

            string nombreEquipo;
            Console.WriteLine("Escriba el nombre del equipo");
            nombreEquipo = Console.ReadLine();

            foreach (var equipo in score)
            {
                if (equipo.Key == nombreEquipo)
                {
                    Console.WriteLine("El nombre del equipo ya esta elegido");
                    return false;
                }
            }

            Console.WriteLine("Escriba los puntos del equipo");
            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Escribe una puntuacion valida");
                return false;
            }

            score.Add(nombreEquipo, result);
            return true;
        }

        static bool EliminarScore()
        {
            MostrarScore();

            Console.WriteLine("Escriba el nombre del equipo que quieres borrar");
            String nombreEquipo = Console.ReadLine();

            foreach (var equipo in score)
            {
                if (nombreEquipo == equipo.Key)
                {
                    score.Remove(nombreEquipo);
                    return true;
                }
            }

            Console.WriteLine("No se ha encontrado un equipo con ese nombre, vuelva a intentarlo");
            return false;
        }

        static bool ModificarScore()
        {
            MostrarScore();

            Console.WriteLine("Escriba el nombre del equipo que quieres modificar");
            String nombreEquipo = Console.ReadLine();

            Console.WriteLine("Escriba que score quiere ponerle al equipo " + nombreEquipo);
            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Introduzca un numero valido");
                return false;
            }

            foreach (var equipo in score)
            {
                if (nombreEquipo == equipo.Key)
                {
                    score[nombreEquipo] = result;
                    return true;
                }
            }

            Console.WriteLine("No se ha encontrado un equipo con ese nombre, vuelva a intentarlo");
            return false;
        }

        static void DictionaryToCSV()
        {
            using (StreamWriter wr = new StreamWriter(filePath))
            {
                foreach (var equipo in score)
                    wr.WriteLine(equipo.Key + "," + equipo.Value);
            }
        }
    }
}

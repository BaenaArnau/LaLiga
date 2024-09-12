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
                Console.WriteLine(@"
┌───────────────────────────────────┐
│        MENU  PRINCIPAL            │
├───────────────────────────────────┤
│  (1)  - Añadir equipo             │
│  (2)  - Eliminar equipo           │
│  (3)  - Modificar score           │
│  (0)  - Salir                     │
└───────────────────────────────────┘
");

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
                Console.WriteLine("Nombre: " + equipo.Key + "   Score: " + equipo.Value);
                Console.WriteLine("---------------------------------------------------");
            }
        }

        static bool AñadirScore()
        {
            MostrarScore();

            String nombreEquipo = PedirNombre();

            foreach (var equipo in score)
            {
                if (equipo.Key == nombreEquipo)
                {
                    Console.WriteLine("El nombre del equipo ya esta elegido");
                    return false;
                }
            }

            int? result = PedirPuntuacion();

            if (result == null)
                return false;

            score.Add(nombreEquipo, (int)result);
            return true;
        }

        static bool EliminarScore()
        {
            MostrarScore();

            String nombreEquipo = PedirNombre();

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

            String nombreEquipo = PedirNombre();

            int? result = PedirPuntuacion();

            if (result == null)
                return false;

            foreach (var equipo in score)
            {
                if (nombreEquipo == equipo.Key)
                {
                    score[nombreEquipo] = (int)result;
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

        static string PedirNombre()
        {
            Console.WriteLine("Escriba el nombre del equipo");
            return Console.ReadLine();
        }

        static int? PedirPuntuacion()
        {
            Console.WriteLine("Escriba que score quiere ponerle al equipo");
            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Introduzca un numero valido");
                return null;
            }

            return result;
        }
    }
}

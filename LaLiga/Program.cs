﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaLiga
{
    /// <summary>
    /// Clase interna que gestiona toda nuestra aplicacion
    /// </summary>
    internal class Program
    {
        // Varible que nos permite gestionar donde se crea el archivo .csv
        static string filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "liga.csv");
        
        // Variable que gestiona el nombre y score de los equipos
        static Dictionary<string, InfoEquipos> score = new Dictionary<string, InfoEquipos>();

        /// <summary>
        /// Metodo principal de la aplicacion que se ejecuta al comenzar el rpograma
        /// </summary>
        /// <param name="args">Lista de argumentos para iniciar el metodo</param>
        static void Main(string[] args)
        {
            LeerArchivo();
            while (true)
            {
                switch (Menu())
                {
                    case 1:
                        while (!AñadirScore()) ;
                        break;
                    case 2:
                        while (!EliminarScore()) ;
                        break;
                    case 3:
                        while (!ModificarScore()) ;
                        break;
                    case 4:
                        while (!AñadirJugador()) ;
                        break;
                    case 5:
                        while (!EliminarJugador()) ;
                        break;
                    case 6:
                        while (!ModificarJugador()) ;
                        break;
                    case 7:
                        MontarPartido();
                        break;
                    case 0:
                        return;
                }
                DictionaryToCsv();
            }
        }

        /// <summary>
        /// Metodo que muestra el las opciones y gestiona la opcion que has elegido
        /// </summary>
        /// <returns>Devuelve la opcion del menu que el usuario elija</returns>
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
│  (4)  - Añadir jugador            │
│  (5)  - Eliminar jugador          │
│  (6)  - Modificar jugador         │
│  (7)  - Jugar partido             │
│  (0)  - Salir                     │
└───────────────────────────────────┘
");

                if (!int.TryParse(Console.ReadLine(), out option))
                    Console.WriteLine("Opcion invalida");

            } while (option < 0 || option > 7);

            return option;
        }

        /// <summary>
        /// Metodo que lee el fichero .csv si es que existe
        /// </summary>
        static void LeerArchivo()
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string linea = sr.ReadLine();

                while (linea != null)
                {
                    string[] datos = linea.Split(',');

                    if (datos.Length >= 2)
                    {
                        if (int.TryParse(datos[1], out int puntos))
                        {
                            List<string> jugadores = new List<string>();

                            for (int i = 2; i < datos.Length; i++)
                                jugadores.Add(datos[i]);

                            score.Add(datos[0], new InfoEquipos(puntos, jugadores));
                        }
                        else
                            Console.WriteLine($"Advertencia: No se pudo comvertir el puntaje en la línea: {linea}");
                    }
                    else
                        Console.WriteLine("Advertencia: Archivo corrupto");

                    linea = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al leer el archivo: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Metodo que nos muestra los equipos con su respectivo score
        /// </summary>
        static void MostrarScore()
        {
            Console.WriteLine("");
            foreach (var equipo in score)
            {
                Console.WriteLine("Nombre: " + equipo.Key + "   Score: " + equipo.Value.score);
                Console.WriteLine("Jugadores: ");
                foreach (string jugador in equipo.Value.jugadores)
                    Console.WriteLine(" -" + jugador);
                Console.WriteLine("---------------------------------------------------");
            }
        }

        /// <summary>
        /// Este metodo nos permite crear un equipo y añadirle un score
        /// </summary>
        /// <returns>Devuelve un bool de confirmacion</returns>
        static bool AñadirScore()
        {
            int contador = 0;
            List<string> jugadores = new List<string>();

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

            while (true)
            {
                Console.WriteLine("Escribe el nombre del jugador que quiere añadir al equipo o exit si has acabado");
                string entrada = Console.ReadLine();

                if (entrada.ToLower() == "exit" && contador == 0)
                    Console.WriteLine("Tiene que tener como minimo un jugador");
                else if (entrada.ToLower() == "exit")
                    break;
                else
                {
                    jugadores.Add(entrada);
                    contador++;
                }
            }

            score.Add(nombreEquipo, new InfoEquipos((int)result, jugadores));
            return true;
        }

        /// <summary>
        /// Este metodo nos permite eliminar un equipo existente
        /// </summary>
        /// <returns>Variable de confirmacion</returns>
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

        /// <summary>
        /// Metodo que nos permite modificar un score de un equipo ya creado
        /// </summary>
        /// <returns>devuelve una variable de confirmacion</returns>
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
                    score[nombreEquipo].Score = (int)result;
                    return true;
                }
            }

            Console.WriteLine("No se ha encontrado un equipo con ese nombre, vuelva a intentarlo");
            return false;
        }

        /// <summary>
        /// Metodo que convierte los cambios dentro de la aplicacion en un archivo .csv
        /// </summary>
        static void DictionaryToCsv()
        {
            using (StreamWriter wr = new StreamWriter(filePath))
            {
                foreach (var equipo in score)
                {
                    wr.Write(equipo.Key);

                    wr.Write("," + equipo.Value.Score);

                    foreach (var jugador in equipo.Value.Jugadores)
                    {
                        wr.Write("," + jugador);
                    }

                    wr.WriteLine();
                }
            }
        }

        /// <summary>
        /// Metodo que nos pide el nombre del equipo
        /// </summary>
        /// <returns>Devuelve el nombre que el usuario haya escrito por pantalla</returns>
        static string PedirNombre()
        {
            Console.WriteLine("Escriba el nombre del equipo");
            return Console.ReadLine();
        }

        /// <summary>
        /// Metodo que pide la puntuacion por consola
        /// </summary>
        /// <returns>Devuelve un  numero, puede devolver un null si el usuario no ha escrito un numero</returns>
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

        /// <summary>
        /// Metodo que nos permite añadir un jugador de un equipo ya creado
        /// </summary>
        /// <returns>devuelve una variable de confirmacion</returns>
        static bool AñadirJugador()
        {
            MostrarScore();

            string nombreEquipo = PedirNombre();

            string nombreJugador = PedirJugador();

            if (score.ContainsKey(nombreEquipo))
            {
                foreach(string jugador in score[nombreEquipo].jugadores)
                    if(jugador.ToLower() == nombreJugador.ToLower())
                    {
                        Console.WriteLine("Ese jugador ya esta en el equipo.");
                        return false;
                    }

                score[nombreEquipo].jugadores.Add(nombreJugador);
                return true;
            }
            else
            {
                Console.WriteLine("No se ha encontrado un equipo con ese nombre, vuelva a intentarlo");
                return false;
            }
        }

        /// <summary>
        /// Metodo que nos permite eliminar un jugador de un equipo ya creado
        /// </summary>
        /// <returns>devuelve una variable de confirmacion</returns>
        static bool EliminarJugador()
        {
            MostrarScore();

            string nombreEquipo = PedirNombre();

            string nombreJugador = PedirJugador();

            if (score.ContainsKey(nombreEquipo))
            {
                if (score[nombreEquipo].jugadores.Contains(nombreJugador))
                {
                    score[nombreEquipo].jugadores.Remove(nombreJugador);
                    return true;
                }
                else
                {
                    Console.WriteLine("Ese jugador no juega en este equipo.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("No se ha encontrado un equipo con ese nombre, vuelva a intentarlo");
                return false;
            }
        }

        /// <summary>
        /// Metodo que nos permite modificar los jugadores de un equipo
        /// </summary>
        /// <returns>devuelve una variable de confirmacion</returns>
        static bool ModificarJugador()
        {
            MostrarScore();

            string nombreEquipo = PedirNombre();

            if (!score.ContainsKey(nombreEquipo))
            {
                Console.WriteLine("El equipo no existe. Vuelva a intentarlo.");
                return false;
            }

            string nombreJugador = PedirJugador();

            Console.WriteLine("Escribe el nuevo nombre:");
            string nuevoNombre = Console.ReadLine();

            InfoEquipos info = score[nombreEquipo];

            for (int i = 0; i < info.Jugadores.Count; i++)
            {
                if (info.Jugadores[i] == nombreJugador)
                {
                    info.Jugadores[i] = nuevoNombre;
                    Console.WriteLine($"El jugador {nombreJugador} ha sido cambiado a {nuevoNombre}.");
                    return true;
                }
            }

            Console.WriteLine("No se ha encontrado el jugador en el equipo.");
            return false;
        }

        /// <summary>
        /// Metodo que nos pide el nombre del jugador
        /// </summary>
        /// <returns>Devuelve el nombre que el usuario haya escrito por pantalla</returns>
        static string PedirJugador()
        {
            Console.WriteLine("Escribe el nombre del jugador.");
            return Console.ReadLine();
        }

        /// <summary>
        /// Metodo que monta un partido entre dos equipos random
        /// </summary>
        static void MontarPartido()
        {
            Random r = new Random();

            int equipo1 = r.Next(0, score.Count);
            int equipo2 = r.Next(0, score.Count);

            while(equipo1 == equipo2)
                equipo2 = r.Next(0, score.Count);

            string firstTeam = score.ElementAt(equipo1).Key;
            string secondTeam = score.ElementAt(equipo2).Key;

            Console.WriteLine("Se van a enfrentar el " + firstTeam + " contra el " + secondTeam)

            GolesPartido(firstTeam, secondTeam);
        }

    }

    public class InfoEquipos
    {
        public int score;
        public List<string> jugadores;

        public InfoEquipos(int score, List<string> jugadores)
        {
            this.score = score;
            this.jugadores = jugadores;
        }

        public int Score 
        { 
            get { return score; }
            set { score = value; }
        }

        public List<string> Jugadores
        {
            get { return jugadores; }
            set { jugadores = value;  }
        }
    }
}

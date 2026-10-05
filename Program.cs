using System;
using System.IO;
using System.Collections.Generic;

namespace MesaDePartesDigital
{
    public class Solicitud
    {
        public string Id { get; set; }
        public string Dni { get; set; }
        public string NombreCompleto { get; set; }
        public string Asunto { get; set; }
        public string FechaRegistro { get; set; }
        public string Estado { get; set; }

        public override string ToString()
        {
            return $"{Id}|{Dni}|{NombreCompleto}|{Asunto}|{FechaRegistro}|{Estado}";
        }

        public static Solicitud DesdeLinea(string linea)
        {
            var partes = linea.Split('|');
            if (partes.Length < 6) return null;
            return new Solicitud
            {
                Id = partes[0],
                Dni = partes[1],
                NombreCompleto = partes[2],
                Asunto = partes[3],
                FechaRegistro = partes[4],
                Estado = partes[5]
            };
        }
    }

    class Program
    {
        private static string rutaArchivo = "solicitudes.txt";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   MESA DE PARTES DIGITAL - SISTEMA     ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Registrar nueva solicitud");
                Console.WriteLine("2. Listar todas las solicitudes");
                Console.WriteLine("3. Buscar solicitud por DNI");
                Console.WriteLine("4. Salir");
                Console.WriteLine("----------------------------------------");
                Console.Write("Seleccione una opción (1-4): ");

                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        RegistrarSolicitud();
                        break;
                    case "2":
                        ListarSolicitudes();
                        break;
                    case "3":
                        BuscarSolicitud();
                        break;
                    case "4":
                        salir = true;
                        Console.WriteLine("\n¡Gracias por utilizar la Mesa de Partes Digital!");
                        break;
                    default:
                        Console.WriteLine("\n[ERROR] Opción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void RegistrarSolicitud()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRO DE NUEVA SOLICITUD ---");

            string dni;
            do
            {
                Console.Write("Ingrese DNI (8 dígitos): ");
                dni = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(dni) || dni.Length != 8 || !long.TryParse(dni, out _))
                {
                    Console.WriteLine("[ERROR] El DNI debe contener exactamente 8 números.");
                }
                else
                {
                    break;
                }
            } while (true);

            string nombre;
            do
            {
                Console.Write("Ingrese Nombre Completo: ");
                nombre = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("[ERROR] El nombre no puede estar vacío.");
                }
                else
                {
                    break;
                }
            } while (true);

            string asunto;
            do
            {
                Console.Write("Ingrese el Asunto / Documento: ");
                asunto = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(asunto))
                {
                    Console.WriteLine("[ERROR] El asunto no puede estar vacío.");
                }
                else
                {
                    break;
                }
            } while (true);

            string id = "SOL-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string estado = "Pendiente";

            Solicitud nuevaSolicitud = new Solicitud
            {
                Id = id,
                Dni = dni,
                NombreCompleto = nombre,
                Asunto = asunto,
                FechaRegistro = fecha,
                Estado = estado
            };

            GuardarEnArchivo(nuevaSolicitud);

            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine($"[ÉXITO] Solicitud registrada correctamente.");
            Console.WriteLine($"Código asignado: {id}");
            Console.WriteLine("Presione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }

        private static void ListarSolicitudes()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE SOLICITUDES ---");

            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine("\nNo hay solicitudes registradas aún.");
            }
            else
            {
                string[] lineas = File.ReadAllLines(rutaArchivo);
                if (lineas.Length == 0)
                {
                    Console.WriteLine("\nEl registro está vacío.");
                }
                else
                {
                    Console.WriteLine($"\n{"ID",-20} | {"DNI",-10} | {"NOMBRE",-25} | {"ESTADO",-10}");
                    Console.WriteLine(new string('-', 72));

                    foreach (var linea in lineas)
                    {
                        var sol = Solicitud.DesdeLinea(linea);
                        if (sol != null)
                        {
                            Console.WriteLine($"{sol.Id,-20} | {sol.Dni,-10} | {sol.NombreCompleto,-25} | {sol.Estado,-10}");
                        }
                    }
                }
            }

            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine("Presione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }

        private static void BuscarSolicitud()
        {
            Console.Clear();
            Console.WriteLine("--- BUSCAR SOLICITUD POR DNI ---");
            Console.Write("Ingrese el DNI a buscar: ");
            string dniBuscar = Console.ReadLine()?.Trim();

            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine("\nNo hay registros guardados.");
            }
            else
            {
                string[] lineas = File.ReadAllLines(rutaArchivo);
                bool encontrado = false;

                foreach (var linea in lineas)
                {
                    var sol = Solicitud.DesdeLinea(linea);
                    if (sol != null && sol.Dni == dniBuscar)
                    {
                        Console.WriteLine("\n----------------------------------------");
                        Console.WriteLine($"Código ID : {sol.Id}");
                        Console.WriteLine($"DNI       : {sol.Dni}");
                        Console.WriteLine($"Solicitante: {sol.NombreCompleto}");
                        Console.WriteLine($"Asunto    : {sol.Asunto}");
                        Console.WriteLine($"Fecha     : {sol.FechaRegistro}");
                        Console.WriteLine($"Estado    : {sol.Estado}");
                        Console.WriteLine("----------------------------------------");
                        encontrado = true;
                    }
                }

                if (!encontrado)
                {
                    Console.WriteLine($"\n[INFO] No se encontraron solicitudes registradas con el DNI: {dniBuscar}");
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
            Console.ReadKey();
        }

        private static void GuardarEnArchivo(Solicitud solicitud)
        {
            using (StreamWriter sw = File.AppendText(rutaArchivo))
            {
                sw.WriteLine(solicitud.ToString());
            }
        }
    }
}

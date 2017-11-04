using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinControlesProyecto
{
    class Leer
    {
        /// <summary>
        /// Método para leer los archivos con la ruta de dirección de memoria que se desea obtener los datos.
        /// </summary>
        /// <param name="ruta">Atributo string recibe la ruta a donde esta el fichero a leer.</param>
        /// <returns></returns>
        public static List<Contacto> LeerArchivo(String ruta)
        {
            StreamReader leerObjeto = new StreamReader(GenerarStreamDesdeString(ruta));
            List<Contacto> contactos = new List<Contacto>();
            String edad;
            String nombre;
            String dni;
            do
            {

                nombre = leerObjeto.ReadLine();
                edad = leerObjeto.ReadLine();
                dni = leerObjeto.ReadLine();
                if (nombre != null && edad != null && dni != null)
                    contactos.Add(new Contacto(nombre, edad, dni));

            } while (nombre != null && edad != null && dni != null);
   
            return contactos;
        }
        /// <summary>
        /// Método para poder transformar un Stream en un String
        /// </summary>
        /// <param name="rutaEnString">Dicho atributo por parámetro que se recibe es la ruta en String.</param>
        /// <returns></returns>
        public static Stream GenerarStreamDesdeString(string rutaEnString)
        {
            MemoryStream memoriaStream = new MemoryStream();
            StreamWriter escribir = new StreamWriter(memoriaStream);
            escribir.Write(rutaEnString);
            escribir.Flush();
            memoriaStream.Position = 0;
            return memoriaStream;
        }
    }
   
}

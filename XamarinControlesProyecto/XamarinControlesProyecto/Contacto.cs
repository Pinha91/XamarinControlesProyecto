using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinControlesProyecto
{
    public class Contacto
    {
        /// <summary>
        /// Declaración de variables necesarios para el objeto contacto.
        /// </summary>
        private string nombre, edad, dni;
        /// <summary>
        /// Constructor del objecto.
        /// </summary>
        /// <param name="nombre">Nombre del contacto.</param>
        /// <param name="edad">Edad del contacto.</param>
        /// <param name="dni">DNI del contacto.</param>
        public Contacto(string nombre, string edad, string dni)
        {
            this.nombre = nombre;
            this.edad = edad;
            this.dni = dni;
        }
        /// <summary>
        /// Get del nombre del contacto.
        /// </summary>
        public string Nombre
        {
            get
            {
                return nombre;
            }
        }
        /// <summary>
        /// Get de la edad del contacto.
        /// </summary>
        public string Edad
        {
            get
            {
                return edad;
            }
        }
        /// <summary>
        /// Get del DNI del contacto.
        /// </summary>
        public string Dni
        {
            get
            {
                return dni;
            }
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinControlesProyecto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Vistas : ContentPage
    {
        List<Contacto> contactos;//Lista de contactos que obtendremos del fichero que el usuario debe indicar
        List<Contacto> contactosMostrar; //Lista de contactos que obtendremos de la busqueda
        List<Contacto> contactoVacio = new List<Contacto>();//Lista de contacto vacio;
        MatchCollection matches; // Necesario para analizar cadenas con matches y expresiones regulares.
        Boolean error;//Controlar la información del listView si hay un error no mostrar nada.

        public Vistas()
        {
            InitializeComponent();
            //Al seleccionar el botón accede a los datos automáticamente a los datos del archivo.txt
            botonBuscar.Clicked += (sender, args) =>
            {
                contactos = Leer.LeerArchivo(Properties.Resources.Info);//Accede a la ruta dentro del proyecto
                contactosMostrar = new List<Contacto>();//Declaramos contactosMostar, donde ahí tendrá los datos a mostrar en el listView
                mensajeError.Text = "";//Refesco el mensaje
                error = false;//Resfreco el controlador del error.

                buscar();//Iniciacimos la búsqueda
            };

        }
        /// <summary>
        /// Se busca en el array que ha generado el leerArchivo con los valores indicados, sino se indica un error.
        /// </summary>
        private void buscar()
        {
            int edad1, edad2; ///!string.IsNullOrEmpty(txtMaxEdad.Text) es lo mismo que Text=""; es otra solucion

            //Si no hay campos vacios
            if (!txtMaxEdad.Text.Equals("") && !txtMinEdad.Text.Equals("") && !txtBusqueda.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }

            else if (!txtMaxEdad.Text.Equals("") && !txtMinEdad.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }
            else if (!txtMaxEdad.Text.Equals(""))
            {
                if (int.TryParse(txtMaxEdad.Text.Trim(), out edad1))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    txtMaxEdad.Text = txtMinEdad.Text = "";
                    mensajeError.Text = "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.";
                    //Si no entro en ninguna condicion limpio el buffer del ListView.
                    listaContactos.ItemsSource = contactoVacio;
                }
            }
            else if (!txtMinEdad.Text.Equals(""))
            {
                if (int.TryParse(txtMinEdad.Text.Trim(), out edad2))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    txtMaxEdad.Text = txtMinEdad.Text = "";
                    mensajeError.Text = "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.";
                    //Si no entro en ninguna condicion limpio el buffer del ListView.
                    listaContactos.ItemsSource = contactoVacio;
                }
            }
            else { realizarBusqueda(); }

        }
        /// <summary>
        /// Controla que los campos numericos sean correctos
        /// </summary>
        private void controlNumerico()
        {
            int edad1, edad2;

            if (int.TryParse(txtMaxEdad.Text.Trim(), out edad1) && int.TryParse(txtMinEdad.Text.Trim(), out edad2))
            {
                if (int.Parse(txtMinEdad.Text.ToString()) >= int.Parse(txtMaxEdad.Text.ToString()))
                {
                    //Mostrar advertencia
                    mensajeError.Text="La edad mínima no puede ser mayor o igual a la máxima.";
                    //Si no entro en ninguna condicion limpio el buffer del ListView.
                    listaContactos.ItemsSource = contactoVacio;
                }
                else { realizarBusqueda(); }
            }
            else
            {
                txtMaxEdad.Text = txtMinEdad.Text = "";
                mensajeError.Text = "Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.";
                //Si no entro en ninguna condicion limpio el buffer del ListView.
                listaContactos.ItemsSource = contactoVacio;
            }
        }
        /// <summary>
        /// Realiza la busqueda de verdad
        /// </summary>
        private void realizarBusqueda()
        {
            //Limpiamos los contactos cargados para volver a cargar los correctos
            contactosMostrar.Clear();
            //Se obtiene resultado de la busqueda con los valores introducidos y se carga en listView
            buscarContacto(txtBusqueda.Text);
        }
        /// <summary>
        /// Implementa los filtros para obtener una list de contactos a mostrar
        /// </summary>
        /// <param name="filtro"></param>
        public void buscarContacto(string filtro)
        {
            /// Recorremos el array contactos y si encontramos una coincidencia la añadimos al arraylist resultado
            for (int i = 0; i < contactos.Count; i++)
            {
                if (comprobarNombre(contactos[i], filtro))
                {
                    contactosMostrar.Add(contactos[i]);
                }
            }

            if (error)//Si hay algún error mustro la lista vacía
            {
                listaContactos.ItemsSource = contactoVacio;
            }
            else//Sino muestro los datos correspondiente.
            {
                listaContactos.ItemsSource = contactosMostrar;
            }

            //Si no se encontro ninguna coincidencia se informa
            if (contactosMostrar.Count == 0)
            {
                mensajeError.Text = "No se encontro ninguna coincidencia, prueba de nuevo.";
                error = true;
            }

        }
        /// <summary>
        /// Metodo que comprueba si el nombre del contacto tiene alguna coincidencia con el filtro de busqueda.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="filtro">La cadena que queremos usar como filtro. Puede estar vacia. En ese caso, devolveremos siempre true.</param>
        /// <returns> Devuelve true si se ha encontrado coincidencia, devuelve false si no la encuentra.</returns>
        public Boolean comprobarNombre(Contacto contacto, string filtro)
        {
            Boolean ok = false;
            Regex rgx = new Regex("%", RegexOptions.IgnoreCase);

            matches = rgx.Matches(filtro);

            /// Primero tenemos que controlar que en el filtro no hemos introducido mas de un %.
            if (matches.Count > 1)
            {
              mensajeError.Text = "No puede indicar más de un % en una busqueda.";
              error = true;
            }
            else
            {
                /// Si no se ha escrito nada en el patron de busqueda, o solo se ha escrito %...
                if (filtro.Trim().Equals("") || filtro.Trim().Equals("%"))
                {
                    if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else
                    {
                        ok = true;
                    }
                }
                /// Si el ultimo caracter es %...
                else if (filtro.Substring(filtro.Length - 1).Equals("%"))
                {
                    /// Quitamos el caracter % para poder usarlo como patron de busqueda.
                    filtro = filtro.Replace("%", "");
                    rgx = new Regex(String.Format("^" + filtro + ".*"), RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
                /// Si en el patron de busqueda no hemos puesto como ultimo caracter un %...
                else
                {
                    rgx = new Regex("^" + filtro + "$", RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length == 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMaxEdad.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && txtMinEdad.Text.Trim().Length > 0 && txtMaxEdad.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, txtMinEdad.Text.Trim(), txtMaxEdad.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
            }

            return ok;
        }
        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edad">La edad introducida al formulario para comparar.</param>
        /// <param name="modoMayor">Determina si estamos buscando mayores que la edad introducida o menos que la edad introducida</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edad, Boolean modoMayor)
        {
            Boolean ok = false;

            if ((Int32.Parse(contacto.Edad) < Int32.Parse(edad) && !modoMayor) || (Int32.Parse(contacto.Edad) >= Int32.Parse(edad) && modoMayor))
            {
                ok = true;
            }

            return ok;
        }
        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edadMin">La edad minima introducida al formulario para comparar.</param>
        /// <param name="edadMax">La edad maxima introducida al formulario para comparar.</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edadMin, string edadMax)
        {
            Boolean ok = false;

            if (Int32.Parse(contacto.Edad) <= Int32.Parse(edadMax) && Int32.Parse(contacto.Edad) > Int32.Parse(edadMin))
            {
                ok = true;
            }

            return ok;
        }

    }
}
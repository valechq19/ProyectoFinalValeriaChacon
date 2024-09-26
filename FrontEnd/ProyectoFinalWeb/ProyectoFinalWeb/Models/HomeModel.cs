using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using ProyectoFinalWeb.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProyectoFinalWeb.Models
{
    public class HomeModel
    {
        private readonly HttpClient _httpClient;

        public HomeModel()
        {
            _httpClient = new HttpClient();
        }


        public UsuariosEnt ValidarUsuario(UsuariosEnt entidad)
        {
            using (var client = new HttpClient())
            {
                JsonContent body = JsonContent.Create(entidad);
                string url = "https://localhost:44343/api/ValidarUsuario";
                HttpResponseMessage respuesta = client.PostAsync(url, body).GetAwaiter().GetResult();

                if (respuesta.IsSuccessStatusCode)
                    return respuesta.Content.ReadFromJsonAsync<UsuariosEnt>().Result;

                return null;
            }
        }

        public int IniciarSesion(UsuariosEnt entidad)
        {
            using (var client = new HttpClient())
            {
                JsonContent body = JsonContent.Create(entidad);
                string url = "https://localhost:44343/api/RegistrarUsuario";
                HttpResponseMessage respuesta = client.PostAsync(url, body).GetAwaiter().GetResult();

                if (respuesta.IsSuccessStatusCode)
                    return respuesta.Content.ReadFromJsonAsync<int>().Result;

                return 0;
            }
        }

        public async Task<ResultadoBusqueda> ObtenerArticulosAsync()
        {
            ResultadoBusqueda resultado = null;

          
            var response = await client.GetAsync("https://localhost:44343/api/BuscarTodosLosArticulos"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(jsonString); 
                resultado = JsonConvert.DeserializeObject<ResultadoBusqueda>(jsonString);

                if (resultado == null)
                {
                    throw new InvalidOperationException("Deserialized result is null.");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error al cargar los artículos desde la API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            return resultado;
        }


        public int AgregarArticulo(ArticuloEnt entidad)
        {
            using (var client = new HttpClient())
            {
                JsonContent body = JsonContent.Create(entidad);
                string url = "https://localhost:44343/api/AgregarArticulo";
                HttpResponseMessage respuesta = client.PostAsync(url, body).GetAwaiter().GetResult();

                if (respuesta.IsSuccessStatusCode)
                    return respuesta.Content.ReadFromJsonAsync<int>().Result;

                return 0;
            }
        }
        
        public ResultadoBusqueda BuscarNombreProducto(string codigo1, int cantidad)
        {
            using (var client = new HttpClient())
            {
               
                string url = $"https://localhost:44343/api/BuscarNombreProducto"; 

        
                var parametros = new { codigo = codigo1, cantidad = cantidad };
                JsonContent body = JsonContent.Create(parametros); 

               
                HttpResponseMessage respuesta = client.PostAsync(url, body).GetAwaiter().GetResult();

                
                if (respuesta.IsSuccessStatusCode)
                {
             
                    return respuesta.Content.ReadFromJsonAsync<ResultadoBusqueda>().Result;
                }

              
                return null;
            }
        }

        private static readonly HttpClient client = new HttpClient();

        public async Task<bool> EliminarProducto(string codigoEli)
        {
            string url = "https://localhost:44343/api/EliminarProducto";

            var parametros = new CodigoEnt { codigo = codigoEli };
            HttpResponseMessage respuesta = await client.PostAsJsonAsync(url, parametros);

            return respuesta.IsSuccessStatusCode; 
        
    }

        public async Task<ResultadoBusqueda> ObtenerVerFacturasAsync()
        {
            string url = "https://localhost:44343/api/ObtenerVerFacturas"; 

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                
                var resultado = await response.Content.ReadFromJsonAsync<ResultadoBusqueda>();
                return resultado;
            }

            return null; 
        }

     

        public async Task<bool> EliminarArticulo(string codigoArt)
        {
            string url = "https://localhost:44343/api/EliminarArticulo";

            var parametros = new CodigoEnt { codigo = codigoArt };
            HttpResponseMessage respuesta = await client.PostAsJsonAsync(url, parametros);

            return respuesta.IsSuccessStatusCode;

        }

        public async Task EditarArticulo(string codigoEdit, string nuevoNombre, decimal nuevoPrecio)
        {
            string url = "https://localhost:44343/api/EditarArticulo";

            var articulo = new ArticuloEnt
            {
                Codigo = codigoEdit,
                Nombre = nuevoNombre,
                Precio = nuevoPrecio
            };

            HttpResponseMessage respuesta = await client.PostAsJsonAsync(url, articulo);

            if (!respuesta.IsSuccessStatusCode)
            {
                throw new Exception("Error al editar el artículo. Código de estado: " + respuesta.StatusCode);
            }
        }


    }
}














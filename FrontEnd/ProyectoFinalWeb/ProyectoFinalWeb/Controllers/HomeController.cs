using ProyectoFinalWeb.Entities;
using ProyectoFinalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;


namespace ProyectoFinalWeb.Controllers
{
    public class HomeController : Controller
    {
        HomeModel homeModel = new HomeModel();


        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ConsultarProductos(UsuariosEnt entidad)
        {
            var homeModel = new HomeModel();
            var resultado = new ResultadoBusqueda();

            try
            {
                var usuarioValidado = homeModel.ValidarUsuario(entidad);

                if (usuarioValidado != null)
                {
                    
                    Session["id"] = usuarioValidado.id;
                    Session["usuario"] = usuarioValidado.usuario;
                    Session["contrasena"] = usuarioValidado.Contrasena;
                    Session["Token"] = usuarioValidado.Token;

                 
                    resultado = await homeModel.ObtenerArticulosAsync();
                    ViewBag.Articulos = resultado.Articulos;

                   
                    if (resultado.Articulos == null || !resultado.Articulos.Any())
                    {
                        ViewBag.Error = "No se encontraron artículos.";
                    }

                    return View("ConsultarProductos");
                }
                else
                {
                    ViewBag.mensaje = "Sus credenciales no fueron validados";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
        }

        [HttpGet]
        public ActionResult IniciarSesion()
        {
            try
            {
                Session.Clear();
                return View();
            }
            catch (Exception)
            {
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult IniciarSesion(UsuariosEnt entidad)
        {
            try
            {
                var resultado = homeModel.ValidarUsuario(entidad);

                if (resultado != null)
                {
                    Session["Usuario"] = resultado.usuario;
                    Session["Id"] = resultado.id;
                    return View();
                }
                else
                {
                    ViewBag.mensaje = "Sus credenciales no fueron validados";
                    return View("Index");
                }
            }
            catch (Exception)
            {
                return View("Index");
            }
        }



        [HttpGet]
        public ActionResult AgregarArticulo()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult AgregarArticulo(ArticuloEnt entidad)
        {
            try
            {
                if (homeModel.AgregarArticulo(entidad) > 0)
                    return View("Index");
                else
                {
                    ViewBag.mensaje = "No se pudo agregar el articulo";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }
        [HttpGet]
        public ActionResult AgregarProductoAFactura()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AgregarProductoAFactura(string codigo1, int cantidad)
        {
            var homeModel = new HomeModel(); 

            ResultadoBusqueda resultado;

            try
            {
                resultado = homeModel.BuscarNombreProducto(codigo1, cantidad); 

             
                ViewBag.NombreProducto = resultado.NombreProducto;
                ViewBag.TotalTodo = resultado.TotalTodo;
                ViewBag.Facturas = resultado.Facturas;   

              
                if (resultado == null || string.IsNullOrEmpty(resultado.NombreProducto))
                {
                    ViewBag.Error = "Producto no encontrado.";
                }
            }
            catch (Exception ex)
            {
                
                ViewBag.Error = ex.Message; 
            }

            return View();
        }

        [HttpGet]
        public ActionResult EliminarProducto()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EliminarProducto(string codigoEli)
        {
            var homeModel = new HomeModel();

            if (string.IsNullOrEmpty(codigoEli))
            {
                ViewBag.Error = "El código del producto no puede estar vacío o ser nulo.";
                return View();
            }

            try
            {
                bool resultado = await homeModel.EliminarProducto(codigoEli);

                if (resultado)
                {
                    ViewBag.Success = "Producto eliminado con éxito.";
                    return RedirectToAction("AgregarProductoAFactura"); 
                }
                else
                {
                    ViewBag.Error = "Error al eliminar el producto.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al eliminar el producto: {ex.Message}";
            }

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> VerFacturas() 
        {
            var resultado = await homeModel.ObtenerVerFacturasAsync(); 

            if (resultado != null) 
            {
                ViewBag.VerFacturas = resultado.VerFactura; 
                ViewBag.TotalTodo = resultado.TotalTodo; 
            }
            else
            {
                ViewBag.VerFacturas = new List<FacturaEnt>(); 
                ViewBag.TotalTodo = 0; 
            }

            ViewBag.Usuario = Session["usuario"] ?? "Usuario no identificado";

            return View();
        }
        [HttpGet]
        public ActionResult EliminarArticulo()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EliminarArticulo(string codigoArt)
        {
            var homeModel = new HomeModel();

            if (string.IsNullOrEmpty(codigoArt))
            {
                ViewBag.Error = "El código del producto no puede estar vacío o ser nulo.";
                return View();
            }

            try
            {
                bool resultado = await homeModel.EliminarArticulo(codigoArt);

                if (resultado)
                {
                    ViewBag.Success = "Producto eliminado con éxito.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Error al eliminar el producto.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al eliminar el producto: {ex.Message}";
            }

            return View();
        }
        [HttpGet]
        public ActionResult EditarArticulo()
        {
            return View(new ArticuloEnt());
        }

        [HttpPost]
        public async Task<ActionResult> EditarArticulo(string codigoEdit, string nuevoNombre, decimal nuevoPrecio)
        {
          
            var articulo = new ArticuloEnt
            {
                Codigo = codigoEdit,
                Nombre = nuevoNombre,
                Precio = nuevoPrecio
            };

          
            if (ModelState.IsValid)
            {
                try
                {
                  
                    await homeModel.EditarArticulo(articulo.Codigo, articulo.Nombre, articulo.Precio);

             
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                   
                    ModelState.AddModelError("", "Ocurrió un error al editar el artículo: " + ex.Message);
                }
            }


            return View(articulo);
        }
    }


}






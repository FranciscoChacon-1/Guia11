using Firebase.Auth;
using Firebase.Storage;
using Guía11.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Guía11.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            //Leemos el archivo subido.
            Stream archivoSubir = archivo.OpenReadStream();

            //Configuramos la conexión hacia FireBase
            string email = "cargar@gmail.com"; //Correo para autenticar en FireBase
            string clave = "del1alnueve"; //Contraseña establecida en la autenticar en FireBase
            string ruta = "practicadaw-e0c72.appspot.com";// URL donde se guardan los archivos.
            string api_key = "AIzaSyAigkPsW7ZDG4ux3tctWgrnBUsj29z8kdA\r\n"; // API_KEY identificador del proyecto en firebase

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));

            var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);

            var cancellation = new CancellationTokenSource();
            var tokenUser = autenticarFireBase.FirebaseToken;

            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                                            new FirebaseStorageOptions
                                                            {
                                                                AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                                                ThrowOnCancel = true
                                                            }
                                                        ).Child("Archivos")
                                                        .Child(archivo.FileName)
                                                        .PutAsync(archivoSubir, cancellation.Token);

            var urlArchivoCargado = await tareaCargarArchivo;

            return RedirectToAction("VerImagen");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}

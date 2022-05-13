using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DiffieHellman.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DiffieHellmanController : ControllerBase
{
    private readonly Business.DiffieHellman _diffieHellman;

    public DiffieHellmanController(Business.DiffieHellman diffieHellman)
    {
        _diffieHellman = diffieHellman;
    }

    /*  Task description
        1е еще один API Decrypt
        2е Прописать Route
        3е Создать отедьные модели WebApp модель (model) Requst (string key and message) and Reponse (string message)
        4e Сделать Console client в нем создать класс HttpClient (WebClient is legacy). Он будет дергать ручки нашего Web Server
     */

    [HttpPost]
    public ActionResult<string> PostEncrypt(string publicKey, string message)
    {
        return Encoding.UTF8.GetString(_diffieHellman.Encrypt(Encoding.UTF8.GetBytes(publicKey), message));
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJE.Http;

namespace TestGuestbook
{
    public class GuestbookModule : CJE.Http.IHttpModule
    {
        public Type[] RequiredComponents => Type.EmptyTypes;

        public bool RegisterModule(HttpServer server)
        {
            server.RegisterRequestHandler<TestGuestbook.Handler.Message>("/api/message");
            /* TODO */
            server.RegisterRequestHandler<TestGuestbook.Handler.Rating>("/api/rating");
            server.RegisterRequestHandler<TestGuestbook.Handler.Comment>("/api/comment");

            return true;
        }
    }
}

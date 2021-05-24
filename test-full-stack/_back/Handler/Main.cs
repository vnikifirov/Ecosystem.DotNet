using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJE.Http.RequestAnswer;

namespace TestGuestbook.Handler
{
    public class Main : CJE.Http.RequestHandlers.RequestHandlerBase
    {
        public Main(CJE.Http.HttpServer server, System.Net.HttpListenerContext context, CJE.Http.RequestHandlerData data) : base(server, context, data) { }

        public override IAnswer HandleGET()
        {
            IList<DB.Message> list = Server.DBManager.Session.QueryOver<DB.Message>().List();

            return new CJE.Http.RequestAnswer.Data("hello world! "+list[0].Title);
        }
    }
}

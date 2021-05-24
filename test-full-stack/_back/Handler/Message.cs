using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJE.Http.RequestAnswer;

namespace TestGuestbook.Handler
{
    public class Message : CJE.Http.RequestHandlers.RequestHandlerBase
    {
        public Message(CJE.Http.HttpServer server, System.Net.HttpListenerContext context, CJE.Http.RequestHandlerData data) : base(server, context, data) { }

        public override IAnswer HandleGET()
        {
            Guid id = Data.Get.GetGuid("id");
            if (id == Guid.Empty)
            {
                List<Data.Message> messages = DB.Controller.LoadMessageList(Server.DBSession);
                return new CJE.Http.RequestAnswer.Json(messages);
            }
            else
            {
                Data.Message message = DB.Controller.LoadMessage(Server.DBSession, id);
                return new CJE.Http.RequestAnswer.Data(message);
            }
        }

        public override IAnswer HandlePOST()
        {
            Form.Message inputData = new Form.Message(Data.Post.Input);
            Data.Message inputMessage = inputData.ToData();

            Data.Message message = DB.Controller.SaveMessage(Server.DBSession, inputMessage);

            return new CJE.Http.RequestAnswer.Json(message);
        }

        public override IAnswer HandleDELETE()
        {
            Guid id = Data.Get.GetGuid("id");

            Data.Message message = DB.Controller.DeleteMessage(Server.DBSession, id);

            return new CJE.Http.RequestAnswer.Json(message);
        }

    }
}

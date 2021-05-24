using CJE.Http.RequestAnswer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGuestbook.Handler
{
    public class Comment : CJE.Http.RequestHandlers.RequestHandlerBase
    {
        public Comment(CJE.Http.HttpServer server, System.Net.HttpListenerContext context, CJE.Http.RequestHandlerData data) : base(server, context, data) { }

        public override IAnswer HandleGET()
        {
            Guid id = Data.Get.GetGuid("id");
            if (id != Guid.Empty)
            {
                Data.Comment message = DB.Controller.LoadComment(Server.DBSession, id);
                return new CJE.Http.RequestAnswer.Data(message);
            }

            return null;
        }

        public override IAnswer HandlePOST()
        {
            Form.Comment inputData = new Form.Comment(Data.Post.Input);
            Data.Comment inputMessage = inputData.ToData();

            Data.Comment comment = DB.Controller.SaveComment(Server.DBSession, inputMessage);

            return new CJE.Http.RequestAnswer.Json(comment);
        }

        public override IAnswer HandleDELETE()
        {
            Guid id = Data.Get.GetGuid("id");

            Data.Comment comment = DB.Controller.DeleteComment(Server.DBSession, id);

            return new CJE.Http.RequestAnswer.Json(comment);
        }
    }
}

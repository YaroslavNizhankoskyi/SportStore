using SportStore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SportStore.Models
{
    public class SessionOrderEditor : OrderEditor
    {
        public static SessionOrderEditor GetOrderEditor(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
            .HttpContext.Session;
            SessionOrderEditor order = session?.GetJson<SessionOrderEditor>("Order")
            ?? new SessionOrderEditor();
            order.Session = session;
            return order;
        }
        [JsonIgnore]
        public ISession Session { get; set; }


        public override void AddLine(Outfit Outfit, int quantity)
        {
            base.AddLine(Outfit, quantity);
            Session.SetJson("Order", this);
        }
        public override void RemoveLine(int id)
        {
            base.RemoveLine(id);
            Session.SetJson("Order", this);
        }
        public override void Clear()
        {
            base.Clear();
            Session.Remove("Order");
        }

        public override void ChangeLine(int OutfitId, int quantity)
        {
            base.ChangeLine(OutfitId, quantity);
            Session.SetJson("Order", this);
        }
    }
}

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
    public class SessionCart : Cart
    {
        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
            .HttpContext.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart")
            ?? new SessionCart();
            cart.Session = session;
            return cart;
        }
        [JsonIgnore]
        public ISession Session { get; set; }


        public override void AddLine(Outfit Outfit, int quantity)
        {
            base.AddLine(Outfit, quantity);
            Session.SetJson("Cart", this);
        }
        public override void RemoveLine(int id)
        {
            base.RemoveLine(id);
            Session.SetJson("Cart", this);
        }
        public override void Clear()
        {
            base.Clear();
            Session.Remove("Cart");
        }

        public override void ChangeLine(int OutfitId, int quantity)
        {
            base.ChangeLine(OutfitId, quantity);
            Session.SetJson("Cart", this);
        }
    }
}

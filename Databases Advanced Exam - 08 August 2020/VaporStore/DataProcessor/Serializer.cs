using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Runtime.InteropServices;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.ExportDto;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context
                .Genres
                .ToArray()
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games
                        .Where(ga => ga.Purchases.Any())
                        .Select(ga => new
                        {
                            Id = ga.Id,
                            Title = ga.Name,
                            Developer = ga.Developer.Name,
                            Tags = String.Join(", ", ga.GameTags
                                .Select(gt => gt.Tag.Name)
                                .ToArray()),
                            Players = ga.Purchases.Count
                        })
                        .OrderByDescending(ga => ga.Players)
                        .ThenBy(ga => ga.Id)
                        .ToArray(),
                    TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

            string json = JsonConvert.SerializeObject(genres, Formatting.Indented);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string purchaseType)
        {
            StringBuilder sb = new StringBuilder();

            var type = Enum.Parse<PurchaseType>(purchaseType);

            var dataRaw = context.Users
                .Where(x => x.Cards.Any(y => y.Purchases.Any(p => p.Type == type)))

                .Select(u => new
                {
                    u.Username,
                    Purchases = u.Cards
                       .SelectMany(c => c.Purchases)
                       .Where(p => p.Type == type)
                       .OrderBy(p => p.Date)
                       .Select(p => new
                       {
                           Card = p.Card.Number,
                           Cvc = p.Card.Cvc,
                           Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                           Game = new
                           {
                               Title = p.Game.Name,
                               Genre = p.Game.Genre.Name,
                               Price = p.Game.Price
                           }
                       })
                           .ToArray(),
                    TotalMoneySpent = u.Cards
                    .SelectMany(c=>c.Purchases)
                       .Where(p => p.Type == type)
                       .Sum(p=>p.Game.Price)
                })
                .Where(u => u.Purchases.Any())
                .OrderByDescending(u => u.TotalMoneySpent)
                .ThenBy(u => u.Username)
                .ToArray();

            var users = dataRaw
                .Select(x => new ExportUserPurchaseTypeDtoWrapper
                {
                    Username = x.Username,
                    Purchases = x.Purchases.Select(x => new ExportUserPurchaseDto
                    {
                         CardNumber = x.Card,
                         CardCvc = x.Cvc,
                         Date = x.Date,
                         Game = new ExportUserPurchaseGameDto
                         {
                               Name = x.Game.Title,
                                Genre = x.Game.Genre,
                                 Price = x.Game.Price
                         }
                    })
                      .ToArray(),
                        TotalSpent = x.TotalMoneySpent
                })
                .ToArray();


            var serializer = new XmlSerializer(typeof(ExportUserPurchaseTypeDtoWrapper[]), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var writer = new StringWriter(sb);
            serializer.Serialize(writer, users, ns);

            return sb.ToString().TrimEnd();

        }
    }
}
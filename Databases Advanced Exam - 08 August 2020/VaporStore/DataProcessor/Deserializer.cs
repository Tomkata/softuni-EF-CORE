namespace VaporStore.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;

    public static class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";

        public const string SuccessfullyImportedGame = "Added {0} ({1}) with {2} tags";

        public const string SuccessfullyImportedUser = "Imported {0} with {1} cards";

        public const string SuccessfullyImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var getData = JsonConvert.DeserializeObject<ImportGameWrapperDto[]>(jsonString);

            var gamesList = new List<Game>();
            var genres = new HashSet<Genre>();
            var developers = new HashSet<Developer>();
            var tags = new HashSet<Tag>();

            var sb = new StringBuilder();
            foreach (var dto in getData)
            {


                if (!IsValid(dto)
                  || string.IsNullOrEmpty(dto.Name)
                  || string.IsNullOrEmpty(dto.ReleaseDate)
                  || string.IsNullOrEmpty(dto.Developer)
                  || string.IsNullOrEmpty(dto.Genre)
                  || dto.Price < 0
                  || dto.Tags.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var developer = developers.FirstOrDefault(x=>x.Name == dto.Developer);

                if (!developers.Contains(developer))
                {
                    developer = new Developer { Name = dto.Developer };
                    developers.Add(developer);
                }


                var genre = genres.FirstOrDefault(x => x.Name == dto.Genre);

                if (!genres.Contains(genre))
                {
                    genre = new Genre { Name = dto.Genre };
                    genres.Add(genre);
                }



                var game = new Game
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    ReleaseDate = DateTime.ParseExact(dto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Developer = developer,
                    Genre = genre,
                    GameTags = new List<GameTag>()
                };

                bool tagInvalid = false;
                foreach (var tagName in dto.Tags)
                {
                    var tag = tags.FirstOrDefault(x => x.Name == tagName);
                    if (string.IsNullOrEmpty(tagName))
                    {
                        tagInvalid = true;
                        break;
                    }
                    if (tags.Contains(tag))
                    {
                        game.GameTags.Add(new GameTag
                        {
                            Tag = tag
                        });
                        
                        continue;
                    }

                    var createTag = new Tag { Name = tagName };
                    game.GameTags.Add(new GameTag
                    {
                        Tag = createTag
                    });

                    tags.Add(createTag);
                }

                if (tagInvalid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                gamesList.Add(game);
                sb.AppendLine(string.Format(SuccessfullyImportedGame, game.Name, game.Genre.Name, game.GameTags.Count));
            }

            context.Games.AddRange(gamesList);
            context.SaveChanges();

            return sb.AppendLine().ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var getData = JsonConvert.DeserializeObject<ImportUsersDto[]>(jsonString);

            var users = new HashSet<User>();
            var sb = new StringBuilder();

            foreach (var dto in getData)
            {
                if (string.IsNullOrEmpty(dto.FullName)
                 ||  dto.FullName.Length > GlobalConstants.UserUsernameMaxLength
                 ||  dto.FullName.Length < GlobalConstants.UserUsernameMinLength
                 ||  string.IsNullOrEmpty(dto.UserName)
                 ||  !Regex.IsMatch(dto.FullName,GlobalConstants.UserFullNameRegex)
                 ||  string.IsNullOrEmpty(dto.Email)
                 ||   dto.Age > GlobalConstants.UserAgeMaxValue
                 ||   dto.Age < GlobalConstants.UserAgeMinValue)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var user = new User
                {
                    FullName = dto.FullName,
                    Username = dto.UserName,
                    Email = dto.Email,
                    Age = dto.Age,
                    Cards = new List<Card>()
                };
                var isValid = false;
                foreach (var card in dto.Cards)
                {
                    var type = Enum.TryParse<CardType>(card.Type,true,out var cardType);

                    if (string.IsNullOrEmpty(card.Number)
                      || card.CVC.Length > GlobalConstants.CardCvcMaxLength
                      || !Regex.IsMatch(card.Number,GlobalConstants.CardNumberRegex)
                      || !Regex.IsMatch(card.CVC,GlobalConstants.CardCvcRegex)
                      || !type)
                    {
                        sb.AppendLine(ErrorMessage);
                        isValid = true;
                        break;
                    }

                    user.Cards.Add(new Card
                    {
                         Number = card.Number,
                          Cvc = card.CVC,
                          Type = cardType
                    });
                }

                if (isValid)
                     continue;

                users.Add(user);
                sb.AppendLine(String.Format(SuccessfullyImportedUser,dto.UserName,dto.Cards.Count));
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(PurshaseImportWrapperDto),new XmlRootAttribute("Purchases"));

            using var reader = new StringReader(xmlString);

            var purchases = (PurshaseImportWrapperDto)serializer.Deserialize(reader)!;

            var purchaseSet = new HashSet<Purchase>();

            var sb = new StringBuilder();
            foreach (var dto in purchases.Purshases)
            {
                var type = Enum.TryParse<PurchaseType>(dto.Type, true, out var cardType);
                var dateParse = DateTime.TryParseExact(dto.Date, "dd/MM/yyyy HH:mm",CultureInfo.InvariantCulture,DateTimeStyles.None,out var date);
                if (string.IsNullOrEmpty(dto.Title)
                  ||  !type
                  ||   string.IsNullOrEmpty(dto.Key)
                  ||  !dateParse)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var game = context.Games.FirstOrDefault(x=>x.Name == dto.Title);
                var card = context.Cards.FirstOrDefault(x => x.Number == dto.Card);
                if (game == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if (card == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Purchase purchase = new Purchase
                {
                     Game = game,
                      Type = cardType,
                       ProductKey = dto.Key,
                        Date = date,
                         Card = card
                };
                purchaseSet.Add(purchase);
                sb.AppendLine(String.Format(SuccessfullyImportedPurchase, purchase.Game.Name,purchase.Card.User.Username));
            }

            context.Purchases.AddRange(purchaseSet);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
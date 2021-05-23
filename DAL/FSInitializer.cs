using FifaStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FifaStore.DAL
{
    public class FSInitializer : DropCreateDatabaseIfModelChanges<FSContext>
    {
        protected override void Seed(FSContext context)
        {
            /* Adding users, roles */
            var roleManager = new RoleManager<IdentityRole>(
                              new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var userManager = new UserManager<ApplicationUser>(
                              new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("Moderator"));
            roleManager.Create(new IdentityRole("User"));
            
            /* Adding nationalities */
            var nationalities = new List<Nationality>
            {
                new Nationality { Name = "Austria", Short = "AUS", Flag = "NationalityFlags/austria.png" },
                new Nationality { Name = "Argentyna", Short = "ARG", Flag = "NationalityFlags/argentyna.png" },
                new Nationality { Name = "Belgia", Short = "BEL", Flag = "NationalityFlags/belgia.png" },
                new Nationality { Name = "Dania", Short = "DEN", Flag = "NationalityFlags/dania.png" },
                new Nationality { Name = "Francja", Short = "FRA", Flag = "NationalityFlags/francja.png" },
                new Nationality { Name = "Holandia", Short = "NED", Flag = "NationalityFlags/holandia.png" },
                new Nationality { Name = "Polska", Short = "POL", Flag = "NationalityFlags/polska.png" },
                new Nationality { Name = "Portugalia", Short = "POR", Flag = "NationalityFlags/portugalia.png" },
                new Nationality { Name = "Włochy", Short = "ITA", Flag = "NationalityFlags/wlochy.png" }
            };
            nationalities.ForEach(n => context.Nationalities.Add(n));
            context.SaveChanges();

            /* Adding leagues */
            var leagues = new List<League>
            {
                new League { Name = "Premier League", Short = "ENG1", LeagueCrest = "LeagueCrests/pl.png"},
                new League { Name = "The Championship", Short = "ENG2", LeagueCrest = "LeagueCrests/championship.png"},
                new League { Name = "Ligue 1 Conforama", Short = "FRA1", LeagueCrest = "LeagueCrests/ligue1.png"},
                new League { Name = "LaLiga Santander", Short = "ESP1", LeagueCrest = "LeagueCrests/laliga.png"},
                new League { Name = "Serie A", Short = "ITA1", LeagueCrest = "LeagueCrests/seriea.png"},
                new League { Name = "Eredivisie", Short = "NED", LeagueCrest = "LeagueCrests/eredivisie.png"},
                new League { Name = "Bundesliga", Short = "GER1", LeagueCrest = "LeagueCrests/bundesliga.png"}
            };
            leagues.ForEach(l => context.Leagues.Add(l));
            context.SaveChanges();

            /* Adding clubs */
            var clubs = new List<Club>
            {
                new Club { Name = "FC Arsenal", Short = "ARS", ClubCrest = "ClubCrests/arsenal.png", League = leagues[0] },
                new Club { Name = "Aston Villa", Short = "AV", ClubCrest = "ClubCrests/aston_villa.png", League = leagues[0] },
                new Club { Name = "Chelsea FC", Short = "CHE", ClubCrest = "ClubCrests/chelsea.png", League = leagues[0] },
                new Club { Name = "FC Everton", Short = "EVE", ClubCrest = "ClubCrests/everton.png", League = leagues[0] },
                new Club { Name = "FC Liverpool", Short = "LIV", ClubCrest = "ClubCrests/liverpool.png", League = leagues[0] },
                new Club { Name = "Bayern Monachium", Short = "BM", ClubCrest = "ClubCrests/bayern.png", League = leagues[6] },
            };
            clubs.ForEach(c => context.Clubs.Add(c));
            context.SaveChanges();

            /* Adding footballers */
            var footballers = new List<Footballer>
            {
                new Footballer { FirstName = "Olivier", LastName = "Giroud", Alias = "", Club = clubs[2], Nationality = nationalities[4], Photo = "FootballerPortraits/giroud.png" },
                new Footballer { FirstName = "Virgil", LastName = "van Dijk", Alias = "", Club = clubs[4], Nationality = nationalities[5], Photo = "FootballerPortraits/vandijk.png" },
                new Footballer { FirstName = "Robert", LastName = "Lewandowski", Alias = "", Club = clubs[5], Nationality = nationalities[6], Photo = "FootballerPortraits/lewy.png" }
            };
            footballers.ForEach(f => context.Footballers.Add(f));
            context.SaveChanges();

            /* Adding card types */
            var cardTypes = new List<CardType>
            {
                new CardType { Name = "Gold", CardBorder = "CardBorders/0_gold.png" },
                new CardType { Name = "Gold rare", CardBorder = "CardBorders/1_gold.png" },
                new CardType { Name = "Silver", CardBorder = "CardBorders/0_silver.png" },
                new CardType { Name = "Silver rare", CardBorder = "CardBorders/1_silver.png" },
                new CardType { Name = "Bronze", CardBorder = "CardBorders/0_bronze.png" },
                new CardType { Name = "Bronze rare", CardBorder = "CardBorders/1_bronze.png" }
            };
            cardTypes.ForEach(c => context.CardTypes.Add(c));
            context.SaveChanges();

            /* Adding cards*/
            var cards = new List<Card>
            {
                new Card { CardType = cardTypes[1], Overall = 82, Pace = 46, Shooting = 81, Passing = 71, Dribbling = 73, Defending = 42, Physicality = 79, Footballer = footballers[0], Position = Position.N },
                new Card { CardType = cardTypes[1], Overall = 90, Pace = 77, Shooting = 60, Passing = 70, Dribbling = 72, Defending = 90, Physicality = 86, Footballer = footballers[1], Position = Position.ŚO },
                new Card { CardType = cardTypes[1], Overall = 89, Pace = 77, Shooting = 87, Passing = 74, Dribbling = 85, Defending = 41, Physicality = 82, Footballer = footballers[2], Position = Position.N }
            };
            cards.ForEach(c => context.Cards.Add(c));
            context.SaveChanges();
        }
    }
}
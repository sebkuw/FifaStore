using FifaStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace FifaStore.DAL
{
    public class FSContext : DbContext
    {
        public FSContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Footballer> Footballers { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Opinion> Opinions { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
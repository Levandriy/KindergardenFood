﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KindergardenFood.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KindergardenFoodDataBaseEntities : DbContext
    {
        public KindergardenFoodDataBaseEntities()
            : base("name=KindergardenFoodDataBaseEntities")
        {
            Database.CreateIfNotExists();
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Cooked> Cooked { get; set; }
        public virtual DbSet<Food> Food { get; set; }
        public virtual DbSet<Food_Norm> Food_Norm { get; set; }
        public virtual DbSet<Kids_eating> Kids_eating { get; set; }
        public virtual DbSet<Security_levels> Security_levels { get; set; }
        public virtual DbSet<Units> Units { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}

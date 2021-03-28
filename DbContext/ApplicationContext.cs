using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().HasKey(x=>x.Id);

            modelBuilder.Entity<Produto>().HasKey(x => x.Id);

            modelBuilder.Entity<Pedido>().HasKey(x => x.Id);
            modelBuilder.Entity<Pedido>().HasMany(x=>x.Itens).WithOne(x=>x.Pedido); //1:N (Pedido:Itens)
            modelBuilder.Entity<Pedido>().HasOne(x => x.Cadastro).WithOne(x => x.Pedido).IsRequired();//1:1 (Pedido:Cadastro)

            modelBuilder.Entity<ItemPedido>().HasKey(x => x.Id);
            modelBuilder.Entity<ItemPedido>().HasOne(x=>x.Pedido);//1:1 (ItemPedido:Pedido)
            modelBuilder.Entity<ItemPedido>().HasOne(x => x.Produto);//1:1 (ItemPedido:Produto)

            modelBuilder.Entity<Cadastro>().HasKey(x => x.Id);
            modelBuilder.Entity<Cadastro>().HasOne(x => x.Pedido);//1:1 (Cadastro:Pedido)
        }
    }
}

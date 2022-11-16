using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alura.Loja.Testes.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Criando uma nova compra, passando a qtd.
            var compra = new Compra();
            compra.Quantidade = 26;

            using (var context = new LojaContext())
            {
                var serviceProvider = context.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var cliente = context
                    .Clientes
                    .Include(c => c.EnderecoDeEntrega)
                    .FirstOrDefault(); 

                Console.WriteLine($"Endereço de entrega: {cliente.EnderecoDeEntrega.Logradouro}.");

                //1
                //var produto = context
                //    .Produtos
                //    .Include(p => p.Compras)
                //    .Where(p => p.Id == 5002)
                //    .FirstOrDefault();


                //Para o carregamento explícito, tivemos que remover o Include, pois a cláusula 'Where' será apenas do tipo 'Produto'.
                var produto = context
                    .Produtos
                    .Where(p => p.Id == 5002)
                    .FirstOrDefault();

                //Carregamento Explícito
                context.Entry(produto)
                    .Collection(p => p.Compras)
                    .Query()
                    .Where(c => c.Preco > 10)
                    .Load();


                //compra.Produto (recebe) o produto com Id (p.Id == 5002) que é o Pão Francês.
                compra.Produto = produto;
                compra.Preco = produto.PrecoUnitario * compra.Quantidade;

                //2
                //var compra = context
                //    .Compras
                //    .Include(p => p.Produto)
                //    .Where(p => p.Id == 2)
                //    .FirstOrDefault();

                //1
                Console.WriteLine($"Mostrando as compras do produto: {produto.Nome}");

                //2
                //Console.WriteLine($"Mostrando as compras do produto: {compra.Produto.Nome}");

                foreach (var item in produto.Compras)
                {
                    Console.WriteLine($"Compra de {item.Quantidade} do produto {produto.Nome}.");
                }

                //Adiciona e salva as mudanças no contexto.
                context.Compras.Add(compra);
                context.SaveChanges();
            }
        }

        private static void ExibeProdutosDaPromocao()
        {
            using (var context2 = new LojaContext())
            {
                var serviceProvider = context2.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var promocao = context2
                    .Promocoes
                    .Include(p => p.Produtos)
                    .ThenInclude(pp => pp.Produto)
                    .FirstOrDefault();

                Console.WriteLine("\nMostrando os produtos da promoção...");

                foreach (var item in promocao.Produtos)
                {
                    Console.WriteLine(item.Produto);
                }
            }
        }

        private static void IncluirPromocao()
        {
            using (var context = new LojaContext())
            {
                //Logar o SQL no console.
                var serviceProvider = context.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var promocao = new Promocao();
                promocao.Descricao = "Queima Total Janeiro de 2022";
                promocao.DataInicio = new DateTime(2022, 1, 1);
                promocao.DataTermino = new DateTime(2022, 1, 31);

                var produtos = context
                    .Produtos
                    .Where(p => p.Categoria == "Bebidas")
                    .ToList();

                foreach (var item in produtos)
                {
                    promocao.IncluiProduto(item);
                }

                context.Promocoes.Add(promocao);

                ExibeEntries(context.ChangeTracker.Entries());

                context.SaveChanges();
            }
        }

        private static void UmParaUm()
        {
            var fulano = new Cliente();
            fulano.Nome = "Fulano Detal";
            fulano.EnderecoDeEntrega = new Endereco()
            {
                Numero = 12,
                Logradouro = "Rua dos Inválidos",
                Complemento = "Casa",
                Bairro = "Centro",
                Cidade = "Anastexas"
            };

            using (var context = new LojaContext())
            {
                var serviceProvider = context.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var deleteFulano = context.Clientes.SingleOrDefault(x => x.Id == 1);

                context.Clientes.Remove(deleteFulano);
                //context.Clientes.Add(fulano);
                context.SaveChanges();
            }
        }

        private static void ExibeEntries(IEnumerable<EntityEntry> entries)
        {
            foreach (var e in entries)
            {
                Console.WriteLine(e.Entity.ToString() + " - " + e.State);
            }
        }

        private static void MuitosParaMuitos()
        {
            var p1 = new Produto() { Nome = "Suco de Laranja", Categoria = "Bebidas", PrecoUnitario = 8.79, Unidade = "Litros" };
            var p2 = new Produto() { Nome = "Café", Categoria = "Bebidas", PrecoUnitario = 12.45, Unidade = "Gramas" };
            var p3 = new Produto() { Nome = "Macarrão", Categoria = "Alimentos", PrecoUnitario = 4.23, Unidade = "Kilogramas" };

            var promocaoDePascoa = new Promocao();
            promocaoDePascoa.Descricao = "Páscia Feliz";
            promocaoDePascoa.DataInicio = DateTime.Now;
            promocaoDePascoa.DataTermino = DateTime.Now.AddMonths(3);

            promocaoDePascoa.IncluiProduto(p1);
            promocaoDePascoa.IncluiProduto(p2);
            promocaoDePascoa.IncluiProduto(p3);



            //compra de 6 pães
            //var paoFrances = new Produto();
            //paoFrances.Nome = "Pão Francês";
            //paoFrances.PrecoUnitario = 0.40;
            //paoFrances.Unidade = "Unidade";
            //paoFrances.Categoria = "Padaria";

            //var compra = new Compra();
            //compra.Quantidade = 6;
            //compra.Produto = paoFrances;
            //compra.Preco = paoFrances.PrecoUnitario * compra.Quantidade;

            using (var context = new LojaContext())
            {
                var serviceProvider = context.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                //context.Compras.Add(compra);

                //context.Promocoes.Add(promocaoDePascoa);

                var promocao = context.Promocoes.Find(1);
                context.Promocoes.Remove(promocao);

                ExibeEntries(context.ChangeTracker.Entries());

                //context.SaveChanges();

                ExibeEntries(context.ChangeTracker.Entries());
            }
        }

        private static void ChangeTrackerEntityComLog()
        {
            using (var context = new LojaContext())
            {

                var serviceProvider = context.GetInfrastructure<IServiceProvider>();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(SqlLoggerProvider.Create());

                var produtos = context.Produtos.ToList();

                ExibeEntries(context.ChangeTracker.Entries());

                var novoProduto = new Produto()
                {
                    Nome = "Desinfetante",
                    Categoria = "Limpeza",
                    PrecoUnitario = 2.99
                };
                context.Produtos.Add(novoProduto);

                var p1 = produtos.First();
                context.Produtos.Remove(p1);

                ExibeEntries(context.ChangeTracker.Entries());

                context.SaveChanges();

                ExibeEntries(context.ChangeTracker.Entries());
            }
        }
    }

}

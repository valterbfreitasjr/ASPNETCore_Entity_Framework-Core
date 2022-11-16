using System;
using System.Collections.Generic;
using System.Linq;

namespace Alura.Loja.Testes.ConsoleApp
{
    class ProdutoDAOEntity : IDisposable, IProdutoDAO
    {
        private LojaContext context;

        public ProdutoDAOEntity()
        {
            this.context = new LojaContext();
        }

        public void Adicionar(Produto p)
        {
            context.Produtos.Add(p);
            context.SaveChanges();
        }

        public void Atualizar(Produto p)
        {
            context.Produtos.Update(p);
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public IList<Produto> Produtos()
        {
            return context.Produtos.ToList();
        }

        public void Remover(Produto p)
        {
            context.Produtos.Remove(p);
            context.SaveChanges();
        }
    }
}

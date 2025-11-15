using System.Linq;
using AspNetMinimalsApi.Dominio.Entidades;
using AspNetMinimalsApi.Dominio.Interfaces;
using AspNetMinimalsApi.DTOs;
using AspNetMinimalsApi.Infraestrutura.Db;

namespace AspNetMinimalsApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto db)
    {
        _contexto = db;
    }

    public void Apagar(Veiculo Veiculo)
    {
        _contexto.Veiculos.Remove(Veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo Veiculo)
    {
        _contexto.Veiculos.Update(Veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir (Veiculo Veiculo)
    {
        _contexto.Veiculos.Add(Veiculo);
        _contexto.SaveChanges();
    }

   public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
{
    var query = _contexto.Veiculos.AsQueryable();
    if (!string.IsNullOrEmpty(nome))
        query = query.Where(v => v.Nome.ToLower().Contains(nome.ToLower()));
    if (!string.IsNullOrEmpty(marca))
        query = query.Where(v => v.Marca.ToLower().Contains(marca.ToLower()));
    return query.ToList();
}
}
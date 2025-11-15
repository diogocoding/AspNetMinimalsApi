using AspNetMinimalsApi.Dominio.Entidades;
using AspNetMinimalsApi.Dominio.Interfaces;
using AspNetMinimalsApi.DTOs;
using AspNetMinimalsApi.Infraestrutura.Db;

namespace AspNetMinimalsApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto db)
    {
        _contexto = db;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
     
        return adm;
    }
}
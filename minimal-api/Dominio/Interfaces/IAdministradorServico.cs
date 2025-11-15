using AspNetMinimalsApi.Dominio.Entidades;
using AspNetMinimalsApi.DTOs;

namespace AspNetMinimalsApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}
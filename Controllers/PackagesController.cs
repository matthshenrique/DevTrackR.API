using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Persistence;
using DevTrackR.API.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrackR.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        public PackagesController(IPackageRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obter todos os pacotes.
        /// </summary> 
        /// <response code="200">OK</response>
        [HttpGet]
        public IActionResult GetAll()
        {
            var packages = _repository.GetAll();

            return Ok(packages);
        }

        /// <summary>
        /// Obter pacote por código.
        /// </summary> 
        /// <param name="model">Código do pacote</param>
        /// <returns>Objeto encontrado</returns>
        /// <response code="200">OK</response>
        /// <response code="404">Pacote não encontrado</response>
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code)
        {
            var package = _repository
                .GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        /// <summary>
        /// Cadastro de um pacote.
        /// </summary>
        /// <remarks>  
        ///{
        ///  "title": "Pacote com Coleção de selos",
        ///  "weight": 4
        ///}
        /// </remarks>  
        /// <param name="model">Dados do pacote</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        public IActionResult Post(AddPackageInputModel model)
        {
            if (model.Title.Length < 10)
            {
                return BadRequest("Title length must be at least 10 characters long.");
            }

            var package = new Package(model.Title, model.Weight);

            _repository.Add(package);

            return CreatedAtAction(
                "GetByCode",
                new { code = package.Code },
                package);
        }

        /// <summary>
        /// Atualização do status de um pacote.
        /// </summary>
        /// <remarks>  
        ///{
        ///  "status": "Pacote saiu para entrega",
        ///  "delivered": false
        ///}
        /// </remarks>  
        /// <param name="code">Código do pacote</param> 
        /// <param name="model">Dados do pacote</param>
        /// <returns>Objeto atualizado</returns>
        /// <response code="204">Atualização realizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost("{code}/updates")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model)
        {
            var package = _repository.GetByCode(code);

            if (package == null)
            {
                return NotFound();
            }

            package.AddUpdate(model.Status, model.Delivered);

            _repository.Update(package);

            return NoContent();
        }
    }
}
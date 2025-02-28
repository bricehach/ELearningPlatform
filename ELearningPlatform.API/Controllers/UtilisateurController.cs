using Microsoft.AspNetCore.Mvc;
using ELearningPlatform.DAL.Models;
using ELearningPlatform.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurRepository _utilisateurRepository;

        public UtilisateurController(IUtilisateurRepository utilisateurRepository)
        {
            _utilisateurRepository = utilisateurRepository;
        }

        // ✅ Endpoint pour récupérer tous les utilisateurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            var utilisateurs = await _utilisateurRepository.GetAllAsync();
            return Ok(utilisateurs);
        }

        // ✅ Endpoint pour récupérer un utilisateur par ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            var utilisateur = await _utilisateurRepository.GetByIdAsync(id);
            if (utilisateur == null)
                return NotFound();

            return Ok(utilisateur);
        }

        // ✅ Endpoint pour ajouter un nouvel utilisateur
        [HttpPost]
        public async Task<ActionResult> AddUtilisateur(Utilisateur utilisateur)
        {
            await _utilisateurRepository.AddAsync(utilisateur);
            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, utilisateur);
        }

        // ✅ Endpoint pour modifier un utilisateur
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUtilisateur(int id, Utilisateur utilisateur)
        {
            if (id != utilisateur.Id)
                return BadRequest();

            await _utilisateurRepository.UpdateAsync(utilisateur);
            return NoContent();
        }

        // ✅ Endpoint pour supprimer un utilisateur
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUtilisateur(int id)
        {
            await _utilisateurRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Mineman.Service.Repositories;
using Mineman.Web.Models.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mineman.Web.Controllers
{
    [Route("api/player")]
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> Get(string uuid)
        {
            var profile = await _playerRepository.Get(uuid);
            if (profile == null)
                return NoContent();

            return Ok(new ClientPlayerProfile
            {
                Id = profile.UUID,
                Name = profile.Name,
                SkinUrl = profile.SkinURL
            });
        }
    }
}

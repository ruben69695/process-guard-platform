using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProcessGuardAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProcessGuardAPI.Controllers
{
    [Route("api/processitems")]
    public class ProcessItems : Controller
    {
        private readonly processguarddbContext _context;

        public ProcessItems(processguarddbContext context)
        {
            _context = context;
        }

        // GET: api/processitems
        [HttpGet]
        public IEnumerable<ProcessList> Get()
        {
            return _context.ProcessList.ToList();
        }

        // GET: api/processitems/{color}
        [HttpGet("{color}")]
        public List<ProcessList> GetBlacks(string color)
        {
            var items = _context.ProcessList
                .Where(process => process.Color == color)
                .ToList();

            return items;
        }

        // GET api/processitems/item/{name}
        [HttpGet("item/{name}")]
        public IActionResult Get(string name)
        {
            var item = _context.ProcessList
                .FirstOrDefault(x => x.Exe.Contains(name));

            if (item == null)
                return NotFound();
            return new JsonResult(item);
        }

        // POST api/processitems
        [HttpPost]
        public IActionResult Post([FromBody]ProcessList item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            if (_context.ProcessList.Count(x => x.Exe == item.Exe && x.Filename == item.Filename) > 0)
            {
                return NotFound();
            }

            _context.ProcessList.Add(item);
            _context.SaveChanges();
            return new JsonResult(item);
        }

        // PUT api/processitems/item/{name}
        [HttpPut("item/{name}")]
        public IActionResult Put(string name, [FromBody]ProcessList item)
        {
            if (item == null)
                return BadRequest();

            var process = _context.ProcessList.FirstOrDefault(x => x.Exe == item.Exe && x.Filename == item.Filename);
            if (process == null)
                return NotFound();
            _context.ProcessList.Remove(process);
            _context.SaveChanges();

            var blackProcess = new ProcessList { Exe = item.Exe, Description = item.Description, Color = item.Color, Filename = item.Filename };
            _context.ProcessList.Add(blackProcess);
            _context.SaveChanges();
            return new NoContentResult();
        }

        // DELETE api/processitems/item/{name}
        [HttpDelete("item/{name}")]
        public IActionResult Delete(string name, string filename)
        {
            var item = _context.ProcessList.FirstOrDefault(x => x.Exe == name && x.Filename == filename);
            if (item == null)
                return NotFound();
            _context.ProcessList.Remove(item);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}

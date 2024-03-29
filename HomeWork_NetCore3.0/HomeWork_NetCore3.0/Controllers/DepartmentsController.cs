﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWork_NetCore3._0.Models;

namespace HomeWork_NetCore3._0.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.Where(x=>x.IsDeleted==false).ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department =
                await _context.Department.FirstOrDefaultAsync(x => x.DepartmentId == id && x.IsDeleted == false);//.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        [HttpGet("vwDepartmentCourseCount")]
        public async Task<ActionResult<List<VwDepartmentCourseCount>>> GetVwDepartmentCourseCount()
        {
            var result = _context.VwDepartmentCourseCount
                .FromSqlInterpolated($"SELECT * FROM dbo.vwDepartmentCourseCount").AsNoTracking()
                .ToList();
            return result;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            //_context.Update(department);
            //_context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.Database
                    .ExecuteSqlInterpolatedAsync(
                        $"EXECUTE dbo.Department_Update {department.DepartmentId}, {department.Name}, {department.Budget}, {department.StartDate}, {department.InstructorId}, {department.RowVersion}");
                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXECUTE dbo.Department_Insert {department.Name}, {department.Budget}, {department.StartDate}, {department.InstructorId}");

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            //await _context.Database.ExecuteSqlInterpolatedAsync($"EXECUTE Department_Delete {id},{department.RowVersion}");
            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();
            department.IsDeleted = true;
            _context.Department.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}

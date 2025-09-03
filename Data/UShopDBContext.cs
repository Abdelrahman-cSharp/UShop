using Microsoft.EntityFrameworkCore;

namespace UShop.Data;


public class UShopDBContext : DbContext
{

	public UShopDBContext(DbContextOptions<UShopDBContext> options)
	: base(options)
	{
	}

	//public DbSet<Department> Departments { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Seed Data

		/*modelBuilder.Entity<Department>().HasData(
			new Department { Id = 1, Name = "HR" },
			new Department { Id = 2, Name = "Finance" },
			new Department { Id = 3, Name = "IT" }
		);

		modelBuilder.Entity<Employee>().HasData(
			new Employee { Id = 1, DepartmentId = 1, Name = "Alice Johnson", Salary = 40000 },
			new Employee { Id = 2, DepartmentId = 2, Name = "Bob Smith", Salary = 5000 },
			new Employee { Id = 3, DepartmentId = 3, Name = "Charlie Brown", Salary = 10000 },
			new Employee { Id = 4, DepartmentId = 1, Name = "Max John", Salary = 80000 }
		);

		modelBuilder.Entity<Project>().HasData(
			new Project { Id = 1, Name = "Recruitment System" },
			new Project { Id = 2, Name = "Payroll Automation" },
			new Project { Id = 3, Name = "Network Upgrade" }
		);

		modelBuilder.Entity<Employee>()
			.HasMany(e => e.Projects)
			.WithMany(p => p.Employees)
			.UsingEntity(j => j.HasData(
				new { EmployeesId = 1, ProjectsId = 1 },
				new { EmployeesId = 2, ProjectsId = 2 },
				new { EmployeesId = 3, ProjectsId = 3 },
				new { EmployeesId = 4, ProjectsId = 1 },
				new { EmployeesId = 4, ProjectsId = 3 }
			));*/
	}
}

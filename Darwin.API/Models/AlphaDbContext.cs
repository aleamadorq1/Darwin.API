using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Alpha.API.Models;

public partial class AlphaDbContext : DbContext
{
    public AlphaDbContext()
    {
    }

    public AlphaDbContext(DbContextOptions<AlphaDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Labor> Labors { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModuleCompositeDetail> ModuleCompositeDetails { get; set; }

    public virtual DbSet<ModulesComposite> ModulesComposites { get; set; }

    public virtual DbSet<ModulesLabor> ModulesLabors { get; set; }

    public virtual DbSet<ModulesMaterial> ModulesMaterials { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectAllowance> ProjectAllowances { get; set; }

    public virtual DbSet<ProjectLabor> ProjectLabors { get; set; }

    public virtual DbSet<ProjectMaterial> ProjectMaterials { get; set; }

    public virtual DbSet<ProjectModule> ProjectModules { get; set; }

    public virtual DbSet<ProjectModuleComposite> ProjectModuleComposites { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Filename=./Database/AlphaDB.db")
                .ConfigureWarnings(warnings =>
                {
                    warnings.Default(WarningBehavior.Ignore)
                            .Log(CoreEventId.FirstWithoutOrderByAndFilterWarning, CoreEventId.RowLimitingOperationWithoutOrderByWarning)
                            .Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
                });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName).HasColumnName("category_name");
            entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasForeignKey(d => d.ParentCategoryId);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ClientName).HasColumnName("client_name");
            entity.Property(e => e.ContactInfo).HasColumnName("contact_info");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
        });

        modelBuilder.Entity<Labor>(entity =>
        {
            entity.ToTable("Labor");

            entity.Property(e => e.LaborId).HasColumnName("labor_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HourlyRate).HasColumnName("hourly_rate");
            entity.Property(e => e.LaborType).HasColumnName("labor_type");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CifPrice).HasColumnName("cif_price");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.MaterialName).HasColumnName("material_name");
            entity.Property(e => e.Sku).HasColumnName("sku");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TaxStatus).HasColumnName("tax_status");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
            entity.Property(e => e.Uom).HasColumnName("uom");

            entity.HasOne(d => d.Category).WithMany(p => p.Materials)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Supplier).WithMany(p => p.Materials)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.ModuleName).HasColumnName("module_name");
            entity.Property(e => e.ModuleType).HasColumnName("module_type");
        });

        modelBuilder.Entity<ModuleCompositeDetail>(entity =>
        {
            entity.ToTable("Module_Composite_Details");

            entity.HasIndex(e => e.ModuleCompositeDetailId, "IX_Module_Composite_Details_module_composite_detail_id").IsUnique();

            entity.Property(e => e.ModuleCompositeDetailId).HasColumnName("module_composite_detail_id");
            entity.Property(e => e.ModuleCompositeId).HasColumnName("module_composite_id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ModuleComposite).WithMany(p => p.ModuleCompositeDetails).HasForeignKey(d => d.ModuleCompositeId);

            entity.HasOne(d => d.Module).WithMany(p => p.ModuleCompositeDetails).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<ModulesComposite>(entity =>
        {
            entity.HasKey(e => e.ModuleCompositeId);

            entity.ToTable("Modules_Composite");

            entity.Property(e => e.ModuleCompositeId).HasColumnName("module_composite_id");
            entity.Property(e => e.CompositeName).HasColumnName("composite_name");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<ModulesLabor>(entity =>
        {
            entity.HasKey(e => e.ModuleLaborId);

            entity.ToTable("Modules_Labor");

            entity.Property(e => e.ModuleLaborId).HasColumnName("module_labor_id");
            entity.Property(e => e.HoursRequired).HasColumnName("hours_required");
            entity.Property(e => e.LaborId).HasColumnName("labor_id");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");

            entity.HasOne(d => d.Labor).WithMany(p => p.ModulesLabors)
                .HasForeignKey(d => d.LaborId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Module).WithMany(p => p.ModulesLabors)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ModulesMaterial>(entity =>
        {
            entity.HasKey(e => e.ModuleMaterialId);

            entity.ToTable("Modules_Materials");

            entity.Property(e => e.ModuleMaterialId).HasColumnName("module_material_id");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Material).WithMany(p => p.ModulesMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Module).WithMany(p => p.ModulesMaterials)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.OrganizationName).HasColumnName("organization_name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("DATE")
                .HasColumnName("end_date");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.LocationAddress).HasColumnName("location_address");
            entity.Property(e => e.LocationCoordinates).HasColumnName("location_coordinates");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.ProfitMargin).HasColumnName("profit_margin");
            entity.Property(e => e.ProjectName).HasColumnName("project_name");
            entity.Property(e => e.StartDate)
                .HasColumnType("DATE")
                .HasColumnName("start_date");
            entity.Property(e => e.TotalArea).HasColumnName("total_area");
            entity.Property(e => e.TotalFloors).HasColumnName("total_floors");

            entity.HasOne(d => d.Client).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Organization).WithMany(p => p.Projects).HasForeignKey(d => d.OrganizationId);
        });

        modelBuilder.Entity<ProjectAllowance>(entity =>
        {
            entity.HasKey(e => e.AllowanceId);

            entity.ToTable("Project_Allowances");

            entity.Property(e => e.AllowanceId).HasColumnName("allowance_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LaborId).HasColumnName("labor_id");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Labor).WithMany(p => p.ProjectAllowances)
                .HasForeignKey(d => d.LaborId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectAllowances)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectLabor>(entity =>
        {
            entity.ToTable("Project_Labor");

            entity.HasIndex(e => e.ProjectLaborId, "IX_Project_Labor_project_labor_id").IsUnique();

            entity.Property(e => e.ProjectLaborId).HasColumnName("project_labor_id");
            entity.Property(e => e.HourlyRate).HasColumnName("hourly_rate");
            entity.Property(e => e.LaborId).HasColumnName("labor_id");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Labor).WithMany(p => p.ProjectLabors)
                .HasForeignKey(d => d.LaborId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Module).WithMany(p => p.ProjectLabors).HasForeignKey(d => d.ModuleId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectLabors)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectMaterial>(entity =>
        {
            entity.ToTable("Project_Materials");

            entity.Property(e => e.ProjectMaterialId).HasColumnName("project_material_id");
            entity.Property(e => e.CifPrice).HasColumnName("cif_price");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TaxStatus).HasColumnName("tax_status");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price");

            entity.HasOne(d => d.Material).WithMany(p => p.ProjectMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Module).WithMany(p => p.ProjectMaterials).HasForeignKey(d => d.ModuleId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMaterials)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectModule>(entity =>
        {
            entity.ToTable("Project_Modules");

            entity.HasIndex(e => e.ProjectModuleId, "IX_Project_Modules_project_module_Id").IsUnique();

            entity.Property(e => e.ProjectModuleId).HasColumnName("project_module_Id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Module).WithMany(p => p.ProjectModules)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectModules)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ProjectModuleComposite>(entity =>
        {
            entity.ToTable("Project_Module_Composites");

            entity.HasIndex(e => e.ProjectModuleCompositeId, "IX_Project_Module_Composites_project_module_composite_id").IsUnique();

            entity.Property(e => e.ProjectModuleCompositeId).HasColumnName("project_module_composite_id");
            entity.Property(e => e.ModuleCompositeId).HasColumnName("module_composite_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.ModuleComposite).WithMany(p => p.ProjectModuleComposites)
                .HasForeignKey(d => d.ModuleCompositeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectModuleComposites)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactInfo).HasColumnName("contact_info");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.SupplierName).HasColumnName("supplier_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace P1611706.Model
{
    public class P1611706DB: Microsoft.EntityFrameworkCore.DbContext
    {

        public P1611706DB(Microsoft.EntityFrameworkCore.DbContextOptions<P1611706DB> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VendorProducts>().HasKey(t => new { t.VendorNo, t.ProductNo });

            modelBuilder.Entity<Records>().HasKey(t => new { t.BagNo, t.updateDateTime });

            modelBuilder.Entity<tbEmpGroup>().HasKey(t => new { t.EmployeeNo, t.GroupID });

			modelBuilder.Entity<OrderLog>().HasKey(t => new { t.LogDate, t.LogTime, t.OrderType, t.OrderNo, t.ProductNo });
            modelBuilder.Entity<vRecords>().HasKey(t => new { t.BagNo, t.WarehouseNo });
            modelBuilder.Entity<InventoryBody>().HasKey(t => new { t.InventoryNo, t.BagNo });
            modelBuilder.Entity<InventoryAdjust>().HasKey(t => new { t.InventoryNo, t.BagNo });

        }


        public DbSet<ProductInfoes> ProductInfoes { get; set; }
        public DbSet<VendorProducts> VendorProducts { get; set; }
        public DbSet<Records> Records { get; set; }
        public DbSet<tbGroup> tbGroup { get; set; }
        public DbSet<tbProgram> tbProgram { get; set; }
        public DbSet<tbEmployee> tbEmployee { get; set; }
        public DbSet<tbEmpGroup> tbEmpGroup { get; set; }
		public DbSet<PackingLists> PackingLists { get; set; }
		public DbSet<OrderLog> OrderLog { get; set; }
        public DbSet<InventoryHead> InventoryHead { get; set; }
        public DbSet<InventoryBody> InventoryBody { get; set; }
        public DbSet<vRecords> vRecords { get; set; }
        public DbSet<InventoryAdjust> InventoryAdjust { get; set; }

    }

	public class LeaderDemoDB : Microsoft.EntityFrameworkCore.DbContext
	{

		public LeaderDemoDB(Microsoft.EntityFrameworkCore.DbContextOptions<LeaderDemoDB> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<COPTG>().HasKey(t => new { t.TG001, t.TG002 });
			modelBuilder.Entity<MOCTA>().HasKey(t => new { t.TA001, t.TA002 });
			modelBuilder.Entity<INVTA>().HasKey(t => new { t.TA001, t.TA002 });
			modelBuilder.Entity<INVTB>().HasKey(t => new { t.TB001, t.TB002, t.TB003 });
			modelBuilder.Entity<PURTG>().HasKey(t => new { t.TG001, t.TG002 });
			modelBuilder.Entity<INVTF>().HasKey(t => new { t.TF001, t.TF002 });
			modelBuilder.Entity<INVTH>().HasKey(t => new { t.TH001, t.TH002 });
			modelBuilder.Entity<COPTI>().HasKey(t => new { t.TI001, t.TI002 });
			modelBuilder.Entity<COPTN>().HasKey(t => new { t.TN001, t.TN002 });

		}

		public DbSet<COPTG> COPTG { get; set; }
		public DbSet<MOCTA> MOCTA { get; set; }
		public DbSet<INVTA> INVTA { get; set; }
		public DbSet<INVTB> INVTB { get; set; }
		public DbSet<PURTG> PURTG { get; set; }
		public DbSet<INVTF> INVTF { get; set; }
		public DbSet<INVTH> INVTH { get; set; }
		public DbSet<COPTI> COPTI { get; set; }
		public DbSet<COPTN> COPTN { get; set; }
		public DbSet<CMSMB> CMSMB { get; set; }
		public DbSet<CMSMC> CMSMC { get; set; }


	}
	

}

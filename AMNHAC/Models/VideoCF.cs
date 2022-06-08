using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace AMNHAC.Models
{
    public partial class VideoCF : DbContext
    {
        public VideoCF()
            : base("name=VideoCF")
        {
        }

        public virtual DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}

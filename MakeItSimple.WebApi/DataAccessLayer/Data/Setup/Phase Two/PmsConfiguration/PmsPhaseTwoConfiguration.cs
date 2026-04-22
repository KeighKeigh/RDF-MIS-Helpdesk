using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.PmsConfiguration
{
    public class PmsPhaseTwoConfiguration : IEntityTypeConfiguration<PmsPhaseTwo>
    {
        public void Configure(EntityTypeBuilder<PmsPhaseTwo> builder)
        {
            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.OneChargingMIS)
            .WithMany()
            .HasForeignKey(u => u.ChargingCode)
            .HasPrincipalKey(o => o.code)
            .OnDelete(DeleteBehavior.SetNull);

        }
    }
}

using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.PmsConfiguration
{
    public class PmsPhaseTwoAnswerConfiguration : IEntityTypeConfiguration<PmsPhaseTwoAnswer>
    {
        public void Configure(EntityTypeBuilder<PmsPhaseTwoAnswer> builder)
        {
            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

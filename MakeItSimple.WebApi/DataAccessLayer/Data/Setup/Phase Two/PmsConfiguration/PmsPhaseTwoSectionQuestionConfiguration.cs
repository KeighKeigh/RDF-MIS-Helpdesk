using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.PmsConfiguration
{
    public class PmsPhaseTwoSectionQuestionConfiguration : IEntityTypeConfiguration<PmsSectionQuestion>
    {
        public void Configure(EntityTypeBuilder<PmsSectionQuestion> builder)
        {
            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.UpdatedByUser)
           .WithMany()
           .HasForeignKey(u => u.UpdatedBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}

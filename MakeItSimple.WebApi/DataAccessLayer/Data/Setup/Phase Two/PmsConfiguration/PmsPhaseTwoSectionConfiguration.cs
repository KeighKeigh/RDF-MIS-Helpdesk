//using MakeItSimple.WebApi.Models.Phase_Two.PMS;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.PmsConfiguration
//{
//    public class PmsPhaseTwoSectionConfiguration : IEntityTypeConfiguration<PmsPhaseTwoSection>
//    {
//        public void Configure(EntityTypeBuilder<PmsPhaseTwoSection> builder)
//        {
//            builder.HasOne(u => u.AddedByUser)
//           .WithMany()
//           .HasForeignKey(u => u.AddedBy)
//           .OnDelete(DeleteBehavior.Restrict);

//        }
//    }
//}

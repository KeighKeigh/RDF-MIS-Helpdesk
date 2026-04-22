using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two
{
    public class PmsError
    {
        public static Error PmsTechnicianNotExist() =>
        new Error("Pms.PmsTechnicianNotExist", "Pms technician not exist!");
        public static Error PmsNotExist() =>
        new Error("Pms.PmsNotExist", "Pms not exist!");

        public static Error PmsAlreadyInApproval() =>
        new Error("Pms.PmsAlreadyInApproval", "Pms already in approval!");

        public static Error PmsNotAuthorized() =>
        new Error("Pms.PmsNotAuthorized", "Pms not authorized!");

        public static Error PmsAttachmentNotExist() =>
        new Error("Pms.PmsAttachmentNotExist", "Pms attachment not exist!");
        public static Error SiteAlreadyExist() =>
        new Error("Pms.SiteAlreadyExist", "Site Name already exist!");

        public static Error SiteDoesNotExist() =>
        new Error("Pms.SiteDoesNotExist", "Site Does not exist!");

        public static Error SiteInUse(string site) =>
        new Error("Pms.SiteInUse", $"Site {site} is being use!");

        public static Error SiteBusinessUnitDoesNotExist() =>
        new Error("Pms.SiteBusinessUnitDoesNotExist", "404:Does not exist!");

        public static Error SiteIsInactive(string site) =>
        new Error("Pms.SiteIsInactive", $"Site {site} is inactive!");


        public static Error BusinessUnitDoesNotExist() =>
        new Error("Pms.BusinessUnitDoesNotExist", "Business unit Does not exist!");

        public static Error BusinessAlreadyExist() =>
        new Error("Pms.BusinessAlreadyExist", "Business unit already exist!");

        public static Error DataAlreadyExist() =>
        new Error("Pms.DataALreadyExist", "Data already exist!");

        public static Error TypeDoesNotExist() =>
        new Error("Pms.TypeDoesNotExist", "Type Does not exist!");

        public static Error ScheduledPmsDoesNotExist() =>
        new Error("Pms.ScheduledPmsDoesNotExist", "Scheduled Pms Does not exist!");

        public static Error QuestionDoesNotExist() =>
        new Error("Pms.QuestionDoesNotExist", "Question Does not exist!");

        public static Error CheckListDoesNotExist() =>
        new Error("Pms.CheckListDoesNotExist", "Checklist Does not exist!");

    }
}

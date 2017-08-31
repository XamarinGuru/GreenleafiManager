using System.ComponentModel.DataAnnotations;

namespace GreenleafiManager.Api.ViewModels {
    public class ChangeEmailViewModel {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string NewEmail { get; set; }
    }
}
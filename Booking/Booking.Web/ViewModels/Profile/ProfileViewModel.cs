using Booking.Web.ViewModels.Manage;

namespace Booking.Web.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        public UserInfoViewModel UserInfo { get; set; }

        public ChangePasswordViewModel ChangePasswordForm { get; set; }

        public bool IsOwner { get; set; }
    }
}
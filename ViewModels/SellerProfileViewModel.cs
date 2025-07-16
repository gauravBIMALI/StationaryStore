namespace ClzProject.ViewModels
{
    public class SellerProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string BusinessName { get; set; }
        public string BusinessType { get; set; }

        // Fix for CS1061: Adding the missing property 'ProfileImagePath'  
        public string ProfileImagePath { get; set; }
        //public bool EmailConfirmed { get; internal set; }
    }
}
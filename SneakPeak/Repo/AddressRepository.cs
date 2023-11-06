
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Data;
using SneakPeak.Models;
using System.Net;

namespace SneakPeak.Repo
{
    public class AddressRepository: IAddressRepository
    {
        private readonly SneakPeakDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<SneakPeakUser> _userManager;

        public AddressRepository(SneakPeakDbContext db,
            UserManager<SneakPeakUser> userManager,
             IHttpContextAccessor httpContextAccessor) {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<bool> SaveContact(Contact contact)
        {
            if (contact == null)
            {
                return false;
            }
            _db.Contact.Add(contact);
            _db.SaveChanges();
            return true;
        }
        

            public async Task<Address> UserAddress()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            var address = await _db.Address
                            .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return address;
        }

         public async Task<bool> SaveAddress(Address address)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User Not Found");
            }
            if (address.Id == 0)
            {
                // Add a new address if the Id is 0 (assuming Id is auto-generated)

                address.UserId= userId; 
                _db.Address.Add(address);
            }
            else
            {
                // Update an existing address
                var existingAddress = _db.Address.Find(address.Id);
                if (existingAddress != null)
                {
                    // Update the properties of the existing address
                    existingAddress.UserId= userId; 
                    existingAddress.FirstName = address.FirstName;
                    existingAddress.LastName = address.LastName;
                    existingAddress.Street = address.Street;
                    existingAddress.City = address.City;
                    existingAddress.PostalCode = address.PostalCode;
                    existingAddress.Country = address.Country;
                    existingAddress.Phone = address.Phone;
                    existingAddress.Email = address.Email;
                }
            }

            // Save changes to the database
            _db.SaveChanges();
            return true;
        }
    
       

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

       
    }

}

using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface IAddressRepository
    {
        Task<Address> UserAddress();
        Task<bool> SaveAddress(Address address);
    }
}
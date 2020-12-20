using DakarRally.Models;
using System.Threading.Tasks;

namespace DakarRally.Repository
{
    public interface IDakarRally
    {
        /// <summary>
        /// List of vehicles.
        /// </summary>
        IRepository<Vehicle> Vehicles { get; }

        /// <summary>
        /// Races list.
        /// </summary>
        IRepository<Race> Races { get; }

        /// <summary>
        /// Commit changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Commit changes async.
        /// </summary>
        /// <returns></returns>
        Task CommitAsync();
    }
}
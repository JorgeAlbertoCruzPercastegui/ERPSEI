namespace ERPSEI.Data.Managers
{
	public interface IRWCatalogoManager<T>
	{
		public Task<int> CreateAsync(T element);

		public Task UpdateAsync(T element);

		public Task DeleteAsync(T element);

		public Task DeleteByIdAsync(int id);

		public Task DeleteMultipleByIdAsync(string[] ids);

		public Task<List<T>> GetAllAsync();

		public Task<T?> GetByIdAsync(int id);

		public Task<T?> GetByNameAsync(string name);
    }
}

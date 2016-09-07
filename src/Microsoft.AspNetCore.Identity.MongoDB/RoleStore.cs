﻿namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using global::MongoDB.Driver;

	/// <summary>
	///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
	///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
	///     perform a document modification on the users collection before a delete or a rename.
	/// </summary>
	/// <typeparam name="TRole"></typeparam>
	public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
		where TRole : IdentityRole
	{
		private readonly IMongoCollection<TRole> _Roles;

		public RoleStore(IMongoCollection<TRole> roles)
		{
			_Roles = roles;
		}

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken token)
		{
			await _Roles.InsertOneAsync(role, cancellationToken: token);
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken token)
		{
			await _Roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: token);
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken token)
		{
			await _Roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken: token);
			return IdentityResult.Success;
		}

		public virtual async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) => role.Id;

		public virtual async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) => role.Name;

		public virtual async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken) => role.Name = roleName;

		public virtual async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) => role.NormalizedName;

		public virtual async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) => role.NormalizedName = normalizedName;

		public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken token)
		{
			return _Roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(token);
		}

		public virtual Task<TRole> FindByNameAsync(string roleName, CancellationToken token)
		{
			// todo thoughts on searching now by normalized name without changing api...
			return _Roles.Find(r => r.NormalizedName == roleName).FirstOrDefaultAsync(token);
		}

		public virtual IQueryable<TRole> Roles => _Roles.AsQueryable();
	}
}
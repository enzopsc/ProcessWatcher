using System.Data.SQLite;

namespace ProcessWatcher.Database
{
	public class ProcessWatcherModel : BECCore.EntityHelper.DbModelBase<ProcessWatcherModel>
	{
		public ProcessWatcherModel(string connectionString, bool disableIdentitiesForDefaultKeys) : base(connectionString, disableIdentitiesForDefaultKeys)
		{
		}

		public ProcessWatcherModel(SQLiteConnection filename, bool disableIdentitiesForDefaultKeys) : base(filename, disableIdentitiesForDefaultKeys)
		{
		}

	}
}
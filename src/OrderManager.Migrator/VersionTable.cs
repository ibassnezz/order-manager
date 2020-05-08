using FluentMigrator.Runner.VersionTableInfo;

namespace OrderManager.Migrator
{
    [VersionTableMetaData]
    public class VersionTable : IVersionTableMetaData
    {
        public object ApplicationContext { get; set; }

        public bool OwnsSchema => false;

        public string SchemaName => "public";

        public string TableName => "version_info";

        public string ColumnName => "version";

        public string DescriptionColumnName => "description";

        public string AppliedOnColumnName => "applied_on";

        public string UniqueIndexName => "uc_version";
    }
}
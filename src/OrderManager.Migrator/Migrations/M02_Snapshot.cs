using FluentMigrator;

namespace OrderManager.Migrator.Migrations
{
    [Migration(3)]
    class M02_Snapshot : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
            create table order_snapshots
            (
                order_number varchar(64) not null,
                last_version int not null,
                data jsonb
                );
                create unique index order_number_last_version_idx ON order_events(order_number, last_version);
            ");
        }

        public override void Down()
        {
            Execute.Sql("drop table order_snapshots");
        }
    }
}

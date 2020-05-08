using FluentMigrator;

namespace OrderManager.Migrator.Migrations
{
    [Migration(1)]
    public class InitMigration: Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            create table order_events
                            (
	                            id BIGSERIAL,
	                            order_number varchar(64),
                                event_type varchar(128),
                                version int not null,
	                            data jsonb
                            );
                            create unique index order_number_version_idx ON order_events (order_number, version);
                        ");

        }

        public override void Down()
        {
            Execute.Sql("DROP TABLE IF EXISTS order_events");
        }
    }
}
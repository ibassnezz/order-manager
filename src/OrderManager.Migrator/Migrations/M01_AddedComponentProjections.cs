using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace OrderManager.Migrator.Migrations
{
    [Migration(2)]
    public class M01_AddedComponentProjections : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"create table order_components
                            (
	                            id serial
		                            constraint order_components_pk
			                            primary key,
	                            order_number varchar(64) not null,
	                            componentId bigint,
	                            is_processed boolean not null
                            );

                            create unique index order_components_componentId_uindex
	                            on order_components (componentId);
                        ");
        }

        public override void Down()
        {
            Execute.Sql(@"drop table order_components");
        }
    }
}

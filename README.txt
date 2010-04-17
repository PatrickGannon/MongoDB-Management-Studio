MongoDB Management Studio is a MongoDB client based on WPF/MVVM Light, using MongoDB/C# driver.
This project is very new - more to come... hopefully.

Query syntax is as follows:
{collection}:{query} [limit {limit}]
where 
	{collection} is the name of your collection
	{query} is a javascript condition ($where clause)
	{limit} is the maximum number of rows to return

examples:
mycollection:this.myfield == 'somevalue'
mycollection:this.myfield == 'somevalue' limit 5


You can also enter just a collection name to show all the data in that collection.
